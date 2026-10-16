using System;
using GomokuPlus.Exceptions;
using GomokuPlus.Models;
using GomokuPlus.Persistence;
using GomokuPlus.Players;

namespace GomokuPlus.GameLogic
{
    // The central coordinator: owns the board and the two players, drives
    // the turn loop, validates and applies moves, and handles saving,
    // loading, and automated-script execution.
    public class GomokuGame
    {
        private Board board;
        private Player player1;
        private Player player2;
        private Player currentPlayer;
        private GameMode mode;
        private int turnCount;
        private bool gameOver;

        public GomokuGame(GameMode mode, string player1Name, string player2Name)
        {
            this.mode = mode;
            board = new Board();
            player1 = new HumanPlayer(player1Name, 1);

            if (mode == GameMode.HumanVsHuman)
                player2 = new HumanPlayer(player2Name, 2);
            else
                player2 = new ComputerPlayer(player2Name, 2);

            currentPlayer = player1;
            turnCount = 1;
            gameOver = false;
        }

        // Only used internally when restoring a saved game.
        private GomokuGame(GameMode mode, Player player1, Player player2)
        {
            this.mode = mode;
            this.player1 = player1;
            this.player2 = player2;
            board = new Board();
            currentPlayer = player1;
            turnCount = 1;
            gameOver = false;
        }

        public Board Board
        {
            get { return board; }
        }

        public Player Player1
        {
            get { return player1; }
        }

        public Player Player2
        {
            get { return player2; }
        }

        public Player CurrentPlayer
        {
            get { return currentPlayer; }
        }

        public int TurnCount
        {
            get { return turnCount; }
        }

        private Player Opponent()
        {
            if (currentPlayer == player1)
                return player2;
            else
                return player1;
        }

        // --- Interactive play -------------------------------------------------

        public void Run()
        {
            while (!gameOver)
            {
                Console.WriteLine();
                board.Render();
                Console.WriteLine("Turn " + turnCount + ": " + currentPlayer.Name + "'s move (Player " + currentPlayer.PlayerNumber + ").");

                Move move = currentPlayer.GetMove(board, Opponent().PlayerNumber);

                if (move.Type == MoveType.Quit)
                {
                    Console.WriteLine("Returning to the main menu.");
                    return;
                }

                if (move.Type == MoveType.Save)
                {
                    SaveGame();
                    continue;
                }

                try
                {
                    bool won = ApplyMove(currentPlayer, move);
                    if (won)
                    {
                        board.Render();
                        Console.WriteLine("*** " + currentPlayer.Name + " (Player " + currentPlayer.PlayerNumber + ") wins! ***");
                        gameOver = true;
                        break;
                    }
                }
                catch (InvalidMoveException ex)
                {
                    // Illegal move: reprompt the same player, no turn consumed.
                    Console.WriteLine("Invalid move: " + ex.Message);
                    continue;
                }

                SwitchTurn();
            }
        }

        // --- Move validation and application ------------------------------------

        public bool ApplyMove(Player player, Move move)
        {
            int opponentNumber;
            if (player.PlayerNumber == 1)
                opponentNumber = 2;
            else
                opponentNumber = 1;

            ValidateMove(player, move, opponentNumber);

            if (move.Type == MoveType.Ordinary)
            {
                board.PlaceStone(move.Row, move.Col, player.OrdinarySymbol);
                return WinChecker.CheckWin(board, move.Row, move.Col, player.PlayerNumber);
            }
            else if (move.Type == MoveType.Heavy)
            {
                board.PlaceStone(move.Row, move.Col, player.HeavySymbol);
                player.HeavyStonesRemaining = player.HeavyStonesRemaining - 1;
                return WinChecker.CheckWin(board, move.Row, move.Col, player.PlayerNumber);
            }
            else if (move.Type == MoveType.Eraser)
            {
                board.ClearCell(move.Row, move.Col);
                player.EraserStonesRemaining = player.EraserStonesRemaining - 1;
                return false;
            }
            else
            {
                throw new InvalidMoveException("Unknown move type.");
            }
        }

        private void ValidateMove(Player player, Move move, int opponentNumber)
        {
            if (!Board.IsInsideBoard(move.Row, move.Col))
                throw new InvalidMoveException("(" + move.Row + "," + move.Col + ") is outside the 10x10 grid.");

            if (move.Type == MoveType.Ordinary)
            {
                if (!board.IsEmpty(move.Row, move.Col))
                    throw new InvalidMoveException("Cell (" + move.Row + "," + move.Col + ") is already occupied.");
            }
            else if (move.Type == MoveType.Heavy)
            {
                if (player.HeavyStonesRemaining <= 0)
                    throw new InvalidMoveException(player.Name + " has no Heavy Stones left.");
                if (!board.IsEmpty(move.Row, move.Col))
                    throw new InvalidMoveException("Cell (" + move.Row + "," + move.Col + ") is already occupied.");
            }
            else if (move.Type == MoveType.Eraser)
            {
                if (player.EraserStonesRemaining <= 0)
                    throw new InvalidMoveException(player.Name + " has no Eraser Stones left.");

                StoneType target = board.GetCell(move.Row, move.Col);

                StoneType opponentOrdinary;
                StoneType opponentHeavy;
                if (opponentNumber == 1)
                {
                    opponentOrdinary = StoneType.OrdinaryP1;
                    opponentHeavy = StoneType.HeavyP1;
                }
                else
                {
                    opponentOrdinary = StoneType.OrdinaryP2;
                    opponentHeavy = StoneType.HeavyP2;
                }

                if (target == StoneType.Empty)
                    throw new InvalidMoveException("Cell (" + move.Row + "," + move.Col + ") is empty; there is nothing to erase.");
                if (target == opponentHeavy)
                    throw new InvalidMoveException("Heavy Stones cannot be targeted or removed by an Eraser Stone.");
                if (target != opponentOrdinary)
                    throw new InvalidMoveException("The Eraser Stone must target an opponent's Ordinary Stone.");
            }
        }

        private void SwitchTurn()
        {
            if (currentPlayer == player1)
                currentPlayer = player2;
            else
                currentPlayer = player1;

            turnCount = turnCount + 1;
        }

        // --- Save and Load ---------------------------------------------------------

        private void SaveGame()
        {
            Console.Write("Enter file name to save (e.g. save1.json) >> ");
            string fileName = Console.ReadLine();
            if (fileName == null || fileName.Length == 0)
            {
                Console.WriteLine("Save cancelled: no file name provided.");
                return;
            }

            GameStateData data = new GameStateData();
            data.Grid = board.ToJaggedArray();
            data.CurrentPlayerNumber = currentPlayer.PlayerNumber;
            data.TurnCount = turnCount;
            data.GameMode = mode.ToString();
            data.Player1Name = player1.Name;
            data.Player2Name = player2.Name;
            data.Player1HeavyRemaining = player1.HeavyStonesRemaining;
            data.Player1EraserRemaining = player1.EraserStonesRemaining;
            data.Player2HeavyRemaining = player2.HeavyStonesRemaining;
            data.Player2EraserRemaining = player2.EraserStonesRemaining;

            try
            {
                SaveLoadManager.Save(data, fileName);
                Console.WriteLine("Game saved to '" + fileName + "'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not save the game: " + ex.Message);
            }
        }

        public static GomokuGame LoadGame(string fileName)
        {
            try
            {
                GameStateData data = SaveLoadManager.Load(fileName);

                GameMode loadedMode;
                if (data.GameMode == GameMode.HumanVsComputer.ToString())
                    loadedMode = GameMode.HumanVsComputer;
                else
                    loadedMode = GameMode.HumanVsHuman;

                Player loadedPlayer1 = new HumanPlayer(data.Player1Name, 1);
                loadedPlayer1.HeavyStonesRemaining = data.Player1HeavyRemaining;
                loadedPlayer1.EraserStonesRemaining = data.Player1EraserRemaining;

                Player loadedPlayer2;
                if (loadedMode == GameMode.HumanVsHuman)
                    loadedPlayer2 = new HumanPlayer(data.Player2Name, 2);
                else
                    loadedPlayer2 = new ComputerPlayer(data.Player2Name, 2);

                loadedPlayer2.HeavyStonesRemaining = data.Player2HeavyRemaining;
                loadedPlayer2.EraserStonesRemaining = data.Player2EraserRemaining;

                GomokuGame game = new GomokuGame(loadedMode, loadedPlayer1, loadedPlayer2);
                game.turnCount = data.TurnCount;
                game.board.LoadFromJaggedArray(data.Grid);

                if (data.CurrentPlayerNumber == 1)
                    game.currentPlayer = loadedPlayer1;
                else
                    game.currentPlayer = loadedPlayer2;

                Console.WriteLine("Game loaded from '" + fileName + "'.");
                return game;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not load the game: " + ex.Message);
                return null;
            }
        }

        // --- Automated testing mode ---------------------------------------------
        // Parses a comma-separated command string (e.g. "O3:2,O3:3,...") from the
        // command line, applies each move alternating P1/P2, and renders the
        // final board. No interactive menus are shown in this mode.

        public static void RunAutomatedScript(string script)
        {
            GomokuGame game = new GomokuGame(GameMode.HumanVsHuman, "Player 1", "Player 2");
            string[] commands = script.Split(',');

            Console.WriteLine("Running automated testing mode...");
            Console.WriteLine("Script: " + script);
            Console.WriteLine();

            for (int i = 0; i < commands.Length; i++)
            {
                string command = commands[i];
                if (command.Length == 0)
                    continue;

                Player player = game.currentPlayer;
                try
                {
                    Move move = MoveParser.Parse(command);
                    bool won = game.ApplyMove(player, move);
                    Console.WriteLine("P" + player.PlayerNumber + ": " + DescribeMove(move) + " -> command '" + command + "' applied.");
                    if (won)
                    {
                        Console.WriteLine("*** Player " + player.PlayerNumber + " (" + player.Name + ") wins! ***");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("P" + player.PlayerNumber + ": '" + command + "' rejected - " + ex.Message);
                }

                game.turnCount = game.turnCount + 1;
                if (game.currentPlayer == game.player1)
                    game.currentPlayer = game.player2;
                else
                    game.currentPlayer = game.player1;
            }

            Console.WriteLine();
            Console.WriteLine("Final board state:");
            game.board.Render();
        }

        private static string DescribeMove(Move move)
        {
            string kind;
            if (move.Type == MoveType.Ordinary)
                kind = "Ordinary stone";
            else if (move.Type == MoveType.Heavy)
                kind = "Heavy stone";
            else if (move.Type == MoveType.Eraser)
                kind = "Eraser stone";
            else
                kind = "Move";

            return kind + " at (" + move.Row + "," + move.Col + ")";
        }
    }
}
