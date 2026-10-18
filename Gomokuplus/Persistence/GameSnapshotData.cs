using System;

namespace GomokuPlus.Persistence
{
    public class GameSnapshotData
    {
        public int[][] Grid { get; set; }
        public int CurrentPlayerNumber { get; set; }
        public int TurnCount { get; set; }
        public int Player1HeavyRemaining { get; set; }
        public int Player1EraserRemaining { get; set; }
        public int Player2HeavyRemaining { get; set; }
        public int Player2EraserRemaining { get; set; }

        public GameSnapshotData Clone()
        {
            GameSnapshotData copy = new GameSnapshotData();
            copy.Grid = CloneGrid(Grid);
            copy.CurrentPlayerNumber = CurrentPlayerNumber;
            copy.TurnCount = TurnCount;
            copy.Player1HeavyRemaining = Player1HeavyRemaining;
            copy.Player1EraserRemaining = Player1EraserRemaining;
            copy.Player2HeavyRemaining = Player2HeavyRemaining;
            copy.Player2EraserRemaining = Player2EraserRemaining;
            return copy;
        }

        private static int[][] CloneGrid(int[][] source)
        {
            if (source == null)
                return null;

            int[][] result = new int[source.Length][];
            for (int row = 0; row < source.Length; row++)
            {
                if (source[row] == null)
                    continue;

                result[row] = new int[source[row].Length];
                Array.Copy(source[row], result[row], source[row].Length);
            }

            return result;
        }
    }
}
