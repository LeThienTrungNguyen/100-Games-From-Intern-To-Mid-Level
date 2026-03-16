using UnityEngine;

public class GiftBox : MonoBehaviour
{
    public float rotateSpeed = 100f;
    private float lifeTime = 15f; // Tự biến mất nếu không ăn

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWorldRescale += HandleRescale;
        }
        Destroy(gameObject, lifeTime);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWorldRescale -= HandleRescale;
        }
    }

    void HandleRescale(float factor, float duration)
    {
        StartCoroutine(SmoothRescaleCoroutine(factor, duration));
    }

    private System.Collections.IEnumerator SmoothRescaleCoroutine(float factor, float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos * factor;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * factor;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            transform.position = Vector3.Lerp(startPos, targetPos, smoothT);
            transform.localScale = Vector3.Lerp(startScale, targetScale, smoothT);
            yield return null;
        }

        transform.position = targetPos;
        transform.localScale = targetScale;
    }

    void Update()
    {
        // Hiệu ứng xoay cho hộp quà
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        
        // Giảm thời gian sống
        lifeTime -= Time.deltaTime;

        // Hiệu ứng nhấp nháy khi sắp biến mất (dưới 5 giây)
        if (lifeTime < 5f)
        {
            float alpha = Mathf.PingPong(Time.time * 5f, 1f);
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = new Color(1, 1, 1, alpha);
        }
    }
}
