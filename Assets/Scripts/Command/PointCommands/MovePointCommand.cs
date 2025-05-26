namespace Commands
{
    public class MovePointCommand : BaseCommand
    {
        private bool _moveDown;
        private int _id;

        public MovePointCommand(PointsModel dataModel, bool moveDown, int id) : base(dataModel)
        {
            _moveDown = moveDown;
            _id = id;
        }

        public override void Execute()
        {
            _dataModel.MovePoint(_id, _moveDown);
        }

        public override void Redo()
        {
            _dataModel.MovePoint(_id, _moveDown);
        }

        public override void Undo()
        {
            _dataModel.MovePoint(_id, !_moveDown);
        }
    }
}