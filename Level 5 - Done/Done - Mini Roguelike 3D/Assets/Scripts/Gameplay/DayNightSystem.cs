using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    [Header("References")]
    public Light sunLight; // Kéo Directional Light của bạn vào đây

    [Header("Light Settings")]
    public Gradient sunColor; // Gradient màu từ sáng đến chiều tối
    public float startIntensity = 0.8f; // Cường độ lúc 8:00 sáng
    public float maxIntensity = 1.5f;   // Cường độ cao nhất (12:00 trưa)
    public float minIntensity = 0.2f;   // Cường độ thấp nhất (18:00 tối)
    public AnimationCurve intensityCurve; // Không dùng nữa nhưng giữ lại để tránh lỗi Inspector nếu cần

    [Header("Rotation Settings (24h Logic: 15°/hour)")]
    public float noonAngle = 90f;   // 12:00 trưa - Đỉnh đầu
    public float nightAngle = 225f; // 21:00 tối - Sau 9 tiếng (9 * 15 = 135 độ từ 12:00)

    private void Update()
    {
        if (TimeManager.Instance == null || sunLight == null) return;

        // Lấy tiến độ ngày (0.0 -> 1.0)
        float progress = TimeManager.Instance.GetDayProgress();

        // 1. Xoay mặt trời theo logic 24h: Từ 90 (trưa) đến 225 (tối)
        float currentAngle = Mathf.Lerp(noonAngle, nightAngle, progress);
        sunLight.transform.rotation = Quaternion.Euler(new Vector3(currentAngle, -30f, 0f));

        // 2. Cập nhật màu sắc (Dùng Gradient)
        if (sunColor != null)
        {
            sunLight.color = sunColor.Evaluate(progress);
        }

        // 3. Cường độ sáng:
        // 12:00 (progress 0) -> Max Intensity (1.5)
        // 18:00 (progress 0.66) -> Bắt đầu giảm mạnh về Min
        // 21:00 (progress 1) -> Min Intensity (0.1)
        float intensity;
        if (progress < 0.66f) // Trước 18:00
            intensity = maxIntensity; 
        else // Sau 18:00 bắt đầu lặn
            intensity = Mathf.Lerp(maxIntensity, minIntensity, (progress - 0.66f) / 0.34f);

        sunLight.intensity = intensity;

        // 4. Đồng bộ với Ambient Light
        RenderSettings.ambientLight = sunLight.color * sunLight.intensity * 0.5f;
    }
}