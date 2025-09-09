using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    private Transform player;
    public float followSpeed = 5f;
    public float acceleration = 10f;
    public float drag = 2f; // Hệ số giảm tốc

    private Vector3 velocity = Vector3.zero;

    void Awake()
    {
        // Thêm PolygonCollider2D và set trigger
        PolygonCollider2D col = gameObject.AddComponent<PolygonCollider2D>();
        col.isTrigger = true;

        // Thêm Rigidbody2D cho obstacle
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Thêm Rigidbody2D cho player nếu chưa có
        if (PlayerController.Instance != null)
        {
            Rigidbody2D playerRb = PlayerController.Instance.GetComponent<Rigidbody2D>();
            if (playerRb == null)
            {
                playerRb = PlayerController.Instance.gameObject.AddComponent<Rigidbody2D>();
                playerRb.gravityScale = 0f;
                playerRb.freezeRotation = true;
            }
        }
    }

    void Start()
    {
        if (PlayerController.Instance != null)
            player = PlayerController.Instance.transform;
    }

    void Update()
    {
        if (player != null)
        {
            // Tính hướng về phía player
            Vector3 direction = (player.position - transform.position).normalized;
            float burstMultiplier = 1f;
            // Nếu tốc độ thấp hơn 30% followSpeed, burst acceleration
            if (velocity.magnitude < followSpeed * 0.3f)
            {
                burstMultiplier = 3f; // Nhân acceleration lên 3 lần
            }
            // Tăng tốc về phía player
            velocity += direction * acceleration * burstMultiplier * Time.deltaTime;
            // Áp dụng drag (giảm tốc)
            velocity -= velocity * drag * Time.deltaTime;
            // Giới hạn tốc độ tối đa
            velocity = Vector3.ClampMagnitude(velocity, followSpeed);
            // Di chuyển obstacle
            transform.position += velocity * Time.deltaTime;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (collision.CompareTag("Obstacle"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
