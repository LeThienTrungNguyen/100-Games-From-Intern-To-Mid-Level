using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    public Transform bulletPrefab;
    public float bulletSpawnInterval;
    void OnEnable()
    {
        StartSpawnBullet();
    }

    void OnDisable()
    {
        StopSpawnBullet();
    }
    void StartSpawnBullet()
    {
        Debug.Log("Start Spawn Bullet Coroutine");
        StartCoroutine(SpawnBullet());
    }
    void StopSpawnBullet()
    {
        Debug.Log("Stop Spawn Bullet Coroutine");
        StopCoroutine(SpawnBullet());
    }
    public virtual IEnumerator SpawnBullet()
    {
        while (true)
        {
            //Debug.Log("Start Spawn Bullet From Pool");
            EnemyBulletPool.Instance.SpawnBullet(bulletPrefab.GetComponent<EnemyBulletController>(),transform.position,transform.rotation,transform);
            yield return new WaitForSeconds (bulletSpawnInterval);
        }
    }
}
