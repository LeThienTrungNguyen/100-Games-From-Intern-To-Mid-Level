using UnityEngine;

public class FlashlightSystem : MonoBehaviour
{
    [Header("References")]
    public Light flashlightLight; // Kéo Spot Light gắn trên Camera của Player vào đây

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.F;
    
    private bool isOn = false;

    private void Start()
    {
        if (flashlightLight != null) flashlightLight.enabled = false;
    }

    private void Update()
    {
        if (PlayerStats.Instance == null) return;

        // Hiện/Ẩn UI Đèn pin dựa trên việc đã mua chưa
        if (UIManager.Instance.flashlightUIPanel != null)
            UIManager.Instance.flashlightUIPanel.SetActive(PlayerStats.Instance.hasFlashlight);

        if (!PlayerStats.Instance.hasFlashlight) return;

        // Bật/Tắt đèn
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFlashlight();
        }

        if (isOn)
        {
            HandleBattery();
        }

        // Cập nhật UI
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (UIManager.Instance == null) return;

        if (UIManager.Instance.batteryCountTxt != null)
            UIManager.Instance.batteryCountTxt.text = $"x{PlayerStats.Instance.batteryCount}";

        if (UIManager.Instance.batteryEnergyTxt != null)
        {
            float ratio = PlayerStats.Instance.currentBattery / PlayerStats.Instance.maxBattery;
            int percentage = Mathf.CeilToInt(ratio * 100f);
            
            // Format: "Energy: 85%"
            UIManager.Instance.batteryEnergyTxt.text = $"{percentage}%";

            // Đổi màu chữ dựa trên lượng pin
            if (ratio < 0.2f) UIManager.Instance.batteryEnergyTxt.color = Color.red;
            else if (ratio < 0.5f) UIManager.Instance.batteryEnergyTxt.color = Color.yellow;
            else UIManager.Instance.batteryEnergyTxt.color = Color.black;
        }
    }

    public void ToggleFlashlight()
    {
        // Nếu đang tắt và muốn bật nhưng hết pin
        if (!isOn && PlayerStats.Instance.currentBattery <= 0)
        {
            if (!TryAutoRecharge()) 
            {
                Debug.Log("<color=red>Đèn pin hết sạch năng lượng và không còn pin dự phòng!</color>");
                return; 
            }
        }

        isOn = !isOn;
        if (flashlightLight != null) flashlightLight.enabled = isOn;
        Debug.Log($"<color=cyan>Đèn pin: {(isOn ? "BẬT" : "TẮT")}</color>");
    }

    private void HandleBattery()
    {
        // Giảm pin theo thời gian
        PlayerStats.Instance.currentBattery -= PlayerStats.Instance.batteryDrainRate * Time.deltaTime;

        if (PlayerStats.Instance.currentBattery <= 0)
        {
            PlayerStats.Instance.currentBattery = 0;
            
            // Thử nạp pin tự động
            if (!TryAutoRecharge())
            {
                isOn = false;
                if (flashlightLight != null) flashlightLight.enabled = false;
                Debug.Log("<color=red>Đèn pin đã tắt do hết năng lượng!</color>");
            }
        }
    }

    private bool TryAutoRecharge()
    {
        if (PlayerStats.Instance.batteryCount > 0)
        {
            PlayerStats.Instance.batteryCount--;
            PlayerStats.Instance.currentBattery = PlayerStats.Instance.maxBattery;
            Debug.Log($"<color=green>Đã tự động nạp pin mới! Còn lại: {PlayerStats.Instance.batteryCount} cục pin.</color>");
            return true;
        }
        return false;
    }
}