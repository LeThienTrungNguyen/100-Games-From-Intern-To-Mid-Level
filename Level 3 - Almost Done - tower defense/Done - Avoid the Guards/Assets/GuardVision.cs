using UnityEngine;
public class GuardVision : MonoBehaviour
{
    public float viewRadius = 5f;         // Bán kính tầm nhìn
    [Range(0, 360)]
    public float viewAngle = 90f;         // Góc nhìn (độ)
    public LayerMask targetMask;          // Layer của player
    public LayerMask obstacleMask;        // Layer của tường

    void Update()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);
        foreach (Collider2D target in targets)
        {
            Vector2 dirToTarget = (target.transform.position - transform.position).normalized;
            if (Vector2.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                float dist = Vector2.Distance(transform.position, target.transform.position);
                if (!Physics2D.Raycast(transform.position, dirToTarget, dist, obstacleMask))
                {
                    Debug.Log("🎯 Thấy player!");
                }
            }
        }
    }
}
