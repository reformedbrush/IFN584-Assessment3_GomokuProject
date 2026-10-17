using System.IO;
using BoardGameFramework.Players;

namespace BoardGameFramework.Help
{
    public class GomokuHelpProvider : IHelpProvider
    {
        private string title;
        private int size;
        private bool specialStones;
        private bool fog;

        public GomokuHelpProvider(string title, int size, bool specialStones, bool fog)
        {
            this.title = title;
            this.size = size;
            this.specialStones = specialStones;
            this.fog = fog;
        }

        public void WriteHelp(TextWriter output, Player player)
        {
            output.WriteLine();
            output.WriteLine("================ " + title + " ================");
            output.WriteLine("Family: Gomoku    Board: " + size + "x" + size + " (rows/cols 1-" + size + ")");
            output.WriteLine("Win: align 5 of your stones horizontally, vertically, or diagonally.");
            if (specialStones)
            {
                output.WriteLine("Special stones: Heavy (cannot be erased) and Eraser (removes opponent Ordinary).");
                if (player != null)
                    output.WriteLine("Your inventory: Heavy " + player.Inventory.HeavyRemaining + ", Eraser " + player.Inventory.EraserRemaining + ".");
            }
            if (fog)
                output.WriteLine("Fog: you only see your own stones and the 8 adjacent cells. Hidden cells show as ?.");
            output.WriteLine("Pass: not used in Gomoku.");
            output.WriteLine();
            output.WriteLine("Commands:");
            output.WriteLine("  O[r]:[c]   Ordinary stone     e.g. O5:3");
            if (specialStones)
            {
                output.WriteLine("  H[r]:[c]   Heavy stone        e.g. H3:3");
                output.WriteLine("  E[r]:[c]   Eraser stone       e.g. E3:3");
            }
            WriteSharedCommands(output);
            output.WriteLine("================================================");
        }

        public static void WriteSharedCommands(TextWriter output)
        {
            output.WriteLine("  UNDO       Revert the previous full turn (both players / human+AI)");
            output.WriteLine("  REDO       Restore an undone turn");
            output.WriteLine("  SAVE [file]  Save the current game");
            output.WriteLine("  LOAD [file]  Load a saved game");
            output.WriteLine("  HELP       Show this help");
            output.WriteLine("  QUIT       Return to the main menu");
        }
    }

    public class ReversiHelpProvider : IHelpProvider
    {
        private string title;
        private int size;
        private string winText;

        public ReversiHelpProvider(string title, int size, string winText)
        {
            this.title = title;
            this.size = size;
            this.winText = winText;
        }

        public void WriteHelp(TextWriter output, Player player)
        {
            output.WriteLine();
            output.WriteLine("================ " + title + " ================");
            output.WriteLine("Family: Reversi    Board: " + size + "x" + size + " (rows/cols 1-" + size + ")");
            output.WriteLine("Pieces: X = Dark/P1, O = Light/P2. Opening: (4,4)=O (4,5)=X (5,4)=X (5,5)=O.");
            output.WriteLine("Placement must flank at least one opponent disk (H/V/diagonal). Flanked disks flip.");
            output.WriteLine("PASS: required when you have no legal placement. If neither can move, the game ends.");
            output.WriteLine("Win: " + winText);
            output.WriteLine();
            output.WriteLine("Commands:");
            output.WriteLine("  P[r]:[c]   Place a disk       e.g. P3:4");
            output.WriteLine("  PASS       Pass the turn when no legal move exists");
            GomokuHelpProvider.WriteSharedCommands(output);
            output.WriteLine("================================================");
        }
    }
}
