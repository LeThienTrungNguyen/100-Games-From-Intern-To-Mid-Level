using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private float moveSpeed = 5f; // tốc độ di chuyển

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        // Tìm GameController trong scene
        gameController = FindFirstObjectByType(typeof(GameController)) as GameController;

        // Lấy Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("⚠️ PlayerController cần một Rigidbody2D để hoạt động!");
        }
    }

    void Update()
    {
        // Lấy input WASD
        moveInput.x = Input.GetAxisRaw("Horizontal"); // A/D hoặc ← →
        moveInput.y = Input.GetAxisRaw("Vertical");   // W/S hoặc ↑ ↓
        moveInput.Normalize(); // giữ tốc độ đều khi di chuyển chéo
    }

    void FixedUpdate()
    {
        // Di chuyển player
        if (rb != null)
            rb.velocity = moveInput * moveSpeed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("End"))
        {
            Debug.Log("🏁 Player reached the end!");
            rb.velocity = Vector2.zero;
            Invoke(nameof(NextScene), 2f);
        }
    }

    void NextScene()
    {
        gameController.NextLevel();
    }
}
