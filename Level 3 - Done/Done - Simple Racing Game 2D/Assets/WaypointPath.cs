using UnityEngine;

[ExecuteAlways] // Cho phép chạy cả khi không Play
public class WaypointPath : MonoBehaviour
{
    [Header("Waypoints (drag Transform vào đây)")]
    public Transform[] waypoints;

    [Header("Debug Settings")]
    public bool loop = true;              // Có nối cuối về đầu không
    public Color lineColor = Color.green; // Màu đường
    public Color pointColor = Color.red;  // Màu waypoint
    public float sphereRadius = 0.3f;     // Kích thước sphere
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        // Vẽ từng waypoint
        for (int i = 0; i < waypoints.Length; i++)
        {
            Transform wp = waypoints[i];
            if (wp == null) continue;

            Gizmos.color = pointColor;
            Gizmos.DrawSphere(wp.position, sphereRadius);

            // Vẽ line sang waypoint kế tiếp
            int nextIndex = i + 1;
            if (nextIndex < waypoints.Length && waypoints[nextIndex] != null)
            {
                Gizmos.color = lineColor;
                Gizmos.DrawLine(wp.position, waypoints[nextIndex].position);
            }
        }

        // Nối cuối → đầu nếu có loop
        if (loop && waypoints[0] != null && waypoints[waypoints.Length - 1] != null)
        {
            Gizmos.color = lineColor;
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
        }
    }
}
