using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PointsManager : MonoBehaviour
{
    public float radius = 2f;

    private float phiStep = 30;

    private float thetaStep = 30;

    private float _prefabRadius = 0.1f;

    private const int LevelOfPartitioning = 4;
    
    private Quaternion _startRotation;
    private Quaternion _endRotation;
    private float _rotationStartTime;
    private float _rotationDuration;
    private bool _justStartedAnimation = false;

    public int AmountOfPoints { get; private set; }

    public GameObject target;

    public Material debugSphereMaterial;

    private List<GameObject> _points = new();

    private GameObject _debugSphere;

    private (Vector3, float)? _currentPoint;

    private event EventHandler _animationEnded;
    private event EventHandler<SpherePoint> _pointChose;
    public EventHandler<SpherePoint> PointChose { get { return _pointChose; } set { _pointChose = value; } }
    public event EventHandler OnAnimationEnded { add {_animationEnded += value; } remove {_animationEnded -= value; } }

    private Queue<(Vector3, float)> _currentQueue;
    private bool _animationStopped = true;
    
    #region point cloud
    private void CalculateCoordSteps()
    {
        AmountOfPoints = 10 * (int)Mathf.Pow(2, 2 * LevelOfPartitioning) + 2;

        //_prefabRadius = 2 * Mathf.PI * radius / (2 * Mathf.Sqrt(AmountOfPoints));
        _prefabRadius = radius * 2f * Mathf.Sqrt(4f * Mathf.PI / AmountOfPoints);
    }

    private void CreatePointCloudFib()
    {
        var prefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        prefab.transform.localScale = new Vector3(_prefabRadius, _prefabRadius, _prefabRadius);

        var phi = Mathf.PI * (Mathf.Sqrt(5) - 1);
        for (int i = 0; i < AmountOfPoints; i++)
        {
            float y = 1 - (i / (float)(AmountOfPoints - 1)) * 2;
            float r = Mathf.Sqrt(1 - y * y);
            float theta = phi * i;

            float x = Mathf.Cos(theta) * r;
            float z = Mathf.Sin(theta) * r;

            var position = new Vector3(
                x * radius + target.transform.position.x,
                y * radius + target.transform.position.y,
                z * radius + target.transform.position.z
                );

            GameObject point = Instantiate(prefab, position, Quaternion.identity);
            point.name = $"point {theta} {phi}";
            if (point.GetComponent<SpherePoint>() == null)
            {
                point.AddComponent<SpherePoint>();
            }
            var comp = point.GetComponent<SpherePoint>();
            comp.OnPointClicked += SphereChose;
            comp.GeneratedId = i;
            _points.Add(point);
        }
        Destroy(prefab);
    }

    private void CreatePointCloud()
    {
        var prefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        prefab.transform.localScale = new Vector3(_prefabRadius, _prefabRadius, _prefabRadius);

        for (float phiAngle = 0; phiAngle <= 360; phiAngle += phiStep)
        {
            for (float thetaAngle = 0; thetaAngle <= 180; thetaAngle += thetaStep)
            {
                float phi = phiAngle * Mathf.Deg2Rad;
                float theta = thetaAngle * Mathf.Deg2Rad;
                float x = radius * Mathf.Sin(theta) * Mathf.Cos(phi) + target.transform.position.x;
                float y = radius * Mathf.Sin(theta) * Mathf.Sin(phi) + target.transform.position.y;
                float z = radius * Mathf.Cos(theta) + target.transform.position.z;
                var position = new Vector3(x, y, z);

                GameObject point = Instantiate(prefab, position, Quaternion.identity);
                point.name = $"point {theta} {phi}";
                if (point.GetComponent<SpherePoint>() == null)
                {
                    point.AddComponent<SpherePoint>();
                }
                _points.Add(point);
            }
        }
    }

    private void CreateDebugSphere()
    {
        _debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _debugSphere.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        _debugSphere.transform.position = target.transform.position;

        var collider = _debugSphere.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        if (!debugSphereMaterial)
            debugSphereMaterial = Resources.Load("materials/glass", typeof(Material)) as Material;
        _debugSphere.GetComponent<Renderer>().SetMaterials(new List<Material>() { debugSphereMaterial });
    }

    void Start()
    {
        CalculateCoordSteps();
        CreatePointCloudFib();
        //CreatePointCloud();
        CreateDebugSphere();
    }

    #endregion


    public List<Vector3> GetPointsPositions(List<int> ids)
    {
        var res = new List<Vector3>();
        foreach (var id in ids)
        {
            foreach (var point in _points)
            {
                if (PointIdEqual(point, id)) 
                    res.Add(point.transform.position);
            }
        }
        return res;
    }

    private bool PointIdEqual(GameObject point, int id)
    {
        return id == point.GetComponent<SpherePoint>().GeneratedId;
    }
    
    #region sphere
    public void HighlightSphere(int id)
    {
        foreach (var point in _points)
        {
            if (PointIdEqual(point, id))
            {
                point.GetComponent<SpherePoint>().Highlight();
                AimAt(point.transform.position);
            }
            else
            {
                point.GetComponent<SpherePoint>().StopHighlight();
            }
        }
    }

    private void AimAt(Vector3 position)
    {
        Vector3 direction = position - target.transform.position;
        target.transform.rotation = Quaternion.LookRotation(direction);
    }

    public void StopHighlightSpheres()
    {
        foreach (var point in _points)
        {
            point.GetComponent<SpherePoint>().StopHighlight();
        }
    }

    #endregion
    void Update()
    {
        if (_animationStopped || _currentPoint == null)
            return;

        if (_justStartedAnimation) {
            _justStartedAnimation = false;
            return;
        }

        float t = Mathf.Clamp01((Time.time - _rotationStartTime) / _rotationDuration);
        target.transform.rotation = Quaternion.Slerp(_startRotation, _endRotation, t);

        if (t >= 1.0f) {
            _currentQueue.Dequeue();
            if (_currentQueue.Count > 0) {
                _currentPoint = _currentQueue.Peek();
                PrepareNextRotation(_currentPoint.Value.Item1, _currentPoint.Value.Item2);
            } else {
                _currentPoint = null;
                _animationEnded?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    void SphereChose(object sender, SpherePoint point)
    {
        if (_pointChose != null)
            _pointChose(this, point);
    }

    public void StartAnimation(Queue<(Vector3, float)> posTimes)
    {
        _currentQueue = new Queue<(Vector3, float)>(posTimes);
        _currentPoint = _currentQueue.Peek();
        _animationStopped = false;

        AimAt(_currentPoint.Value.Item1);
        _currentQueue.Dequeue(); // Первая точка уже выставлена, убираем из очереди
        if (_currentQueue.Count > 0) {
            _currentPoint = _currentQueue.Peek();
            PrepareNextRotation(_currentPoint.Value.Item1, _currentPoint.Value.Item2);
            _justStartedAnimation = true;
        } else {
            _currentPoint = null;
            _animationEnded?.Invoke(this, EventArgs.Empty);
        }
    }
    
    private void PrepareNextRotation(Vector3 position, float rotationTime)
    {
        _startRotation = target.transform.rotation;
        Vector3 direction = position - target.transform.position;
        _endRotation = Quaternion.LookRotation(direction);
        _rotationStartTime = Time.time;
        _rotationDuration = rotationTime;
    }
    
    void SetNewTarget(Vector3 position, float rotationTime)
    {
        _startRotation = target.transform.rotation;
        Vector3 direction = position - target.transform.position;
        _endRotation = Quaternion.LookRotation(direction);
        _rotationStartTime = Time.time;
        _rotationDuration = rotationTime;
    }

    public void ContinueAnimation()
    {
        _animationStopped = false;
    }

    public void StopAnimation()
    {
        _animationStopped = true;
    }
}
