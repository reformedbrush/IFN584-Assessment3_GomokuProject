using System;

namespace GomokuPlus.GameLogic.Reversi
{
    public static class ReversiScriptRunner
    {
        public static void Run(string script, ReversiRules rules)
        {
            ReversiBoard board = new ReversiBoard();
            ReversiCell currentPlayer = ReversiCell.Player1;
            string[] commands = script.Split(',');

            Console.WriteLine("Running Reversi script...");
            Console.WriteLine("Script: " + script);

            foreach (string rawCommand in commands)
            {
                string command = rawCommand.Trim();
                if (command.Length == 0)
                    continue;

                try
                {
                    ReversiMove move = Parse(command);
                    rules.Apply(board, move, currentPlayer);
                    Console.WriteLine(currentPlayer + ": " + command + " applied.");
                    if (rules.IsFinished(board))
                    {
                        Console.WriteLine("Winner: " + rules.Winner(board));
                        break;
                    }

                    currentPlayer = currentPlayer == ReversiCell.Player1
                        ? ReversiCell.Player2
                        : ReversiCell.Player1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(currentPlayer + ": " + command + " rejected - " + ex.Message);
                }
            }

            Console.WriteLine("Final score: P1=" + board.Count(ReversiCell.Player1) +
                ", P2=" + board.Count(ReversiCell.Player2));
        }

        private static ReversiMove Parse(string command)
        {
            if (command.Equals("pass", StringComparison.OrdinalIgnoreCase))
                return ReversiMove.Pass();

            string[] parts = command.Split(':');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int row) ||
                !int.TryParse(parts[1], out int col))
                throw new FormatException("Use Row:Col or pass.");

            return new ReversiMove(row, col);
        }
    }
}
