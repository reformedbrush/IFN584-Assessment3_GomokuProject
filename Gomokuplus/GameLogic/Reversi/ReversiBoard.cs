using System;
using System.Collections.Generic;

namespace GomokuPlus.GameLogic.Reversi
{
    public class ReversiBoard
    {
        public const int Size = 8;
        private readonly ReversiCell[,] cells = new ReversiCell[Size, Size];

        public ReversiBoard()
        {
            Reset();
        }

        public ReversiCell GetCell(int row, int col)
        {
            Validate(row, col);
            return cells[row - 1, col - 1];
        }

        public void SetCell(int row, int col, ReversiCell cell)
        {
            Validate(row, col);
            cells[row - 1, col - 1] = cell;
        }

        public bool IsEmpty(int row, int col)
        {
            return GetCell(row, col) == ReversiCell.Empty;
        }

        public int Count(ReversiCell cell)
        {
            int count = 0;
            for (int row = 1; row <= Size; row++)
            {
                for (int col = 1; col <= Size; col++)
                {
                    if (GetCell(row, col) == cell)
                        count++;
                }
            }
            return count;
        }

        public ReversiBoard Clone()
        {
            ReversiBoard copy = new ReversiBoard();
            for (int row = 1; row <= Size; row++)
                for (int col = 1; col <= Size; col++)
                    copy.SetCell(row, col, GetCell(row, col));
            return copy;
        }

        public void Reset()
        {
            for (int row = 1; row <= Size; row++)
                for (int col = 1; col <= Size; col++)
                    cells[row - 1, col - 1] = ReversiCell.Empty;

            SetCell(4, 4, ReversiCell.Player1);
            SetCell(5, 5, ReversiCell.Player1);
            SetCell(4, 5, ReversiCell.Player2);
            SetCell(5, 4, ReversiCell.Player2);
        }

        public static bool IsInside(int row, int col)
        {
            return row >= 1 && row <= Size && col >= 1 && col <= Size;
        }

        private static void Validate(int row, int col)
        {
            if (!IsInside(row, col))
                throw new ArgumentOutOfRangeException(nameof(row), "Reversi coordinates must be between 1 and 8.");
        }
    }
}
