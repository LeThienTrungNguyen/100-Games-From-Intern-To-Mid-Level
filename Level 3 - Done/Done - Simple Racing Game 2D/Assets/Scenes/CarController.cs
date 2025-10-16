using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float acceleration = 8f;      // Lực tăng tốc
    public float maxSpeed = 12f;         // Tốc độ tối đa
    public float steering = 2.5f;        // Độ nhạy khi quay
    public float drag = 0.98f;           // Ma sát (tự giảm tốc)

    [Header("Runtime Info")]
    public float throttle;  // -1..1 (lùi / ga)
    public float steer;     // -1..1 (trái / phải)
    
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.drag = 0;   // Tắt drag mặc định, ta tự tính
        rb.angularDrag = 0;
    }

    void FixedUpdate()
{
    Vector2 forward = transform.up;
    Vector2 right = transform.right;

    // Phân tích vận tốc thành 2 thành phần
    float forwardVel = Vector2.Dot(rb.velocity, forward);
    float rightVel = Vector2.Dot(rb.velocity, right);

    // Giảm vận tốc ngang (làm xe bám đường hơn)
    float sideFriction = 0.9f; // 0.0 = trơn hoàn toàn, 1.0 = bám tuyệt đối
    rb.velocity = forward * forwardVel + right * (rightVel * sideFriction);

    // Tăng tốc
    rb.velocity += forward * throttle * acceleration * Time.fixedDeltaTime;

    // Giới hạn tốc độ
    rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);

    // Quay xe tỉ lệ với tốc độ
    float effectiveSpeed = Mathf.Max(rb.velocity.magnitude, 2f);
    float steerAmount = steer * steering * effectiveSpeed * 0.02f;
    rb.rotation -= steerAmount;

    // Ma sát dọc (giảm dần khi thả ga)
    rb.velocity *= drag;
}
public void SetInput(float throttleInput, float steerInput)
{
    throttle = Mathf.Clamp(throttleInput, -1f, 1f);
    steer = Mathf.Clamp(steerInput, -1f, 1f);
}

}
