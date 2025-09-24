using UnityEngine;

public class IceBullet : Bullet
{
    public float slowAmount = 0.7f; // giảm 30%
    public float slowDuration = 2f;

    protected override void HitTarget()
    {
        EnemyHealth hp = target.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
            hp.ApplySlow(slowAmount, slowDuration);
        }
        Destroy(gameObject);
    }
}
