using GomokuPlus.Models;
using GomokuPlus.Players;

namespace GomokuPlus.Framework
{
    public interface IGameRules
    {
        string VariantName { get; }

        void ValidateMove(Board board, Player player, Move move, int opponentNumber);

        bool ApplyMove(Board board, Player player, Move move, int opponentNumber);

        string GetHelpText();
    }
}
