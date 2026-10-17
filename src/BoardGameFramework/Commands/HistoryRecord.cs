using System.Collections.Generic;

namespace BoardGameFramework.Commands
{
    public class HistoryRecord
    {
        public string Kind { get; set; }
        public int PlayerNumber { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public int PlacedPiece { get; set; }
        public int ErasedPiece { get; set; }
        public List<int> FlipRows { get; set; }
        public List<int> FlipCols { get; set; }

        public HistoryRecord()
        {
            FlipRows = new List<int>();
            FlipCols = new List<int>();
        }
    }
}
