using System.Collections.Generic;
using BoardGameFramework.Board;
using BoardGameFramework.Parsing;
using BoardGameFramework.Players;
using BoardGameFramework.Rules;

namespace BoardGameFramework.AI
{
    public interface IAiStrategy
    {
        ParsedInput ChooseMove(GridBoard board, Player player, int opponentNumber, IMoveRules rules);
    }
}
