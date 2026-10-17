using System.Collections.Generic;
using BoardGameFramework.Board;
using BoardGameFramework.Commands;
using BoardGameFramework.Core;
using BoardGameFramework.Exceptions;
using BoardGameFramework.Parsing;
using BoardGameFramework.Players;

namespace BoardGameFramework.Rules
{
    public class GomokuWinStrategy : IWinStrategy
    {
        private const int WinLength = 5;

        public GameOutcome Evaluate(GridBoard board, int lastPlayer, int lastRow, int lastCol, bool bothCannotMove)
        {
            int winner = FindFiveWinner(board);
            if (winner != 0)
                return GameOutcome.Win(winner, "5-in-a-row");

            if (board.EmptyCount() == 0)
                return GameOutcome.Draw("The board is full with no 5-in-a-row.");

            return GameOutcome.Continue();
        }

        public static int FindFiveWinner(GridBoard board)
        {
            for (int r = 1; r <= board.Size; r++)
            {
                for (int c = 1; c <= board.Size; c++)
                {
                    int owner = board.OwnerAt(r, c);
                    if (owner != 0 && HasFive(board, r, c, owner))
                        return owner;
                }
            }
            return 0;
        }

        public static bool HasFive(GridBoard board, int row, int col, int playerNumber)
        {
            int[] rowSteps = { 0, 1, 1, 1 };
            int[] colSteps = { 1, 0, 1, -1 };

            for (int i = 0; i < rowSteps.Length; i++)
            {
                int count = 1;
                count = count + CountDirection(board, row, col, rowSteps[i], colSteps[i], playerNumber);
                count = count + CountDirection(board, row, col, -rowSteps[i], -colSteps[i], playerNumber);
                if (count >= WinLength)
                    return true;
            }
            return false;
        }

        public static bool WouldWin(GridBoard board, int row, int col, int playerNumber)
        {
            if (!board.IsInside(row, col) || !board.IsEmpty(row, col))
                return false;
            PieceKind previous = board.GetCell(row, col);
            board.SetCell(row, col, GridBoard.OrdinaryFor(playerNumber));
            bool win = HasFive(board, row, col, playerNumber);
            board.SetCell(row, col, previous);
            return win;
        }

        private static int CountDirection(GridBoard board, int row, int col, int dr, int dc, int playerNumber)
        {
            int count = 0;
            int r = row + dr;
            int c = col + dc;
            while (board.IsInside(r, c) && board.BelongsTo(r, c, playerNumber))
            {
                count++;
                r = r + dr;
                c = c + dc;
            }
            return count;
        }
    }

    public class GomokuMoveRules : IMoveRules
    {
        private bool specialStones;

        public GomokuMoveRules(bool specialStones)
        {
            this.specialStones = specialStones;
        }

        public bool AllowsPass
        {
            get { return false; }
        }

        public List<LegalMove> ListLegalMoves(GridBoard board, Player player, int opponentNumber)
        {
            List<LegalMove> moves = new List<LegalMove>();
            for (int r = 1; r <= board.Size; r++)
            {
                for (int c = 1; c <= board.Size; c++)
                {
                    if (board.IsEmpty(r, c))
                    {
                        moves.Add(new LegalMove(InputKind.PlaceOrdinary, r, c));
                        if (specialStones && player.Inventory.HeavyRemaining > 0)
                            moves.Add(new LegalMove(InputKind.PlaceHeavy, r, c));
                    }
                    else if (specialStones && player.Inventory.EraserRemaining > 0 && IsErasable(board, r, c, opponentNumber))
                    {
                        moves.Add(new LegalMove(InputKind.PlaceEraser, r, c));
                    }
                }
            }
            return moves;
        }

        public IMoveCommand CreateCommand(GridBoard board, Player player, int opponentNumber, ParsedInput input)
        {
            if (input.Kind == InputKind.PlaceOrdinary)
            {
                EnsureEmpty(board, input.Row, input.Col);
                return new PlaceStoneCommand(board, player.Inventory, player.PlayerNumber, input.Row, input.Col, GridBoard.OrdinaryFor(player.PlayerNumber), false);
            }

            if (input.Kind == InputKind.PlaceHeavy)
            {
                if (!specialStones)
                    throw new InvalidMoveException("Heavy stones are not used in this variant.");
                if (player.Inventory.HeavyRemaining <= 0)
                    throw new InvalidMoveException(player.Name + " has no Heavy Stones left.");
                EnsureEmpty(board, input.Row, input.Col);
                return new PlaceStoneCommand(board, player.Inventory, player.PlayerNumber, input.Row, input.Col, GridBoard.HeavyFor(player.PlayerNumber), true);
            }

            if (input.Kind == InputKind.PlaceEraser)
            {
                if (!specialStones)
                    throw new InvalidMoveException("Eraser stones are not used in this variant.");
                if (player.Inventory.EraserRemaining <= 0)
                    throw new InvalidMoveException(player.Name + " has no Eraser Stones left.");

                PieceKind target = board.GetCell(input.Row, input.Col);
                if (target == PieceKind.Empty)
                    throw new InvalidMoveException("Cell (" + input.Row + "," + input.Col + ") is empty; there is nothing to erase.");
                if (!IsErasable(board, input.Row, input.Col, opponentNumber))
                {
                    if (board.OwnerAt(input.Row, input.Col) == opponentNumber && (target == PieceKind.HeavyP1 || target == PieceKind.HeavyP2))
                        throw new InvalidMoveException("Heavy Stones cannot be targeted or removed by an Eraser Stone.");
                    throw new InvalidMoveException("The Eraser Stone must target an opponent's Ordinary Stone.");
                }
                return new EraseStoneCommand(board, player.Inventory, player.PlayerNumber, input.Row, input.Col, target);
            }

            if (input.Kind == InputKind.PlaceDisk || input.Kind == InputKind.Pass)
                throw new InvalidMoveException("That command is not valid in Gomoku.");

            throw new InvalidMoveException("Unknown move type.");
        }

        private static void EnsureEmpty(GridBoard board, int row, int col)
        {
            if (!board.IsInside(row, col))
                throw new InvalidMoveException("(" + row + "," + col + ") is outside the " + board.Size + "x" + board.Size + " grid.");
            if (!board.IsEmpty(row, col))
                throw new InvalidMoveException("Cell (" + row + "," + col + ") is already occupied.");
        }

        private static bool IsErasable(GridBoard board, int row, int col, int opponentNumber)
        {
            PieceKind expected = GridBoard.OrdinaryFor(opponentNumber);
            return board.GetCell(row, col) == expected;
        }
    }
}
