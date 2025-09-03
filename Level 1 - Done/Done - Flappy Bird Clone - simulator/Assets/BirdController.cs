using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    public Rigidbody2D rb2d;
    public float jumpHeight;
    // Start is called before the first frame update
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.instance.gameStarted)
        {
            rb2d.gravityScale = 0;
            return;
        }
        rb2d.gravityScale = 0.7f;
        //
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                rb2d.velocity = Vector2.up * jumpHeight;

            }
        }
    }
}
