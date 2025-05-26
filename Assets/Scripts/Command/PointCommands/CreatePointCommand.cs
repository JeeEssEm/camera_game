using DTOs;

namespace Commands
{
    public class CreatePointCommand : BaseCommand
    {
        private PointDTO _point;
        //private int _position;

        public CreatePointCommand(PointsModel dataModel, PointDTO point) : base(dataModel)
        {
            _point = point;
        }

        public override void Execute()
        {
            _dataModel.AddPoint(_point);
        }

        public override void Redo()
        {
            _dataModel.AddPoint(_point);
        }

        public override void Undo()
        {
            //_position = _dataModel.GetPointPosition(_point);
            _dataModel.RemovePoint(_point);
        }
    }
}
