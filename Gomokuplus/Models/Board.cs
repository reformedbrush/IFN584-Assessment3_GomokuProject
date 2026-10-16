using System;

namespace GomokuPlus.Models
{
    // Encapsulates the 10x10 grid: nothing outside this class ever touches
    // the underlying array directly. Row and Col on the public methods are
    // always 1-based (1..10), matching what the player types.
    public class Board
    {
        public const int Size = 10;

        private StoneType[,] cells;

        public Board()
        {
            cells = new StoneType[Size, Size];
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    cells[r, c] = StoneType.Empty;
                }
            }
        }

        public static bool IsInsideBoard(int row, int col)
        {
            if (row >= 1 && row <= Size && col >= 1 && col <= Size)
                return true;
            else
                return false;
        }

        public StoneType GetCell(int row, int col)
        {
            ValidateCoordinates(row, col);
            return cells[row - 1, col - 1];
        }

        public void PlaceStone(int row, int col, StoneType stone)
        {
            ValidateCoordinates(row, col);
            cells[row - 1, col - 1] = stone;
        }

        public void ClearCell(int row, int col)
        {
            ValidateCoordinates(row, col);
            cells[row - 1, col - 1] = StoneType.Empty;
        }

        public bool IsEmpty(int row, int col)
        {
            if (GetCell(row, col) == StoneType.Empty)
                return true;
            else
                return false;
        }

        private void ValidateCoordinates(int row, int col)
        {
            if (!IsInsideBoard(row, col))
                throw new ArgumentOutOfRangeException("row", "(" + row + "," + col + ") is outside the " + Size + "x" + Size + " grid.");
        }

        // --- Persistence helpers -------------------------------------------
        // A jagged int[][] serialises cleanly with System.Text.Json (multi-dim
        // arrays do not serialise as nicely), so this is the shape saved to disk.

        public int[][] ToJaggedArray()
        {
            int[][] result = new int[Size][];
            for (int r = 0; r < Size; r++)
            {
                result[r] = new int[Size];
                for (int c = 0; c < Size; c++)
                {
                    result[r][c] = (int)cells[r, c];
                }
            }
            return result;
        }

        public void LoadFromJaggedArray(int[][] data)
        {
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    cells[r, c] = (StoneType)data[r][c];
                }
            }
        }

        // --- Rendering -------------------------------------------------------

        private char SymbolFor(StoneType stone)
        {
            if (stone == StoneType.Empty)
                return ' ';
            else if (stone == StoneType.OrdinaryP1)
                return 'X';
            else if (stone == StoneType.OrdinaryP2)
                return 'O';
            else if (stone == StoneType.HeavyP1)
                return '@';
            else if (stone == StoneType.HeavyP2)
                return '#';
            else
                return '?';
        }

        // Pads a 1 or 2 digit number out to 2 characters wide, by hand
        // (no PadLeft, since that was not covered in class).
        private string TwoDigits(int number)
        {
            string text = number.ToString();
            if (text.Length < 2)
                return " " + text;
            else
                return text;
        }

        // Each cell is printed "three whitespace characters wide" (" X "),
        // separated by '|' with '+---+' horizontal rules, as required by the spec.
        // Built with plain string concatenation and Console.Write/WriteLine,
        // the same way strings were built up in the workshop exercises.
        public void Render()
        {
            string header = "   ";
            for (int c = 1; c <= Size; c++)
            {
                header = header + TwoDigits(c) + "  ";
            }
            Console.WriteLine(header);

            Console.WriteLine("   " + HorizontalLine());

            for (int r = 1; r <= Size; r++)
            {
                string rowText = TwoDigits(r) + " ";
                for (int c = 1; c <= Size; c++)
                {
                    rowText = rowText + "| " + SymbolFor(cells[r - 1, c - 1]) + " ";
                }
                rowText = rowText + "|";
                Console.WriteLine(rowText);

                Console.WriteLine("   " + HorizontalLine());
            }
        }

        private string HorizontalLine()
        {
            string line = "";
            for (int c = 0; c < Size; c++)
            {
                line = line + "+---";
            }
            line = line + "+";
            return line;
        }
    }
}
