using GomokuPlus.Persistence;

namespace GomokuPlus.Framework
{
    public interface IGameSnapshot
    {
        GameSnapshotData Capture();
    }
}
