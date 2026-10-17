using System.Collections.Generic;
using BoardGameFramework.Board;
using BoardGameFramework.Commands;
using BoardGameFramework.Core;
using BoardGameFramework.Exceptions;
using BoardGameFramework.Parsing;
using BoardGameFramework.Players;

namespace BoardGameFramework.Rules
{
    public class ReversiMoveRules : IMoveRules
    {
        private static readonly int[] RowDelta = { -1, -1, -1, 0, 0, 1, 1, 1 };
        private static readonly int[] ColDelta = { -1, 0, 1, -1, 1, -1, 0, 1 };

        public bool AllowsPass
        {
            get { return true; }
        }

        public List<LegalMove> ListLegalMoves(GridBoard board, Player player, int opponentNumber)
        {
            List<LegalMove> moves = new List<LegalMove>();
            for (int r = 1; r <= board.Size; r++)
            {
                for (int c = 1; c <= board.Size; c++)
                {
                    List<int> flipRows;
                    List<int> flipCols;
                    if (TryCollectFlips(board, player.PlayerNumber, r, c, out flipRows, out flipCols))
                        moves.Add(new LegalMove(InputKind.PlaceDisk, r, c));
                }
            }
            return moves;
        }

        public IMoveCommand CreateCommand(GridBoard board, Player player, int opponentNumber, ParsedInput input)
        {
            if (input.Kind == InputKind.Pass)
            {
                if (ListLegalMoves(board, player, opponentNumber).Count > 0)
                    throw new InvalidMoveException("PASS is only allowed when you have no legal placement.");
                return new PassCommand(player.PlayerNumber);
            }

            if (input.Kind != InputKind.PlaceDisk)
                throw new InvalidMoveException("Reversi placements use P[row]:[col], e.g. P3:4.");

            List<int> flipRows;
            List<int> flipCols;
            if (!TryCollectFlips(board, player.PlayerNumber, input.Row, input.Col, out flipRows, out flipCols))
                throw new InvalidMoveException("P" + input.Row + ":" + input.Col + " does not flank any opponent disks.");

            return new PlaceDiskCommand(board, player.PlayerNumber, input.Row, input.Col, flipRows, flipCols);
        }

        public static bool TryCollectFlips(GridBoard board, int playerNumber, int row, int col, out List<int> flipRows, out List<int> flipCols)
        {
            flipRows = new List<int>();
            flipCols = new List<int>();

            if (!board.IsInside(row, col) || !board.IsEmpty(row, col))
                return false;

            int opponent = playerNumber == 1 ? 2 : 1;

            for (int d = 0; d < 8; d++)
            {
                int r = row + RowDelta[d];
                int c = col + ColDelta[d];
                List<int> lineRows = new List<int>();
                List<int> lineCols = new List<int>();

                if (!board.IsInside(r, c) || board.OwnerAt(r, c) != opponent)
                    continue;

                while (board.IsInside(r, c) && board.OwnerAt(r, c) == opponent)
                {
                    lineRows.Add(r);
                    lineCols.Add(c);
                    r = r + RowDelta[d];
                    c = c + ColDelta[d];
                }

                if (board.IsInside(r, c) && board.OwnerAt(r, c) == playerNumber && lineRows.Count > 0)
                {
                    for (int i = 0; i < lineRows.Count; i++)
                    {
                        flipRows.Add(lineRows[i]);
                        flipCols.Add(lineCols[i]);
                    }
                }
            }

            return flipRows.Count > 0;
        }
    }

    public class ReversiMajorityWinStrategy : IWinStrategy
    {
        public GameOutcome Evaluate(GridBoard board, int lastPlayer, int lastRow, int lastCol, bool bothCannotMove)
        {
            if (!bothCannotMove && board.EmptyCount() > 0)
                return GameOutcome.Continue();
            return MajorityResult(board, "majority disk count");
        }

        public static GameOutcome MajorityResult(GridBoard board, string reason)
        {
            int p1 = board.CountPieces(1);
            int p2 = board.CountPieces(2);
            if (p1 > p2)
                return GameOutcome.Win(1, "P1 has " + p1 + " disks vs P2 " + p2 + " (" + reason + ")");
            if (p2 > p1)
                return GameOutcome.Win(2, "P2 has " + p2 + " disks vs P1 " + p1 + " (" + reason + ")");
            return GameOutcome.Draw("Both players have " + p1 + " disks.");
        }
    }

    public class AntiReversiWinStrategy : IWinStrategy
    {
        public GameOutcome Evaluate(GridBoard board, int lastPlayer, int lastRow, int lastCol, bool bothCannotMove)
        {
            if (!bothCannotMove && board.EmptyCount() > 0)
                return GameOutcome.Continue();

            int p1 = board.CountPieces(1);
            int p2 = board.CountPieces(2);
            if (p1 < p2)
                return GameOutcome.Win(1, "P1 has the fewest disks (" + p1 + " vs " + p2 + ")");
            if (p2 < p1)
                return GameOutcome.Win(2, "P2 has the fewest disks (" + p2 + " vs " + p1 + ")");
            return GameOutcome.Draw("Both players have " + p1 + " disks.");
        }
    }

    public class CornerReversiWinStrategy : IWinStrategy
    {
        public GameOutcome Evaluate(GridBoard board, int lastPlayer, int lastRow, int lastCol, bool bothCannotMove)
        {
            int p1Corners = CountCorners(board, 1);
            int p2Corners = CountCorners(board, 2);
            if (p1Corners >= 3)
                return GameOutcome.Win(1, "P1 captured " + p1Corners + " corners (sudden-death)");
            if (p2Corners >= 3)
                return GameOutcome.Win(2, "P2 captured " + p2Corners + " corners (sudden-death)");

            if (!bothCannotMove && board.EmptyCount() > 0)
                return GameOutcome.Continue();

            return ReversiMajorityWinStrategy.MajorityResult(board, "fallback majority");
        }

        public static int CountCorners(GridBoard board, int playerNumber)
        {
            int size = board.Size;
            int count = 0;
            if (board.OwnerAt(1, 1) == playerNumber) count++;
            if (board.OwnerAt(1, size) == playerNumber) count++;
            if (board.OwnerAt(size, 1) == playerNumber) count++;
            if (board.OwnerAt(size, size) == playerNumber) count++;
            return count;
        }
    }
}
