using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public float downSpeed;
    public float moveSpeed;
    public static int score;
    public float speedIncrease = 0.5f;
    public float scoreIncrease = 5;
    public int hp;


    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hpText;
    void Awake()
    {
        float r = Random.Range(0f, 256f)/255f;
        float g = Random.Range(127f, 256f)/255f;
        float b = Random.Range(0f, 256f)/255f;
        Camera.main.backgroundColor = new Color(r, g, b, 1);
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        hp = 3;
        score = 0;
        ShowHp();
        ShowScore();
    }
    public void AddScore()
    {
        score++;
        if (score % scoreIncrease == 0)
        {
            downSpeed += speedIncrease;
            moveSpeed += speedIncrease;
            hp++;
        }
        ShowScore();
        ShowHp();
    }
    public void ShowHp()
    {
        hpText.text = "Hp : " + hp;
    }
    public void ShowScore()
    {
        scoreText.text = score + "";
    }
    // Update is called once per frame
    public void DeduceHp()
    {
        hp--;
        ShowHp();
        if (hp < 0) { hp = 0; StartCoroutine(QuitGame()); }
    }
    IEnumerator QuitGame()
    {
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }
}
