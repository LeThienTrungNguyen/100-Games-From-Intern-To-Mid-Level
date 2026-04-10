using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;
using System.Collections; 

public class MathGameController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI mathText;      
    public TextMeshProUGUI scoreText;     
    public TextMeshProUGUI timeText;      
    
    [Header("Button Labels")]
    public TextMeshProUGUI trueBtnText;   
    public TextMeshProUGUI falseBtnText;  

    [Header("Game Settings")]
    public float baseTime = 5f; 

    // --- BIẾN NỘI BỘ ---
    private float currentTime;           
    private int score = 0;
    private int maxUnlockedLevel = 1; 
    private bool isGameActive = true;
    private bool isCorrect; 
    private bool isProcessingAnswer = false; 
    private Color defaultColor = Color.white; 

    // [MỚI] Biến kiểm tra xem có phải câu đầu tiên không
    private bool isFirstQuestion = true; 
    // [MỚI] Tham chiếu đến Camera chính để đổi màu
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // Lấy Camera chính
        if (trueBtnText != null) defaultColor = trueBtnText.color;
        Debug.Log("GAME START!");
        StartGame();
    }

    void StartGame()
    {
        score = 0;
        maxUnlockedLevel = 1;
        isGameActive = true;
        isProcessingAnswer = false;
        
        // [MỚI] Reset lại trạng thái câu đầu tiên
        isFirstQuestion = true; 
        
        ResetUIColors();

        if (scoreText != null) scoreText.text = "Score: 0";
        GenerateQuestion();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) { RestartGame(); return; }

        if (!isGameActive || isProcessingAnswer) return;

        // [MỚI] LOGIC TIMER: Nếu là câu hỏi đầu tiên thì KHÔNG trừ thời gian
        if (!isFirstQuestion)
        {
            currentTime -= Time.deltaTime;
        }

        // Cập nhật UI thời gian
        if (timeText != null)
        {
            if (isFirstQuestion)
            {
                // Nếu là câu đầu, hiện chữ "Ready" hoặc thời gian đầy
                timeText.text = "Ready?"; 
                timeText.color = Color.yellow; // Màu vàng báo hiệu chờ
            }
            else
            {
                timeText.text = Mathf.Max(0, currentTime).ToString("F1") + "s";
                if (currentTime <= 2.0f) timeText.color = Color.red;
                else timeText.color = defaultColor;
            }
        }

        if (currentTime <= 0 && !isFirstQuestion)
        {
            StartCoroutine(ProcessTimeOut()); 
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) StartCoroutine(ProcessAnswer(true));
        if (Input.GetKeyDown(KeyCode.RightArrow)) StartCoroutine(ProcessAnswer(false));
    }

    IEnumerator ProcessAnswer(bool playerChoseTrue)
    {
        isProcessingAnswer = true; 

        TextMeshProUGUI selectedText = playerChoseTrue ? trueBtnText : falseBtnText;
        bool playerIsCorrect = (playerChoseTrue == isCorrect);
        
        if (selectedText != null)
            selectedText.color = playerIsCorrect ? Color.green : Color.red;

        yield return new WaitForSeconds(0.5f);

        if (playerIsCorrect)
        {
            score++;
            
            // [MỚI] Người chơi đã trả lời đúng câu đầu, bắt đầu tính giờ từ câu sau
            if (isFirstQuestion) isFirstQuestion = false;

            ResetUIColors();
            GenerateQuestion();
            isProcessingAnswer = false; 
        }
        else
        {
            GameOver();
        }
    }
    
    IEnumerator ProcessTimeOut()
    {
        isProcessingAnswer = true;
        if (trueBtnText != null) trueBtnText.color = Color.red;
        if (falseBtnText != null) falseBtnText.color = Color.red;
        if (timeText != null) timeText.text = "0.0s";

        yield return new WaitForSeconds(0.5f);
        GameOver();
    }

    void ResetUIColors()
    {
        if (trueBtnText != null) trueBtnText.color = defaultColor;
        if (falseBtnText != null) falseBtnText.color = defaultColor;
        if (mathText != null) mathText.color = defaultColor;
        if (timeText != null) timeText.color = defaultColor; 
    }

    void GenerateQuestion()
    {
        // [MỚI] ĐỔI MÀU NỀN CAMERA
        ChangeBackgroundColor();

        maxUnlockedLevel = (score / 10) + 1; 
        if (maxUnlockedLevel > 4) maxUnlockedLevel = 4; 

        int questionDifficulty = Random.Range(1, maxUnlockedLevel + 1);
        
        currentTime = baseTime + (questionDifficulty - 1) * 0.5f; 
        
        // Cập nhật UI ngay lập tức
        if (timeText != null) 
        {
             // Nếu là câu đầu thì chưa hiện số giây vội (để Update lo việc hiện chữ Ready)
             if (!isFirstQuestion)
             {
                timeText.text = currentTime.ToString("F1") + "s";
                timeText.color = defaultColor;
             }
        }

        long a = 0; long b = 0; long realResult = 0; string mathSymbol = "";
        int op = Random.Range(0, 4); 

        switch (op)
        {
            case 0: // CỘNG
                a = GetRandomByDigits(questionDifficulty);
                b = GetRandomByDigits(questionDifficulty);
                if (a < b) { long temp = a; a = b; b = temp; }
                realResult = a + b;
                mathSymbol = "+";
                break;

            case 1: // TRỪ
                a = GetRandomByDigits(questionDifficulty);
                b = GetRandomByDigits(questionDifficulty);
                if (a < b) { long temp = a; a = b; b = temp; }
                realResult = a - b;
                mathSymbol = "-";
                break;

            case 2: // NHÂN
                a = GetRandomByDigits(questionDifficulty);
                b = GetRandomByDigits(1); 
                if (a < b) { long temp = a; a = b; b = temp; }
                realResult = a * b;
                mathSymbol = "×"; 
                break;

            case 3: // CHIA
                b = Random.Range(2, 10); 
                long minA = (long)Mathf.Pow(10, questionDifficulty - 1); 
                long maxA = (long)Mathf.Pow(10, questionDifficulty) - 1; 
                long minRes = minA / b; 
                long maxRes = maxA / b; 
                if (minRes < 1) minRes = 1; 
                if (maxRes < minRes) maxRes = minRes + 1; 
                realResult = Random.Range((int)minRes, (int)maxRes + 1); 
                a = realResult * b; 
                mathSymbol = "÷"; 
                break;
        }

        isCorrect = (Random.value > 0.5f);
        long displayResult;

        if (isCorrect)
        {
            displayResult = realResult;
        }
        else
        {
            long offset = 0;
            if (questionDifficulty <= 2) offset = Random.Range(1, 4); 
            else offset = Random.Range(1, 11);

            if (Random.value > 0.5f) displayResult = realResult + offset;
            else displayResult = realResult - offset;
            
            if (displayResult < 0) displayResult = -displayResult; 
            if (displayResult == realResult) displayResult += Random.Range(1, 3);
        }

        if (mathText != null) mathText.text = $"{a} {mathSymbol} {b} = {displayResult}";
        if (scoreText != null) scoreText.text = $"Score: {score}";
    }

    // [MỚI] HÀM ĐỔI MÀU NỀN
    void ChangeBackgroundColor()
    {
        if (mainCamera == null) return;

        // Logic tạo màu tối (Dark Color) để chữ trắng nổi bật
        // H (Hue): Ngẫu nhiên (0 -> 1) để ra đủ loại màu
        // S (Saturation): 0.5 -> 1.0 (Màu đậm, không nhợt nhạt)
        // V (Value/Brightness): 0.2 -> 0.45 (Quan trọng: Phải tối thì chữ trắng mới hiện rõ)
        
        float h = Random.value; 
        float s = Random.Range(0.5f, 1.0f);
        float v = Random.Range(0.2f, 0.45f); // Đừng chỉnh cao quá 0.5, nền sẽ bị sáng

        mainCamera.backgroundColor = Color.HSVToRGB(h, s, v);
    }

    long GetRandomByDigits(int digits)
    {
        if (digits <= 1) return Random.Range(1, 10); 
        int min = (int)Mathf.Pow(10, digits - 1);
        int max = (int)Mathf.Pow(10, digits); 
        return Random.Range(min, max);
    }

    void GameOver()
    {
        isGameActive = false;
        
        if (mathText != null) 
        {
            mathText.color = Color.red; 
            mathText.text += "\n<size=60%>Press 'R' to restart</size>"; 
        }
        Debug.Log("=== GAME OVER ===");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}