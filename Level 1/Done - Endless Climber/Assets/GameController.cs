using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public float score;
    public PlayerController pc;
    public bool ready;
    public RectTransform panel;
    public bool isGameOver;
    void Awake()
    {
        Time.timeScale = 1;
        isGameOver = false;
        ready = false;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Game");
        }
    }
    void FixedUpdate()
    {
        score = pc.transform.position.y - 0.8f;
        scoreText.text = score.ToString("0.#");
    }

    public void GameOver()
    {
        isGameOver = true;
        panel.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
}
