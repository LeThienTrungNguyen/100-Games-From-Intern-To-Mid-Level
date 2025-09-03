using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TowerController : MonoBehaviour
{
    Rigidbody2D rb;
    bool canActive = true;
    bool canInteract = true;
    bool isLeft = false;
    // Start is called before the first frame update
    void Start()
    {
        var sPosX = Random.Range(StackController.minX, StackController.maxX);
        transform.position = new Vector2(sPosX, Camera.main.transform.position.y);
        rb = transform.GetComponent<Rigidbody2D>();

    }
    // Update is called once per frame
    void Update()
    {
        
        if (!canActive) return;
        rb.gravityScale = 0;
        if (!StackController.isThrowing)
        {
            transform.DOMoveY(Camera.main.transform.position.y+2, 0);
            rb.velocity = (!isLeft ? Vector2.right : Vector2.left) * StackController.instance.moveSpeed;
        }
        else
        {
            rb.gravityScale = 1;
            rb.velocity = new Vector2(0, rb.velocity.y);
            transform.parent = StackController.instance.parent;
        }
        if (transform.position.x >= StackController.maxX) isLeft = true;
        if (transform.position.x <= StackController.minX) isLeft = false;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StackController.isThrowing = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameController.score = StackController.instance.FloorCount();
        if (collision.transform.CompareTag("Tower"))
        {
            //GameController.score++;
            if (StackController.instance.FloorCount() > 4)
            {
                StackController.instance.parent.GetChild(StackController.instance.FloorCount() - 5).GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            }
        }
        if (canInteract)
        {
            canInteract = false;

            StackController.isThrowing = false;
            canActive = false;
            if (StackController.instance.stacked >= StackController.maxStackedMoveCamera)
            {
                Camera.main.transform.DOMoveY(StackController.instance.parent.GetChild(StackController.instance.FloorCount()-1).transform.position.y + 3.5f, 1f).OnComplete(() =>
                {

                    StackController.instance.isStackPutted = true;
                });
                StackController.instance.stacked = 0;
            }
            else
            {
                StackController.instance.stacked++;
                StackController.instance.isStackPutted = true;
            }


        }

    }
}
