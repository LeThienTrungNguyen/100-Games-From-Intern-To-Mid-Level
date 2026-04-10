using UnityEngine;
using TMPro;
using DG.Tweening;

public class Mailbox : MonoBehaviour
{
    public static Mailbox Instance;
    public static bool IsReadingMail = false;

    [Header("Visuals")]
    public GameObject notificationIcon;
    public GameObject mailUIPanel;
    public TextMeshProUGUI txtTitle;
    public TextMeshProUGUI txtContent;

    [Header("Pagination")]
    public UnityEngine.UI.Button btnNext;
    public UnityEngine.UI.Button btnPrev;
    public TextMeshProUGUI txtPageIndicator;
    public GameObject btnClose;

    [Header("Mail Data")]
    public System.Collections.Generic.List<MailContentSO> mailTemplates = new System.Collections.Generic.List<MailContentSO>();

    [System.Serializable]
    public class MailInstance
    {
        public MailType type;
        public string title;
        public string message;
    }

    private System.Collections.Generic.List<MailInstance> receivedMails = new System.Collections.Generic.List<MailInstance>();
    private int currentMailIndex = -1;
    private int lastReadIndex = -1; // Lưu chỉ số lá thư cuối cùng đã đọc xong
    private int currentPage = 1;
    public bool hasNewMail = false; // Cờ hiệu để hiện icon thông báo
    
    private RectTransform panelRect;
    private Vector2 originalPanelPos;
    private bool isAnimating = false;
    private Tween notificationTween;

    private void Awake()
    {
        Instance = this;
        if (mailUIPanel != null)
        {
            panelRect = mailUIPanel.GetComponent<RectTransform>();
            originalPanelPos = panelRect.anchoredPosition;
        }
    }

    private void Start()
    {
        if (btnNext) btnNext.onClick.AddListener(NextPage);
        if (btnPrev) btnPrev.onClick.AddListener(PrevPage);
        
        SetupNotificationAnimation();
        // ReceiveNewMail(MailType.Welcome); // Đã chuyển sang IntroManager gọi sau khi kết thúc Intro
    }

    private void Update()
    {
        if (IsReadingMail && currentMailIndex >= 0 && currentMailIndex < receivedMails.Count)
        {
            MailInstance currentMail = receivedMails[currentMailIndex];

            // Nếu đang đọc thư nhiệm vụ
            if (currentMail.type == MailType.QuestNew)
            {
                // Nếu nhiệm vụ đã hoàn thành nhưng chưa nộp
                if (QuestManager.Instance != null && QuestManager.Instance.isQuestCompletedToday && !QuestManager.Instance.isQuestSubmitted)
                {
                    if (Input.GetKeyDown(KeyCode.Y))
                    {
                        QuestManager.Instance.SubmitActiveQuest();
                    }
                    else if (Input.GetKeyDown(KeyCode.N))
                    {
                        CloseMail();
                    }
                }
            }
        }
    }

    private void SetupNotificationAnimation()
    {
        if (notificationIcon == null) return;

        Vector3 originalScale = notificationIcon.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        notificationIcon.transform.DOLocalRotate(new Vector3(0, 0, 360), 3f, RotateMode.LocalAxisAdd)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);

        Sequence s = DOTween.Sequence();
        s.Append(notificationIcon.transform.DOMoveY(notificationIcon.transform.position.y + 0.5f, 1f).SetEase(Ease.InOutQuad));
        s.Join(notificationIcon.transform.DOScale(targetScale, 1f).SetEase(Ease.InOutQuad));
        s.Append(notificationIcon.transform.DOMoveY(notificationIcon.transform.position.y, 1f).SetEase(Ease.InOutQuad));
        s.Join(notificationIcon.transform.DOScale(originalScale, 1f).SetEase(Ease.InOutQuad));
        
        s.SetLoops(-1, LoopType.Restart);
        notificationTween = s;
    }

    public void ReceiveNewMail(MailType type, string customTitle = "", string customMessage = "")
    {
        // 1. Nếu là thư kết quả nhiệm vụ, xóa tất cả thư nhiệm vụ (QuestNew) cũ
        if (type == MailType.QuestSuccess || type == MailType.QuestFailed)
        {
            receivedMails.RemoveAll(m => m.type == MailType.QuestNew);
            if (lastReadIndex >= receivedMails.Count) lastReadIndex = receivedMails.Count - 1;
        }

        MailInstance newMail = new MailInstance { type = type };
        
        if (string.IsNullOrEmpty(customTitle) || string.IsNullOrEmpty(customMessage))
        {
            MailContentSO template = mailTemplates.Find(m => m.mailType == type);
            if (template != null)
            {
                newMail.title = string.IsNullOrEmpty(customTitle) ? template.title : customTitle;
                newMail.message = string.IsNullOrEmpty(customMessage) ? template.message : customMessage;
            }
        }
        else
        {
            newMail.title = customTitle;
            newMail.message = customMessage;
        }

        receivedMails.Add(newMail);

        // 2. Đảm bảo thư nhiệm vụ ĐANG HOẠT ĐỘNG (QuestNew mới nhất) luôn ở CUỐI danh sách
        if (type != MailType.QuestNew)
        {
            MailInstance activeQuestMail = receivedMails.FindLast(m => m.type == MailType.QuestNew);
            if (activeQuestMail != null)
            {
                receivedMails.Remove(activeQuestMail);
                receivedMails.Add(activeQuestMail);
            }
        }

        hasNewMail = true;
        if (notificationIcon) notificationIcon.SetActive(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayNotificationSound(transform.position);
    }

    public void UpdateMailContent(MailType type, string newTitle, string newMessage)
    {
        // Sử dụng FindLast để đảm bảo cập nhật đúng lá thư nhiệm vụ hiện tại
        MailInstance target = receivedMails.FindLast(m => m.type == type);
        if (target != null)
        {
            target.title = newTitle;
            target.message = newMessage;

            if (mailUIPanel.activeSelf && currentMailIndex >= 0 && currentMailIndex < receivedMails.Count && receivedMails[currentMailIndex] == target)
            {
                txtTitle.text = newTitle;
                txtContent.text = newMessage;
                txtContent.ForceMeshUpdate();
            }
        }
    }

    public void OpenMail()
    {
        if (receivedMails.Count == 0) return;

        // Khi mở, bắt đầu từ lá thư chưa đọc đầu tiên (cũ nhất trong số thư mới)
        if (!mailUIPanel.activeSelf)
        {
            // PHÁT ÂM THANH MỞ THƯ
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayLetterOpenSound(transform.position);

            currentMailIndex = lastReadIndex + 1;
            
            // Nếu đã đọc hết rồi thì mở lá thư cuối cùng
            if (currentMailIndex >= receivedMails.Count)
                currentMailIndex = receivedMails.Count - 1;

            hasNewMail = false; // Đã vào xem
            if (notificationIcon) notificationIcon.SetActive(false);

            // Bật UI TRƯỚC khi DisplayMail để TMP có kích thước khung hình chính xác để tính số trang
            mailUIPanel.SetActive(true);
            if (panelRect != null) panelRect.anchoredPosition = originalPanelPos;
            
            DisplayMail(receivedMails[currentMailIndex]);

            DotweenAnimationName.Instance.DoScaleUp(mailUIPanel.transform, 1, 0.2f);
            SetUIMode(true);
        }
    }

    private void DisplayMail(MailInstance mail)
    {
        if (mail != null)
        {
            txtTitle.text = mail.title;
            txtContent.text = mail.message;
            
            txtContent.overflowMode = TextOverflowModes.Page;
            currentPage = 1;
            txtContent.pageToDisplay = currentPage;
            
            txtContent.ForceMeshUpdate();
            UpdatePaginationButtons();
        }
    }

    public void NextPage()
    {
        if (isAnimating) return;

        // PHÁT ÂM THANH TRƯỢT GIẤY
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPaperSlideSound(transform.position);

        int totalPages = (txtContent != null && txtContent.textInfo != null) ? txtContent.textInfo.pageCount : 1;
        if (currentPage < totalPages)
        {
            AnimatePageSlide(true, () => {
                currentPage++;
                txtContent.pageToDisplay = currentPage;
            });
        }
    }

    public void PrevPage()
    {
        if (isAnimating) return;

        // PHÁT ÂM THANH TRƯỢT GIẤY
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPaperSlideSound(transform.position);

        if (currentPage > 1)
        {
            AnimatePageSlide(false, () => {
                currentPage--;
                txtContent.pageToDisplay = currentPage;
            });
        }
    }

    private void AnimatePageSlide(bool isNext, System.Action changePageLogic)
    {
        if (panelRect == null) return;
        isAnimating = true;
        float slideDistance = 1200f; 
        float duration = 0.25f;

        Vector2 exitPos = originalPanelPos + new Vector2(isNext ? -slideDistance : slideDistance, 0);
        Vector2 enterPos = originalPanelPos + new Vector2(isNext ? slideDistance : -slideDistance, 0);

        panelRect.DOAnchorPos(exitPos, duration).SetEase(Ease.InCubic).SetUpdate(true).OnComplete(() => {
            changePageLogic.Invoke();
            UpdatePaginationButtons();
            panelRect.anchoredPosition = enterPos;
            panelRect.DOAnchorPos(originalPanelPos, duration).SetEase(Ease.OutCubic).SetUpdate(true).OnComplete(() => {
                isAnimating = false;
            });
        });
    }

    private void UpdatePaginationButtons()
    {
        if (txtContent == null || txtContent.textInfo == null) return;
        int totalPages = txtContent.textInfo.pageCount;
        bool isLastPage = (currentPage >= totalPages);
        
        // Thư kết thúc game NoMoreOres luôn là lá thư cuối cùng
        bool isEndingMail = (currentMailIndex >= 0 && receivedMails[currentMailIndex].type == MailType.NoMoreOres);

        if (btnNext) btnNext.gameObject.SetActive(!isLastPage);
        if (btnPrev) btnPrev.gameObject.SetActive(currentPage > 1);
        
        if (btnClose)
        {
            if (isEndingMail) btnClose.SetActive(isLastPage);
            else btnClose.SetActive(true);
        }

        if (txtPageIndicator)
        {
            txtPageIndicator.gameObject.SetActive(totalPages > 1);
            txtPageIndicator.text = $"Page {currentPage}/{totalPages}";
        }
    }

    public void CloseMail()
    {
        if (isAnimating) return;

        // Cập nhật tiến độ đọc
        if (currentMailIndex > lastReadIndex) lastReadIndex = currentMailIndex;

        // Nếu vẫn còn thư mới hơn chưa xem (index < Count - 1), trượt xuống để xem thư tiếp theo
        if (currentMailIndex < receivedMails.Count - 1)
        {
            // PHÁT ÂM THANH TRƯỢT GIẤY KHI CHUYỂN THƯ
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayPaperSlideSound(transform.position);

            AnimateMailSlideDown(() => {
                currentMailIndex++;
                DisplayMail(receivedMails[currentMailIndex]);
            });
        }
        else
        {
            // Nếu là lá thư mới nhất trong danh sách, đóng UI
            MailType currentType = receivedMails[currentMailIndex].type;
            
            SetUIMode(false);
            DotweenAnimationName.Instance.DoScaleDown(mailUIPanel.transform, 0, 0.2f, true);
            UIManager.Instance.SetUIState(false);

            // PHÁT ÂM THANH ĐÓNG THƯ
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayLetterCloseSound(transform.position);

            if (currentType == MailType.NoMoreOres)
            {
                if (OutroManager.Instance != null) OutroManager.Instance.StartOutro();
            }
        }
    }

    private void AnimateMailSlideDown(System.Action changeMailLogic)
    {
        if (panelRect == null) return;
        isAnimating = true;
        float slideDistance = 1000f; // Trượt xuống dưới
        float duration = 0.3f;

        Vector2 exitPos = originalPanelPos + new Vector2(0, -slideDistance);
        Vector2 enterPos = originalPanelPos + new Vector2(0, slideDistance); // Bay từ trên xuống

        panelRect.DOAnchorPos(exitPos, duration).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() => {
            changeMailLogic.Invoke();
            panelRect.anchoredPosition = enterPos;
            panelRect.DOAnchorPos(originalPanelPos, duration).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() => {
                isAnimating = false;
            });
        });
    }

    private void SetUIMode(bool enable)
    {
        IsReadingMail = enable;
        if (enable)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }
}
