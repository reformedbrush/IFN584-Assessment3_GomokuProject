using System;

namespace GomokuPlus.UI
{
    // Built-in help system. Any human player can bring this up mid-game
    // by typing "?".
    public class HelpMenu
    {
        public static void Show()
        {
            Console.WriteLine();
            Console.WriteLine("========================== GomokuPlus Help ==========================");
            Console.WriteLine("Move syntax: [Type][Row]:[Col]   (rows and columns run 1 to 10)");
            Console.WriteLine("  O[Row]:[Col]  Place an Ordinary Stone            e.g. O3:4");
            Console.WriteLine("  H[Row]:[Col]  Place a Heavy Stone  (2 per game)  e.g. H5:5");
            Console.WriteLine("  E[Row]:[Col]  Eraser targeting an opponent's     e.g. E3:3");
            Console.WriteLine("                Ordinary Stone      (2 per game)");
            Console.WriteLine();
            Console.WriteLine("Other commands during your turn:");
            Console.WriteLine("  S   Save the current game to a file");
            Console.WriteLine("  ?   Show this help menu");
            Console.WriteLine("  Q   Quit the current game and return to the main menu");
            Console.WriteLine();
            Console.WriteLine("Rules:");
            Console.WriteLine("  - Align 5 of your stones (Ordinary or Heavy) in a row - horizontal,");
            Console.WriteLine("    vertical, or diagonal - to win instantly.");
            Console.WriteLine("  - Heavy Stones cannot be targeted or removed by an Eraser Stone.");
            Console.WriteLine("  - Each player starts with 2 Heavy Stones and 2 Eraser Stones.");
            Console.WriteLine("  - Ordinary Stones are unlimited.");
            Console.WriteLine("=======================================================================");
            Console.WriteLine();
        }
    }
}
