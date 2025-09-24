using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform waypointsParent;   // GameObject chứa các waypoint (child)
    public float speed = 2f;            // Tốc độ di chuyển
    public float rotationSpeed = 180f;  // Tốc độ xoay (độ/giây)
    public float reachDist = 0.1f;      // Khoảng cách để coi như "đã đến waypoint"

    private List<Transform> waypoints = new List<Transform>();
    private int currentIndex = 0;

    void Start()
    {
        foreach (Transform child in waypointsParent)
        {
            waypoints.Add(child);
        }

        if (waypoints.Count == 0)
        {
            Debug.LogError("⚠️ Không có waypoint nào trong WaypointsParent!");
        }
    }

    void Update()
    {
        if (waypoints.Count == 0) return;

        Transform target = waypoints[currentIndex];

        // Hướng tới waypoint
        Vector2 dir = (target.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f; // -90 nếu sprite enemy nhìn lên
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        // Xoay từ từ về hướng waypoint
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // Di chuyển theo hướng "mặt trước" của enemy
        transform.position += transform.up * speed * Time.deltaTime;

        // Nếu gần waypoint thì chuyển waypoint tiếp theo
        if (Vector2.Distance(transform.position, target.position) < reachDist)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Count)
            {
                currentIndex = 0;
            }
        }
    }
}
