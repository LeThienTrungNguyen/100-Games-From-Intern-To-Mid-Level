using UnityEngine;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI txtNarrative; // Kéo TextTMP con của Background vào đây
    
    [Header("Settings")]
    public float waitBeforeFade = 1.0f;
    public float fadeDuration = 1.5f;
    public float textDisplayDuration = 4.0f;

    [Header("Narrative Content")]
    [TextArea(3, 10)]
    public string[] introTexts = new string[] {
        "It's been several months since my previous company went bankrupt. Life has been drifting by in total uncertainty...",
        "I've spent most of my time searching for a new opportunity, but luck seems to have completely abandoned me.",
        "The small amount of savings left in my pocket is dwindling day by day. I'm running out of choices.",
        "Then I stumbled upon information about this sector. A mining job deep beneath the surface.",
        "They say this line of work is grueling and dangerous, with risks lurking around every corner.",
        "But honestly... who cares? If I don't take this job, I'll starve to death long before any danger can find me.",
        "Today is my first day on the shift. I hope these caves will grant me a way to survive."
    };

    private void Start()
    {
        StartCoroutine(Co_PlayFullIntro());
    }

    private IEnumerator Co_PlayFullIntro()
    {
        if (UIManager.Instance == null || UIManager.Instance.introBackground == null)
        {
            Debug.LogWarning("IntroManager: Thiếu UIManager hoặc introBackground!");
            yield break;
        }

        // 1. KHÓA TOÀN BỘ
        UIManager.Instance.SetUIState(true);
        UIManager.Instance.IsPlayerLocked = true;
        
        // Setup Background
        Image bgImage = UIManager.Instance.introBackground.GetComponent<Image>();
        if (bgImage != null) {
            Color c = bgImage.color; c.a = 1; bgImage.color = c;
            UIManager.Instance.introBackground.gameObject.SetActive(true);
        }

        // Setup Text ban đầu (mờ)
        if (txtNarrative != null) {
            Color tc = txtNarrative.color; tc.a = 0; txtNarrative.color = tc;
            txtNarrative.gameObject.SetActive(true);
        }

        yield return new WaitForSecondsRealtime(waitBeforeFade);

        // 2. CHẠY CHUỖI TỰ SỰ
        foreach (string line in introTexts)
        {
            if (txtNarrative != null)
            {
                txtNarrative.text = line;
                // Fade In chữ
                yield return txtNarrative.DOFade(1, 1.5f).SetUpdate(true).WaitForCompletion();
                // Chờ người chơi đọc
                yield return new WaitForSecondsRealtime(textDisplayDuration);
                // Fade Out chữ
                yield return txtNarrative.DOFade(0, 1f).SetUpdate(true).WaitForCompletion();
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }

        // 3. FADE OUT BACKGROUND (Làm mờ dần toàn bộ để vào game)
        if (bgImage != null)
        {
            bgImage.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() => {
                // 4. MỞ KHÓA VÀ BẮT ĐẦU GAME
                UIManager.Instance.introBackground.gameObject.SetActive(false);
                UIManager.Instance.SetUIState(false);
                UIManager.Instance.IsPlayerLocked = false;

                // 5. KÍCH HOẠT CÁC HỆ THỐNG GAME SAU INTRO
                if (Mailbox.Instance != null) {
                    Mailbox.Instance.ReceiveNewMail(MailType.Welcome);
                }
                
                if (DeliveryManager.Instance != null) {
                    DeliveryManager.Instance.SpawnDeliveryBox();
                }
                
                Debug.Log("<color=green>Intro tự sự hoàn tất. Bắt đầu game!</color>");
            });
        }
    }
}