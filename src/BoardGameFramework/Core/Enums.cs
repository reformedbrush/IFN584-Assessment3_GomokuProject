namespace BoardGameFramework.Core
{
    public enum GameFamily
    {
        Gomoku,
        Reversi
    }

    public enum GameVariant
    {
        Standard,
        Plus,
        Fog,
        Anti,
        Corner
    }

    public enum GameMode
    {
        HumanVsHuman,
        HumanVsComputer
    }

    public enum AiDifficulty
    {
        Dumb,
        Smarter
    }

    public enum PieceKind
    {
        Empty = 0,
        OrdinaryP1 = 1,
        OrdinaryP2 = 2,
        HeavyP1 = 3,
        HeavyP2 = 4
    }

    public enum InputKind
    {
        PlaceOrdinary,
        PlaceHeavy,
        PlaceEraser,
        PlaceDisk,
        Pass,
        Undo,
        Redo,
        Save,
        Load,
        Help,
        Quit
    }
}
