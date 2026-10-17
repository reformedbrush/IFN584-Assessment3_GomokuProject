namespace GomokuPlus.Framework
{
    public class GameVariantDefinition
    {
        public GameVariant Variant { get; }
        public string Name { get; }
        public bool IsImplemented { get; }
        public IGameRules Rules { get; }

        public GameVariantDefinition(GameVariant variant, string name, bool isImplemented, IGameRules rules)
        {
            Variant = variant;
            Name = name;
            IsImplemented = isImplemented;
            Rules = rules;
        }
    }
}
