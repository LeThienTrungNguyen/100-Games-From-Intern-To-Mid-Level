using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfController : MonoBehaviour
{
    public Vector2 direction;
    public Vector2 direction1;
    public float force;
    public Vector2 startPos;
    public Vector2 endPos;
    private Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
        }
        if(Input.GetMouseButtonUp(0))
        {
            direction = startPos - endPos;
            HitTheGolfBall();
        }
    }

    void HitTheGolfBall()
    {
        rb.AddForce(direction * force * 50, ForceMode2D.Force);
    }
}
