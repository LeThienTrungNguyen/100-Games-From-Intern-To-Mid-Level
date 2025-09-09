using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teeter : MonoBehaviour
{
    public Vector2 startMouseDownPos;
    public Vector2 currentMousePos;
    public Vector2 direction;
    Rigidbody2D rb;
    public float speed;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private bool isMouseHeld = false;
    public float directionDamping = 8f; // tốc độ giảm direction về 0

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMouseDownPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isMouseHeld = true;
        }
        if (Input.GetMouseButton(0))
        {
            currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = currentMousePos - startMouseDownPos;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isMouseHeld = false;
        }
        // Nếu thả chuột, direction giảm dần về 0
        if (!isMouseHeld)
        {
            direction = Vector2.Lerp(direction, Vector2.zero, directionDamping * Time.deltaTime);
        }
        rb.velocity = direction * speed;
    }
}
