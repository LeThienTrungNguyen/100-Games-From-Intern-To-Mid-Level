using UnityEngine;

public class EnemyBulletControllerType2 : EnemyBulletController
{
    [SerializeField] private PlayerController pc;
    public Vector3 movedirection;
    public override void OnEnable()
    {

        base.OnEnable();
        pc = FindAnyObjectByType(typeof(PlayerController)) as PlayerController;
        movedirection = Vector3.zero;
        Debug.Log(pc, pc);
    }
    public override void OnDisable()
    {
        movedirection = Vector3.zero;
    }
    public override void Move()
    {
        if (pc != null && parent != null)
        {
            if (movedirection == Vector3.zero)
            {
                movedirection = (pc.transform.position - parent.position).normalized;
            }
            else
            {
                transform.Translate(movedirection * movespeed * Time.deltaTime);
            }
            

        }
            
    }
    
}
