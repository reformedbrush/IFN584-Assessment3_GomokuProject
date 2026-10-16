namespace GomokuPlus.Models
{
    // The three real gameplay commands, plus two meta-commands (Save/Quit)
    // that a human player can type instead of a move.
    public enum MoveType
    {
        Ordinary,
        Heavy,
        Eraser,
        Save,
        Quit
    }
}
