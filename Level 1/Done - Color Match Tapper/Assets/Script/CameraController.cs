using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 5f;        // Tốc độ zoom khi lăn chuột
    public float minZoom = 2f;          // Giới hạn zoom nhỏ nhất
    public float maxZoom = 20f;         // Giới hạn zoom lớn nhất

    public float dragSpeed = 0.1f;      // Tốc độ rê camera khi giữ chuột phải

    private Camera cam;
    private Vector3 lastMousePos;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleZoom();
        HandleDrag();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            // Nếu camera 2D (Orthographic)
            if (cam.orthographic)
            {
                cam.orthographicSize -= scroll * zoomSpeed;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
            }
            else // Nếu camera 3D (Perspective)
            {
                cam.fieldOfView -= scroll * zoomSpeed;
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
            }
        }
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            Vector3 move = new Vector3(-delta.x * dragSpeed, -delta.y * dragSpeed, 0);

            // Dịch chuyển camera trong không gian thế giới
            transform.Translate(move, Space.Self);

            lastMousePos = Input.mousePosition;
        }
    }
}
