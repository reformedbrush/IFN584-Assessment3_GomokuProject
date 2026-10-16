namespace GomokuPlus.Models
{
    // A single parsed command. Row and Col are stored 1-based, exactly as
    // the player typed them (row 1..10, column 1..10).
    public class Move
    {
        public MoveType Type { get; }
        public int Row { get; }
        public int Col { get; }

        public Move(MoveType type, int row, int col)
        {
            Type = type;
            Row = row;
            Col = col;
        }
    }
}
