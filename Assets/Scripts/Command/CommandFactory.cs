using Commands;
using DTOs;

public class CommandFactory : ICommandFactory
{
    private readonly PointsModel _dataModel;

    public CommandFactory(PointsModel dataModel)
    {
        _dataModel = dataModel;
    }

    public ChangePointCommand ChangePointCommand(PointDTO point, string name, float time)
    {
        return new ChangePointCommand(_dataModel, point, name, time);
    }

    public CreatePointCommand CreatePointCommand(PointDTO point)
    {
        return new CreatePointCommand(_dataModel, point);
    }

    public MovePointCommand MovePointCommand(int id, bool moveDown)
    {
        return new MovePointCommand(_dataModel, moveDown, id);
    }

    public RemovePointCommand RemovePointCommand(int id)
    {
        return new RemovePointCommand(_dataModel, id);
    }


}
