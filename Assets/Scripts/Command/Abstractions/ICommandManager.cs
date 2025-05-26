namespace Commands
{
    public interface ICommandManager
    {
        public void Execute(BaseCommand command);

        public void Undo();
        public void Redo();

        public bool CanUndo { get; }
        public bool CanRedo { get; }

    }
}
