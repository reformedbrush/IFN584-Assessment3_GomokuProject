using System;
using System.Collections.Generic;
using GomokuPlus.Models;

namespace GomokuPlus.Players
{
    // A simple but always-legal AI: it randomly chooses between an Ordinary
    // move, a Heavy move (if any left), or an Eraser move (if a legal target
    // exists), then picks a random legal cell for that move type.
    public class ComputerPlayer : Player
    {
        private Random randomGenerator = new Random();

        public ComputerPlayer(string name, int playerNumber) : base(name, playerNumber)
        {
        }

        public override Move GetMove(Board board, int opponentNumber)
        {
            List<MoveType> candidateTypes = new List<MoveType>();
            candidateTypes.Add(MoveType.Ordinary);

            if (HeavyStonesRemaining > 0)
                candidateTypes.Add(MoveType.Heavy);

            bool hasEraserTarget = FindRandomOpponentOrdinaryStone(board, opponentNumber, out int targetRow, out int targetCol);
            if (EraserStonesRemaining > 0 && hasEraserTarget)
                candidateTypes.Add(MoveType.Eraser);

            int choice = randomGenerator.Next(candidateTypes.Count);
            MoveType chosenType = candidateTypes[choice];

            if (chosenType == MoveType.Eraser)
            {
                if (hasEraserTarget)
                    return new Move(MoveType.Eraser, targetRow, targetCol);
                else
                    chosenType = MoveType.Ordinary;
            }

            int cellRow;
            int cellCol;
            FindRandomEmptyCell(board, out cellRow, out cellCol);
            return new Move(chosenType, cellRow, cellCol);
        }

        private bool FindRandomOpponentOrdinaryStone(Board board, int opponentNumber, out int foundRow, out int foundCol)
        {
            StoneType opponentOrdinary;
            if (opponentNumber == 1)
                opponentOrdinary = StoneType.OrdinaryP1;
            else
                opponentOrdinary = StoneType.OrdinaryP2;

            List<int> matchRows = new List<int>();
            List<int> matchCols = new List<int>();

            for (int r = 1; r <= Board.Size; r++)
            {
                for (int c = 1; c <= Board.Size; c++)
                {
                    if (board.GetCell(r, c) == opponentOrdinary)
                    {
                        matchRows.Add(r);
                        matchCols.Add(c);
                    }
                }
            }

            if (matchRows.Count == 0)
            {
                foundRow = 0;
                foundCol = 0;
                return false;
            }

            int index = randomGenerator.Next(matchRows.Count);
            foundRow = matchRows[index];
            foundCol = matchCols[index];
            return true;
        }

        private void FindRandomEmptyCell(Board board, out int foundRow, out int foundCol)
        {
            List<int> emptyRows = new List<int>();
            List<int> emptyCols = new List<int>();

            for (int r = 1; r <= Board.Size; r++)
            {
                for (int c = 1; c <= Board.Size; c++)
                {
                    if (board.IsEmpty(r, c))
                    {
                        emptyRows.Add(r);
                        emptyCols.Add(c);
                    }
                }
            }

            if (emptyRows.Count == 0)
            {
                // Board full: fall back to (1,1); GomokuGame will surface the
                // resulting "cell occupied" error, which effectively ends the game.
                foundRow = 1;
                foundCol = 1;
                return;
            }

            int index = randomGenerator.Next(emptyRows.Count);
            foundRow = emptyRows[index];
            foundCol = emptyCols[index];
        }
    }
}
