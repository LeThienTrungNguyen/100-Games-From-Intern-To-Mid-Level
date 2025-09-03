using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundController : MonoBehaviour
{
    public Vector2 startPosition;
    public float speed;
    public float destroyTime;
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision);
        if (collision.transform.CompareTag("Player"))
        {
            GameController.instance.GameOver();
        }
    }
    void Update()
    {
        if (!GameController.instance.gameStarted) return;

        if (transform.position.x <= -1.648)
        {
            MoveToStartPosition();
        }
        transform.position -= Vector3.right * Time.deltaTime * speed;
    }

    void MoveToStartPosition()
    {
        transform.position = startPosition;
    }
}
