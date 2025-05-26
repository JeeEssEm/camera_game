namespace Commands
{
    public abstract class BaseCommand
    {
        protected PointsModel _dataModel;

        public BaseCommand(PointsModel dataModel)
        {
            _dataModel = dataModel;
        }

        public abstract void Execute();
        public abstract void Undo();
        public abstract void Redo();

    }
}
