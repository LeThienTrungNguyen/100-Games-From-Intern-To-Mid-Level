using UnityEngine;

public class EnemyFish : FishBase
{
    private Vector2 direction;
    private float screenBoundX;
    private Camera mainCam;
    private PlayerFish player;
    private bool isAlreadyFaded = false;
    private Color originalColor;

    void Start()
    {
        mainCam = Camera.main;
        player = FindObjectOfType<PlayerFish>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;

        // Đăng ký sự kiện thu nhỏ thế giới
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWorldRescale += HandleRescale;
        }

        // Setup based on spawn direction
        direction = (transform.position.x > 0) ? Vector2.left : Vector2.right;
        FlipSprite(direction.x);
    }

    void OnDestroy()
    {
        // Hủy đăng ký sự kiện để tránh lỗi bộ nhớ
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWorldRescale -= HandleRescale;
        }
    }

    void HandleRescale(float factor, float duration)
    {
        // Khi nhận được thông báo, bắt đầu một Coroutine để thu nhỏ vị trí từ từ
        StartCoroutine(SmoothRescalePositionCoroutine(factor, duration));
    }

    private System.Collections.IEnumerator SmoothRescalePositionCoroutine(float factor, float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos * factor;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        transform.position = targetPos;
    }

    void Update()
    {
        if (player == null) 
        {
            player = FindObjectOfType<PlayerFish>();
            return;
        }

        // Điều chỉnh Scale (làm cá địch nhỏ lại khi Player to lên để tạo cảm giác Player lớn)
        transform.localScale = Vector3.one * (fishSize / player.virtualSize);
        
        // Tốc độ cá là độc lập, không phụ thuộc vào scale tạm thời của player
        float actualSpeed = movementSpeed; 

        Vector3 moveStep;

        // 1. TIME FREEZE
        if (player.isTimeFrozen) actualSpeed *= 0.1f;

        // 2. MAGNET
        if (player.isMagnetActive && this.fishSize < player.virtualSize)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            float magnetRadius = 3f; 
            
            if (dist < magnetRadius)
            {
                float attractStrength = Mathf.Lerp(actualSpeed * 4f, actualSpeed * 1.5f, dist / magnetRadius);
                Vector3 attractDir = (player.transform.position - transform.position).normalized;
                moveStep = attractDir * attractStrength * Time.deltaTime;
            }
            else moveStep = (Vector3)direction * actualSpeed * Time.deltaTime;
        }
        else moveStep = (Vector3)direction * actualSpeed * Time.deltaTime;

        transform.position += moveStep;

        // Fade khi player to hơn
        if (!isAlreadyFaded && player.virtualSize > this.fishSize) FadeFish();

        // Giới hạn màn hình cố định
        float screenBoundX = 15f; 
        if (Mathf.Abs(transform.position.x) > screenBoundX) Destroy(gameObject);
    }

    void FadeFish()
    {
        isAlreadyFaded = true;
        StartCoroutine(SmoothFadeCoroutine());
    }

    System.Collections.IEnumerator SmoothFadeCoroutine()
    {
        if (spriteRenderer == null) yield break;

        float duration = 1.0f; // Thời gian chuyển đổi (giây)
        float elapsed = 0f;

        Color startColor = spriteRenderer.color;
        Color targetColor = Color.Lerp(originalColor, Color.white, 0.6f);
        targetColor.a = 0.5f; // Độ mờ mong muốn

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Sử dụng SmoothStep để hiệu ứng trông tự nhiên hơn
            float smoothT = Mathf.SmoothStep(0, 1, t);
            
            spriteRenderer.color = Color.Lerp(startColor, targetColor, smoothT);
            yield return null;
        }

        spriteRenderer.color = targetColor;
    }

    public void SetProperties(float size, float speed, PlayerFish playerRef)
    {
        player = playerRef; // Gán tham chiếu ngay lập tức
        fishSize = size;
        movementSpeed = speed;
        
        // Cài đặt kích thước hiển thị tương đối với player ngay khi sinh ra
        if (player != null)
        {
            transform.localScale = Vector3.one * (fishSize / player.virtualSize);
        }
    }
}
