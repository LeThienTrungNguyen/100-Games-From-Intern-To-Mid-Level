using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public float followSpeed = 10f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Cursor.visible = false;

        // Thêm PolygonCollider2D và set trigger
        PolygonCollider2D col = gameObject.AddComponent<PolygonCollider2D>();
        col.isTrigger = true;

        // Thêm Rigidbody2D và set velocity = 0
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        rb.freezeRotation = true;
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            if (GameController.Instance != null)
                GameController.Instance.GameOver();
        }
    }
}
