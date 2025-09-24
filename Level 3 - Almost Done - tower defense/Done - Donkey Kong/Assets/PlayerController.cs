using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;      // quản lý UI
using UnityEngine.SceneManagement; // nếu muốn restart lại scene

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;       
    public float jumpForce = 7f;       
    private float horizontalInput;
    public float scale = 1f;

    [Header("Ground Check")]
    public Transform groundCheck;      
    public float groundRadius = 0.2f;  
    public LayerMask groundLayer;      
    private bool isGrounded;

    [Header("Ladder")]
    public float climbSpeed = 4f;
    private bool isClimbing;
    private float verticalInput;

    private Rigidbody2D rb;
    private Animator anim;

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject winPanel;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); 

        // Ẩn panel lúc đầu
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    void Update()
    {
        // Input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Kiểm tra đang đứng trên đất
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // Nhảy
        if (Input.GetButtonDown("Jump") && isGrounded && !isClimbing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Animation
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(horizontalInput));
            anim.SetBool("isGrounded", isGrounded);
            anim.SetBool("isClimbing", isClimbing);
        }

        if (Input.GetKeyDown(KeyCode.R)) Restart();
    }

    void FixedUpdate()
    {
        // Di chuyển trái/phải
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        // Nếu đang leo thang
        if (isClimbing)
        {
            rb.velocity = new Vector2(rb.velocity.x, verticalInput * climbSpeed);
            rb.gravityScale = 0f; 
        }
        else
        {
            rb.gravityScale = 1f; 
        }

        // Lật sprite
        if (horizontalInput > 0.1f)
            transform.localScale = new Vector3(1 * scale, 1 * scale, 1);
        else if (horizontalInput < -0.1f)
            transform.localScale = new Vector3(-1 * scale, 1 * scale, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = true;
        }
        
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Barrel"))
        {
            GameOver();
        }
        if (collision.transform.CompareTag("Princess"))
        {
            Win();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isClimbing = false;
        }
    }

    // ---------------- UI Functions ----------------

    public void GameOver()
    {
        Time.timeScale = 0;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void Win()
    {
        Time.timeScale = 0;
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        // Tắt cả 2 panel
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        // Nếu bạn muốn reload scene:
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
