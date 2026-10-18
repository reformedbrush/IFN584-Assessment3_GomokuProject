namespace GomokuPlus.GameLogic.Reversi
{
    public class AntiReversiRules : ReversiRules
    {
        public override bool PlayerWithLowestScoreWins
        {
            get { return true; }
        }
    }
}
