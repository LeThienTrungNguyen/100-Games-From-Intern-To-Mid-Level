using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public LayerMask groundLayer;
    public Transform groundCheck; // tạo 1 empty object dưới chân player, gán vào đây
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        Jump();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Target")) NextLevel();
    }
    
    void NextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Nếu chưa tới màn cuối thì load màn kế tiếp
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Đã đến màn cuối cùng!");
            // Có thể load lại màn đầu tiên:
            // SceneManager.LoadScene(0);
        }
    }
    void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal"); // A/D hoặc phím mũi tên
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        
    }

    void Jump()
    {
        // kiểm tra có chạm đất không
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    // các hàm có sẵn
    public void Inactive()
    {
        rb.bodyType = RigidbodyType2D.Static;
        enabled = false;
    }

    public void Active()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        enabled = true;
    }

    // để hiển thị vùng kiểm tra chạm đất trong Scene View
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
