using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderIgnoreCollision : MonoBehaviour
{
    public Collider2D wallAttached;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Physics2D.IgnoreLayerCollision(6, 3,true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Physics2D.IgnoreLayerCollision(6, 3,false);
        }
    }
}
