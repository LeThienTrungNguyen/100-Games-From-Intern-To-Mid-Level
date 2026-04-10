using UnityEngine;
using System.Collections;

public class EnemyBulletControllerType4 : EnemyBulletController
{
    public float appearTime = 0.5f;    // thời gian hiện dần
    public float activeTime = 1f;      // thời gian hoạt động (collider bật)
    public float fadeOutTime = 0.5f;   // thời gian mờ dần
    public float lengthMultiplier = 50f; // độ dài tia

    private PlayerController pc;
    private BoxCollider2D col;
    private SpriteRenderer sr;
    
    public override void OnEnable()
    {
        base.OnEnable();
        // Lấy thông tin player controller
        pc = FindAnyObjectByType<PlayerController>();
        col = GetComponent<BoxCollider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        if (col == null || sr == null || pc == null)
        {
            Debug.LogWarning("Thiếu thành phần cho EnemyBulletControllerType4!");
            return;
        }

        // Tắt collider lúc đầu
        col.enabled = false;

        // Xoay đạn hướng về player
        Vector3 dir = pc.transform.position - transform.position;
        Debug.Log("pc"+pc.transform.position,pc.transform);
        Debug.Log("this "+transform.position,transform);
        Debug.Log(dir);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg-90f;
        Debug.Log(angle);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Đặt scale ban đầu: dài theo khoảng cách đến player, nhưng chiều ngang = 0
        float distance = Vector3.Distance(transform.position, pc.transform.position);
        Debug.Log("khoảng cách tới player:" + distance);
        transform.localScale = new Vector3(0f, 20, 1f);
        // Alpha ban đầu = 0 (ẩn)
        Color c = sr.color;
        c.a = 0f;
        sr.color = c;

        // Bắt đầu hiệu ứng
        StartCoroutine(BeamSequence());
    }

    private IEnumerator BeamSequence()
{
    // --- Giai đoạn 1: Scale xuất hiện nhanh ---
    float t = 0f; // thời gian mờ 0.5 trước khi full sáng
    while (t < appearTime)
    {
        t += Time.deltaTime;
        float p = Mathf.Clamp01(t / appearTime);

        // Scale ngang (right) từ 0 -> 1
        Vector3 s = transform.localScale;
        s.x = p;
        transform.localScale = s;

        yield return null;
    }

    // --- Giai đoạn 2: Hiện mờ ---
    Color c = sr.color;
    c.a = 0.5f;
    sr.color = c;
    yield return new WaitForSeconds(appearTime);

    // --- Giai đoạn 3: Full sáng và bật collider ---
    c.a = 1f;
    sr.color = c;
    col.enabled = true;

    yield return new WaitForSeconds(activeTime);

    // --- Giai đoạn 4: Tắt collider và mờ dần ---
    col.enabled = false;

    t = 0f;
    while (t < fadeOutTime)
    {
        t += Time.deltaTime;
        float p = 1f - Mathf.Clamp01(t / fadeOutTime);

        Vector3 s = transform.localScale;
        s.x = p;
        transform.localScale = s;

        c.a = p;
        sr.color = c;

        yield return null;
    }

    // --- Giai đoạn 5: Trả về pool ---
    EnemyBulletPool.Instance.DestroyBullet(transform);
}

    public void SetupDirection(PlayerController target)
    {
        if (target == null) return;

        Vector3 dir = target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Đặt lại scale ban đầu, alpha = 0
        float distance = Vector3.Distance(transform.position, target.transform.position);
        transform.localScale = new Vector3(0f, 20, 1f);

        Color c = sr.color;
        c.a = 0f;
        sr.color = c;
    }
}
