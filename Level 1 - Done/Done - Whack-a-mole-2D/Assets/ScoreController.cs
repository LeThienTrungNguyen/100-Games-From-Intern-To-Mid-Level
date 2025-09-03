using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public static ScoreController instance;
    void Awake()
    {
        instance = this;
    }
    public static int score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    public void AddScore(int scoreAdd)
    {
        score += scoreAdd;
        scoreText.text = score + "";
    }

    public void ResetScore()
    {
        score = 0;scoreText.text = score + "";
    }
}
