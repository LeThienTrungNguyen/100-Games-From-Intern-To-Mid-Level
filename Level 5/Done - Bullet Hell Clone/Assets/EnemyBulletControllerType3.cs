using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletControllerType3 : EnemyBulletController
{
    public PlayerController target;
    public override void OnEnable()
    {
        base.OnEnable();
        target = FindAnyObjectByType(typeof(PlayerController)) as PlayerController;
    }

    public float rotateSpeed = 200f; // độ nhanh khi xoay hướng

    public override void Move()
    {
        if (target == null) return;

        // Hướng mục tiêu
        Vector3 dir = (target.transform.position - transform.position).normalized;

        // Hướng hiện tại của đạn
        Vector3 newDir = Vector3.RotateTowards(transform.up, dir, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, newDir);

        // Tiến về hướng mới
        transform.Translate(Vector3.up * movespeed * Time.deltaTime, Space.Self);
    }


}
