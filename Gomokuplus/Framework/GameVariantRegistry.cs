using System;
using System.Collections.Generic;
using GomokuPlus.GameLogic;

namespace GomokuPlus.Framework
{
    public static class GameVariantRegistry
    {
        public static IReadOnlyList<GameVariantDefinition> All
        {
            get
            {
                return new List<GameVariantDefinition>
                {
                    new GameVariantDefinition(GameVariant.StandardGomoku, "Standard Gomoku", true, new StandardGomokuRules()),
                    new GameVariantDefinition(GameVariant.GomokuPlus, "GomokuPlus", true, new GomokuPlusRules()),
                    new GameVariantDefinition(GameVariant.GomokuFog, "GomokuFog", true, new GomokuFogRules()),
                    new GameVariantDefinition(GameVariant.StandardReversi, "Standard Reversi", true, null),
                    new GameVariantDefinition(GameVariant.AntiReversi, "Anti-Reversi", true, null),
                    new GameVariantDefinition(GameVariant.CornerReversi, "Corner Reversi", true, null)
                };
            }
        }

        public static GameVariantDefinition Find(string name)
        {
            foreach (GameVariantDefinition definition in All)
            {
                if (string.Equals(definition.Name, name, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(definition.Variant.ToString(), name, StringComparison.OrdinalIgnoreCase))
                    return definition;
            }

            return null;
        }
    }
}
