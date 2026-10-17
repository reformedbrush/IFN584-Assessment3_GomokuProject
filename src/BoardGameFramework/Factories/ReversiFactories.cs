using BoardGameFramework.AI;
using BoardGameFramework.Board;
using BoardGameFramework.Core;
using BoardGameFramework.Help;
using BoardGameFramework.Players;
using BoardGameFramework.Rules;
using BoardGameFramework.Views;

namespace BoardGameFramework.Factories
{
    public abstract class ReversiFamilyFactory : IGameFactory
    {
        public GameFamily Family { get { return GameFamily.Reversi; } }
        public string FamilyName { get { return "Reversi"; } }
        public int BoardSize { get { return 8; } }
        public abstract GameVariant Variant { get; }
        public abstract string VariantTitle { get; }
        protected abstract string WinHelp { get; }
        protected abstract IWinStrategy WinStrategy { get; }
        protected abstract IAiStrategy SmartAi { get; }

        public GridBoard CreateBoard()
        {
            return new GridBoard(BoardSize);
        }

        public void SeedBoard(GridBoard board)
        {
            board.SetCell(4, 4, PieceKind.OrdinaryP2);
            board.SetCell(4, 5, PieceKind.OrdinaryP1);
            board.SetCell(5, 4, PieceKind.OrdinaryP1);
            board.SetCell(5, 5, PieceKind.OrdinaryP2);
        }

        public IWinStrategy CreateWinStrategy()
        {
            return WinStrategy;
        }

        public IMoveRules CreateMoveRules()
        {
            return new ReversiMoveRules();
        }

        public IBoardView CreateView()
        {
            return new FullBoardView();
        }

        public IHelpProvider CreateHelp()
        {
            return new ReversiHelpProvider(VariantTitle, BoardSize, WinHelp);
        }

        public IAiStrategy CreateDumbAi()
        {
            return new DumbAi();
        }

        public IAiStrategy CreateSmartAi()
        {
            return SmartAi;
        }

        public PlayerInventory CreateInventory()
        {
            return new PlayerInventory(0, 0);
        }
    }

    public class StandardReversiFactory : ReversiFamilyFactory
    {
        public override GameVariant Variant { get { return GameVariant.Standard; } }
        public override string VariantTitle { get { return "Standard Reversi"; } }
        protected override string WinHelp { get { return "The player with strictly more disks wins. Equal counts are a draw."; } }
        protected override IWinStrategy WinStrategy { get { return new ReversiMajorityWinStrategy(); } }
        protected override IAiStrategy SmartAi { get { return new ReversiSmartAi(false, false, false); } }
    }

    public class AntiReversiFactory : ReversiFamilyFactory
    {
        public override GameVariant Variant { get { return GameVariant.Anti; } }
        public override string VariantTitle { get { return "Anti-Reversi"; } }
        protected override string WinHelp { get { return "Misère: the player with the fewest disks wins. Equal counts are a draw."; } }
        protected override IWinStrategy WinStrategy { get { return new AntiReversiWinStrategy(); } }
        protected override IAiStrategy SmartAi { get { return new ReversiSmartAi(true, false, true); } }
    }

    public class CornerReversiFactory : ReversiFamilyFactory
    {
        public override GameVariant Variant { get { return GameVariant.Corner; } }
        public override string VariantTitle { get { return "Corner Reversi"; } }
        protected override string WinHelp { get { return "Sudden-death if a player owns 3 of 4 corners; otherwise majority disk count at the end."; } }
        protected override IWinStrategy WinStrategy { get { return new CornerReversiWinStrategy(); } }
        protected override IAiStrategy SmartAi { get { return new ReversiSmartAi(false, true, false); } }
    }
}
