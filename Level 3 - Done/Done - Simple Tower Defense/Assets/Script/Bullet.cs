using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 10;
    protected Transform target;

    public void SetTarget(Transform enemy)
    {
        target = enemy;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distThisFrame = speed * 0.16f * Time.deltaTime;

        if (dir.magnitude <= distThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distThisFrame, Space.World);
    }

    protected virtual void HitTarget()
    {
        EnemyHealth hp = target.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
