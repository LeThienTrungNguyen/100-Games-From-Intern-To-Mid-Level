using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;   // tốc độ di chuyển

    private Rigidbody2D rb;
    private Vector2 movement;
    public MazeGenerator mazeGenerator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mazeGenerator = GameObject.FindFirstObjectByType<MazeGenerator>();
    }

    void Update()
    {
        // Lấy input từ bàn phím (WASD hoặc phím mũi tên)
        movement.x = Input.GetAxisRaw("Horizontal"); // -1 (trái), 1 (phải)
        movement.y = Input.GetAxisRaw("Vertical");   // -1 (xuống), 1 (lên)

        movement = movement.normalized; // tránh đi chéo nhanh hơn
    }

    void FixedUpdate()
    {
        // Di chuyển bằng Rigidbody2D
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("End")) return;
        mazeGenerator.Win();
        mazeGenerator.win = true;
    }
}
