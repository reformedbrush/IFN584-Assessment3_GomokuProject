using System.Collections.Generic;
using GomokuPlus.GameLogic;
using GomokuPlus.Models;

namespace GomokuPlus.Players
{
    public class HardAiStrategy : IAiStrategy
    {
        public AiLevel Level
        {
            get { return AiLevel.Hard; }
        }

        public Move ChooseMove(Board board, Player player, int opponentNumber)
        {
            List<Move> candidates = EasyAiStrategy.LegalMoves(board, player, opponentNumber);
            Move firstOrdinary = null;
            Move firstHeavy = null;
            Move firstEraser = null;

            foreach (Move candidate in candidates)
            {
                if (candidate.Type == MoveType.Eraser && firstEraser == null)
                    firstEraser = candidate;
                else if (candidate.Type == MoveType.Heavy && firstHeavy == null)
                    firstHeavy = candidate;
                else if (candidate.Type == MoveType.Ordinary && firstOrdinary == null)
                    firstOrdinary = candidate;

                if (candidate.Type != MoveType.Eraser)
                {
                    board.PlaceStone(candidate.Row, candidate.Col, player.OrdinarySymbol);
                    bool wins = WinChecker.CheckWin(board, candidate.Row, candidate.Col, player.PlayerNumber);
                    board.ClearCell(candidate.Row, candidate.Col);
                    if (wins)
                        return candidate;
                }
            }

            foreach (Move candidate in candidates)
            {
                if (candidate.Type != MoveType.Eraser)
                {
                    board.PlaceStone(candidate.Row, candidate.Col,
                        opponentNumber == 1 ? StoneType.OrdinaryP1 : StoneType.OrdinaryP2);
                    bool wins = WinChecker.CheckWin(board, candidate.Row, candidate.Col, opponentNumber);
                    board.ClearCell(candidate.Row, candidate.Col);
                    if (wins)
                        return candidate;
                }
            }

            if (firstEraser != null)
                return firstEraser;
            if (firstHeavy != null)
                return firstHeavy;
            if (firstOrdinary != null)
                return firstOrdinary;

            return new Move(MoveType.Ordinary, 1, 1);
        }
    }
}
