using System;
using System.Collections.Generic;
using GomokuPlus.Models;

namespace GomokuPlus.Players
{
    public class EasyAiStrategy : IAiStrategy
    {
        private readonly Random random;

        public EasyAiStrategy(int seed = 12345)
        {
            random = new Random(seed);
        }

        public AiLevel Level
        {
            get { return AiLevel.Easy; }
        }

        public Move ChooseMove(Board board, Player player, int opponentNumber)
        {
            List<Move> moves = LegalMoves(board, player, opponentNumber);
            if (moves.Count == 0)
                return new Move(MoveType.Ordinary, 1, 1);

            return moves[random.Next(moves.Count)];
        }

        public static List<Move> LegalMoves(Board board, Player player, int opponentNumber)
        {
            List<Move> moves = new List<Move>();

            for (int row = 1; row <= Board.Size; row++)
            {
                for (int col = 1; col <= Board.Size; col++)
                {
                    if (board.IsEmpty(row, col))
                    {
                        moves.Add(new Move(MoveType.Ordinary, row, col));
                        if (player.HeavyStonesRemaining > 0)
                            moves.Add(new Move(MoveType.Heavy, row, col));
                    }
                    else if (player.EraserStonesRemaining > 0 &&
                        board.GetCell(row, col) == (opponentNumber == 1 ? StoneType.OrdinaryP1 : StoneType.OrdinaryP2))
                    {
                        moves.Add(new Move(MoveType.Eraser, row, col));
                    }
                }
            }

            return moves;
        }
    }
}
