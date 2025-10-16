using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    public LineRenderer ringRenderer;
    public int segments = 60;
    public float ringRadius = 1.2f;
    private float rotationSpeed = 5f;

    private bool isDragging = false;
    private int mouseButton = -1;

    void Start()
    {
        if (ringRenderer == null)
        {
            // Tự động tạo LineRenderer nếu chưa có
            ringRenderer = gameObject.AddComponent<LineRenderer>();
            ringRenderer.positionCount = segments + 1;
            ringRenderer.loop = true;
            ringRenderer.startWidth = 0.03f;
            ringRenderer.endWidth = 0.03f;
            ringRenderer.material = new Material(Shader.Find("Sprites/Default"));
            ringRenderer.startColor = Color.cyan;
            ringRenderer.endColor = Color.cyan;
        }

        ringRenderer.enabled = false;
        DrawRing();
    }

    void DrawRing()
    {
        Vector3[] points = new Vector3[segments + 1];
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            points[i] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * ringRadius;
        }
        ringRenderer.useWorldSpace = false;
        ringRenderer.SetPositions(points);
    }

    void Update()
    {
        // Khi nhấn chuột
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit && hit.gameObject == gameObject)
            {
                isDragging = true;
                ringRenderer.enabled = true;

                if (Input.GetMouseButtonDown(0)) mouseButton = 0; // trái
                if (Input.GetMouseButtonDown(1)) mouseButton = 1; // phải
            }
        }

        // Khi đang giữ chuột
        if (isDragging)
        {
            float direction = (mouseButton == 0) ? -1f : 1f; // Trái = xoay thuận, Phải = xoay ngược
            transform.Rotate(0, 0, direction * rotationSpeed * Time.deltaTime);
        }

        // Khi thả chuột
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            isDragging = false;
            mouseButton = -1;
            ringRenderer.enabled = false;
        }
    }
}
