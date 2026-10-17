using System;
using System.Collections.Generic;
using System.IO;
using BoardGameFramework.AI;
using BoardGameFramework.Board;
using BoardGameFramework.Commands;
using BoardGameFramework.Exceptions;
using BoardGameFramework.Factories;
using BoardGameFramework.Help;
using BoardGameFramework.Parsing;
using BoardGameFramework.Persistence;
using BoardGameFramework.Players;
using BoardGameFramework.Rules;
using BoardGameFramework.Views;

namespace BoardGameFramework.Core
{
    public class GameEngine
    {
        private IGameFactory factory;
        private GridBoard board;
        private IWinStrategy winStrategy;
        private IMoveRules moveRules;
        private IHelpProvider help;
        private IAiStrategy ai;
        private CommandHistory history;
        private Player player1;
        private Player player2;
        private Player currentPlayer;
        private GameMode mode;
        private AiDifficulty difficulty;
        private List<IGameObserver> observers;
        private TextReader input;
        private TextWriter output;
        private bool gameOver;
        private GameOutcome outcome;
        private int turnCount;
        private bool exitToMenu;

        private GameEngine()
        {
            observers = new List<IGameObserver>();
            history = new CommandHistory();
            outcome = GameOutcome.Continue();
            turnCount = 1;
        }

        public static GameEngine Create(IGameFactory factory, GameMode mode, AiDifficulty difficulty, string player1Name, string player2Name, TextReader input, TextWriter output)
        {
            GameEngine engine = new GameEngine();
            engine.factory = factory;
            engine.mode = mode;
            engine.difficulty = difficulty;
            engine.input = input;
            engine.output = output;
            engine.board = factory.CreateBoard();
            factory.SeedBoard(engine.board);
            engine.winStrategy = factory.CreateWinStrategy();
            engine.moveRules = factory.CreateMoveRules();
            engine.help = factory.CreateHelp();
            engine.ai = difficulty == AiDifficulty.Smarter ? factory.CreateSmartAi() : factory.CreateDumbAi();
            engine.player1 = new HumanPlayer(player1Name, 1, factory.CreateInventory());
            if (mode == GameMode.HumanVsComputer)
                engine.player2 = new ComputerPlayer(player2Name, 2, factory.CreateInventory());
            else
                engine.player2 = new HumanPlayer(player2Name, 2, factory.CreateInventory());
            engine.currentPlayer = engine.player1;
            engine.observers.Add(new ConsoleObserver(factory.CreateView(), output));
            return engine;
        }

        public static GameEngine FromSnapshot(GameSnapshot snapshot, TextReader input, TextWriter output)
        {
            GameMode mode = snapshot.Mode == GameMode.HumanVsComputer.ToString() ? GameMode.HumanVsComputer : GameMode.HumanVsHuman;
            AiDifficulty difficulty = snapshot.AiDifficulty == AiDifficulty.Smarter.ToString() ? AiDifficulty.Smarter : AiDifficulty.Dumb;
            IGameFactory factory = GameFactoryProvider.Create(snapshot.Family, snapshot.Variant);
            GameEngine engine = Create(factory, mode, difficulty, snapshot.Player1Name, snapshot.Player2Name, input, output);
            engine.board.LoadFromJaggedArray(snapshot.Grid);
            engine.player1.Inventory.HeavyRemaining = snapshot.Player1HeavyRemaining;
            engine.player1.Inventory.EraserRemaining = snapshot.Player1EraserRemaining;
            engine.player2.Inventory.HeavyRemaining = snapshot.Player2HeavyRemaining;
            engine.player2.Inventory.EraserRemaining = snapshot.Player2EraserRemaining;
            engine.turnCount = snapshot.TurnCount;
            engine.currentPlayer = snapshot.CurrentPlayerNumber == 2 ? engine.player2 : engine.player1;
            engine.RebuildHistory(snapshot.History);
            return engine;
        }

        public IGameFactory Factory { get { return factory; } }
        public GridBoard Board { get { return board; } }
        public Player CurrentPlayer { get { return currentPlayer; } }
        public bool GameOver { get { return gameOver; } }
        public GameOutcome Outcome { get { return outcome; } }
        public CommandHistory History { get { return history; } }
        public bool ExitToMenu { get { return exitToMenu; } }

        // Template Method: the game-loop algorithm is fixed here.
        // Variant-specific behaviour is supplied by Strategy objects from the factory.
        public void PlayInteractive()
        {
            StartSession();
            while (!IsSessionFinished())
            {
                NotifyObservers();
                if (gameOver)
                    break;
                ProcessTurn();
            }
            FinishSession();
        }

        protected virtual void StartSession()
        {
            output.WriteLine();
            output.WriteLine("Starting " + factory.VariantTitle + " (" + ModeLabel() + "). Type HELP for commands.");
        }

        protected virtual bool IsSessionFinished()
        {
            return gameOver || exitToMenu;
        }

        protected virtual void FinishSession()
        {
            if (!exitToMenu)
            {
                NotifyObservers();
                WriteOutcome();
            }
        }

        public void RunScript(string script)
        {
            output.WriteLine("Running automated testing mode...");
            output.WriteLine("Game: " + factory.FamilyName + " / " + factory.VariantTitle);
            output.WriteLine("Script: " + script);
            output.WriteLine();

            string[] commands = InputParser.SplitScript(script);
            for (int i = 0; i < commands.Length; i++)
            {
                if (gameOver)
                    break;

                string token = commands[i];
                try
                {
                    ParsedInput parsed = InputParser.Parse(token);
                    if (!IsGameplay(parsed.Kind))
                    {
                        output.WriteLine("P" + currentPlayer.PlayerNumber + ": '" + token + "' ignored in script (not a gameplay command).");
                        continue;
                    }
                    int actor = currentPlayer.PlayerNumber;
                    ApplyGameplay(parsed);
                    output.WriteLine("P" + actor + ": command '" + token + "' applied.");
                    if (gameOver)
                    {
                        output.WriteLine("*** " + OutcomeText() + " ***");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    output.WriteLine("P" + currentPlayer.PlayerNumber + ": '" + token + "' rejected - " + ex.Message);
                }
            }

            output.WriteLine();
            output.WriteLine("Final board state:");
            RenderFinal();
            WriteStatus();
            if (gameOver)
                WriteOutcome();
        }

        protected virtual void ProcessTurn()
        {
            if (currentPlayer.IsComputer)
            {
                ParsedInput parsed = ai.ChooseMove(board, currentPlayer, Opponent().PlayerNumber, moveRules);
                output.WriteLine(currentPlayer.Name + " (" + difficulty + " AI) plays " + Describe(parsed));
                ApplyGameplay(parsed);
                return;
            }

            output.Write(Prompt());
            string line = input.ReadLine();
            if (line == null)
                return;

            try
            {
                ParsedInput parsed = InputParser.Parse(line);
                HandleInput(parsed);
            }
            catch (Exception ex)
            {
                output.WriteLine("Invalid input: " + ex.Message);
            }
        }

        private void HandleInput(ParsedInput parsed)
        {
            if (parsed.Kind == InputKind.Help)
            {
                help.WriteHelp(output, currentPlayer);
                return;
            }
            if (parsed.Kind == InputKind.Quit)
            {
                exitToMenu = true;
                output.WriteLine("Returning to the main menu.");
                return;
            }
            if (parsed.Kind == InputKind.Save)
            {
                SaveGame(parsed.FileName);
                return;
            }
            if (parsed.Kind == InputKind.Load)
            {
                LoadGame(parsed.FileName);
                return;
            }
            if (parsed.Kind == InputKind.Undo)
            {
                UndoTurn();
                return;
            }
            if (parsed.Kind == InputKind.Redo)
            {
                RedoTurn();
                return;
            }

            ApplyGameplay(parsed);
        }

        private void ApplyGameplay(ParsedInput parsed)
        {
            IMoveCommand command = moveRules.CreateCommand(board, currentPlayer, Opponent().PlayerNumber, parsed);
            command.Execute();
            history.Record(command);
            turnCount = turnCount + 1;

            int lastRow = 0;
            int lastCol = 0;
            PlaceStoneCommand stone = command as PlaceStoneCommand;
            PlaceDiskCommand disk = command as PlaceDiskCommand;
            if (stone != null)
            {
                lastRow = stone.Row;
                lastCol = stone.Col;
            }
            else if (disk != null)
            {
                lastRow = disk.Row;
                lastCol = disk.Col;
            }

            EvaluateAfterMove(currentPlayer.PlayerNumber, lastRow, lastCol);
            if (gameOver)
                return;

            SwitchTurn();
            DetectTerminalPassLock();
        }

        private void EvaluateAfterMove(int lastPlayer, int lastRow, int lastCol)
        {
            outcome = winStrategy.Evaluate(board, lastPlayer, lastRow, lastCol, false);
            gameOver = outcome.IsOver;
        }

        private void DetectTerminalPassLock()
        {
            if (gameOver || !moveRules.AllowsPass)
                return;

            if (moveRules.ListLegalMoves(board, currentPlayer, Opponent().PlayerNumber).Count > 0)
                return;

            if (moveRules.ListLegalMoves(board, Opponent(), currentPlayer.PlayerNumber).Count == 0)
            {
                outcome = winStrategy.Evaluate(board, Opponent().PlayerNumber, 0, 0, true);
                gameOver = outcome.IsOver;
            }
        }

        private void UndoTurn()
        {
            if (!history.CanUndo)
            {
                output.WriteLine("Nothing to undo.");
                return;
            }

            int steps = history.UndoTurn();
            if (steps % 2 == 1)
                SwitchTurn();
            gameOver = false;
            outcome = GameOutcome.Continue();
            output.WriteLine("Undo restored the previous full turn (" + steps + " action(s)).");
        }

        private void RedoTurn()
        {
            if (!history.CanRedo)
            {
                output.WriteLine("Nothing to redo.");
                return;
            }

            int steps = history.RedoTurn();
            if (steps % 2 == 1)
                SwitchTurn();
            EvaluateAfterMove(Opponent().PlayerNumber, 0, 0);
            if (!gameOver)
                DetectTerminalPassLock();
            output.WriteLine("Redo restored " + steps + " action(s).");
        }

        private void SwitchTurn()
        {
            currentPlayer = Opponent();
        }

        private Player Opponent()
        {
            if (currentPlayer == player1)
                return player2;
            return player1;
        }

        private void NotifyObservers()
        {
            string status = StatusLine();
            for (int i = 0; i < observers.Count; i++)
                observers[i].OnStateChanged(board, currentPlayer.PlayerNumber, status);
        }

        private void RenderFinal()
        {
            FogBoardView fog = factory.CreateView() as FogBoardView;
            if (fog != null)
            {
                output.WriteLine("Active player perspective (P" + currentPlayer.PlayerNumber + "):");
                fog.Render(board, currentPlayer.PlayerNumber, output);
                output.WriteLine("True board:");
            }
            new FullBoardView().Render(board, currentPlayer.PlayerNumber, output);
        }

        private void WriteStatus()
        {
            output.WriteLine(StatusLine());
        }

        private string StatusLine()
        {
            string counts = "P1=" + board.CountPieces(1) + " P2=" + board.CountPieces(2);
            string extra = "";
            if (player1.Inventory.HeavyRemaining + player1.Inventory.EraserRemaining + player2.Inventory.HeavyRemaining + player2.Inventory.EraserRemaining > 0)
            {
                extra = " | P1 Heavy/Eraser " + player1.Inventory.HeavyRemaining + "/" + player1.Inventory.EraserRemaining +
                    " P2 Heavy/Eraser " + player2.Inventory.HeavyRemaining + "/" + player2.Inventory.EraserRemaining;
            }
            string who = gameOver ? "Game over" : "Turn " + turnCount + ": " + currentPlayer.Name + " (P" + currentPlayer.PlayerNumber + ")";
            return who + " | " + counts + extra;
        }

        private void WriteOutcome()
        {
            output.WriteLine(OutcomeText());
        }

        private string OutcomeText()
        {
            if (outcome == null || !outcome.IsOver)
                return "Game is still in progress.";
            if (outcome.IsDraw)
                return "Draw. " + outcome.Message;
            string name = outcome.Winner == 1 ? player1.Name : player2.Name;
            return name + " (Player " + outcome.Winner + ") wins! " + outcome.Message;
        }

        private string Prompt()
        {
            return currentPlayer.Name + " (P" + currentPlayer.PlayerNumber + ") >> ";
        }

        private string ModeLabel()
        {
            if (mode == GameMode.HumanVsComputer)
                return "HvC / " + difficulty + " AI";
            return "HvH";
        }

        private bool IsGameplay(InputKind kind)
        {
            return kind == InputKind.PlaceOrdinary || kind == InputKind.PlaceHeavy || kind == InputKind.PlaceEraser
                || kind == InputKind.PlaceDisk || kind == InputKind.Pass;
        }

        private string Describe(ParsedInput parsed)
        {
            if (parsed.Kind == InputKind.Pass)
                return "PASS";
            if (parsed.Kind == InputKind.PlaceHeavy)
                return "H" + parsed.Row + ":" + parsed.Col;
            if (parsed.Kind == InputKind.PlaceEraser)
                return "E" + parsed.Row + ":" + parsed.Col;
            if (parsed.Kind == InputKind.PlaceDisk)
                return "P" + parsed.Row + ":" + parsed.Col;
            return "O" + parsed.Row + ":" + parsed.Col;
        }

        public GameSnapshot ToSnapshot()
        {
            GameSnapshot snapshot = new GameSnapshot();
            snapshot.Family = factory.FamilyName;
            snapshot.Variant = VariantKey(factory.Variant);
            snapshot.Mode = mode.ToString();
            snapshot.AiDifficulty = difficulty.ToString();
            snapshot.Grid = board.ToJaggedArray();
            snapshot.CurrentPlayerNumber = currentPlayer.PlayerNumber;
            snapshot.TurnCount = turnCount;
            snapshot.Player1Name = player1.Name;
            snapshot.Player2Name = player2.Name;
            snapshot.Player1HeavyRemaining = player1.Inventory.HeavyRemaining;
            snapshot.Player1EraserRemaining = player1.Inventory.EraserRemaining;
            snapshot.Player2HeavyRemaining = player2.Inventory.HeavyRemaining;
            snapshot.Player2EraserRemaining = player2.Inventory.EraserRemaining;
            snapshot.History = history.ToRecords();
            return snapshot;
        }

        private void SaveGame(string fileName)
        {
            if (fileName == null || fileName.Length == 0)
            {
                output.Write("Enter file name to save (e.g. save1.json) >> ");
                fileName = input.ReadLine();
            }
            if (fileName == null || fileName.Length == 0)
            {
                output.WriteLine("Save cancelled: no file name provided.");
                return;
            }
            SaveLoadService.Save(ToSnapshot(), fileName);
            output.WriteLine("Game saved to '" + fileName + "'.");
        }

        private void LoadGame(string fileName)
        {
            if (fileName == null || fileName.Length == 0)
            {
                output.Write("Enter file name to load >> ");
                fileName = input.ReadLine();
            }
            if (fileName == null || fileName.Length == 0)
            {
                output.WriteLine("Load cancelled: no file name provided.");
                return;
            }

            GameEngine loaded = FromSnapshot(SaveLoadService.Load(fileName), input, output);
            CopyFrom(loaded);
            output.WriteLine("Game loaded from '" + fileName + "'. Undo is available if history was saved.");
        }

        public static GameEngine LoadFromFile(string fileName, TextReader input, TextWriter output)
        {
            return FromSnapshot(SaveLoadService.Load(fileName), input, output);
        }

        private void CopyFrom(GameEngine other)
        {
            factory = other.factory;
            board = other.board;
            winStrategy = other.winStrategy;
            moveRules = other.moveRules;
            help = other.help;
            ai = other.ai;
            history = other.history;
            player1 = other.player1;
            player2 = other.player2;
            currentPlayer = other.currentPlayer;
            mode = other.mode;
            difficulty = other.difficulty;
            observers = other.observers;
            gameOver = other.gameOver;
            outcome = other.outcome;
            turnCount = other.turnCount;
        }

        private void RebuildHistory(List<HistoryRecord> records)
        {
            history.Clear();
            if (records == null)
                return;
            for (int i = 0; i < records.Count; i++)
            {
                IMoveCommand command = CommandFromRecord(records[i]);
                history.RecordWithoutClearingRedo(command);
            }
        }

        private IMoveCommand CommandFromRecord(HistoryRecord record)
        {
            Player actor = record.PlayerNumber == 1 ? player1 : player2;
            if (record.Kind == "Ordinary")
                return new PlaceStoneCommand(board, actor.Inventory, actor.PlayerNumber, record.Row, record.Col, (PieceKind)record.PlacedPiece, false);
            if (record.Kind == "Heavy")
                return new PlaceStoneCommand(board, actor.Inventory, actor.PlayerNumber, record.Row, record.Col, (PieceKind)record.PlacedPiece, true);
            if (record.Kind == "Eraser")
                return new EraseStoneCommand(board, actor.Inventory, actor.PlayerNumber, record.Row, record.Col, (PieceKind)record.ErasedPiece);
            if (record.Kind == "Disk")
            {
                List<int> rows = record.FlipRows ?? new List<int>();
                List<int> cols = record.FlipCols ?? new List<int>();
                return new PlaceDiskCommand(board, actor.PlayerNumber, record.Row, record.Col, rows, cols);
            }
            return new PassCommand(actor.PlayerNumber);
        }

        private static string VariantKey(GameVariant variant)
        {
            if (variant == GameVariant.Plus)
                return "plus";
            if (variant == GameVariant.Fog)
                return "fog";
            if (variant == GameVariant.Anti)
                return "anti";
            if (variant == GameVariant.Corner)
                return "corner";
            return "standard";
        }
    }
}
