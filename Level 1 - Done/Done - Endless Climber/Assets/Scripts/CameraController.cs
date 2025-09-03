using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public Camera cam;
    public float smoothTime = 0.2f; // thời gian mượt
    private Vector3 velocity = Vector3.zero;

    [Range(-1f, 1f)] public float minY;
    [Range(0f, 1f)] public float maxY;

    void LateUpdate()
    {
        Vector3 camPos = transform.position;
        Vector3 viewportPos = cam.WorldToViewportPoint(player.position);

        // Nếu player vượt dưới
        if (viewportPos.y < minY)
        {
            float delta = minY - viewportPos.y;
            camPos.y -= delta * GetCameraHeight();
        }
        // Nếu player vượt trên
        else if (viewportPos.y > maxY)
        {
            float delta = viewportPos.y - maxY;
            camPos.y += delta * GetCameraHeight();
        }

        // Di chuyển mượt với SmoothDamp
        transform.position = Vector3.SmoothDamp(transform.position, camPos, ref velocity, smoothTime);
    }

    // Lấy chiều cao camera (world units)
    private float GetCameraHeight()
    {
        return cam.orthographicSize * 2f;
    }
}
