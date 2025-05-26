using Commands;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using Files;
using SFB;
using UI;
using UnityEngine;
using Button = UnityEngine.UI.Button;


public class PointsManagerMenu : MonoBehaviour
{
    private PointsManager _pointsManager;
    private HandleInput _inputHandler;
    private FileWorker _fileWorker;

    public Transform ContentParent;

    public GameObject ListItemPrefab;
    public GameObject ObjectWithPointsManager;

    public GameObject InputPanel;

    private bool _animationStopped = true;

    [Header("UI Controls")] [SerializeField]
    private Button playPauseButton;

    [SerializeField] private Sprite playIcon;
    [SerializeField] private Sprite pauseIcon;

    #region commands

    private PointsModel _pointsModel;
    private ICommandManager _commandManager;
    private ICommandFactory _commandFactory;
    private UIModel _uiModel;

    #endregion

    private void Start()
    {
        _commandManager = new CommandManager();
        _pointsModel = new PointsModel();
        _commandFactory = new CommandFactory(_pointsModel);

        _inputHandler = InputPanel.GetComponent<HandleInput>();
        _inputHandler.OnValidated += (_, args) => ChangePointData(args.Item1, args.Item2);

        _uiModel = ScriptableObject.CreateInstance<UIModel>();
        _uiModel.listItemPrefab = ListItemPrefab;
        _uiModel.contentParent = ContentParent;
        _uiModel.pointsMenu = this;
        _uiModel.listManager = ContentParent.GetComponent<ListManager>();
        _uiModel.InputHandler = _inputHandler;
        _uiModel.playPauseButton = playPauseButton;
        _uiModel.playIcon = playIcon;
        _uiModel.pauseIcon = pauseIcon;

        _pointsManager = ObjectWithPointsManager.GetComponent<PointsManager>();
        _pointsManager.PointChose += (_, arg) => AddPoint(arg);

        _pointsModel.OnPointAdded += (_, args) => _uiModel.AddPoint(args);
        _pointsModel.OnPointRemoved += (_, args) => _uiModel.RemovePoint(args.Id);
        _pointsModel.OnPointMovedDown += (_, _) => _uiModel.MovePointDown();
        _pointsModel.OnPointMovedUp += (_, _) => _uiModel.MovePointUp();
        _pointsModel.OnPointChanged += (_, args) => _uiModel.ChangePoint(args.Id, args.Name, args.Time);
        _pointsModel.OnUpdated += (_, args) => _uiModel.InitPointsList(args);
        _pointsManager.OnAnimationEnded += (_, _) => EndAnimation();

        playPauseButton.onClick.AddListener(StartAnimation);

        _fileWorker = ScriptableObject.CreateInstance<FileWorker>();
        _fileWorker.DataModel = _pointsModel;
    }

    #region points manipulation

    public void AddPoint(SpherePoint point)
    {
        if (!_animationStopped)
            return;

        var guid = Guid.NewGuid().ToString();
        var data = new PointDTO
        {
            Id = point.GeneratedId,
            Time = 5f,
            Name = guid.Substring(0, 10)
        };
        var cmd = _commandFactory.CreatePointCommand(data);
        _commandManager.Execute(cmd);
    }

    public void RemovePoint()
    {
        if (!_animationStopped || !_uiModel.SelectedItem)
            return;
        var id = _uiModel.SelectedItem.GetComponent<ListItem>().Data.Id;
        var cmd = _commandFactory.RemovePointCommand(id);
        _commandManager.Execute(cmd);
    }

    public void MovePointUp()
    {
        if (!_animationStopped || !_uiModel.SelectedItem)
            return;
        var id = _uiModel.SelectedItem.GetComponent<ListItem>().Data.Id;

        var cmd = _commandFactory.MovePointCommand(id, false);
        _commandManager.Execute(cmd);
    }

    public void MovePointDown()
    {
        if (!_animationStopped || !_uiModel.SelectedItem)
            return;
        var guid = _uiModel.SelectedItem.GetComponent<ListItem>().Data.Id;

        var cmd = _commandFactory.MovePointCommand(guid, true);
        _commandManager.Execute(cmd);
    }

    public void PointSelected()
    {
        var data = _uiModel.SelectedItem.GetComponent<ListItem>().Data;
        _pointsManager.HighlightSphere(data.Id);
    }

    public void PointsUnselected()
    {
        _pointsManager.StopHighlightSpheres();
    }

    public void ChangePointData(string newName, float time)
    {
        var point = _uiModel.SelectedItem.GetComponent<ListItem>().Data;

        var cmd = _commandFactory.ChangePointCommand(point, newName, time);
        _commandManager.Execute(cmd);
    }

    #endregion

    #region commands

    public void Undo()
    {
        _commandManager.Undo();
    }

    public void Redo()
    {
        _commandManager.Redo();
    }

    #endregion

    #region animation

    public void StartAnimation()
    {
        var selectedItem = _uiModel.SelectedItem;
        Queue<(Vector3, float)> items;
        List<(int, float)> points;
        if (selectedItem)
        {
            var comp = selectedItem.GetComponent<ListItem>();
            var index = _pointsModel.Points.IndexOf(
                _pointsModel.Points.Find(p => comp.Data.Id == p.Id)
            );
            points = new List<(int, float)>(
                _pointsModel.Points.GetRange(index, _pointsModel.Points.Count - index)
                    .Select(p => (p.Id, p.Time))
            );
        }
        else
        {
            points = _pointsModel.Points.Select(p => (p.Id, p.Time)).ToList();
        }

        var positions = _pointsManager.GetPointsPositions(points.Select(p => p.Item1).ToList());
        var res = positions.Zip(points, (pos, point) => (pos, point.Item2));
        items = new Queue<(Vector3, float)>(res);

        _pointsManager.StartAnimation(items);
        playPauseButton.onClick.RemoveAllListeners();
        playPauseButton.onClick.AddListener(SetPause);
        _uiModel.SetPause();
        _animationStopped = false;
    }

    public void SetPause()
    {
        _animationStopped = true;
        _uiModel.SetPlay();
        _pointsManager.StopAnimation();
        playPauseButton.onClick.RemoveAllListeners();
        playPauseButton.onClick.AddListener(ContinueAnimation);
    }

    public void ContinueAnimation()
    {
        _animationStopped = false;
        _uiModel.SetPause();
        _pointsManager.ContinueAnimation();
        playPauseButton.onClick.RemoveAllListeners();
        playPauseButton.onClick.AddListener(SetPause);
    }

    public void ResetAnimation()
    {
        _animationStopped = false;
        _uiModel.SetPlay();
        StartAnimation();
    }

    public void EndAnimation()
    {
        _animationStopped = true;
        _uiModel.SetPlay();
        playPauseButton.onClick.RemoveAllListeners();
        playPauseButton.onClick.AddListener(StartAnimation);
    }

    #endregion

    public void OpenFile()
    {
        // FileBrowser.ShowLoadDialog(
        //     (paths) => _fileWorker.LoadData(paths[0]),
        //     () => { },
        //     FileBrowser.PickMode.Files,
        //     false
        // );
        // var filePath = FileDialog.OpenJsonFile();
        var filePath = StandaloneFileBrowser.OpenFilePanel("Select file with scene", "", "json", false);
        if (filePath.Length > 0)
            _fileWorker.LoadData(filePath[0]);
    }

    public void SaveFile()
    {
        _fileWorker.SaveData();
    }

    public void CreateNewFile()
    {
        _fileWorker.CreateNew();
    }
}