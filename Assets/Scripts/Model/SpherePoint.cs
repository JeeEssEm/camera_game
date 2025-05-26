using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpherePoint : MonoBehaviour
{
    // ���� ��� ��������� �������
    [Tooltip("���� ��� ��������� �������")]
    public Color hoverColor = Color.yellow;
    public Color HighlightColor = Color.red;

    private event EventHandler<SpherePoint> _onPointClicked;
    public EventHandler<SpherePoint> OnPointClicked { get { return _onPointClicked; } set { _onPointClicked = value; } }

    private Color originalColor;
    private Renderer pointRenderer;

    public float SecondsToSwitch = 10;
    private bool isHovering = false;
    private bool isSelected = false;
    public int GeneratedId { get; set; }

    void Start()
    {
        pointRenderer = GetComponent<Renderer>();

        if (pointRenderer != null)
        {
            pointRenderer.enabled = false;
            originalColor = hoverColor;
        }
    }

    void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (pointRenderer != null)
        {
            pointRenderer.enabled = true;
            pointRenderer.material.color = hoverColor;
            isHovering = true;
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        OnPointClicked?.Invoke(this, this);
    }

    void OnMouseExit()
    {
        if (pointRenderer != null)
        {
            isHovering = false;
            if (isSelected)
            {
                pointRenderer.material.color = HighlightColor;
                return;
            }

            pointRenderer.enabled = false;
            pointRenderer.material.color = originalColor;
        }
    }

    public void Highlight()
    {
        if (pointRenderer != null)
        {
            isSelected = true;
            pointRenderer.enabled = true;
            pointRenderer.material.color = HighlightColor;
        }
    }

    public void StopHighlight()
    {
        if (pointRenderer != null)
        {
            isSelected = false;
            pointRenderer.enabled = false;
            pointRenderer.material.color = originalColor;
        }
    }

    public override string ToString()
    {
        return $"<Point> 2";
    }

}