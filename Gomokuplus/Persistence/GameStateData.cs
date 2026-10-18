namespace GomokuPlus.Persistence
{
    // Plain data holder: the exact shape written to and read from the save
    // file as JSON. Kept deliberately simple (no behaviour) so that
    // System.Text.Json can (de)serialise it directly.
    public class GameStateData
    {
        public int[][] Grid { get; set; }
        public int CurrentPlayerNumber { get; set; }
        public int TurnCount { get; set; }
        public string GameMode { get; set; }
        public string GameVariant { get; set; }
        public string AiLevel { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public int Player1HeavyRemaining { get; set; }
        public int Player1EraserRemaining { get; set; }
        public int Player2HeavyRemaining { get; set; }
        public int Player2EraserRemaining { get; set; }
        public System.Collections.Generic.List<GameSnapshotData> UndoHistory { get; set; }
        public System.Collections.Generic.List<GameSnapshotData> RedoHistory { get; set; }
    }
}
