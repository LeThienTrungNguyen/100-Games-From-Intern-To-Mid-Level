using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    GameController gameController;
    void Awake()
    {
        gameController = GameObject.FindFirstObjectByType(typeof(GameController)) as GameController;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameController.NextLevel();
        }
    }
}
