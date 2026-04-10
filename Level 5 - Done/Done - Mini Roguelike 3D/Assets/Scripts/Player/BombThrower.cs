using UnityEngine;

public class BombThrower : MonoBehaviour
{
    [Header("References")]
    public GameObject bombPrefab;
    public Transform throwPoint; // Điểm ném (thường là Camera hoặc tay nhân vật)

    [Header("Settings")]
    public float throwForce = 15f;
    public KeyCode throwKey = KeyCode.G;

    void Update()
    {
        // 1. Kiểm tra phím bấm và số lượng bom còn lại
        if (Input.GetKeyDown(throwKey))
        {
            // Kiểm tra số lượng bom
            if (PlayerStats.Instance == null || PlayerStats.Instance.bombCount <= 0) return;

            // Kiểm tra trạng thái làm việc (Chỉ được ném khi đã bắt đầu ca làm)
            if (TimeManager.Instance != null && !TimeManager.Instance.IsTimerRunning())
            {
                Debug.Log("<color=red>Chưa bắt đầu ca làm việc! Không thể ném bom.</color>");
                return;
            }

            // Không cho ném khi đang mở UI hoặc bị khóa
            if (UIManager.Instance != null && (UIManager.Instance.IsUIOpen || UIManager.Instance.IsPlayerLocked)) 
                return;

            ThrowBomb();
        }
    }

    public LayerMask chunkLayer; // Gán Layer chứa các khối mỏ vào đây

    void ThrowBomb()
    {
        if (throwPoint == null) { Debug.LogError("BombThrower: Thiếu ThrowPoint (hãy kéo Camera vào)!"); return; }

        // 2. Trừ số lượng bom
        PlayerStats.Instance.bombCount--;
        Debug.Log($"<color=orange>Đã ném bom! Còn lại: {PlayerStats.Instance.bombCount}</color>");

        if (AudioManager.Instance != null) AudioManager.Instance.PlayBombThrowSound(throwPoint.position);

        // 3. Xác định hướng ném
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;
        
        // CHỈ Raycast vào các khối (Layer Chunk), bỏ qua Player và Loot để tránh ném ngược
        if (Physics.Raycast(ray, out RaycastHit hit, 50f, chunkLayer))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(20f);
        }

        Vector3 throwDirection = (targetPoint - throwPoint.position).normalized;

        // 4. Tạo quả bom
        GameObject bombObj = Instantiate(bombPrefab, throwPoint.position, Quaternion.LookRotation(throwDirection));
        
        // --- NEW: Bỏ qua va chạm với Player ---
        Collider bombCollider = bombObj.GetComponent<Collider>();
        Collider playerCollider = GetComponent<Collider>(); // Giả định script này nằm trên cùng GameObject có Collider của Player
        
        if (bombCollider != null && playerCollider != null)
        {
            Physics.IgnoreCollision(bombCollider, playerCollider);
        }
        // ---------------------------------------

        Rigidbody rb = bombObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }

        // 5. Kích hoạt logic nổ với bán kính hiện tại từ Stats
        Bomb bombScript = bombObj.GetComponent<Bomb>();
        if (bombScript != null)
        {
            int currentRadius = PlayerStats.Instance.GetCurrentBombRadius();
            bombScript.Activate(currentRadius);
            Debug.Log($"<color=cyan>Bom được kích hoạt với bán kính: {currentRadius}</color>");
        }
    }
}