namespace GomokuPlus.Models
{
    // Every possible state a single grid cell can be in.
    // P1 stones render as X / @, P2 stones render as O / #.
    public enum StoneType
    {
        Empty,
        OrdinaryP1,
        OrdinaryP2,
        HeavyP1,
        HeavyP2
    }
}
