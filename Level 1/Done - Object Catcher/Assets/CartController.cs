using UnityEditor;
using UnityEngine;

public class CartController : MonoBehaviour
{
    Rigidbody2D rb2d;

    void Awake()
    {

        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        rb2d.velocity = Vector2.right * horizontal * GameController.instance.moveSpeed;

        float xPosition = Mathf.Clamp(transform.position.x, -7, 7);
        float yPosition = transform.position.y;
        transform.position = new Vector2(xPosition, yPosition);
    }
}
