using UnityEngine;
using System.Collections;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("Settings")]
    [Tooltip("One in-game day duration in real-life minutes")]
    public float dayDurationMinutes = 20f;
    
    [Header("Current Status")]
    public int currentDay = 1;
    public float timeRemaining;
    private bool isTimerRunning = false;

    [Header("UI References")]
    public TextMeshProUGUI dayTxt;
    public TextMeshProUGUI timerTxt;
    public GameObject startDayTrigger; // Kéo GameObject "Start New Day Trigger" vào đây

    private Color originalTimerColor;
    private float lastPulseTime = 0f;
    private bool showWorkAreaWarning = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (timerTxt != null) originalTimerColor = timerTxt.color;
    }

    private void Start()
    {
        PrepareDay(); // Chỉ chuẩn bị dữ liệu, không chạy giờ
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;

                // Hiệu ứng Pulse trong 10 giây cuối (Real-time)
                if (timeRemaining <= 10f)
                {
                    if (Time.time - lastPulseTime >= 1.0f)
                    {
                        lastPulseTime = Time.time;
                        if (timerTxt != null && DotweenAnimationName.Instance != null)
                        {
                            DotweenAnimationName.Instance.DoPulseEffect(timerTxt.rectTransform, 1f, 1.3f, 0.3f);
                        }
                    }
                }
            }
            else
            {
                isTimerRunning = false;
                timeRemaining = 0;
                OnDayEnd();
            }
        }

        // Cập nhật UI liên tục để xử lý hiệu ứng cầu vồng và cảnh báo
        UpdateTimerUI();
    }

    public void SetWorkAreaWarning(bool show) => showWorkAreaWarning = show;

    /// <summary>
    /// Chuẩn bị các thông số cho ngày mới nhưng chưa bắt đầu đếm ngược.
    /// </summary>
    public void PrepareDay()
    {
        if (QuestManager.Instance != null) 
        {
            QuestManager.Instance.OnNewDayStarted();
        }
        else
        {
            Debug.LogError("<color=red>[TimeManager] KHÔNG TÌM THẤY QuestManager! Hãy gắn script QuestManager vào đối tượng Managers trong Scene.</color>");
        }

        timeRemaining = dayDurationMinutes * 60f;
        isTimerRunning = false;
        
        // Kiểm tra UI có bị mất tham chiếu không
        if (UIManager.Instance != null) UIManager.Instance.ValidateReferences();

        // Hiện lại Trigger cho ngày mới
        if (startDayTrigger != null) startDayTrigger.SetActive(true);

        // Xuất hiện hộp hàng nếu có
        if (DeliveryManager.Instance != null)
        {
            DeliveryManager.Instance.SpawnDeliveryBox();
        }

        UpdateDayUI();
        UpdateTimerUI();
        Debug.Log($"<color=cyan>Đã chuẩn bị Ngày {currentDay}. Chờ lệnh StartDay...</color>");
    }

    /// <summary>
    /// Kích hoạt bắt đầu tính giờ làm việc.
    /// </summary>
    [ContextMenu("Start Day")]
    public void StartDay()
    {
        if (isTimerRunning) return;
        
        isTimerRunning = true;

        // Ẩn Trigger khi đã bắt đầu ngày
        if (startDayTrigger != null) startDayTrigger.SetActive(false);

        Debug.Log($"<color=green>Bắt đầu làm việc Ngày {currentDay}! Đồng hồ đã chạy.</color>");
    }

    public void NextDay()
    {
        currentDay++;
        PrepareDay();
    }

    public bool IsTimerRunning() => isTimerRunning;

    /// <summary>
    /// Trả về tiến độ của ngày (0.0 lúc 8:00 sáng, 1.0 lúc 18:00 tối)
    /// </summary>
    public float GetDayProgress()
    {
        float totalSeconds = dayDurationMinutes * 60f;
        float elapsedSeconds = totalSeconds - timeRemaining;
        return Mathf.Clamp01(elapsedSeconds / totalSeconds);
    }

    public void OnDayEnd()
    {
        if (!isTimerRunning && timeRemaining > 0) return; // Tránh gọi 2 lần nếu đã kết thúc rồi

        isTimerRunning = false;
        Debug.Log("<color=orange>Kích hoạt quy trình kết thúc ngày...</color>");
        
        StartCoroutine(Co_FullEndDayFlow());
    }

    private IEnumerator Co_FullEndDayFlow()
    {
        // 1. Khóa tương tác người chơi ngay lập tức
        if (UIManager.Instance != null) UIManager.Instance.IsPlayerLocked = true;

        // 2. Hiển thị bảng lương và đợi nó đóng
        bool reportFinished = false;
        if (KPIManager.Instance != null)
        {
            KPIManager.Instance.ShowSalaryReport(() => {
                reportFinished = true;
            });
        }
        else reportFinished = true;

        yield return new WaitUntil(() => reportFinished);
        yield return new WaitForSeconds(0.5f);

        // 3. Thực hiện hiệu ứng Blink (Nhắm mắt - Sang ngày mới - Mở mắt)
        if (UIManager.Instance != null && DotweenAnimationName.Instance != null && 
            UIManager.Instance.top != null && UIManager.Instance.bottom != null)
        {
            bool daySwitchTriggered = false;
            
            // Gọi Blink: Action1 là khi mắt đã nhắm hoàn toàn
            DotweenAnimationName.Instance.DoBlinkEffect(
                UIManager.Instance.top, 
                UIManager.Instance.bottom, 
                1.2f, // Giảm từ 2.5f xuống 1.2f để nhanh hơn
                true, 
                () => {
                    // KHI MẮT ĐANG NHẮM: Thực hiện các thay đổi sang ngày mới
                    if (KPIManager.Instance != null) KPIManager.Instance.EndDay();
                    daySwitchTriggered = true;
                    Debug.Log("<color=yellow>[TimeManager] Day switched while eyes closed.</color>");
                },
                () => {
                    // KHI MẮT ĐÃ MỞ LẠI XONG
                    if (UIManager.Instance != null) UIManager.Instance.IsPlayerLocked = false;
                    Debug.Log("<color=green>[TimeManager] Blink completed. New day started!</color>");
                }
            );

            // Đợi cho đến khi lệnh chuyển ngày được thực thi bên trong Blink
            yield return new WaitUntil(() => daySwitchTriggered);
        }
        else
        {
            // Fallback nếu không có UI Blink
            if (KPIManager.Instance != null) KPIManager.Instance.EndDay();
            if (UIManager.Instance != null) UIManager.Instance.IsPlayerLocked = false;
        }
    }

    private void UpdateTimerUI()
    {
        if (timerTxt == null) return;
        
        // Tính toán tỷ lệ thời gian đã trôi qua (từ 0 đến 1)
        float totalSeconds = dayDurationMinutes * 60f;
        float elapsedSeconds = totalSeconds - timeRemaining;
        float progress = Mathf.Clamp01(elapsedSeconds / totalSeconds);

        // Quy đổi ra giờ trong game (từ 12:00 đến 21:00 = 9 tiếng)
        float startHour = 12f;
        float totalGameHours = 9f;
        float currentGameTimeInHours = startHour + (progress * totalGameHours);

        int hours = Mathf.FloorToInt(currentGameTimeInHours);
        int minutes = Mathf.FloorToInt((currentGameTimeInHours - hours) * 60f);

        // Hiển thị định dạng HH:mm
        string timeStr = string.Format("{0:00}:{1:00}", hours, minutes);

        // Thêm chữ "Shift Started" với hiệu ứng cầu vồng trong 10 giây đầu (Real-time)
        if (isTimerRunning && elapsedSeconds <= 10f)
        {
            float hue = (Time.time * 1.5f) % 1f; // Tốc độ xoay vòng màu (1.5f)
            Color rainbowColor = Color.HSVToRGB(hue, 0.8f, 1f); // 0.8f saturation cho màu đẹp hơn
            string hexColor = ColorUtility.ToHtmlStringRGB(rainbowColor);
            timeStr += $" <color=#{hexColor}><size=80%> Shift Started</size></color>";
        }
        else if (!isTimerRunning && showWorkAreaWarning)
        {
            // Cảnh báo màu nóng (OrangeRed) khi chưa bắt đầu ca làm, kích thước nhỏ hơn 80%
            timeStr += " <color=#FF4500><size=80%> Please go to Start Working area !!</size></color>";
        }

        timerTxt.text = timeStr;

        // Đổi màu nếu sắp hết giờ (sau 17:00 / còn dưới 10% thời gian)
        if (progress > 0.9f) timerTxt.color = Color.red;
        else timerTxt.color = originalTimerColor;
    }

    private void UpdateDayUI()
    {
        if (dayTxt != null) dayTxt.text = $"Ngày: {currentDay}";
    }
    
    // Hàm để dừng/tiếp tục timer khi mở UI (tùy chọn)
    public void SetTimerActive(bool active) => isTimerRunning = active;
}