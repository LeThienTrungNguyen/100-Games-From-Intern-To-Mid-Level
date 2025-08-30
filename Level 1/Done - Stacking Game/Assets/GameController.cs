using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public TextMeshProUGUI scoreText;
    public static int score;
    void Awake()
    {
        instance = this;
    }

    void FixedUpdate()
    {
        
        scoreText.text = score + "";
    }
}
