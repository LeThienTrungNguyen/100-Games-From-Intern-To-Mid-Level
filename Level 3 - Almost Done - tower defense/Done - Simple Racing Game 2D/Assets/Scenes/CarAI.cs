using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CarAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float waypointReachDist = 1.5f;
    public float lookAheadDist = 3f;
    public float maxThrottle = 1f;
    public float slowDownFactor = 0.5f;

    [Header("Obstacle Avoidance")]
    public float rayDist = 3f;             // Độ dài ray dò
    public LayerMask obstacleMask;         // Layer mask cho vật cản
    public float avoidSteerStrength = 1f;  // Độ mạnh khi né
    public float avoidSlowFactor = 0.3f;   // Giảm tốc khi gặp chướng ngại vật

    private int currentWaypoint = 0;
    private CarController car;
    GameController gameController;

    void Awake()
    {
        car = GetComponent<CarController>();
        gameController = FindAnyObjectByType<GameController>();
    }

    void FixedUpdate()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        Transform target = waypoints[currentWaypoint];

        // ------------------------------
        // 1. Tính toán hướng tới waypoint
        // ------------------------------
        Vector2 dirToTarget = (target.position - transform.position);
        Vector2 targetPos = (Vector2)target.position;
        Vector2 toTarget = -(targetPos - (Vector2)transform.position).normalized;
        Vector2 forward = transform.up;

        float angle = Vector2.SignedAngle(forward, toTarget);
        float steerInput = Mathf.Clamp(angle / 45f, -1f, 1f);
        float throttleInput = maxThrottle;

        if (Mathf.Abs(angle) > 30f)
            throttleInput *= slowDownFactor;

        // ------------------------------
        // 2. Dò obstacle bằng raycast
        // ------------------------------
        RaycastHit2D hitCenter = Physics2D.Raycast(transform.position, forward, rayDist, obstacleMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, (Quaternion.Euler(0, 0, 30) * forward), rayDist, obstacleMask);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, (Quaternion.Euler(0, 0, -30) * forward), rayDist, obstacleMask);

        // Debug rays
        Debug.DrawRay(transform.position, forward * rayDist, Color.white);
        Debug.DrawRay(transform.position, (Quaternion.Euler(0, 0, 30) * forward) * rayDist, Color.cyan);
        Debug.DrawRay(transform.position, (Quaternion.Euler(0, 0, -30) * forward) * rayDist, Color.cyan);

        if (hitCenter.collider != null)
        {
            // Vật cản ngay trước mặt → giảm tốc
            throttleInput *= avoidSlowFactor;
        }
        if (hitLeft.collider != null)
        {
            // Vật cản bên trái → bẻ sang phải
            steerInput += avoidSteerStrength;
        }
        if (hitRight.collider != null)
        {
            // Vật cản bên phải → bẻ sang trái
            steerInput -= avoidSteerStrength;
        }

        // ------------------------------
        // 3. Gửi input vào CarController
        // ------------------------------
        car.SetInput(throttleInput, Mathf.Clamp(steerInput, -1f, 1f));

        // ------------------------------
        // 4. Kiểm tra waypoint
        // ------------------------------
        if (Vector2.Distance(transform.position, target.position) < waypointReachDist)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Start"))
        gameController.GameOver();
    }
}
