using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public GameObject gameOverUI;
    public float currentScore = 0f;
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Khi game đã kết thúc, kiểm tra nếu người chơi nhấn phím R
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    [Header("World Scaling")]
    public float worldScaleMultiplier = 1f; // Tỉ lệ hiện tại
    public float totalWorldScale = 1f; // TỔNG tỉ lệ thu nhỏ tích lũy
    public event System.Action<float, float> OnWorldRescale; // Thêm tham số duration

    public void RequestWorldRescale(float factor, float duration = 0.5f)
    {
        worldScaleMultiplier = factor;
        totalWorldScale *= factor; 
        if (OnWorldRescale != null) OnWorldRescale(factor, duration);
        Debug.Log("<color=cyan>WORLD RESCALED! Total Scale: " + totalWorldScale + "</color>");
    }

    public void AddScore(float amount)
    {
        // Điểm số phải nhân với worldScaleMultiplier để bù đắp việc cá bị thu nhỏ
        currentScore += (amount / worldScaleMultiplier) * 10f; 
        Debug.Log("Current Score: " + Mathf.FloorToInt(currentScore));
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("Game Over! Final Score: " + Mathf.FloorToInt(currentScore));
        if (gameOverUI != null) gameOverUI.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
