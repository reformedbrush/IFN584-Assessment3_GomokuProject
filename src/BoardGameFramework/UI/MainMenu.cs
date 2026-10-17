using System;
using System.IO;
using BoardGameFramework.Core;
using BoardGameFramework.Factories;

namespace BoardGameFramework.UI
{
    public class MainMenu
    {
        private TextReader input;
        private TextWriter output;

        public MainMenu(TextReader input, TextWriter output)
        {
            this.input = input;
            this.output = output;
        }

        public void Run()
        {
            bool exit = false;
            while (!exit)
            {
                output.WriteLine();
                output.WriteLine("=======================================");
                output.WriteLine("     Board Game Framework (IFN584)");
                output.WriteLine("=======================================");
                output.WriteLine("1. New game");
                output.WriteLine("2. Load saved game");
                output.WriteLine("3. Help");
                output.WriteLine("4. Exit");
                output.Write("Choose an option (1-4) >> ");
                string choice = input.ReadLine();

                if (choice == "1")
                    StartNewGame();
                else if (choice == "2")
                    LoadGame();
                else if (choice == "3")
                    ShowOverviewHelp();
                else if (choice == "4")
                {
                    exit = true;
                    output.WriteLine("Thanks for playing. Goodbye!");
                }
                else
                    output.WriteLine("Invalid option, please choose between 1 and 4.");
            }
        }

        private void StartNewGame()
        {
            output.WriteLine("Game family: 1) Gomoku  2) Reversi");
            output.Write("Choose family >> ");
            string familyChoice = input.ReadLine();
            string family;
            string variant;
            if (familyChoice == "2")
            {
                family = "reversi";
                output.WriteLine("Variant: 1) Standard  2) Anti  3) Corner");
                output.Write("Choose variant >> ");
                string v = input.ReadLine();
                if (v == "2")
                    variant = "anti";
                else if (v == "3")
                    variant = "corner";
                else
                    variant = "standard";
            }
            else
            {
                family = "gomoku";
                output.WriteLine("Variant: 1) Standard  2) Plus  3) Fog");
                output.Write("Choose variant >> ");
                string v = input.ReadLine();
                if (v == "2")
                    variant = "plus";
                else if (v == "3")
                    variant = "fog";
                else
                    variant = "standard";
            }

            output.WriteLine("Mode: 1) Human vs Human  2) Human vs Computer");
            output.Write("Choose mode >> ");
            string modeChoice = input.ReadLine();
            GameMode mode = modeChoice == "2" ? GameMode.HumanVsComputer : GameMode.HumanVsHuman;
            AiDifficulty difficulty = AiDifficulty.Dumb;
            if (mode == GameMode.HumanVsComputer)
            {
                output.WriteLine("AI: 1) Dumb  2) Smarter");
                output.Write("Choose AI >> ");
                string aiChoice = input.ReadLine();
                if (aiChoice == "2")
                    difficulty = AiDifficulty.Smarter;
            }

            output.Write("Player 1 name >> ");
            string p1 = ReadName("Player 1");
            string p2;
            if (mode == GameMode.HumanVsHuman)
            {
                output.Write("Player 2 name >> ");
                p2 = ReadName("Player 2");
            }
            else
            {
                p2 = difficulty == AiDifficulty.Smarter ? "Smarter AI" : "Dumb AI";
            }

            IGameFactory factory = GameFactoryProvider.Create(family, variant);
            GameEngine engine = GameEngine.Create(factory, mode, difficulty, p1, p2, input, output);
            engine.PlayInteractive();
        }

        private void LoadGame()
        {
            output.Write("Enter the save file name to load (e.g. save1.json) >> ");
            string fileName = input.ReadLine();
            if (fileName == null || fileName.Length == 0)
            {
                output.WriteLine("No file name provided.");
                return;
            }

            try
            {
                GameEngine engine = GameEngine.LoadFromFile(fileName, input, output);
                output.WriteLine("Loaded " + engine.Factory.VariantTitle + ".");
                engine.PlayInteractive();
            }
            catch (Exception ex)
            {
                output.WriteLine("Could not load the game: " + ex.Message);
            }
        }

        private void ShowOverviewHelp()
        {
            output.WriteLine();
            output.WriteLine("This framework hosts Gomoku (standard / plus / fog) and Reversi (standard / anti / corner).");
            output.WriteLine("Start a game to see variant-specific rules. In play, type HELP, UNDO, REDO, SAVE, LOAD, QUIT.");
            output.WriteLine("CLI tests: dotnet run -- --game gomoku --variant standard \"O5:3,O6:3,...\"");
        }

        private string ReadName(string fallback)
        {
            string value = input.ReadLine();
            if (value == null || value.Length == 0)
                return fallback;
            return value;
        }
    }
}
