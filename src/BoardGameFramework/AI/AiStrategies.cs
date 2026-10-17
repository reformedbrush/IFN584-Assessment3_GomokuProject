using System;
using System.Collections.Generic;
using BoardGameFramework.Board;
using BoardGameFramework.Commands;
using BoardGameFramework.Core;
using BoardGameFramework.Parsing;
using BoardGameFramework.Players;
using BoardGameFramework.Rules;

namespace BoardGameFramework.AI
{
    public class DumbAi : IAiStrategy
    {
        private Random randomGenerator = new Random();

        public ParsedInput ChooseMove(GridBoard board, Player player, int opponentNumber, IMoveRules rules)
        {
            List<LegalMove> legal = rules.ListLegalMoves(board, player, opponentNumber);
            ParsedInput parsed = new ParsedInput();
            if (legal.Count == 0)
            {
                parsed.Kind = InputKind.Pass;
                parsed.Raw = "PASS";
                return parsed;
            }

            LegalMove pick = legal[randomGenerator.Next(legal.Count)];
            parsed.Kind = pick.Kind;
            parsed.Row = pick.Row;
            parsed.Col = pick.Col;
            parsed.Raw = "AI";
            return parsed;
        }
    }

    public class GomokuSmartAi : IAiStrategy
    {
        private DumbAi fallback = new DumbAi();

        public ParsedInput ChooseMove(GridBoard board, Player player, int opponentNumber, IMoveRules rules)
        {
            List<LegalMove> legal = rules.ListLegalMoves(board, player, opponentNumber);
            ParsedInput win = FindWinningOrdinary(legal, board, player.PlayerNumber);
            if (win != null)
                return win;

            ParsedInput block = FindWinningOrdinary(legal, board, opponentNumber);
            if (block != null)
                return block;

            return fallback.ChooseMove(board, player, opponentNumber, rules);
        }

        private static ParsedInput FindWinningOrdinary(List<LegalMove> legal, GridBoard board, int playerNumber)
        {
            for (int i = 0; i < legal.Count; i++)
            {
                LegalMove move = legal[i];
                if (move.Kind != InputKind.PlaceOrdinary && move.Kind != InputKind.PlaceHeavy)
                    continue;
                if (GomokuWinStrategy.WouldWin(board, move.Row, move.Col, playerNumber))
                {
                    ParsedInput parsed = new ParsedInput();
                    parsed.Kind = InputKind.PlaceOrdinary;
                    parsed.Row = move.Row;
                    parsed.Col = move.Col;
                    parsed.Raw = "AI-win-or-block";
                    return parsed;
                }
            }
            return null;
        }
    }

    public class ReversiSmartAi : IAiStrategy
    {
        private bool minimise;
        private bool favourCorners;
        private bool avoidCorners;

        public ReversiSmartAi(bool minimise, bool favourCorners, bool avoidCorners)
        {
            this.minimise = minimise;
            this.favourCorners = favourCorners;
            this.avoidCorners = avoidCorners;
        }

        public ParsedInput ChooseMove(GridBoard board, Player player, int opponentNumber, IMoveRules rules)
        {
            List<LegalMove> legal = rules.ListLegalMoves(board, player, opponentNumber);
            ParsedInput parsed = new ParsedInput();
            if (legal.Count == 0)
            {
                parsed.Kind = InputKind.Pass;
                parsed.Raw = "PASS";
                return parsed;
            }

            int bestIndex = 0;
            int bestScore = ScoreMove(board, player.PlayerNumber, legal[0]);

            for (int i = 1; i < legal.Count; i++)
            {
                int score = ScoreMove(board, player.PlayerNumber, legal[i]);
                bool better = minimise ? score < bestScore : score > bestScore;
                if (better)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }

            LegalMove pick = legal[bestIndex];
            parsed.Kind = pick.Kind;
            parsed.Row = pick.Row;
            parsed.Col = pick.Col;
            parsed.Raw = "AI-greedy";
            return parsed;
        }

        private int ScoreMove(GridBoard board, int playerNumber, LegalMove move)
        {
            List<int> flipRows;
            List<int> flipCols;
            ReversiMoveRules.TryCollectFlips(board, playerNumber, move.Row, move.Col, out flipRows, out flipCols);
            int score = flipRows.Count;

            bool corner = PlaceDiskCommand.IsCorner(move.Row, move.Col, board.Size);
            if (favourCorners && corner)
                score = score + 100;
            if (avoidCorners && corner)
                score = score + 100; // with minimise, a large score is worse

            if (favourCorners)
            {
                int opponent = playerNumber == 1 ? 2 : 1;
                if (WouldGiveOpponentThirdCorner(board, playerNumber, move, opponent))
                    score = score - 80;
            }

            return score;
        }

        private bool WouldGiveOpponentThirdCorner(GridBoard board, int playerNumber, LegalMove move, int opponent)
        {
            GridBoard clone = board.Clone();
            List<int> flipRows;
            List<int> flipCols;
            if (!ReversiMoveRules.TryCollectFlips(clone, playerNumber, move.Row, move.Col, out flipRows, out flipCols))
                return false;
            PlaceDiskCommand command = new PlaceDiskCommand(clone, playerNumber, move.Row, move.Col, flipRows, flipCols);
            command.Execute();
            if (CornerReversiWinStrategy.CountCorners(clone, playerNumber) >= 3)
                return false;

            for (int r = 1; r <= clone.Size; r++)
            {
                for (int c = 1; c <= clone.Size; c++)
                {
                    List<int> oppRows;
                    List<int> oppCols;
                    if (ReversiMoveRules.TryCollectFlips(clone, opponent, r, c, out oppRows, out oppCols))
                    {
                        if (PlaceDiskCommand.IsCorner(r, c, clone.Size))
                        {
                            GridBoard next = clone.Clone();
                            new PlaceDiskCommand(next, opponent, r, c, oppRows, oppCols).Execute();
                            if (CornerReversiWinStrategy.CountCorners(next, opponent) >= 3)
                                return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
