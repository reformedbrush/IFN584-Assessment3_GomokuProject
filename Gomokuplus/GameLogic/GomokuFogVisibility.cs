using GomokuPlus.Models;

namespace GomokuPlus.GameLogic
{
    public class GomokuFogVisibility
    {
        public bool[,] GetVisibleCells(Board board, int playerNumber, int radius = 1)
        {
            bool[,] visible = new bool[Board.Size, Board.Size];

            for (int row = 1; row <= Board.Size; row++)
            {
                for (int col = 1; col <= Board.Size; col++)
                {
                    StoneType cell = board.GetCell(row, col);
                    if (BelongsToPlayer(cell, playerNumber))
                        MarkAround(visible, row, col, radius);
                }
            }

            return visible;
        }

        public StoneType VisibleCell(Board board, bool[,] visibility, int row, int col)
        {
            if (!Board.IsInsideBoard(row, col))
                throw new System.ArgumentOutOfRangeException(nameof(row));
            return visibility[row - 1, col - 1] ? board.GetCell(row, col) : StoneType.Empty;
        }

        private static void MarkAround(bool[,] visible, int row, int col, int radius)
        {
            for (int visibleRow = row - radius; visibleRow <= row + radius; visibleRow++)
            {
                for (int visibleCol = col - radius; visibleCol <= col + radius; visibleCol++)
                {
                    if (Board.IsInsideBoard(visibleRow, visibleCol))
                        visible[visibleRow - 1, visibleCol - 1] = true;
                }
            }
        }

        private static bool BelongsToPlayer(StoneType cell, int playerNumber)
        {
            return playerNumber == 1
                ? cell == StoneType.OrdinaryP1 || cell == StoneType.HeavyP1
                : cell == StoneType.OrdinaryP2 || cell == StoneType.HeavyP2;
        }
    }
}
