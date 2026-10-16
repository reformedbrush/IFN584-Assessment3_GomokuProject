using System;
using GomokuPlus.GameLogic;
using GomokuPlus.Models;
using GomokuPlus.UI;

namespace GomokuPlus
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("               GomokuPlus");
            Console.WriteLine("=======================================");

            // Automated testing mode: a single command-line execution string,
            // e.g. dotnet run -- "O3:2,O3:3,O3:1,O4:3,O3:4,O5:3,E3:3,H3:3"
            if (args.Length > 0 && args[0].Length > 0)
            {
                GomokuGame.RunAutomatedScript(args[0]);
                return;
            }

            MainMenu();
        }

        private static void MainMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine();
                Console.WriteLine("1. New game - Human vs Human");
                Console.WriteLine("2. New game - Human vs Computer");
                Console.WriteLine("3. Load saved game");
                Console.WriteLine("4. Help");
                Console.WriteLine("5. Exit");
                Console.Write("Choose an option (1-5) >> ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    StartNewGame(GameMode.HumanVsHuman);
                }
                else if (choice == "2")
                {
                    StartNewGame(GameMode.HumanVsComputer);
                }
                else if (choice == "3")
                {
                    LoadGame();
                }
                else if (choice == "4")
                {
                    HelpMenu.Show();
                }
                else if (choice == "5")
                {
                    exit = true;
                    Console.WriteLine("Thanks for playing GomokuPlus. Goodbye!");
                }
                else
                {
                    Console.WriteLine("Invalid option, please choose between 1 and 5.");
                }
            }
        }

        private static void StartNewGame(GameMode mode)
        {
            Console.Write("Enter Player 1 name >> ");
            string p1 = ReadNonEmpty("Player 1");

            string p2;
            if (mode == GameMode.HumanVsHuman)
            {
                Console.Write("Enter Player 2 name >> ");
                p2 = ReadNonEmpty("Player 2");
            }
            else
            {
                p2 = "Computer";
            }

            GomokuGame game = new GomokuGame(mode, p1, p2);
            game.Run();
        }

        private static void LoadGame()
        {
            Console.Write("Enter the save file name to load (e.g. save1.json) >> ");
            string fileName = Console.ReadLine();
            if (fileName == null || fileName.Length == 0)
            {
                Console.WriteLine("No file name provided.");
                return;
            }

            GomokuGame game = GomokuGame.LoadGame(fileName);
            if (game != null)
                game.Run();
        }

        private static string ReadNonEmpty(string label)
        {
            string value = Console.ReadLine();
            if (value == null || value.Length == 0)
                return label;
            else
                return value;
        }
    }
}
