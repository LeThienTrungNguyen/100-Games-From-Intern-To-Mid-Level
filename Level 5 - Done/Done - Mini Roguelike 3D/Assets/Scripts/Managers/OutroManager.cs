using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class OutroManager : MonoBehaviour
{
    public static OutroManager Instance;

    [Header("UI References")]
    public GameObject outroContainer;    // Toàn bộ UI Outro (nên có phông nền đen)
    public TextMeshProUGUI txtNarrative; // Text để chạy tự sự
    public GameObject finalChoicePanel;  // Panel chứa câu hỏi "Play again?" và 2 nút
    public TextMeshProUGUI txtFinalQuestion; // Text để hiện câu hỏi "Play again?"
    public Button btnYes;
    public Button btnNo;

    [Header("Settings")]
    public float textDisplayDuration = 4.0f;
    public float fadeDuration = 1.5f;

    [Header("Narrative Content")]
    [TextArea(3, 10)]
    public string[] outroTexts = new string[] {
        "Phew... finally, it's over. That cursed mine and those devilish KPIs nearly drained every bit of my strength.",
        "But I made it through. It's incredible what someone pushed to the edge is capable of doing.",
        "Whatever... the point is, I conquered it. I'm exhausted, but I've earned a decent amount—enough to live comfortably for a long time.",
        "I wonder... should I actually stick with this line of work?",
        "Thank you for completing this demo. If you enjoyed the experience, please consider leaving some feedback. I truly appreciate your time."
    };

    private void Awake() => Instance = this;

    private void Start()
    {
        if (btnYes) btnYes.onClick.AddListener(RestartGame);
        if (btnNo) btnNo.onClick.AddListener(QuitGame);
        
        outroContainer.SetActive(false);
        finalChoicePanel.SetActive(false);
    }

    public void StartOutro()
    {
        StartCoroutine(Co_PlayFullOutro());
    }

    private IEnumerator Co_PlayFullOutro()
    {
        // 1. CHUẨN BỊ
        outroContainer.SetActive(true);
        UIManager.Instance.SetUIState(true);
        UIManager.Instance.IsPlayerLocked = true;

        // Reset Choice Panel ban đầu
        if (finalChoicePanel != null) finalChoicePanel.SetActive(false);

        // Fade In toàn bộ Outro Panel (Yêu cầu có CanvasGroup)
        CanvasGroup group = outroContainer.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 0;
            group.interactable = true;
            group.blocksRaycasts = true;
            yield return group.DOFade(1, fadeDuration).SetUpdate(true).WaitForCompletion();
        }

        if (txtNarrative != null) {
            Color tc = txtNarrative.color; tc.a = 0; txtNarrative.color = tc;
        }

        yield return new WaitForSecondsRealtime(0.5f);

        // 2. CHẠY TỰ SỰ
        foreach (string line in outroTexts)
        {
            if (txtNarrative != null)
            {
                txtNarrative.text = line;
                yield return txtNarrative.DOFade(1, fadeDuration).SetUpdate(true).WaitForCompletion();
                yield return new WaitForSecondsRealtime(textDisplayDuration);
                yield return txtNarrative.DOFade(0, 1f).SetUpdate(true).WaitForCompletion();
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }

        // Ẩn Text tự sự sau khi xong
        if (txtNarrative != null) txtNarrative.gameObject.SetActive(false);

        // 3. HIỆN LỰA CHỌN CUỐI CÙNG
        if (finalChoicePanel != null)
        {
            if (txtFinalQuestion != null) txtFinalQuestion.text = "Do you want to play again?";
            
            finalChoicePanel.SetActive(true);
            finalChoicePanel.transform.localScale = Vector3.zero;
            
            // Hiện các nút bấm
            if (btnYes) btnYes.gameObject.SetActive(true);
            if (btnNo) btnNo.gameObject.SetActive(true);

            // Đảm bảo CanvasGroup của container hoặc chính nó cho phép bấm
            CanvasGroup choiceGroup = finalChoicePanel.GetComponent<CanvasGroup>();
            if (choiceGroup != null) { choiceGroup.interactable = true; choiceGroup.blocksRaycasts = true; }

            yield return finalChoicePanel.transform.DOScale(1, 0.8f).SetEase(Ease.OutBack).SetUpdate(true).WaitForCompletion();
            
            // Hiện chuột để người chơi chọn
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}