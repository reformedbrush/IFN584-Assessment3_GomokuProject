using System;
using BoardGameFramework.Core;

namespace BoardGameFramework.Board
{
    public class GridBoard
    {
        private PieceKind[,] cells;

        public int Size { get; private set; }

        public GridBoard(int size)
        {
            Size = size;
            cells = new PieceKind[size, size];
            Clear();
        }

        public void Clear()
        {
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                    cells[r, c] = PieceKind.Empty;
            }
        }

        public bool IsInside(int row, int col)
        {
            return row >= 1 && row <= Size && col >= 1 && col <= Size;
        }

        public PieceKind GetCell(int row, int col)
        {
            EnsureInside(row, col);
            return cells[row - 1, col - 1];
        }

        public void SetCell(int row, int col, PieceKind piece)
        {
            EnsureInside(row, col);
            cells[row - 1, col - 1] = piece;
        }

        public bool IsEmpty(int row, int col)
        {
            return GetCell(row, col) == PieceKind.Empty;
        }

        public int OwnerOf(PieceKind piece)
        {
            if (piece == PieceKind.OrdinaryP1 || piece == PieceKind.HeavyP1)
                return 1;
            if (piece == PieceKind.OrdinaryP2 || piece == PieceKind.HeavyP2)
                return 2;
            return 0;
        }

        public int OwnerAt(int row, int col)
        {
            return OwnerOf(GetCell(row, col));
        }

        public bool BelongsTo(int row, int col, int playerNumber)
        {
            return OwnerAt(row, col) == playerNumber;
        }

        public int CountPieces(int playerNumber)
        {
            int count = 0;
            for (int r = 1; r <= Size; r++)
            {
                for (int c = 1; c <= Size; c++)
                {
                    if (OwnerAt(r, c) == playerNumber)
                        count++;
                }
            }
            return count;
        }

        public int EmptyCount()
        {
            int count = 0;
            for (int r = 1; r <= Size; r++)
            {
                for (int c = 1; c <= Size; c++)
                {
                    if (IsEmpty(r, c))
                        count++;
                }
            }
            return count;
        }

        public int[][] ToJaggedArray()
        {
            int[][] result = new int[Size][];
            for (int r = 0; r < Size; r++)
            {
                result[r] = new int[Size];
                for (int c = 0; c < Size; c++)
                    result[r][c] = (int)cells[r, c];
            }
            return result;
        }

        public void LoadFromJaggedArray(int[][] data)
        {
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                    cells[r, c] = (PieceKind)data[r][c];
            }
        }

        public GridBoard Clone()
        {
            GridBoard copy = new GridBoard(Size);
            copy.LoadFromJaggedArray(ToJaggedArray());
            return copy;
        }

        public static char SymbolFor(PieceKind piece)
        {
            if (piece == PieceKind.OrdinaryP1)
                return 'X';
            if (piece == PieceKind.OrdinaryP2)
                return 'O';
            if (piece == PieceKind.HeavyP1)
                return '@';
            if (piece == PieceKind.HeavyP2)
                return '#';
            return ' ';
        }

        public static PieceKind OrdinaryFor(int playerNumber)
        {
            if (playerNumber == 1)
                return PieceKind.OrdinaryP1;
            return PieceKind.OrdinaryP2;
        }

        public static PieceKind HeavyFor(int playerNumber)
        {
            if (playerNumber == 1)
                return PieceKind.HeavyP1;
            return PieceKind.HeavyP2;
        }

        private void EnsureInside(int row, int col)
        {
            if (!IsInside(row, col))
                throw new ArgumentOutOfRangeException("row", "(" + row + "," + col + ") is outside the " + Size + "x" + Size + " grid.");
        }
    }
}
