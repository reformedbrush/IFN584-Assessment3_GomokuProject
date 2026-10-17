namespace BoardGameFramework.Players
{
    public class PlayerInventory
    {
        public int HeavyRemaining { get; set; }
        public int EraserRemaining { get; set; }

        public PlayerInventory(int heavy, int eraser)
        {
            HeavyRemaining = heavy;
            EraserRemaining = eraser;
        }

        public PlayerInventory Clone()
        {
            return new PlayerInventory(HeavyRemaining, EraserRemaining);
        }
    }
}
