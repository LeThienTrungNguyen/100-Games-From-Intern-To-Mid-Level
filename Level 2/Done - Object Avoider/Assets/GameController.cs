
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    public float score = 0f;
    public float highScore = 0f;
    public List<GameObject> obstaclePrefabs;
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI ScorePanel;
    public TextMeshProUGUI HighScorePanel;
    public float spawnInterval = 2f;
    public Transform obstacleSpawnPoint;

    float timer = 0f;
    float intervalTimer = 0f;
    float elapsedTime = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        highScore = PlayerPrefs.GetFloat("HighScore", 0f);
    }
    void Update()
    {
        timer += Time.deltaTime;
        intervalTimer += Time.deltaTime;
        elapsedTime += Time.deltaTime;
        score = elapsedTime; // score tính bằng giây
        if (timer >= spawnInterval)
        {
            SpawnObstacle();
            timer = 0f;
        }
        if (intervalTimer >= 10f)
        {
            spawnInterval = Mathf.Max(0.05f, spawnInterval - 0.1f);
            intervalTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score:F2}";
    }

    public void AddScore(float amount)
    {
        score += amount;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    void SpawnObstacle()
    {
        if (obstaclePrefabs != null && obstaclePrefabs.Count > 0)
        {
            int randomIndex = Random.Range(0, obstaclePrefabs.Count);
            Vector3 spawnPos = GetRandomOutsideCameraPosition();
            GameObject obstacle = Instantiate(obstaclePrefabs[randomIndex], spawnPos, Quaternion.identity);
            float randomScale = Random.Range(0.5f, 1.1f);
            obstacle.transform.localScale = new Vector3(randomScale, randomScale, 1f);
            obstacle.tag = "Obstacle";
        }
    }

    public void GameOver()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat("HighScore", highScore);
            PlayerPrefs.Save();
        }
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        if (ScorePanel != null)
            ScorePanel.text = $"{score:F2}";
        if (HighScorePanel != null)
            HighScorePanel.text = $"{highScore:F2}";
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        // Xóa tất cả obstacle
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obj in obstacles)
        {
            Destroy(obj);
        }
        Cursor.visible = false;
        Time.timeScale = 1f;
        elapsedTime = 0f;
        score = 0;
    }

    Vector3 GetRandomOutsideCameraPosition()
    {
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        Vector3 camPos = cam.transform.position;

        // Chọn cạnh ngẫu nhiên: 0=trên, 1=dưới, 2=trái, 3=phải
        int edge = Random.Range(0, 4);
        float x = 0, y = 0;
        switch (edge)
        {
            case 0: // trên
                x = Random.Range(camPos.x - camWidth / 2, camPos.x + camWidth / 2);
                y = camPos.y + camHeight / 2 + 1f;
                break;
            case 1: // dưới
                x = Random.Range(camPos.x - camWidth / 2, camPos.x + camWidth / 2);
                y = camPos.y - camHeight / 2 - 1f;
                break;
            case 2: // trái
                x = camPos.x - camWidth / 2 - 1f;
                y = Random.Range(camPos.y - camHeight / 2, camPos.y + camHeight / 2);
                break;
            case 3: // phải
                x = camPos.x + camWidth / 2 + 1f;
                y = Random.Range(camPos.y - camHeight / 2, camPos.y + camHeight / 2);
                break;
        }
        return new Vector3(x, y, 0);
    }

    [ContextMenu("Reset High Score")]
    public void ResetHighScore()
    {
        highScore = 0f;
        PlayerPrefs.SetFloat("HighScore", highScore);
        PlayerPrefs.Save();
    }
}
