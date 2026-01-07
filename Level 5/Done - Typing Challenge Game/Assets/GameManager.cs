using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 1. Thêm thư viện này để Restart game

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI currentWord_TMP;
    public TextMeshProUGUI scoreTextUI;
    public TextMeshProUGUI wordCountTextUI;
    public TextMeshProUGUI timerTextUI;

    [Header("Game Settings")]
    public float startDuration = 10f; 
    public float bonusTime = 2f;      

    [Header("Game Stats")]
    public int score;
    public int wordsCompleted;
    public float currentTime;         

    private string originalWordString;
    private Camera mainCamera;
    private bool isGameOver = false;

    void Awake()
    {
        mainCamera = Camera.main;
        score = 0;
        wordsCompleted = 0;
        isGameOver = false;
        currentTime = startDuration;

        UpdateScoreUI();
        SpawnNewWord();
    }

    void Update()
    {
        // 2. Nếu Game Over thì kiểm tra phím R để chơi lại
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReloadScene();
            }
            return; // Dừng các xử lý khác (Timer, Typing)
        }

        // --- Logic Game đang chạy ---

        // Timer
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();

            if (currentTime <= 0)
            {
                currentTime = 0;
                GameOver();
            }
        }

        // Typing Input
        if (!string.IsNullOrEmpty(Input.inputString))
        {
            foreach (char c in Input.inputString)
            {
                if (char.IsLetter(c))
                {
                    CheckInput(c);
                }
            }
        }
    }

    void UpdateTimerUI()
    {
        if (timerTextUI != null)
        {
            timerTextUI.text = "Time: " + currentTime.ToString("F1") + "s";
            if (currentTime <= 3f) timerTextUI.color = Color.red;
            else timerTextUI.color = Color.white;
        }
    }

    void CheckInput(char inputChar)
    {
        string currentText = currentWord_TMP.text;

        if (currentText.Length > 0)
        {
            char firstChar = currentText[0];
            
            if (char.ToLower(inputChar) == char.ToLower(firstChar))
            {
                currentWord_TMP.text = currentText.Substring(1);

                if (currentWord_TMP.text.Length == 0)
                {
                    score += originalWordString.Length;
                    wordsCompleted++;
                    currentTime += bonusTime;
                    
                    UpdateScoreUI();
                    SpawnNewWord();
                }
            }
        }
    }

    void SpawnNewWord()
    {
        string newWord = WordData.GetRandomWord();
        currentWord_TMP.text = newWord;
        originalWordString = newWord;
        ChangeCameraBackgroundColor();
    }

    void ChangeCameraBackgroundColor()
    {
        if (mainCamera == null) return;
        mainCamera.backgroundColor = Random.ColorHSV(0f, 1f, 0.4f, 1f, 0.7f, 1f);
    }

    // 3. Hàm xử lý Game Over
    void GameOver()
    {
        isGameOver = true;
        
        // Đổi text thông báo thua
        // \n là ký tự xuống dòng
        currentWord_TMP.text = "Time out! You Lose ! /n Press 'R' to replay";
        
        // Chỉnh lại font size nhỏ hơn một chút nếu text quá dài (tùy chọn)
        currentWord_TMP.fontSize = 40; 
        currentWord_TMP.color = Color.red; // Đổi màu chữ thành đỏ cho nổi bật

        if (timerTextUI != null) timerTextUI.text = "Time: 0.0s";
    }

    // 4. Hàm load lại màn chơi
    void ReloadScene()
    {
        // Lấy tên scene hiện tại và load lại nó
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateScoreUI()
    {
        if (scoreTextUI != null) scoreTextUI.text = "Score: " + score;
        if (wordCountTextUI != null) wordCountTextUI.text = "Words: " + wordsCompleted;
    }
}