using System.Collections.Generic;
using BoardGameFramework.Board;
using BoardGameFramework.Core;
using BoardGameFramework.Players;

namespace BoardGameFramework.Commands
{
    public class PlaceStoneCommand : IMoveCommand
    {
        private GridBoard board;
        private PlayerInventory inventory;
        private PieceKind piece;
        private bool usesHeavy;

        public int PlayerNumber { get; private set; }
        public int Row { get; private set; }
        public int Col { get; private set; }

        public PlaceStoneCommand(GridBoard board, PlayerInventory inventory, int playerNumber, int row, int col, PieceKind piece, bool usesHeavy)
        {
            this.board = board;
            this.inventory = inventory;
            this.piece = piece;
            this.usesHeavy = usesHeavy;
            PlayerNumber = playerNumber;
            Row = row;
            Col = col;
        }

        public string Description
        {
            get { return (usesHeavy ? "Heavy" : "Ordinary") + " at (" + Row + "," + Col + ")"; }
        }

        public void Execute()
        {
            board.SetCell(Row, Col, piece);
            if (usesHeavy)
                inventory.HeavyRemaining = inventory.HeavyRemaining - 1;
        }

        public void Undo()
        {
            board.SetCell(Row, Col, PieceKind.Empty);
            if (usesHeavy)
                inventory.HeavyRemaining = inventory.HeavyRemaining + 1;
        }

        public HistoryRecord ToRecord()
        {
            HistoryRecord record = new HistoryRecord();
            record.Kind = usesHeavy ? "Heavy" : "Ordinary";
            record.PlayerNumber = PlayerNumber;
            record.Row = Row;
            record.Col = Col;
            record.PlacedPiece = (int)piece;
            return record;
        }
    }

    public class EraseStoneCommand : IMoveCommand
    {
        private GridBoard board;
        private PlayerInventory inventory;
        private PieceKind erased;

        public int PlayerNumber { get; private set; }
        public int Row { get; private set; }
        public int Col { get; private set; }

        public EraseStoneCommand(GridBoard board, PlayerInventory inventory, int playerNumber, int row, int col, PieceKind erased)
        {
            this.board = board;
            this.inventory = inventory;
            this.erased = erased;
            PlayerNumber = playerNumber;
            Row = row;
            Col = col;
        }

        public string Description
        {
            get { return "Eraser at (" + Row + "," + Col + ")"; }
        }

        public void Execute()
        {
            board.SetCell(Row, Col, PieceKind.Empty);
            inventory.EraserRemaining = inventory.EraserRemaining - 1;
        }

        public void Undo()
        {
            board.SetCell(Row, Col, erased);
            inventory.EraserRemaining = inventory.EraserRemaining + 1;
        }

        public HistoryRecord ToRecord()
        {
            HistoryRecord record = new HistoryRecord();
            record.Kind = "Eraser";
            record.PlayerNumber = PlayerNumber;
            record.Row = Row;
            record.Col = Col;
            record.ErasedPiece = (int)erased;
            return record;
        }
    }

    public class PlaceDiskCommand : IMoveCommand
    {
        private GridBoard board;
        private PieceKind placed;
        private List<int> flipRows;
        private List<int> flipCols;

        public int PlayerNumber { get; private set; }
        public int Row { get; private set; }
        public int Col { get; private set; }

        public PlaceDiskCommand(GridBoard board, int playerNumber, int row, int col, List<int> flipRows, List<int> flipCols)
        {
            this.board = board;
            this.placed = GridBoard.OrdinaryFor(playerNumber);
            this.flipRows = new List<int>(flipRows);
            this.flipCols = new List<int>(flipCols);
            PlayerNumber = playerNumber;
            Row = row;
            Col = col;
        }

        public string Description
        {
            get { return "Place disk at (" + Row + "," + Col + "), flips " + flipRows.Count; }
        }

        public int FlipCount
        {
            get { return flipRows.Count; }
        }

        public bool CapturesCorner(int boardSize)
        {
            return IsCorner(Row, Col, boardSize);
        }

        public void Execute()
        {
            board.SetCell(Row, Col, placed);
            for (int i = 0; i < flipRows.Count; i++)
                board.SetCell(flipRows[i], flipCols[i], placed);
        }

        public void Undo()
        {
            PieceKind opponent = GridBoard.OrdinaryFor(PlayerNumber == 1 ? 2 : 1);
            for (int i = 0; i < flipRows.Count; i++)
                board.SetCell(flipRows[i], flipCols[i], opponent);
            board.SetCell(Row, Col, PieceKind.Empty);
        }

        public HistoryRecord ToRecord()
        {
            HistoryRecord record = new HistoryRecord();
            record.Kind = "Disk";
            record.PlayerNumber = PlayerNumber;
            record.Row = Row;
            record.Col = Col;
            record.PlacedPiece = (int)placed;
            record.FlipRows = new List<int>(flipRows);
            record.FlipCols = new List<int>(flipCols);
            return record;
        }

        public static bool IsCorner(int row, int col, int boardSize)
        {
            bool rowEdge = row == 1 || row == boardSize;
            bool colEdge = col == 1 || col == boardSize;
            return rowEdge && colEdge;
        }
    }

    public class PassCommand : IMoveCommand
    {
        public int PlayerNumber { get; private set; }

        public PassCommand(int playerNumber)
        {
            PlayerNumber = playerNumber;
        }

        public string Description
        {
            get { return "PASS"; }
        }

        public void Execute()
        {
        }

        public void Undo()
        {
        }

        public HistoryRecord ToRecord()
        {
            HistoryRecord record = new HistoryRecord();
            record.Kind = "Pass";
            record.PlayerNumber = PlayerNumber;
            return record;
        }
    }
}
