namespace BoardGameFramework.Players
{
    public abstract class Player
    {
        public string Name { get; private set; }
        public int PlayerNumber { get; private set; }
        public PlayerInventory Inventory { get; private set; }
        public bool IsComputer { get; protected set; }

        protected Player(string name, int playerNumber, PlayerInventory inventory)
        {
            Name = name;
            PlayerNumber = playerNumber;
            Inventory = inventory;
            IsComputer = false;
        }
    }

    public class HumanPlayer : Player
    {
        public HumanPlayer(string name, int playerNumber, PlayerInventory inventory)
            : base(name, playerNumber, inventory)
        {
        }
    }

    public class ComputerPlayer : Player
    {
        public ComputerPlayer(string name, int playerNumber, PlayerInventory inventory)
            : base(name, playerNumber, inventory)
        {
            IsComputer = true;
        }
    }
}
