using GomokuPlus.Models;

namespace GomokuPlus.Players
{
    public class ComputerPlayer : Player
    {
        private readonly IAiStrategy strategy;

        public ComputerPlayer(string name, int playerNumber)
            : this(name, playerNumber, AiLevel.Easy)
        {
        }

        public ComputerPlayer(string name, int playerNumber, AiLevel level)
            : base(name, playerNumber)
        {
            strategy = level == AiLevel.Hard
                ? (IAiStrategy)new HardAiStrategy()
                : new EasyAiStrategy(playerNumber);
        }

        public AiLevel Level
        {
            get { return strategy.Level; }
        }

        public override Move GetMove(Board board, int opponentNumber)
        {
            return strategy.ChooseMove(board, this, opponentNumber);
        }
    }
}
