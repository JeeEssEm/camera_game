using DTOs;

namespace Commands
{
    public interface ICommandFactory
    {
        public CreatePointCommand CreatePointCommand(PointDTO point);
        public RemovePointCommand RemovePointCommand(int id);
        public MovePointCommand MovePointCommand(int id, bool moveDown);
        public ChangePointCommand ChangePointCommand(PointDTO point, string name, float time);
    }
}
