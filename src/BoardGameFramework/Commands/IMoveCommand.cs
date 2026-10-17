namespace BoardGameFramework.Commands
{
    public interface IMoveCommand
    {
        int PlayerNumber { get; }
        string Description { get; }
        void Execute();
        void Undo();
        HistoryRecord ToRecord();
    }
}
