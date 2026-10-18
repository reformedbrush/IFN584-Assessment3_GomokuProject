using System;
using GomokuPlus.Framework;

namespace GomokuPlus.UI
{
    // Built-in help system. Any human player can bring this up mid-game
    // by typing "?".
    public class HelpMenu
    {
        public static void Show()
        {
            Show(new GomokuPlus.GameLogic.GomokuPlusRules());
        }

        public static void Show(IGameRules rules)
        {
            Console.WriteLine();
            Console.WriteLine("========================== " + rules.VariantName + " Help ==========================");
            Console.WriteLine("Move syntax: [Type][Row]:[Col]   (rows and columns run 1 to 10)");
            Console.WriteLine("  O[Row]:[Col]  Place an Ordinary Stone            e.g. O3:4");
            if (rules.VariantName == "GomokuPlus")
            {
                Console.WriteLine("  H[Row]:[Col]  Place a Heavy Stone  (2 per game)  e.g. H5:5");
                Console.WriteLine("  E[Row]:[Col]  Eraser targeting an opponent's     e.g. E3:3");
                Console.WriteLine("                Ordinary Stone      (2 per game)");
            }
            Console.WriteLine();
            Console.WriteLine("Other commands during your turn:");
            Console.WriteLine("  S   Save the current game to a file");
            Console.WriteLine("  U   Undo the last completed move");
            Console.WriteLine("  R   Redo an undone move");
            Console.WriteLine("  ?   Show this help menu");
            Console.WriteLine("  Q   Quit the current game and return to the main menu");
            Console.WriteLine();
            Console.WriteLine(rules.GetHelpText());
            Console.WriteLine("=======================================================================");
            Console.WriteLine();
        }
    }
}
