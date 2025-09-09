using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;

    public float gravity = 9.81f;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float maxSpeed = 8f;

    public Vector2 gravityDirection;
    public Vector3 rotationDirection;

    private bool isRotating = false;
   [SerializeField] bool grounded = false;
    void Awake()
    {
        gravityDirection = Vector2.down;
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0; // disable gravity mặc định
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reload scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
        ;
            grounded = CheckGround();
        if (!isRotating) // khi đang xoay thì disable input
        {
            Movement();
            //Jump();
        }
        if(grounded)GravitySwitch();
        
    }

    void GravitySwitch()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) ApplyGravityRotation(180);
        if (Input.GetKeyDown(KeyCode.DownArrow)) ApplyGravityRotation(-180);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) ApplyGravityRotation(-90);
        if (Input.GetKeyDown(KeyCode.RightArrow)) ApplyGravityRotation(90);
    }
    public float groundCheckRadius = 0.1f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    bool CheckGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    void ApplyGravityRotation(float angle)
    {
        if (isRotating) return; // không xoay khi đang xoay

        gravityDirection = GetVectorAfterRotate(gravityDirection, angle);
        rotationDirection += new Vector3(0, 0, angle);

        StartCoroutine(RotateSafe(rotationDirection, 0.2f));
    }

    IEnumerator RotateSafe(Vector3 targetRotation, float duration)
    {
        isRotating = true;

        // Tạm dừng physics
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;

        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(targetRotation);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            Camera.main.transform.rotation = transform.rotation;
            yield return null;
        }

        // Bật lại physics
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0; // để giữ gravity custom
        isRotating = false;
    }

    void Movement()
    {
        if (!grounded) return;
        Vector2 rightVel = GetRightByVelocityAndRotation(rb.velocity, transform.rotation);
        Vector2 upVel = GetUpByVelocityAndRotation(rb.velocity, transform.rotation);

        if (Input.GetKey(KeyCode.A))
        {
            rightVel = transform.right * -moveSpeed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rightVel = transform.right * moveSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            rightVel = Vector2.zero;
        }

        rb.velocity = rightVel + upVel;
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up * jumpForce * 50, ForceMode2D.Force);
        }
    }

    void FixedUpdate()
    {
        if (rb != null && !isRotating)
        {
            Vector2 rightVel = GetRightByVelocityAndRotation(rb.velocity, transform.rotation);
            Vector2 upVel = GetUpByVelocityAndRotation(rb.velocity, transform.rotation);

            // Thêm gravity vào upVel
            upVel += (Vector2)transform.up * -gravity * Time.fixedDeltaTime;

            rb.velocity = rightVel + upVel;
        }
    }

    // ------------------- Utility -------------------
    Vector2 GetVectorAfterRotate(Vector2 v, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    Vector2 GetRightByVelocityAndRotation(Vector2 playerVelocity, Quaternion playerRotation)
    {
        Vector2 right = playerRotation * Vector2.right;
        float rightValue = Vector2.Dot(playerVelocity, right);
        return right * rightValue;
    }

    Vector2 GetUpByVelocityAndRotation(Vector2 playerVelocity, Quaternion playerRotation)
    {
        Vector2 up = playerRotation * Vector2.up;
        float upValue = Vector2.Dot(playerVelocity, up);
        return up * upValue;
    }
}
