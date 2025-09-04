using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    GameController gameController;
    void Awake()
    {
        gameController = FindObjectOfType<GameController>();
    }
    public float scoreMultiplier = 1.0f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Ball entered score zone with multiplier: " + scoreMultiplier);
            gameController.money += gameController.ballCost * scoreMultiplier;
            // Here you can add code to update the player's score based on the multiplier
            Destroy(collision.gameObject); // Destroy the ball after scoring
            collision.GetComponent<Collider2D>().enabled = false; // Disable further collisions
        }
    }
}
