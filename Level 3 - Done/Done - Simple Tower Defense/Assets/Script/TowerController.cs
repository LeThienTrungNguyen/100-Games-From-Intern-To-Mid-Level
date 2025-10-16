using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [Header("Tower Settings")]
    public float range = 3f;                // Tầm bắn
    public float fireRate = 1f;             // Số phát/giây
    private float fireCooldown = 0f;

    [Header("References")]
    //public Transform firePoint;             // Vị trí bắn (empty object đặt ở chỗ nòng)
    public GameObject bulletPrefab;         // Prefab đạn

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        // Tìm enemy gần nhất trong tầm
        GameObject target = GetNearestEnemy();

        if (target != null && fireCooldown <= 0f)
        {
            Shoot(target.transform);
            fireCooldown = 1f / fireRate;
        }
    }

    GameObject GetNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range * 0.16f);
        float minDist = Mathf.Infinity;
        GameObject nearest = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = hit.gameObject;
                }
            }
        }
        return nearest;
    }

    void Shoot(Transform target)
    {
        GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetTarget(target);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ vòng tròn tầm bắn trong Editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range * 0.16f);
    }
}
