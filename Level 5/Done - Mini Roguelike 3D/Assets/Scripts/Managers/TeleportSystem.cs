using UnityEngine;

public class TeleportSystem : MonoBehaviour
{
    public static TeleportSystem Instance;

    [Header("Settings")]
    public Transform surfacePoint; // Điểm hồi phục trên mặt đất
    public KeyCode teleportKey = KeyCode.P;

    [Header("Status")]
    public bool canTeleportToday = true; // Giới hạn 1 lần/ngày

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(teleportKey))
        {
            TryTeleport();
        }
    }

    public void TryTeleport()
    {
        if (!canTeleportToday)
        {
            Debug.Log("<color=red>CẢNH BÁO: Bạn đã dùng hết lượt cứu hộ hôm nay! Hãy tự leo lên.</color>");
            return;
        }
        UIManager.Instance.IsUIOpen = true;
        DotweenAnimationName.Instance.DoBlinkEffect(UIManager.Instance.top, UIManager.Instance.bottom,0.5f , true , ExecuteTeleport, ()=> { UIManager.Instance.IsUIOpen = false; });
        //ExecuteTeleport();
    }

    public void ExecuteTeleport()
    {
        if (surfacePoint == null)
        {
            Debug.LogError("Chưa gán Surface Point cho hệ thống Teleport!");
            return;
        }
        CharacterController cc = GetComponent<CharacterController>();

        // 1. Tạm thời tắt bộ điều khiển vật lý
        if (cc != null) cc.enabled = false;
        // Thực hiện dịch chuyển
        transform.position = surfacePoint.position;
        canTeleportToday = false; // Khóa lượt dùng
        
        // 4. Bật lại bộ điều khiển vật lý
        if (cc != null) cc.enabled = true;
        Debug.Log("<color=cyan>HỆ THỐNG CỨU HỘ: Đã đưa bạn về mặt đất an toàn. Lượt dùng hôm nay: 0/1.</color>");
    }

    // Hàm này sẽ được gọi bởi KPIManager khi sang ngày mới
    public void ResetTeleportDaily()
    {
        canTeleportToday = true;
        Debug.Log("<color=green>HỆ THỐNG CỨU HỘ: Đã làm mới lượt dùng cho ngày mới.</color>");
    }
}