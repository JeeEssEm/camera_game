using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;


public class PointsModel
{
    private event EventHandler<PointDTO> _onPointAdded;
    private event EventHandler<PointDTO> _onPointRemoved;
    private event EventHandler<PointDTO> _onPointMovedUp;
    private event EventHandler<PointDTO> _onPointMovedDown;
    private event EventHandler<PointDTO> _onPointChanged;
    private event EventHandler<List<PointDTO>> _onUpdated;

    #region public event handlers
    public event EventHandler<PointDTO> OnPointAdded
    {
        add { _onPointAdded += value; }
        remove { _onPointAdded -= value; }
    }

    public event EventHandler<PointDTO> OnPointRemoved
    {
        add { _onPointRemoved += value; }
        remove { _onPointRemoved -= value; }
    }

    public event EventHandler<PointDTO> OnPointMovedUp
    {
        add { _onPointMovedUp += value; }
        remove { _onPointMovedUp -= value; }
    }

    public event EventHandler<PointDTO> OnPointMovedDown
    {
        add { _onPointMovedDown += value; }
        remove { _onPointMovedDown -= value; }
    }

    public event EventHandler<List<PointDTO>> OnUpdated
    {
        add { _onUpdated += value; }
        remove { _onUpdated -= value; }
    }
    
    public event EventHandler<PointDTO> OnPointChanged
    {
        add { _onPointChanged += value; }
        remove { _onPointChanged -= value; }
    }
    
    #endregion
    public List<PointDTO> Points { get; private set; }

    public PointsModel()
    {
        Points = new();
    }

    public void NotifyAboutChanges()
    {
        _onUpdated?.Invoke(this, Points);
    }

    public void AddPoint(PointDTO point, int position = -1)
    {
        if (position != -1)
        {
            if (position < Points.Count)
                Points.Insert(position, point);
        }

        Points.Add(point);
        _onPointAdded?.Invoke(this, point);
    }

    public void RemovePoint(PointDTO point)
    {
        Points.Remove(point);
        _onPointRemoved?.Invoke(this, point);
    }

    public void RemovePoint(int id)
    {
        foreach (var point in Points)
        {
            if (point.Id == id)
            {
                Points.Remove(point);
                _onPointRemoved?.Invoke(this, point);
                return;
            }
        }
    }

    public void MovePoint(int id, bool moveDown)
    {
        if (Points.Count == 1)
            return;

        var point = GetPointById(id);
        var pos = GetPointPosition(point);

        if (moveDown)
            MovePointDown(point, pos);
        else
            MovePointUp(point, pos);
    }

    private void MovePointDown(PointDTO point, int pos)
    {
        if (pos == Points.Count - 1)
            return;


        var temp = Points[pos + 1];
        Points[pos + 1] = point;
        Points[pos] = temp;

        _onPointMovedDown?.Invoke(this, point);
    }

    private void MovePointUp(PointDTO point, int pos)
    {
        if (pos == 0)
            return;
        var temp = Points[pos - 1];
        Points[pos - 1] = point;
        Points[pos] = temp;

        _onPointMovedUp?.Invoke(this, point);
    }

    public void ChangePoint(int id, string name, float time)
    {
        var point = GetPointById(id);
        point.Name = name;
        point.Time = time;

        _onPointChanged?.Invoke(this, point);
    }

    public int GetPointPosition(PointDTO point) => Points.IndexOf(point);
    public PointDTO GetPointById(int id) => Points.Where(p => p.Id == id).First();

    public int Count => Points.Count;
}
