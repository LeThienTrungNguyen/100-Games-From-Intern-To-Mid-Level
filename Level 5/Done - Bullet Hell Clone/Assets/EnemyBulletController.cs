using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    bool canMove = false;
    [SerializeField] protected float movespeed;
    [SerializeField] float destroyTimer;
    public Transform parent;
    void Update()
    {
        if(canMove) Move();
    }
    public virtual void OnEnable()
    {
        Invoke(nameof(Destroy), destroyTimer);
        canMove = true;
    }
    public virtual void OnDisable() { }
    void Destroy()
    {
        if (gameObject.activeSelf)
            EnemyBulletPool.Instance.DestroyBullet(transform);
    }

    public virtual void Move() { }
}
