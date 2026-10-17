using System.Collections.Generic;
using BoardGameFramework.Commands;

namespace BoardGameFramework.Persistence
{
    public class GameSnapshot
    {
        public string Family { get; set; }
        public string Variant { get; set; }
        public string Mode { get; set; }
        public string AiDifficulty { get; set; }
        public int[][] Grid { get; set; }
        public int CurrentPlayerNumber { get; set; }
        public int TurnCount { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public int Player1HeavyRemaining { get; set; }
        public int Player1EraserRemaining { get; set; }
        public int Player2HeavyRemaining { get; set; }
        public int Player2EraserRemaining { get; set; }
        public List<HistoryRecord> History { get; set; }

        public GameSnapshot()
        {
            History = new List<HistoryRecord>();
        }
    }
}
