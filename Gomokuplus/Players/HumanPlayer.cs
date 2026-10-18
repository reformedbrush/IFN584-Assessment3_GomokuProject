using System;
using GomokuPlus.GameLogic;
using GomokuPlus.Models;
using GomokuPlus.UI;

namespace GomokuPlus.Players
{
    public class HumanPlayer : Player
    {
        public HumanPlayer(string name, int playerNumber) : base(name, playerNumber)
        {
        }

        public override Move GetMove(Board board, int opponentNumber)
        {
            while (true)
            {
                Console.Write("\n" + Name + " (Player " + PlayerNumber + ") - [Heavy left: " + HeavyStonesRemaining +
                    ", Eraser left: " + EraserStonesRemaining + "] enter move (O/H/E[Row]:[Col], S=save, ?=help, Q=quit) >> ");

                string input = Console.ReadLine();
                if (input == null)
                    continue;

                if (input == "?")
                {
                    HelpMenu.Show();
                    continue;
                }

                if (input == "S" || input == "s")
                    return new Move(MoveType.Save, 0, 0);

                if (input == "U" || input == "u")
                    return new Move(MoveType.Undo, 0, 0);

                if (input == "R" || input == "r")
                    return new Move(MoveType.Redo, 0, 0);

                if (input == "Q" || input == "q")
                    return new Move(MoveType.Quit, 0, 0);

                try
                {
                    return MoveParser.Parse(input);
                }
                catch (Exception ex)
                {
                    // Bad command: ask the same player again, the turn is not lost.
                    Console.WriteLine("Invalid input: " + ex.Message);
                }
            }
        }
    }
}
