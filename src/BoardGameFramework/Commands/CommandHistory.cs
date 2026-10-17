using System.Collections.Generic;

namespace BoardGameFramework.Commands
{
    // Command pattern caretaker. Undo/redo operate at turn level: each
    // request reverts or restores a pair of completed player actions
    // (both humans in HvH, or human + AI in HvC) when two entries exist.
    public class CommandHistory
    {
        private List<IMoveCommand> undoStack;
        private List<IMoveCommand> redoStack;

        public CommandHistory()
        {
            undoStack = new List<IMoveCommand>();
            redoStack = new List<IMoveCommand>();
        }

        public int UndoCount
        {
            get { return undoStack.Count; }
        }

        public int RedoCount
        {
            get { return redoStack.Count; }
        }

        public bool CanUndo
        {
            get { return undoStack.Count > 0; }
        }

        public bool CanRedo
        {
            get { return redoStack.Count > 0; }
        }

        public void Record(IMoveCommand command)
        {
            undoStack.Add(command);
            redoStack.Clear();
        }

        public void RecordWithoutClearingRedo(IMoveCommand command)
        {
            undoStack.Add(command);
        }

        public int UndoTurn()
        {
            int steps = undoStack.Count >= 2 ? 2 : (undoStack.Count >= 1 ? 1 : 0);
            for (int i = 0; i < steps; i++)
            {
                IMoveCommand command = undoStack[undoStack.Count - 1];
                undoStack.RemoveAt(undoStack.Count - 1);
                command.Undo();
                redoStack.Add(command);
            }
            return steps;
        }

        public int RedoTurn()
        {
            int steps = redoStack.Count >= 2 ? 2 : (redoStack.Count >= 1 ? 1 : 0);
            for (int i = 0; i < steps; i++)
            {
                IMoveCommand command = redoStack[redoStack.Count - 1];
                redoStack.RemoveAt(redoStack.Count - 1);
                command.Execute();
                undoStack.Add(command);
            }
            return steps;
        }

        public List<HistoryRecord> ToRecords()
        {
            List<HistoryRecord> records = new List<HistoryRecord>();
            for (int i = 0; i < undoStack.Count; i++)
                records.Add(undoStack[i].ToRecord());
            return records;
        }

        public void Clear()
        {
            undoStack.Clear();
            redoStack.Clear();
        }
    }
}
