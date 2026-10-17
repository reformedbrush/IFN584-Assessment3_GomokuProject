using System.Collections.Generic;
using BoardGameFramework.Board;
using BoardGameFramework.Commands;
using BoardGameFramework.Core;
using BoardGameFramework.Parsing;
using BoardGameFramework.Players;

namespace BoardGameFramework.Rules
{
    public class LegalMove
    {
        public InputKind Kind { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }

        public LegalMove(InputKind kind, int row, int col)
        {
            Kind = kind;
            Row = row;
            Col = col;
        }
    }

    public interface IMoveRules
    {
        bool AllowsPass { get; }
        List<LegalMove> ListLegalMoves(GridBoard board, Player player, int opponentNumber);
        IMoveCommand CreateCommand(GridBoard board, Player player, int opponentNumber, ParsedInput input);
    }
}
