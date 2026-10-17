using System;
using BoardGameFramework.Core;
using BoardGameFramework.Factories;
using BoardGameFramework.UI;

namespace BoardGameFramework
{
    public class Program
    {
        public static int Main(string[] args)
        {
            string family;
            string variant;
            string script;
            if (TryParseCli(args, out family, out variant, out script))
            {
                IGameFactory factory = GameFactoryProvider.Create(family, variant);
                GameEngine engine = GameEngine.Create(factory, GameMode.HumanVsHuman, AiDifficulty.Dumb, "Player 1", "Player 2", Console.In, Console.Out);
                engine.RunScript(script);
                return 0;
            }

            if (args != null && args.Length > 0)
            {
                Console.WriteLine("Usage: dotnet run -- --game gomoku|reversi --variant standard|plus|fog|anti|corner \"CMD,CMD,...\"");
                Console.WriteLine("Omit arguments to play interactively.");
                return 1;
            }

            new MainMenu(Console.In, Console.Out).Run();
            return 0;
        }

        public static bool TryParseCli(string[] args, out string family, out string variant, out string script)
        {
            family = null;
            variant = null;
            script = null;
            if (args == null || args.Length == 0)
                return false;

            for (int i = 0; i < args.Length; i++)
            {
                string token = args[i];
                if (token == "--game" && i + 1 < args.Length)
                {
                    family = args[i + 1];
                    i++;
                }
                else if (token == "--variant" && i + 1 < args.Length)
                {
                    variant = args[i + 1];
                    i++;
                }
                else if (script == null && !StartsWithDash(token))
                {
                    script = token;
                }
            }

            return family != null && variant != null && script != null;
        }

        private static bool StartsWithDash(string token)
        {
            return token != null && token.Length > 0 && token[0] == '-';
        }
    }
}
