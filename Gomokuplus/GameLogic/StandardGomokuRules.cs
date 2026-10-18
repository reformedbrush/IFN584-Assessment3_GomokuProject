using GomokuPlus.Exceptions;
using GomokuPlus.Framework;
using GomokuPlus.Models;
using GomokuPlus.Players;

namespace GomokuPlus.GameLogic
{
    public class StandardGomokuRules : IGameRules
    {
        public virtual string VariantName
        {
            get { return "Standard Gomoku"; }
        }

        public void ValidateMove(Board board, Player player, Move move, int opponentNumber)
        {
            if (move.Type != MoveType.Ordinary)
                throw new InvalidMoveException("Standard Gomoku only allows Ordinary Stones.");
            if (!Board.IsInsideBoard(move.Row, move.Col))
                throw new InvalidMoveException("(" + move.Row + "," + move.Col + ") is outside the 10x10 grid.");
            if (!board.IsEmpty(move.Row, move.Col))
                throw new InvalidMoveException("Cell (" + move.Row + "," + move.Col + ") is already occupied.");
        }

        public bool ApplyMove(Board board, Player player, Move move, int opponentNumber)
        {
            ValidateMove(board, player, move, opponentNumber);
            board.PlaceStone(move.Row, move.Col, player.OrdinarySymbol);
            return WinChecker.CheckWin(board, move.Row, move.Col, player.PlayerNumber);
        }

        public virtual string GetHelpText()
        {
            return "Rules:\n" +
                "  - Place one Ordinary Stone on an empty cell each turn.\n" +
                "  - Align 5 of your stones horizontally, vertically, or diagonally to win.";
        }
    }
}
