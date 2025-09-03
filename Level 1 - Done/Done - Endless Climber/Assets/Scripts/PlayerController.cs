using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb2d;
    public GameController gc;
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        grounded = IsGround();
        Movement();
        Jump();
        Dash();
    }

    public float mspeed; // move speed
    public float jheight; // jump height
    public bool canJump;
    public int jumpCount = 1;
    [SerializeField] bool grounded;

    // ===== DASH SETTINGS =====
    public float dashSpeed = 15f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;

    private bool isDashing = false;
    private float dashCooldownTimer = 0f;
    private float dashTimer = 0f;
    private int dashDirection = 0;

    public void Dash()
    {
        // đếm ngược cooldown
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;

        // nếu đang dash
        if (isDashing)
        {
            rb2d.velocity = new Vector2(dashDirection * dashSpeed, 0f);
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0)
            {
                isDashing = false;
                dashCooldownTimer = dashCooldown; // bắt đầu cooldown
            }
            return; // bỏ qua input khác khi đang dash
        }

        // bắt đầu dash
        if (Input.GetKeyDown(KeyCode.L) && dashCooldownTimer <= 0)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            if (horizontal != 0)
                dashDirection = (int)Mathf.Sign(horizontal); // dash theo hướng đang nhấn
            else
                dashDirection = transform.localScale.x > 0 ? 1 : -1; // nếu không nhấn thì dash theo hướng đang quay

            isDashing = true;
            dashTimer = dashTime;
        }
    }

    public void Jump()
    {
        if (grounded) { canJump = true; jumpCount = 1; }
        if (jumpCount == 0) canJump = false;
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jheight);
            jumpCount--;
        }
    }

    public void Movement()
    {
        if (isDashing) return; // không cho điều khiển khi dash
        float horizontal = Input.GetAxisRaw("Horizontal");
        rb2d.velocity = new Vector2(horizontal * mspeed, rb2d.velocity.y);
    }

    public LayerMask groundMask;
    public bool IsGround()
    {
        //return Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - 0.5f), new Vector2(0.45f, 0.1f), 0, groundMask);
        // Lấy tất cả collider bị overlap
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            new Vector2(transform.position.x, transform.position.y - 0.5f),
            new Vector2(0.45f, 0.1f),
            0,
            groundMask
        );

        // Kiểm tra trong danh sách có collider nào không phải trigger
        foreach (Collider2D col in hits)
        {
            if (col != null && col.isTrigger == false)
            {
                return true;
            }
        }

        return false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Trap"))
        {
            gc.GameOver();
        }
    }
}
