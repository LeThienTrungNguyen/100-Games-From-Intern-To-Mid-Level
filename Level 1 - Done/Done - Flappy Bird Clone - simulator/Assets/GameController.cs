
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("UI")]
    public Image PlayButton;
    public Image RestartPlayButton;
    public RawImage GetReadyImage;
    public RawImage GameOverImage;
    public Text ScoreText;
    public Text HighScoreText;
    [Header("Logic ")]
    public static GameController instance;
    public bool gameStarted;
    public float startGameTime = 2;
    public Transform PipeHole;
    public float randomHeightSpawnMax = 0.715f;
    public float randomHeightSpawnMin = -0.25f;
    public Vector3 PipeHoleSpawnPoint;
    public float spawnIntervalPipe;
    public float spawnTimePipe = 0;
    public int score;
    void Awake()
    {
        instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        if (!gameStarted) return;
        if (spawnTimePipe <= 0)
        {
            spawnTimePipe = spawnIntervalPipe;
            SpawnPipeHole();
        }
        else
        {
            spawnTimePipe -= Time.deltaTime;
        }
    }

    GameObject parent;
    public void SpawnPipeHole()
    {
        parent = GameObject.Find("Pipes");
        if (parent == null)
        {
            parent = new GameObject("Pipes");
        }
        Transform pipeT = Instantiate(PipeHole,parent.transform);
        PipeHoleSpawnPoint = new Vector3(2, Random.Range(randomHeightSpawnMin, randomHeightSpawnMax));
        pipeT.position = PipeHoleSpawnPoint;
    }
    public void StartGameButton()
    {
        gameStarted = false;
        PlayButton.gameObject.SetActive(false);
        RestartPlayButton.gameObject.SetActive(false);
        GameOverImage.gameObject.SetActive(false);
        GetReadyImage.gameObject.SetActive(true);
        ScoreText.gameObject.SetActive(false);
        HighScoreText.gameObject.SetActive(false);
        Invoke("StartGame", 2);
    }
    public void StartGame()
    {
        score = 0;
        ScoreText.text = score + "";
        ScoreText.gameObject.SetActive(true);
        GetReadyImage.gameObject.SetActive(false);
        HighScoreText.gameObject.SetActive(false);
        gameStarted = true;
    }
    public void GameOver()
    {
        gameStarted = false;
        ScoreText.text = ""+score ;
        ScoreText.gameObject.SetActive(true);
        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }
        HighScoreText.text = "High Score:" + PlayerPrefs.GetInt("HighScore");
        HighScoreText.gameObject.SetActive(true);
        GetReadyImage.gameObject.SetActive(false);
        PlayButton.gameObject.SetActive(false);
        RestartPlayButton.gameObject.SetActive(true);
        gameStarted = false;
        Debug.Log("game over");
    }

    public void AddScore()
    {
        score++;
        ScoreText.text = score + "";
    }

    public void Restart()
    {
        SetDefaultPosition();
        StartGameButton();
    }
    public Transform Bird;
    public void SetDefaultPosition()
    {
        Bird.position = Vector3.zero;
        Bird.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Destroy(parent);
    }
}
