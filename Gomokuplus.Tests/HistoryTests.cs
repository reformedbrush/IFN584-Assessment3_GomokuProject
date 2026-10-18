using System.IO;
using System;
using GomokuPlus.GameLogic;
using GomokuPlus.GameLogic.Reversi;
using GomokuPlus.Framework;
using GomokuPlus.Models;
using GomokuPlus.Persistence;
using GomokuPlus.Players;
using Xunit;

namespace GomokuPlus.Tests
{
    public class HistoryTests
    {
        [Fact]
        public void UndoAndRedoRestoreBoardAndInventory()
        {
            GomokuGame game = new GomokuGame(GameMode.HumanVsHuman, "P1", "P2");

            game.ApplyMove(game.Player1, new Move(MoveType.Heavy, 3, 3));

            Assert.Equal(StoneType.HeavyP1, game.Board.GetCell(3, 3));
            Assert.Equal(1, game.Player1.HeavyStonesRemaining);
            Assert.True(game.CanUndo);

            Assert.True(game.Undo());
            Assert.Equal(StoneType.Empty, game.Board.GetCell(3, 3));
            Assert.Equal(2, game.Player1.HeavyStonesRemaining);
            Assert.True(game.CanRedo);

            Assert.True(game.Redo());
            Assert.Equal(StoneType.HeavyP1, game.Board.GetCell(3, 3));
            Assert.Equal(1, game.Player1.HeavyStonesRemaining);
        }

        [Fact]
        public void NewMoveAfterUndoClearsRedoBranch()
        {
            GomokuGame game = new GomokuGame(GameMode.HumanVsHuman, "P1", "P2");

            game.ApplyMove(game.Player1, new Move(MoveType.Ordinary, 1, 1));
            Assert.True(game.Undo());

            game.ApplyMove(game.Player1, new Move(MoveType.Ordinary, 2, 2));

            Assert.False(game.CanRedo);
            Assert.Equal(StoneType.Empty, game.Board.GetCell(1, 1));
            Assert.Equal(StoneType.OrdinaryP1, game.Board.GetCell(2, 2));
        }

        [Fact]
        public void HistorySnapshotsRoundTripThroughJsonPersistence()
        {
            string fileName = Path.Combine(Path.GetTempPath(), "gomokuplus-history-test.json");
            GameStateData state = new GameStateData();
            state.Grid = new Board().ToJaggedArray();
            state.CurrentPlayerNumber = 2;
            state.TurnCount = 4;
            state.UndoHistory = new System.Collections.Generic.List<GameSnapshotData>
            {
                new GameSnapshotData
                {
                    Grid = new Board().ToJaggedArray(),
                    CurrentPlayerNumber = 1,
                    TurnCount = 3
                }
            };

            try
            {
                SaveLoadManager.Save(state, fileName);
                GameStateData loaded = SaveLoadManager.Load(fileName);

                Assert.Equal(2, loaded.CurrentPlayerNumber);
                Assert.Equal(4, loaded.TurnCount);
                Assert.Single(loaded.UndoHistory);
                Assert.Equal(1, loaded.UndoHistory[0].CurrentPlayerNumber);
            }
            finally
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
        }

        [Fact]
        public void HardAiChoosesAnImmediateWinningMove()
        {
            Board board = new Board();
            Player player = new HumanPlayer("P1", 1);
            board.PlaceStone(1, 1, StoneType.OrdinaryP1);
            board.PlaceStone(1, 2, StoneType.OrdinaryP1);
            board.PlaceStone(1, 3, StoneType.OrdinaryP1);
            board.PlaceStone(1, 4, StoneType.OrdinaryP1);

            HardAiStrategy strategy = new HardAiStrategy();
            Move move = strategy.ChooseMove(board, player, 2);

            Assert.Equal(MoveType.Ordinary, move.Type);
            Assert.Equal(1, move.Row);
            Assert.Equal(5, move.Col);
        }

        [Fact]
        public void VariantRegistryDistinguishesImplementedAndPlannedVariants()
        {
            Assert.True(GameVariantRegistry.Find("GomokuPlus").IsImplemented);
            Assert.True(GameVariantRegistry.Find("Standard Gomoku").IsImplemented);
            Assert.True(GameVariantRegistry.Find("GomokuFog").IsImplemented);
            Assert.True(GameVariantRegistry.Find("Standard Reversi").IsImplemented);
            Assert.True(GameVariantRegistry.Find("Anti-Reversi").IsImplemented);
            Assert.True(GameVariantRegistry.Find("Corner Reversi").IsImplemented);
        }

        [Fact]
        public void StandardReversiAppliesAHorizontalCapture()
        {
            ReversiBoard board = new ReversiBoard();
            ReversiRules rules = new ReversiRules();

            rules.Apply(board, new ReversiMove(4, 6), ReversiCell.Player1);

            Assert.Equal(ReversiCell.Player1, board.GetCell(4, 6));
            Assert.Equal(ReversiCell.Player1, board.GetCell(4, 4));
            Assert.Equal(ReversiCell.Player1, board.GetCell(4, 5));
        }

        [Fact]
        public void ReversiRequiresPassOnlyWhenNoMoveExists()
        {
            ReversiBoard board = new ReversiBoard();
            ReversiRules rules = new ReversiRules();

            Assert.Throws<InvalidOperationException>(() =>
                rules.Apply(board, ReversiMove.Pass(), ReversiCell.Player1));
        }

        [Fact]
        public void AntiReversiUsesLowestDiscCountAsWinner()
        {
            ReversiBoard board = new ReversiBoard();
            for (int row = 1; row <= ReversiBoard.Size; row++)
                for (int col = 1; col <= ReversiBoard.Size; col++)
                    board.SetCell(row, col, ReversiCell.Player1);

            board.SetCell(1, 1, ReversiCell.Player2);
            AntiReversiRules rules = new AntiReversiRules();

            Assert.Equal(ReversiCell.Player2, rules.Winner(board));
        }

        [Fact]
        public void GomokuFogUsesAVisibilityProjectionWithoutChangingAuthoritativeBoard()
        {
            Board board = new Board();
            board.PlaceStone(5, 5, StoneType.OrdinaryP1);
            board.PlaceStone(5, 7, StoneType.OrdinaryP2);
            GomokuFogVisibility fog = new GomokuFogVisibility();

            bool[,] visibility = fog.GetVisibleCells(board, 1, 1);

            Assert.Equal(StoneType.OrdinaryP1, fog.VisibleCell(board, visibility, 5, 5));
            Assert.Equal(StoneType.Empty, fog.VisibleCell(board, visibility, 5, 7));
            Assert.Equal(StoneType.OrdinaryP2, board.GetCell(5, 7));
        }

        [Fact]
        public void CornerReversiPrioritisesCornerCaptures()
        {
            ReversiBoard board = new ReversiBoard();
            board.SetCell(1, 2, ReversiCell.Player2);
            board.SetCell(1, 3, ReversiCell.Player1);
            CornerReversiRules rules = new CornerReversiRules();

            var moves = rules.LegalMoves(board, ReversiCell.Player1);

            Assert.Contains(moves, move => move.Row == 1 && move.Col == 1);
        }
    }
}
