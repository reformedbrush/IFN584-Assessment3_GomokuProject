using BoardGameFramework.Exceptions;

namespace BoardGameFramework.Factories
{
    public static class GameFactoryProvider
    {
        public static IGameFactory Create(string family, string variant)
        {
            string familyKey = Normalize(family);
            string variantKey = Normalize(variant);

            if (familyKey == "gomoku")
            {
                if (variantKey == "standard")
                    return new StandardGomokuFactory();
                if (variantKey == "plus")
                    return new GomokuPlusFactory();
                if (variantKey == "fog")
                    return new GomokuFogFactory();
                throw new InvalidCommandException("Unknown Gomoku variant '" + variant + "'. Use standard, plus, or fog.");
            }

            if (familyKey == "reversi" || familyKey == "othello")
            {
                if (variantKey == "standard")
                    return new StandardReversiFactory();
                if (variantKey == "anti")
                    return new AntiReversiFactory();
                if (variantKey == "corner")
                    return new CornerReversiFactory();
                throw new InvalidCommandException("Unknown Reversi variant '" + variant + "'. Use standard, anti, or corner.");
            }

            throw new InvalidCommandException("Unknown game family '" + family + "'. Use gomoku or reversi.");
        }

        private static string Normalize(string value)
        {
            if (value == null)
                return "";
            return value.Trim().ToLowerInvariant();
        }
    }
}
