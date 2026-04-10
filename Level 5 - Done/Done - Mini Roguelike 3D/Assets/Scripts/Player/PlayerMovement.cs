using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float gravity = -19.62f; // Trọng lực mạnh hơn mặc định để cảm giác FPS thật hơn
    public float jumpHeight = 2f;
    [Header("Climb Settings")]
    public float climbSpeed = 5f;
    private int ladderContactCount = 0; // Đếm số lượng thang đang chạm
    private bool isClimbing => ladderContactCount > 0;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (UIManager.Instance != null && (UIManager.Instance.IsUIOpen || UIManager.Instance.IsPlayerLocked))
            return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Dùng GetAxisRaw để kiểm tra phím bấm thực tế, giúp dừng âm thanh ngay lập tức
        float rawX = Input.GetAxisRaw("Horizontal");
        float rawZ = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // XỬ LÝ LEO THANG
        if (isClimbing)
        {
            velocity.y = 0; // Vô hiệu hóa trọng lực khi đang ở trên thang
            if (Input.GetKey(KeyCode.W))
            {
                move += Vector3.up * (climbSpeed / moveSpeed); 
            }
            else if (Input.GetKey(KeyCode.S))
            {
                move -= Vector3.up * (climbSpeed / moveSpeed);
            }
        }
        else
        {
            // Chỉ áp dụng trọng lực và nhảy nếu KHÔNG leo thang
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            velocity.y += gravity * Time.deltaTime;
        }

        float currentMoveSpeed = moveSpeed;
        if (PlayerStats.Instance != null) currentMoveSpeed *= PlayerStats.Instance.speedMultiplier;

        Vector3 moveDelta = move * currentMoveSpeed;
        Vector3 finalMove = moveDelta + velocity;
        controller.Move(finalMove * Time.deltaTime);

        // XỬ LÝ ÂM THANH BƯỚC CHÂN
        // Dùng Raw để âm thanh dừng KHÔNG trễ (GetAxis thường có delay trượt 0.1s - 0.2s)
        bool isMovingKeysPressed = Mathf.Abs(rawX) > 0f || Mathf.Abs(rawZ) > 0f;

        if (isGrounded && isMovingKeysPressed)
        {
            if (AudioManager.Instance != null)
            {
                // PlayFootstepSound đã có logic check isPlaying, nên nó sẽ đợi âm thanh cũ chạy hết
                AudioManager.Instance.PlayFootstepSound();
            }
        }
        else
        {
            if (AudioManager.Instance != null)
            {
                // Dừng ngay lập tức khi không bấm phím di chuyển hoặc không chạm đất
                AudioManager.Instance.StopFootstepSound();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            LadderController lc = other.GetComponent<LadderController>();
            // Chỉ leo nếu thang không ở trạng thái pending (hoặc không có script điều khiển thì mặc định cho leo)
            if (lc == null || !lc.isPending)
            {
                ladderContactCount++;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            LadderController lc = other.GetComponent<LadderController>();
            if (lc == null || !lc.isPending)
            {
                ladderContactCount = Mathf.Max(0, ladderContactCount - 1);
            }
        }
    }
}