using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverItem : MonoBehaviour
{
    Rigidbody2D rb2d;
    float dir = 0;
    float movespeed = 0.5f;
    void Awake()
    {
        while (dir == 0)
        {
            dir = Random.Range(-10, 11);
        }
        dir = dir / Mathf.Abs(dir);
    }
    void Update()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = Vector2.right * dir * movespeed;
        if (transform.localPosition.x < -0.1666667f)
        {
            SetXPos(1f);

        }
        else if (transform.localPosition.x > 1f)


        {
            SetXPos(-0.1666667f);
        }

    }

    void SetXPos(float x)
    {
        var pos = transform.localPosition;
        pos.x = x;
        transform.localPosition = pos;
    }
}
