using DTOs;

namespace Commands
{
    public class RemovePointCommand : BaseCommand
    {
        private PointDTO _point;
        private int _pointId;
        //private int _position;

        public RemovePointCommand(PointsModel dataModel, int pointId) : base(dataModel)
        {
            _pointId = pointId;
        }

        public override void Execute()
        {
            _point = _dataModel.GetPointById(_pointId);
            //_position = _dataModel.GetPointPosition(_point);
            _dataModel.RemovePoint(_pointId);
        }

        public override void Redo()
        {
            _dataModel.RemovePoint(_pointId);
        }

        public override void Undo()
        {
            _dataModel.AddPoint(_point);
        }
    }
}