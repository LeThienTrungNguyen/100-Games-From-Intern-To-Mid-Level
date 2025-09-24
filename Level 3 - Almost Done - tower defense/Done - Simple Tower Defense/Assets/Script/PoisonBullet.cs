using UnityEngine;

public class PoisonBullet : Bullet
{
    public int poisonDamage = 5;
    public float duration = 3f;   // gây độc trong 3s
    public float tickRate = 1f;   // mỗi 1s

    protected override void HitTarget()
    {
        EnemyHealth hp = target.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
            hp.ApplyPoison(poisonDamage, duration, tickRate);
        }
        Destroy(gameObject);
    }
}
