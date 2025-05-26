using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOrbitWithRightClick : MonoBehaviour
{
    public Transform target;
    public float distance = 10.0f;
    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;
    public float zoomSpeed = 10.0f;
    public float minDistance = 1.0f;
    public float maxDistance = 20.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (target)
        {
            if (Input.GetMouseButton(1))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 desiredPosition = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

            transform.rotation = rotation;
            transform.position = desiredPosition;
        }
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F) angle += 360F;
        if (angle > 360F) angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}