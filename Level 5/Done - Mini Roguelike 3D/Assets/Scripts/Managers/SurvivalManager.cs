using UnityEngine;
using System.Collections;

public class SurvivalManager : MonoBehaviour
{
    public static SurvivalManager Instance;

    [Header("Fall Damage Settings")]
    public float minFallDistance = 4f;
    public float damageMultiplier = 15f;
    private float highestYPoint;
    private bool isFalling;

    [Header("Hunger & Thirst Settings")]
    public float miningDrainPenalty = 0.05f; // Giảm mạnh mức phạt khi đào

    private CharacterController characterController;

    private void Awake()
    {
        Instance = this;
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleFallDamage();
        HandleTimedDrain();
        HandleHealthEffects();
        HandlePenalties();
        CheckDeath();
    }

    private void HandlePenalties()
    {
        if (PlayerStats.Instance == null) return;

        float hpPercent = PlayerStats.Instance.currentHealth / PlayerStats.Instance.maxHealth;
        float hunger = PlayerStats.Instance.currentHunger;

        // 1. TỐC ĐỘ DI CHUYỂN (Chỉ bị ảnh hưởng bởi ĐÓI)
        // Nếu đói bằng 0 -> Giảm 50% tốc độ
        PlayerStats.Instance.speedMultiplier = (hunger <= 0) ? 0.5f : 1.0f;

        // 2. KHẢ NĂNG ĐÀO MỎ (Chỉ bị ảnh hưởng bởi MÁU)
        if (hpPercent < 0.1f) // Máu quá thấp (dưới 10%) -> CẤM ĐÀO
        {
            PlayerStats.Instance.canMine = false;
            PlayerStats.Instance.miningSpeedMultiplier = 0f;
        }
        else if (hpPercent < 0.3f) // Máu hơi thấp (dưới 30%) -> GIẢM 50% SÁT THƯƠNG ĐÀO
        {
            PlayerStats.Instance.canMine = true;
            PlayerStats.Instance.miningSpeedMultiplier = 0.5f;
        }
        else // Máu ổn định -> ĐÀO BÌNH THƯỜNG
        {
            PlayerStats.Instance.canMine = true;
            PlayerStats.Instance.miningSpeedMultiplier = 1.0f;
        }
    }

    private void HandleHealthEffects()
    {
        if (PlayerStats.Instance == null || UIManager.Instance == null) return;

        float hpPercent = PlayerStats.Instance.currentHealth / PlayerStats.Instance.maxHealth;

        if (UIManager.Instance.lowHealthOverlay != null)
        {
            float targetAlpha = (hpPercent < 0.3f) ? (1f - (hpPercent / 0.3f)) * 0.6f : 0f;
            UIManager.Instance.lowHealthOverlay.alpha = Mathf.Lerp(UIManager.Instance.lowHealthOverlay.alpha, targetAlpha, Time.deltaTime * 2f);
        }

        if (UIManager.Instance.heartbeatSFX != null)
        {
            if (hpPercent < 0.3f)
            {
                if (!UIManager.Instance.heartbeatSFX.isPlaying) UIManager.Instance.heartbeatSFX.Play();
                UIManager.Instance.heartbeatSFX.volume = (1f - (hpPercent / 0.3f));
                UIManager.Instance.heartbeatSFX.pitch = 1f + (1f - (hpPercent / 0.3f)) * 0.5f;
            }
            else
            {
                if (UIManager.Instance.heartbeatSFX.isPlaying) UIManager.Instance.heartbeatSFX.Stop();
            }
        }
    }

    private void CheckDeath()
    {
        if (PlayerStats.Instance != null && PlayerStats.Instance.currentHealth <= 0 && !PlayerStats.Instance.isDead)
        {
            StartCoroutine(Co_DeathSequence());
        }
    }

    private IEnumerator Co_DeathSequence()
    {
        PlayerStats.Instance.isDead = true;
        UIManager.Instance.IsPlayerLocked = true;
        
        // Tạm dừng đồng hồ
        if (TimeManager.Instance != null) TimeManager.Instance.SetTimerActive(false);

        // 1. Fade IN (Màn hình đen hoàn toàn)
        if (UIManager.Instance.blackScreenFade != null)
        {
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * 2.5f; // Tăng tốc độ Fade In một chút
                UIManager.Instance.blackScreenFade.alpha = t;
                yield return null;
            }
            UIManager.Instance.blackScreenFade.alpha = 1f;
        }

        yield return new WaitForSeconds(0.3f);

        // 2. RESET CHỈ SỐ NGAY TRONG LÚC ĐEN MÀN HÌNH (60% theo yêu cầu)
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.currentHealth = PlayerStats.Instance.maxHealth * 0.6f;
            PlayerStats.Instance.currentHunger = PlayerStats.Instance.maxHunger * 0.6f;
            PlayerStats.Instance.currentThirst = PlayerStats.Instance.maxThirst * 0.6f;
            Debug.Log("<color=green>[Survival] Stats reset to 60% during death transition.</color>");
        }

        // 3. Hiển thị bảng tổng kết lương (Salary Report)
        bool reportFinished = false;
        if (KPIManager.Instance != null)
        {
            KPIManager.Instance.ShowSalaryReport(() => {
                reportFinished = true;
            });
        }
        else reportFinished = true;

        // Chờ người chơi xem và bấm nút đóng bảng lương
        yield return new WaitUntil(() => reportFinished);
        yield return new WaitForSeconds(0.3f);

        // 4. Fade OUT (Màn hình đen mờ dần để quay lại ngày mới)
        if (UIManager.Instance.blackScreenFade != null)
        {
            float t = 1;
            while (t > 0f)
            {
                t -= Time.deltaTime * 2f;
                UIManager.Instance.blackScreenFade.alpha = t;
                yield return null;
            }
            UIManager.Instance.blackScreenFade.alpha = 0f;
        }

        // 5. SANG NGÀY MỚI
        if (KPIManager.Instance != null)
        {
            KPIManager.Instance.EndDay();
        }

        PlayerStats.Instance.isDead = false;
        UIManager.Instance.IsPlayerLocked = false;
        
        Debug.Log("<color=red>[Survival] Death flow completed. Starting new day.</color>");
    }

    private void HandleFallDamage()
    {
        if (characterController == null) return;

        if (!characterController.isGrounded)
        {
            if (!isFalling)
            {
                isFalling = true;
                highestYPoint = transform.position.y;
            }
            else
            {
                if (transform.position.y > highestYPoint) highestYPoint = transform.position.y;
            }
        }
        else
        {
            if (isFalling)
            {
                float fallDistance = highestYPoint - transform.position.y;
                if (fallDistance > minFallDistance)
                {
                    float damage = (fallDistance - minFallDistance) * damageMultiplier;
                    PlayerStats.Instance.ChangeHealth(-damage);
                }
                isFalling = false;
            }
        }
    }

    private void HandleTimedDrain()
    {
        if (TimeManager.Instance == null || !TimeManager.Instance.IsTimerRunning()) return;

        // --- TÍNH TOÁN TỐC ĐỘ BÙ TRỪ HỆ SỐ BUỔI TRƯA ---
        float totalSecondsInDay = TimeManager.Instance.dayDurationMinutes * 60f;
        float oneGameHourInSeconds = totalSecondsInDay / 9f;

        // Để đạt 5 tiếng khi bị nhân 1.2x, ta cần cơ sở là 100đ / 6 tiếng game
        // 100 / 6 = 16.66 điểm mỗi tiếng game
        float hungerBaseRate = 16.66f / oneGameHourInSeconds;
        
        // Để đạt 3 tiếng khi bị nhân 1.2x, ta cần cơ sở là 100đ / 3.6 tiếng game
        // 100 / 3.6 = 27.77 điểm mỗi tiếng game
        float thirstBaseRate = 27.77f / oneGameHourInSeconds;

        float progress = TimeManager.Instance.GetDayProgress();
        float timeMultiplier = 1.0f;

        // Buổi trưa (12:00 - 16:00)
        if (progress <= (4f / 9f)) 
        {
            timeMultiplier = 1.2f; 
        }

        // Thực hiện trừ điểm
        PlayerStats.Instance.ChangeHunger(-hungerBaseRate * timeMultiplier * Time.deltaTime);
        PlayerStats.Instance.ChangeThirst(-thirstBaseRate * timeMultiplier * Time.deltaTime);

        // Hiển thị tốc độ lên PlayerStats (để bạn kiểm tra trong Inspector)
        PlayerStats.Instance.currentHungerDrainRate = hungerBaseRate * timeMultiplier;
        PlayerStats.Instance.currentThirstDrainRate = thirstBaseRate * timeMultiplier;

        if (PlayerStats.Instance.currentHunger <= 0 || PlayerStats.Instance.currentThirst <= 0)
        {
            PlayerStats.Instance.ChangeHealth(-1f * Time.deltaTime);
        }
    }

    public void OnMiningAction()
    {
        // Đã loại bỏ hình phạt đào mỏ theo yêu cầu.
    }
}
