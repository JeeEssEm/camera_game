using DTOs;

namespace Commands
{
    public class ChangePointCommand : BaseCommand
    {
        private int _id;

        private string _oldName;
        private string _newName;

        private float _oldTime;
        private float _newTime;

        public ChangePointCommand(PointsModel dataModel, PointDTO point, string name, float time) : base(dataModel)
        {
            _id = point.Id;
            _oldName = point.Name;
            _oldTime = point.Time;

            _newName = name;
            _newTime = time;
        }

        public override void Execute()
        {
            _dataModel.ChangePoint(_id, _newName, _newTime);
        }

        public override void Redo()
        {
            _dataModel.ChangePoint(_id, _newName, _newTime);
        }

        public override void Undo()
        {
            _dataModel.ChangePoint(_id, _oldName, _oldTime);
        }
    }
}