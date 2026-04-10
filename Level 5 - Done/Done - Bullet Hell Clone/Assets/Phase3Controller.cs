using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase3Controller : PhaseController
{
    public override IEnumerator SpawnBullet()
    {
        while (true)
        {
            Debug.Log("Start Spawn Bullet From Pool");
            EnemyBulletPool.Instance.SpawnBullet(bulletPrefab.GetComponent<EnemyBulletController>(),transform.position,Quaternion.Euler(0,0,180),transform,BulletMoveType.Type1);
            yield return new WaitForSeconds (bulletSpawnInterval);
        }
    }
}
