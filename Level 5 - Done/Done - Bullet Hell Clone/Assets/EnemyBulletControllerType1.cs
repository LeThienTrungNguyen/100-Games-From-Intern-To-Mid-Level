using UnityEngine;

public class EnemyBulletControllerType1 : EnemyBulletController
{
    public override void Move()
    {
        transform.Translate(Vector3.down * movespeed * Time.deltaTime);
    }
}
