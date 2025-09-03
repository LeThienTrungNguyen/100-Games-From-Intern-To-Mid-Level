using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitController : MonoBehaviour
{
    Rigidbody2D rb2d;
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        rb2d.velocity = Vector2.down * GameController.instance.downSpeed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
        if (collision.CompareTag("Player")) { 
            GameController.instance.AddScore();
        }else
        GameController.instance.DeduceHp();
    }
}
