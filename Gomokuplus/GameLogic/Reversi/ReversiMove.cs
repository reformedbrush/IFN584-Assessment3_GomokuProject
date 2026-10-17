namespace GomokuPlus.GameLogic.Reversi
{
    public class ReversiMove
    {
        public int Row { get; }
        public int Col { get; }
        public bool IsPass { get; }

        public ReversiMove(int row, int col)
        {
            Row = row;
            Col = col;
            IsPass = false;
        }

        private ReversiMove(bool pass)
        {
            IsPass = pass;
        }

        public static ReversiMove Pass()
        {
            return new ReversiMove(true);
        }
    }
}
