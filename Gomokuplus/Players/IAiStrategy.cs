using GomokuPlus.Models;

namespace GomokuPlus.Players
{
    public interface IAiStrategy
    {
        AiLevel Level { get; }

        Move ChooseMove(Board board, Player player, int opponentNumber);
    }
}
