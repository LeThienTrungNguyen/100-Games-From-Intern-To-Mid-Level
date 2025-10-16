using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;   // danh sách waypoint
    public float speed = 0.2f;                // tốc độ di chuyển
    private int currentIndex = 0;             // waypoint hiện tại
    private bool reachedEnd = false;

    void Awake()
    {
        var gameController = FindFirstObjectByType(typeof(GameController)) as GameController;
        waypoints = gameController.waypoints;
    }
    public Transform target;
    void Update()
    {
        if (waypoints == null || waypoints.Length == 0 || reachedEnd) return;
        if (currentIndex >= waypoints.Length) return; // tránh lỗi ngoài mảng

        // waypoint mục tiêu
        target = waypoints[currentIndex];

        // di chuyển về phía waypoint
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // nếu gần waypoint thì chuyển sang waypoint tiếp theo
        if (Vector3.Distance(transform.position, target.position) < 0.05f * 0.16f)
        {
            currentIndex++;

            if (currentIndex >= waypoints.Length)
            {
                ReachEnd();
            }
        }
    }

    void ReachEnd()
    {
        Debug.Log("Enemy tới đích!");
        reachedEnd = true;
        speed = 0;
        //Destroy(gameObject); // hoặc trừ máu base rồi xoá enemy
    }
}
