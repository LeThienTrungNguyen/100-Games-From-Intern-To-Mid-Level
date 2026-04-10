using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.U2D.Path;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;              // điểm bắn ra đạn
    public GameObject bulletPrefab;          // prefab viên đạn

    [Header("Stats")]         // thời gian giữa 2 phát bắn
    private float fireTimer = 0f;

    [SerializeField] public PlayerStat playerStats;
    [SerializeField] public PlayerStatsUpgrade psu;
    public float money;
    public int currentHp;
    public bool isInvincible = false;
    public TextMeshProUGUI healthUI;
    void Start()
    {
        //playerStats = new PlayerStat();
        currentHp = playerStats.maxHp;
        UpdatePlayerUI();
        UpdateMoneyUI();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("BulletE")) return;
        Destroy(collision.gameObject);
        if (isInvincible) return;
        // mở trạng thái bất tử trong 2s
        StartCoroutine(Invincible());

        // trừ máu
        currentHp--;
        // update player health ui
        UpdatePlayerUI();
    }
    void UpdatePlayerUI()
    {
        healthUI.text = currentHp + "";
        if (currentHp < 0)
        {
            psu.gameObject.SetActive(true);
        }
    }
    public void Replay()
    {
        currentHp = playerStats.maxHp;
        UpdatePlayerUI();
        UpdateMoneyUI();
    }
private Coroutine invincibleFxCoroutine;

IEnumerator Invincible()
{
    isInvincible = true;

    // Lưu lại coroutine
    invincibleFxCoroutine = StartCoroutine(InvincibleEffects());

    yield return new WaitForSeconds(2f);

    // Dừng đúng coroutine
    if (invincibleFxCoroutine != null)
    {
        StopCoroutine(invincibleFxCoroutine);
        invincibleFxCoroutine = null;
    }

    // Đảm bảo trả lại màu bình thường
    GetComponent<SpriteRenderer>().color = Color.white;

    isInvincible = false;
}


    IEnumerator InvincibleEffects()
{
    var sr = GetComponent<SpriteRenderer>();

    while (true)
    {
        sr.color = new Color(1f, 1f, 1f, 0f);  // ẩn
        yield return new WaitForSeconds(0.1f);
        sr.color = new Color(1f, 1f, 1f, 1f);  // hiện
        yield return new WaitForSeconds(0.1f);
    }
}


    void Update()
    {
        MovementHandle();
        AutoShootHandle();
    }

    void MovementHandle()
    {
        // Di chuyển theo vị trí chuột
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x, mousePos.y, 0);
    }

    void AutoShootHandle()
    {
        fireTimer += Time.deltaTime;

        // Nếu đã đến thời điểm bắn
        if (fireTimer >= (float)(1f / playerStats.fireRate))
        {
            Fire();
            fireTimer = 0f;
        }
    }

    void Fire()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Thiếu bulletPrefab hoặc firePoint!");
            return;
        }

        int count = Mathf.Max(1, playerStats.bulletCount);

        float maxSpread = 0.5f;     // Tổng biên trải rộng tối đa (-3 -> +3)
        float maxSpacing = 0.5f;    // Khoảng cách tối đa giữa 2 viên liền kề

        Vector3 rightOffset = firePoint.right;

        if (count == 1)
        {
            // Chỉ 1 viên thì bắn giữa
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            return;
        }

        // Tính spread thực tế (giới hạn theo maxSpacing)
        float halfSpread = Mathf.Min(maxSpread, (maxSpacing * (count - 1)) / 2f);
        float minSpread = -halfSpread;
        float maxSpreadActual = halfSpread;

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / (count - 1);
            float offsetAmount = Mathf.Lerp(minSpread, maxSpreadActual, t);
            Vector3 offset = rightOffset * offsetAmount;
            Instantiate(bulletPrefab, firePoint.position + offset, firePoint.rotation);
        }
    }
    public TextMeshProUGUI moneyUI;
    public void UpdateMoneyUI()
    {
        moneyUI.text = money + "";

    }
    public void UpdateShopUI()
    {
        psu.UpdateUI();
    }
}
