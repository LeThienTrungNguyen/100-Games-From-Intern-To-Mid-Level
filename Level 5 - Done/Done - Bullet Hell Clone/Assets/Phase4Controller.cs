using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase4Controller : PhaseController
{
    public override IEnumerator SpawnBullet()
    {
        while (true)
        {
            //Debug.Log("Start Spawn Bullet From Pool");
            EnemyBulletPool.Instance.SpawnBullet(bulletPrefab.GetComponent<EnemyBulletController>(),transform.position,transform.rotation,transform,BulletMoveType.Type4);
            yield return new WaitForSeconds (bulletSpawnInterval);
        }
    }
}
