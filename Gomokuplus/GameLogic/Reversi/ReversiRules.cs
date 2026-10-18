using System;
using System.Collections.Generic;

namespace GomokuPlus.GameLogic.Reversi
{
    public class ReversiRules
    {
        private static readonly int[] RowSteps = { -1, -1, -1, 0, 0, 1, 1, 1 };
        private static readonly int[] ColSteps = { -1, 0, 1, -1, 1, -1, 0, 1 };

        public virtual bool PlayerWithLowestScoreWins
        {
            get { return false; }
        }

        public virtual IReadOnlyList<ReversiMove> LegalMoves(ReversiBoard board, ReversiCell player)
        {
            List<ReversiMove> moves = new List<ReversiMove>();
            for (int row = 1; row <= ReversiBoard.Size; row++)
                for (int col = 1; col <= ReversiBoard.Size; col++)
                    if (Captures(board, row, col, player).Count > 0)
                        moves.Add(new ReversiMove(row, col));
            return moves;
        }

        public virtual bool CanPass(ReversiBoard board, ReversiCell player)
        {
            return LegalMoves(board, player).Count == 0 && LegalMoves(board, Opponent(player)).Count > 0;
        }

        public virtual bool IsFinished(ReversiBoard board)
        {
            return LegalMoves(board, ReversiCell.Player1).Count == 0 &&
                LegalMoves(board, ReversiCell.Player2).Count == 0;
        }

        public virtual void Apply(ReversiBoard board, ReversiMove move, ReversiCell player)
        {
            if (move == null || move.IsPass)
            {
                if (!CanPass(board, player))
                    throw new InvalidOperationException("A pass is only legal when the current player has no legal move.");
                return;
            }

            List<Tuple<int, int>> captured = Captures(board, move.Row, move.Col, player);
            if (captured.Count == 0)
                throw new InvalidOperationException("The Reversi move must be empty and capture at least one opponent disc.");

            board.SetCell(move.Row, move.Col, player);
            foreach (Tuple<int, int> cell in captured)
                board.SetCell(cell.Item1, cell.Item2, player);
        }

        public virtual ReversiCell Winner(ReversiBoard board)
        {
            if (!IsFinished(board))
                return ReversiCell.Empty;

            int player1Score = board.Count(ReversiCell.Player1);
            int player2Score = board.Count(ReversiCell.Player2);
            if (player1Score == player2Score)
                return ReversiCell.Empty;

            bool player1Wins = PlayerWithLowestScoreWins
                ? player1Score < player2Score
                : player1Score > player2Score;
            return player1Wins ? ReversiCell.Player1 : ReversiCell.Player2;
        }

        private static List<Tuple<int, int>> Captures(ReversiBoard board, int row, int col, ReversiCell player)
        {
            List<Tuple<int, int>> result = new List<Tuple<int, int>>();
            if (!ReversiBoard.IsInside(row, col) || !board.IsEmpty(row, col))
                return result;

            ReversiCell opponent = Opponent(player);
            for (int direction = 0; direction < RowSteps.Length; direction++)
            {
                int currentRow = row + RowSteps[direction];
                int currentCol = col + ColSteps[direction];
                List<Tuple<int, int>> line = new List<Tuple<int, int>>();

                while (ReversiBoard.IsInside(currentRow, currentCol) &&
                    board.GetCell(currentRow, currentCol) == opponent)
                {
                    line.Add(Tuple.Create(currentRow, currentCol));
                    currentRow += RowSteps[direction];
                    currentCol += ColSteps[direction];
                }

                if (line.Count > 0 && ReversiBoard.IsInside(currentRow, currentCol) &&
                    board.GetCell(currentRow, currentCol) == player)
                    result.AddRange(line);
            }

            return result;
        }

        private static ReversiCell Opponent(ReversiCell player)
        {
            return player == ReversiCell.Player1 ? ReversiCell.Player2 : ReversiCell.Player1;
        }
    }
}
