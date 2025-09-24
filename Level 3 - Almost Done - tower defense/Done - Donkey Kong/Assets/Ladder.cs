using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    // gán ground collider trong Inspector
    public Collider2D groundCollider;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collider2D playerCollider = collision.GetComponent<Collider2D>();
            if (playerCollider != null && groundCollider != null)
            {
                // Bỏ qua va chạm giữa player và ground
                Physics2D.IgnoreCollision(playerCollider, groundCollider, true);
                Debug.Log("Player vào ladder -> ignore ground");
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collider2D playerCollider = collision.GetComponent<Collider2D>();
            if (playerCollider != null && groundCollider != null)
            {
                // Bật lại va chạm giữa player và ground
                Physics2D.IgnoreCollision(playerCollider, groundCollider, false);
                Debug.Log("Player rời ladder -> bật lại ground");
            }
        }
    }
}
