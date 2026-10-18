using System;
using System.Collections.Generic;

namespace GomokuPlus.GameLogic.Reversi
{
    public class CornerReversiRules : ReversiRules
    {
        public override IReadOnlyList<ReversiMove> LegalMoves(ReversiBoard board, ReversiCell player)
        {
            IReadOnlyList<ReversiMove> normalMoves = base.LegalMoves(board, player);
            List<ReversiMove> cornerMoves = new List<ReversiMove>();

            foreach (ReversiMove move in normalMoves)
            {
                if (IsCorner(move.Row, move.Col))
                    cornerMoves.Add(move);
            }

            return cornerMoves.Count > 0 ? cornerMoves : normalMoves;
        }

        private static bool IsCorner(int row, int col)
        {
            return (row == 1 || row == ReversiBoard.Size) &&
                (col == 1 || col == ReversiBoard.Size);
        }
    }
}
