using UnityEngine;

public class LightningBullet : Bullet
{
    public int chainCount = 2;   // số enemy lan
    public float chainRange = 1.5f;
    public float chainDamageFactor = 0.5f; // mỗi chain = 50% damage

    protected override void HitTarget()
    {
        EnemyHealth hp = target.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
            ChainLightning(target);
        }
        Destroy(gameObject);
    }

    void ChainLightning(Transform firstTarget)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(firstTarget.position, chainRange);

        int chained = 0;
        foreach (var hit in hits)
        {
            if (chained >= chainCount) break;
            if (hit.transform == firstTarget) continue;

            EnemyHealth hp = hit.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(Mathf.RoundToInt(damage * chainDamageFactor));
                chained++;
            }
        }
    }
}
