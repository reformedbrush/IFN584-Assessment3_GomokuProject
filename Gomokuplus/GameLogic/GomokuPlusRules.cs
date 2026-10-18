using GomokuPlus.Exceptions;
using GomokuPlus.Framework;
using GomokuPlus.Models;
using GomokuPlus.Players;

namespace GomokuPlus.GameLogic
{
    public class GomokuPlusRules : IGameRules
    {
        public string VariantName
        {
            get { return "GomokuPlus"; }
        }

        public void ValidateMove(Board board, Player player, Move move, int opponentNumber)
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
                StoneType opponentOrdinary = opponentNumber == 1 ? StoneType.OrdinaryP1 : StoneType.OrdinaryP2;
                StoneType opponentHeavy = opponentNumber == 1 ? StoneType.HeavyP1 : StoneType.HeavyP2;

                if (target == StoneType.Empty)
                    throw new InvalidMoveException("Cell (" + move.Row + "," + move.Col + ") is empty; there is nothing to erase.");
                if (target == opponentHeavy)
                    throw new InvalidMoveException("Heavy Stones cannot be targeted or removed by an Eraser Stone.");
                if (target != opponentOrdinary)
                    throw new InvalidMoveException("The Eraser Stone must target an opponent's Ordinary Stone.");
            }
            else
            {
                throw new InvalidMoveException("Unknown move type.");
            }
        }

        public bool ApplyMove(Board board, Player player, Move move, int opponentNumber)
        {
            ValidateMove(board, player, move, opponentNumber);

            if (move.Type == MoveType.Ordinary)
            {
                board.PlaceStone(move.Row, move.Col, player.OrdinarySymbol);
                return WinChecker.CheckWin(board, move.Row, move.Col, player.PlayerNumber);
            }

            if (move.Type == MoveType.Heavy)
            {
                board.PlaceStone(move.Row, move.Col, player.HeavySymbol);
                player.HeavyStonesRemaining--;
                return WinChecker.CheckWin(board, move.Row, move.Col, player.PlayerNumber);
            }

            board.ClearCell(move.Row, move.Col);
            player.EraserStonesRemaining--;
            return false;
        }

        public string GetHelpText()
        {
            return "Rules:\n" +
                "  - Align 5 of your stones (Ordinary or Heavy) in a row - horizontal,\n" +
                "    vertical, or diagonal - to win instantly.\n" +
                "  - Heavy Stones cannot be targeted or removed by an Eraser Stone.\n" +
                "  - Each player starts with 2 Heavy Stones and 2 Eraser Stones.\n" +
                "  - Ordinary Stones are unlimited.";
        }
    }
}
