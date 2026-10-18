using System.Collections.Generic;
using GomokuPlus.Persistence;

namespace GomokuPlus.Framework
{
    public class HistoryManager
    {
        private readonly Stack<GameSnapshotData> undoStack = new Stack<GameSnapshotData>();
        private readonly Stack<GameSnapshotData> redoStack = new Stack<GameSnapshotData>();

        public bool CanUndo
        {
            get { return undoStack.Count > 0; }
        }

        public bool CanRedo
        {
            get { return redoStack.Count > 0; }
        }

        public void RecordBeforeMove(GameSnapshotData snapshot)
        {
            undoStack.Push(snapshot.Clone());
            redoStack.Clear();
        }

        public GameSnapshotData Undo(GameSnapshotData current)
        {
            if (!CanUndo)
                return null;

            redoStack.Push(current.Clone());
            return undoStack.Pop().Clone();
        }

        public GameSnapshotData Redo(GameSnapshotData current)
        {
            if (!CanRedo)
                return null;

            undoStack.Push(current.Clone());
            return redoStack.Pop().Clone();
        }

        public List<GameSnapshotData> UndoHistory()
        {
            return CloneStack(undoStack);
        }

        public List<GameSnapshotData> RedoHistory()
        {
            return CloneStack(redoStack);
        }

        public void Restore(IEnumerable<GameSnapshotData> undoHistory, IEnumerable<GameSnapshotData> redoHistory)
        {
            undoStack.Clear();
            redoStack.Clear();

            if (undoHistory != null)
            {
                foreach (GameSnapshotData snapshot in undoHistory)
                    undoStack.Push(snapshot.Clone());
            }

            if (redoHistory != null)
            {
                foreach (GameSnapshotData snapshot in redoHistory)
                    redoStack.Push(snapshot.Clone());
            }
        }

        private static List<GameSnapshotData> CloneStack(Stack<GameSnapshotData> stack)
        {
            GameSnapshotData[] snapshots = stack.ToArray();
            List<GameSnapshotData> result = new List<GameSnapshotData>();

            for (int i = snapshots.Length - 1; i >= 0; i--)
                result.Add(snapshots[i].Clone());

            return result;
        }
    }
}
