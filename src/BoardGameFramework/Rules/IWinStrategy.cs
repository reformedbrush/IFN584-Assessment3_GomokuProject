using BoardGameFramework.Board;

namespace BoardGameFramework.Rules
{
    public class GameOutcome
    {
        public bool IsOver { get; set; }
        public int Winner { get; set; }
        public bool IsDraw { get; set; }
        public string Message { get; set; }

        public static GameOutcome Continue()
        {
            GameOutcome outcome = new GameOutcome();
            outcome.IsOver = false;
            outcome.Winner = 0;
            outcome.IsDraw = false;
            outcome.Message = "";
            return outcome;
        }

        public static GameOutcome Win(int player, string message)
        {
            GameOutcome outcome = new GameOutcome();
            outcome.IsOver = true;
            outcome.Winner = player;
            outcome.IsDraw = false;
            outcome.Message = message;
            return outcome;
        }

        public static GameOutcome Draw(string message)
        {
            GameOutcome outcome = new GameOutcome();
            outcome.IsOver = true;
            outcome.Winner = 0;
            outcome.IsDraw = true;
            outcome.Message = message;
            return outcome;
        }
    }

    public interface IWinStrategy
    {
        GameOutcome Evaluate(GridBoard board, int lastPlayer, int lastRow, int lastCol, bool bothCannotMove);
    }
}
