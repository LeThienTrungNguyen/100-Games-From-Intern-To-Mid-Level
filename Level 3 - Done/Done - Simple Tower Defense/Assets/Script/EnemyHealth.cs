using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHp = 100;
    private int currentHp;
    private float originalSpeed;
    private EnemyController enemy;

    void Awake()
    {
        currentHp = maxHp;
        enemy = GetComponent<EnemyController>();
        originalSpeed = enemy.speed;
    }

    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        if (currentHp <= 0) Die();
    }

    public void ApplySlow(float slowAmount, float duration)
    {
        StopCoroutine("SlowEffect");
        StartCoroutine(SlowEffect(slowAmount, duration));
    }

    IEnumerator SlowEffect(float slowAmount, float duration)
    {
        enemy.speed = originalSpeed * slowAmount;
        yield return new WaitForSeconds(duration);
        enemy.speed = originalSpeed;
    }

    public void ApplyPoison(int dmg, float duration, float tickRate)
    {
        StartCoroutine(PoisonEffect(dmg, duration, tickRate));
    }

    IEnumerator PoisonEffect(int dmg, float duration, float tickRate)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            TakeDamage(dmg);
            yield return new WaitForSeconds(tickRate);
            elapsed += tickRate;
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
