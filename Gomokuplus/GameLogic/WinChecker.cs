using GomokuPlus.Models;

namespace GomokuPlus.GameLogic
{
    // Checks the five-in-a-row win condition around a just-placed stone.
    // Both Ordinary and Heavy stones of a player count toward its alignments.
    public class WinChecker
    {
        private const int WinLength = 5;

        public static bool CheckWin(Board board, int row, int col, int playerNumber)
        {
            int[] rowSteps = { 0, 1, 1, 1 };
            int[] colSteps = { 1, 0, 1, -1 };

            for (int i = 0; i < rowSteps.Length; i++)
            {
                int dr = rowSteps[i];
                int dc = colSteps[i];

                int count = 1;
                count = count + CountDirection(board, row, col, dr, dc, playerNumber);
                count = count + CountDirection(board, row, col, -dr, -dc, playerNumber);

                if (count >= WinLength)
                    return true;
            }

            return false;
        }

        private static int CountDirection(Board board, int row, int col, int dr, int dc, int playerNumber)
        {
            int count = 0;
            int r = row + dr;
            int c = col + dc;

            while (Board.IsInsideBoard(r, c) && BelongsToPlayer(board.GetCell(r, c), playerNumber))
            {
                count++;
                r = r + dr;
                c = c + dc;
            }

            return count;
        }

        private static bool BelongsToPlayer(StoneType stone, int playerNumber)
        {
            if (playerNumber == 1)
            {
                if (stone == StoneType.OrdinaryP1 || stone == StoneType.HeavyP1)
                    return true;
                else
                    return false;
            }
            else
            {
                if (stone == StoneType.OrdinaryP2 || stone == StoneType.HeavyP2)
                    return true;
                else
                    return false;
            }
        }
    }
}
