using GomokuPlus.Models;

namespace GomokuPlus.Players
{
    // Common base for a human or a computer opponent. Holds everything
    // that belongs to "being a player": identity, symbols, and remaining
    // special-stone inventory.
    public abstract class Player
    {
        private string name;
        private int playerNumber;

        public int HeavyStonesRemaining { get; set; }
        public int EraserStonesRemaining { get; set; }

        protected Player(string name, int playerNumber)
        {
            this.name = name;
            this.playerNumber = playerNumber;
            HeavyStonesRemaining = 2;
            EraserStonesRemaining = 2;
        }

        public string Name
        {
            get { return name; }
        }

        public int PlayerNumber
        {
            get { return playerNumber; }
        }

        public StoneType OrdinarySymbol
        {
            get
            {
                if (playerNumber == 1)
                    return StoneType.OrdinaryP1;
                else
                    return StoneType.OrdinaryP2;
            }
        }

        public StoneType HeavySymbol
        {
            get
            {
                if (playerNumber == 1)
                    return StoneType.HeavyP1;
                else
                    return StoneType.HeavyP2;
            }
        }

        public abstract Move GetMove(Board board, int opponentNumber);
    }
}
