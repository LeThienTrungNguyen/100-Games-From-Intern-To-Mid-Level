using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    public Transform player;
    public Transform model;
    public Transform moveTarget;

    void OnCollisionEnter2D(Collision2D collision)
    {
        GroundController groundController;
        Debug.Log(collision.transform.root.TryGetComponent<GroundController>(out groundController));
        if (groundController != null)
        {
            if (groundController.isLanded) return;
            //GameController.instance.SetNewStartGround(groundController);
            //groundController.isLanded = true;
        }
    }

    public void IgnoreCollision(Collider2D col1, Collider2D col2, bool ignore)
    {
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), transform.GetComponentInChildren<Collider2D>(), ignore);
    }

}
