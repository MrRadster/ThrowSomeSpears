using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float distance = 5.5f;
    public float height = 2.2f;
    public float mouseSensitivity = 2.5f;
    public float minVertical = -15f;
    public float maxVertical = 65f;
    public float smoothSpeed = 12f;

    private float currentX = 0f;
    private float currentY = 20f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        currentX += Input.GetAxis("Mouse X") * mouseSensitivity;
        currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        currentY = Mathf.Clamp(currentY, minVertical, maxVertical);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);
        Vector3 desiredPos = target.position - (rotation * Vector3.forward * distance) + (Vector3.up * height);

        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.3f);
    }
}