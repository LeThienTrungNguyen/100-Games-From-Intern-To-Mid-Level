using UnityEngine;

public class PlayerFish : FishBase
{
    public enum BuffType { None, Speed, GiantEater, Magnet, Shield, TimeFreeze, SonicBlast, ShrinkRay, GodMode }

    [Header("Player Settings")]
    public float moveSpeed = 5f; // Cố định tốc độ cơ bản là 5
    public float virtualSize = 1.0f; // Sức mạnh thực tế
    public float visualScale = 1.0f; // Kích thước hiển thị (1.0 -> 1.5)
    public float growthThreshold = 2f;
    private float currentEatenSize = 0f;

    [Header("Scaling Settings")]
    public float rescaleThreshold = 1.5f; // Khi to gấp 1.5 lần thì thu nhỏ thế giới

    [Header("Buff Durations")]
    public float speedBuffDuration = 12f;
    public float giantEaterDuration = 10f;
    public float magnetDuration = 15f;
    public float shieldDuration = 30f;
    public float timeFreezeDuration = 10f;
    public float sonicBlastDuration = 2f;
    public float shrinkRayDuration = 12f;
    public float godModeDuration = 60f;
    
    [Range(0f, 100f)]
    public float godModeChance = 1f;

    [Header("Buff Status")]
    public BuffType activeBuff = BuffType.None;
    private float buffTimer = 0f;
    public bool isShielded = false, isMagnetActive = false, isTimeFrozen = false, isShrinkRayActive = false, isGiantEater = false, isGodMode = false;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.drag = 5f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        transform.localScale = Vector3.one; 
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private bool isRescaling = false;

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        if (moveInput.x != 0) FlipSprite(moveInput.x);

        HandleBuffTimer();
        HandleTestInput();
        HandleVisualEffects();

        // Áp dụng kích thước hiển thị
        transform.localScale = Vector3.one * visualScale;

        // KIỂM TRA NGƯỠNG THU NHỎ (RESCALE)
        if (!isRescaling && visualScale >= rescaleThreshold)
        {
            StartCoroutine(SmoothRescaleCoroutine());
        }
    }

    private System.Collections.IEnumerator SmoothRescaleCoroutine()
    {
        isRescaling = true;
        float duration = 0.5f; // Thời gian Zoom Out (0.5 giây)
        float elapsed = 0f;
        float startVisualScale = visualScale;
        float targetVisualScale = 1.0f;
        
        float factor = 1f / startVisualScale;
        
        // Thông báo cho toàn thế giới thu nhỏ mượt mà
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RequestWorldRescale(factor, duration);
        }

        // Tăng virtualSize ngay lập tức để giữ cân bằng sức mạnh
        virtualSize *= startVisualScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Sử dụng SmoothStep để hiệu ứng trông tự nhiên hơn
            visualScale = Mathf.Lerp(startVisualScale, targetVisualScale, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        visualScale = targetVisualScale;
        isRescaling = false;
        
        Debug.Log("<color=cyan><b>SMOOTH ZOOM OUT FINISHED! New VirtualSize: " + virtualSize + "</b></color>");
    }

    // Xóa hàm TriggerWorldRescale cũ
    // (Bỏ hàm private void TriggerWorldRescale() ...)

    void FixedUpdate()
    {
        // Theo yêu cầu: Tốc độ = moveSpeed / visualScale
        float actualSpeed = (activeBuff == BuffType.Speed || isGodMode) ? moveSpeed * 2.5f : moveSpeed;
        rb.velocity = moveInput.normalized * (actualSpeed / visualScale);
    }

    private void HandleVisualEffects()
    {
        if (isGodMode && spriteRenderer != null)
        {
            if (buffTimer < 5f)
            {
                float flash = Mathf.PingPong(Time.time * 15f, 1f);
                spriteRenderer.color = Color.Lerp(Color.red, Color.white, flash);
            }
            else
            {
                Color rainbow = Color.HSVToRGB(Mathf.PingPong(Time.time * 3f, 1f), 1f, 1f);
                spriteRenderer.color = Color.Lerp(rainbow, Color.white, 0.2f);
            }
        }
    }

    private void HandleBuffTimer()
    {
        if (buffTimer > 0)
        {
            buffTimer -= Time.deltaTime;
            if (buffTimer <= 0) EndBuff();
        }
    }

    private void EndBuff()
    {
        if (isGodMode) isShielded = true;
        ResetBuffs();
    }

    private void ResetBuffs()
    {
        activeBuff = BuffType.None;
        isGiantEater = false; isMagnetActive = false; isTimeFrozen = false; 
        isShrinkRayActive = false; isGodMode = false;
        if (spriteRenderer != null) spriteRenderer.color = Color.white;
    }

    private void HandleTestInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ApplyBuffEffect(BuffType.Speed);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ApplyBuffEffect(BuffType.GiantEater);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ApplyBuffEffect(BuffType.Magnet);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ApplyBuffEffect(BuffType.Shield);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ApplyBuffEffect(BuffType.TimeFreeze);
        if (Input.GetKeyDown(KeyCode.Alpha6)) ApplyBuffEffect(BuffType.SonicBlast);
        if (Input.GetKeyDown(KeyCode.Alpha7)) ApplyBuffEffect(BuffType.ShrinkRay);
        if (Input.GetKeyDown(KeyCode.Alpha8)) ApplyBuffEffect(BuffType.GodMode);
        if (Input.GetKeyDown(KeyCode.Alpha0)) ResetBuffs();
    }

    public void ApplyRandomBuff()
    {
        if (Random.value < (godModeChance / 100f)) { ApplyBuffEffect(BuffType.GodMode); return; }
        ApplyBuffEffect((BuffType)Random.Range(1, 8));
    }

    private void ApplyBuffEffect(BuffType type)
    {
        ResetBuffs();
        activeBuff = type;
        switch (type)
        {
            case BuffType.Speed: buffTimer = speedBuffDuration; spriteRenderer.color = Color.yellow; break;
            case BuffType.GiantEater: buffTimer = giantEaterDuration; isGiantEater = true; spriteRenderer.color = Color.magenta; break;
            case BuffType.Magnet: buffTimer = magnetDuration; isMagnetActive = true; spriteRenderer.color = Color.cyan; break;
            case BuffType.Shield: buffTimer = shieldDuration; isShielded = true; spriteRenderer.color = new Color(0.7f, 0.7f, 1f); break;
            case BuffType.TimeFreeze: buffTimer = timeFreezeDuration; isTimeFrozen = true; spriteRenderer.color = new Color(0.5f, 1f, 1f); break;
            case BuffType.SonicBlast: buffTimer = sonicBlastDuration; TriggerSonicBlast(); spriteRenderer.color = Color.red; break;
            case BuffType.ShrinkRay: buffTimer = shrinkRayDuration; isShrinkRayActive = true; spriteRenderer.color = new Color(1f, 0.8f, 0f); break;
            case BuffType.GodMode: ActivateGodMode(); break;
        }
    }

    private void ActivateGodMode()
    {
        buffTimer = godModeDuration;
        isGodMode = true; isGiantEater = true; isShielded = true; isMagnetActive = true; 
        isTimeFrozen = false; isShrinkRayActive = false;
        Debug.Log("GOD MODE!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyFish enemy = other.GetComponent<EnemyFish>();
        if (enemy != null)
        {
            if (isGodMode || isGiantEater || isShrinkRayActive || enemy.fishSize < this.virtualSize)
            {
                EatFish(enemy);
            }
            else
            {
                if (isShielded) { isShielded = false; Destroy(enemy.gameObject); }
                else GameOver();
            }
            return;
        }

        GiftBox gift = other.GetComponent<GiftBox>();
        if (gift != null) { ApplyRandomBuff(); Destroy(gift.gameObject); }
    }

    private void EatFish(EnemyFish enemy)
    {
        currentEatenSize += enemy.fishSize;
        if (GameManager.Instance != null) GameManager.Instance.AddScore(enemy.fishSize);

        // Tăng visualScale (to ra tạm thời)
        float growthFactor = 0.15f * (enemy.fishSize / virtualSize);
        visualScale += growthFactor;
        
        Destroy(enemy.gameObject);
        if (currentEatenSize >= growthThreshold) LevelUp();
    }

    private void TriggerSonicBlast()
    {
        EnemyFish[] allEnemies = FindObjectsOfType<EnemyFish>();
        foreach (EnemyFish e in allEnemies)
        {
            Vector2 pushDir = (e.transform.position - transform.position).normalized;
            e.transform.position += (Vector3)pushDir * 5f;
        }
    }

    private void LevelUp()
    {
        virtualSize += 0.1f;
        currentEatenSize = 0f;
        growthThreshold *= 1.2f;
    }

    private void GameOver()
    {
        if (GameManager.Instance != null) GameManager.Instance.TriggerGameOver();
        this.gameObject.SetActive(false);
    }
}
