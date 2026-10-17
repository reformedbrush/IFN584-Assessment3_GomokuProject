using BoardGameFramework.AI;
using BoardGameFramework.Board;
using BoardGameFramework.Core;
using BoardGameFramework.Help;
using BoardGameFramework.Players;
using BoardGameFramework.Rules;
using BoardGameFramework.Views;

namespace BoardGameFramework.Factories
{
    public abstract class GomokuFamilyFactory : IGameFactory
    {
        public GameFamily Family { get { return GameFamily.Gomoku; } }
        public string FamilyName { get { return "Gomoku"; } }
        public int BoardSize { get { return 10; } }
        public abstract GameVariant Variant { get; }
        public abstract string VariantTitle { get; }
        protected abstract bool SpecialStones { get; }
        protected abstract bool UsesFog { get; }

        public GridBoard CreateBoard()
        {
            return new GridBoard(BoardSize);
        }

        public void SeedBoard(GridBoard board)
        {
        }

        public IWinStrategy CreateWinStrategy()
        {
            return new GomokuWinStrategy();
        }

        public IMoveRules CreateMoveRules()
        {
            return new GomokuMoveRules(SpecialStones);
        }

        public IBoardView CreateView()
        {
            if (UsesFog)
                return new FogBoardView();
            return new FullBoardView();
        }

        public IHelpProvider CreateHelp()
        {
            return new GomokuHelpProvider(VariantTitle, BoardSize, SpecialStones, UsesFog);
        }

        public IAiStrategy CreateDumbAi()
        {
            return new DumbAi();
        }

        public IAiStrategy CreateSmartAi()
        {
            return new GomokuSmartAi();
        }

        public PlayerInventory CreateInventory()
        {
            if (SpecialStones)
                return new PlayerInventory(2, 2);
            return new PlayerInventory(0, 0);
        }
    }

    public class StandardGomokuFactory : GomokuFamilyFactory
    {
        public override GameVariant Variant { get { return GameVariant.Standard; } }
        public override string VariantTitle { get { return "Standard Gomoku"; } }
        protected override bool SpecialStones { get { return false; } }
        protected override bool UsesFog { get { return false; } }
    }

    public class GomokuPlusFactory : GomokuFamilyFactory
    {
        public override GameVariant Variant { get { return GameVariant.Plus; } }
        public override string VariantTitle { get { return "GomokuPlus"; } }
        protected override bool SpecialStones { get { return true; } }
        protected override bool UsesFog { get { return false; } }
    }

    public class GomokuFogFactory : GomokuFamilyFactory
    {
        public override GameVariant Variant { get { return GameVariant.Fog; } }
        public override string VariantTitle { get { return "GomokuFog"; } }
        protected override bool SpecialStones { get { return false; } }
        protected override bool UsesFog { get { return true; } }
    }
}
