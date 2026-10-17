using BoardGameFramework.AI;
using BoardGameFramework.Board;
using BoardGameFramework.Core;
using BoardGameFramework.Help;
using BoardGameFramework.Players;
using BoardGameFramework.Rules;
using BoardGameFramework.Views;

namespace BoardGameFramework.Factories
{
    public interface IGameFactory
    {
        GameFamily Family { get; }
        GameVariant Variant { get; }
        string FamilyName { get; }
        string VariantTitle { get; }
        int BoardSize { get; }

        GridBoard CreateBoard();
        void SeedBoard(GridBoard board);
        IWinStrategy CreateWinStrategy();
        IMoveRules CreateMoveRules();
        IBoardView CreateView();
        IHelpProvider CreateHelp();
        IAiStrategy CreateDumbAi();
        IAiStrategy CreateSmartAi();
        PlayerInventory CreateInventory();
    }
}
