using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public bool IsUIOpen = false;       
    public bool IsInventoryOpen = false; 
    public bool IsPlayerLocked = false;  
    public RectTransform introBackground; 
    public RectTransform blinkPanel;

    [Header("Survival HUD")]
    public TextMeshProUGUI healthTxt;
    public TextMeshProUGUI hungerTxt;
    public TextMeshProUGUI thirstTxt;
    private float displayHealth;
    private float displayHunger;
    private float displayThirst;
    private float lerpSpeed = 5f;

    public Image medkitProgressImg;
    public Image foodProgressImg;
    public Image drinkProgressImg;
    public TextMeshProUGUI medkitStatusTxt;
    public TextMeshProUGUI foodStatusTxt;
    public TextMeshProUGUI drinkStatusTxt;
    public GameObject bombUIPanel; 
    public TextMeshProUGUI bombCountTxt;

    [Header("Low Health VFX/SFX")]
    public CanvasGroup lowHealthOverlay; 
    public AudioSource heartbeatSFX;
    public CanvasGroup blackScreenFade; 
    public GameObject gameOverPanel; 

    [Header("Flashlight UI")]
    public TextMeshProUGUI batteryCountTxt; 
    public TextMeshProUGUI batteryEnergyTxt; 
    public GameObject flashlightUIPanel; 

    public RectTransform top;
    public RectTransform bottom;

    [Header("Salary Report")]
    public RectTransform SalarayReportPanel;
    public TextMeshProUGUI salaryReportTitleTxt; 
    public TextMeshProUGUI stoneTxt;
    public TextMeshProUGUI goalTxt;
    public TextMeshProUGUI ironTxt;
    public TextMeshProUGUI goldTxt;
    public TextMeshProUGUI diamondTxt;
    public TextMeshProUGUI totalCountTxt;
    public TextMeshProUGUI kpiTxt;
    public TextMeshProUGUI ExceededTxt;
    public TextMeshProUGUI salaryDayTxt;
    public TextMeshProUGUI salaryDayBonusTxt;
    public TextMeshProUGUI TotalReceivedTxt;
    public RectTransform CloseSalarayReportPanelBtn;

    [Header("Delivery UI (New Setup)")]
    [Tooltip("Kéo gameobject 'Delivery UI' (Root) vào đây")]
    public RectTransform deliveryUIRoot;
    
    [Tooltip("Kéo gameobject 'Panel' (nơi chứa các Item) vào đây")]
    public Transform itemContainerPanel;
    
    [Tooltip("Prefab của Item hiển thị trong danh sách")]
    public GameObject deliveryItemPrefab;
    
    [Tooltip("Kéo nút 'Btn Claim All' vào đây")]
    public Button btnClaimAll;

    private void Awake() 
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this); 
            return;
        }
    }

    private void Start()
    {
        // Gán sự kiện cho nút Claim All nếu có
        if (btnClaimAll != null && DeliveryManager.Instance != null)
        {
            btnClaimAll.onClick.AddListener(DeliveryManager.Instance.ClaimAll);
        }

        // Khởi tạo các giá trị hiển thị ban đầu
        if (PlayerStats.Instance != null)
        {
            displayHealth = PlayerStats.Instance.currentHealth;
            displayHunger = PlayerStats.Instance.currentHunger;
            displayThirst = PlayerStats.Instance.currentThirst;

            // Đăng ký sự kiện cập nhật các thành phần UI tĩnh hơn
            PlayerStats.Instance.OnInventoryStatsChanged += UpdateInventoryDependentUI;
            
            // Cập nhật lần đầu
            UpdateInventoryDependentUI();
        }
    }

    private void OnDestroy()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnInventoryStatsChanged -= UpdateInventoryDependentUI;
        }
    }

    private void Update() => UpdateHUD();

    private void UpdateHUD()
    {
        if (PlayerStats.Instance == null) return;

        // Nội suy các giá trị hiển thị để tạo hiệu ứng số tăng/giảm mượt mà (Vẫn để trong Update vì chỉ số Survival giảm liên tục)
        displayHealth = Mathf.MoveTowards(displayHealth, PlayerStats.Instance.currentHealth, Time.deltaTime * lerpSpeed * 20f);
        displayHunger = Mathf.MoveTowards(displayHunger, PlayerStats.Instance.currentHunger, Time.deltaTime * lerpSpeed * 20f);
        displayThirst = Mathf.MoveTowards(displayThirst, PlayerStats.Instance.currentThirst, Time.deltaTime * lerpSpeed * 20f);

        if (healthTxt) healthTxt.text = Mathf.CeilToInt(displayHealth).ToString();
        if (hungerTxt) hungerTxt.text = Mathf.CeilToInt(displayHunger).ToString();
        if (thirstTxt) thirstTxt.text = Mathf.CeilToInt(displayThirst).ToString();

        // Cập nhật năng lượng đèn pin (vẫn cần trong Update vì pin giảm theo thời gian thực khi bật)
        if (flashlightUIPanel && flashlightUIPanel.activeSelf && batteryEnergyTxt)
        {
            batteryEnergyTxt.text = Mathf.CeilToInt(PlayerStats.Instance.currentBattery) + "%";
        }
    }

    // Chỉ cập nhật khi số lượng bom, pin, hoặc quyền sở hữu thay đổi (Thông qua Event)
    private void UpdateInventoryDependentUI()
    {
        if (PlayerStats.Instance == null) return;

        // --- Cập nhật UI Bom ---
        if (bombUIPanel)
        {
            bool shouldShowBomb = PlayerStats.Instance.hasBombPermission && 
                                 (PlayerStats.Instance.hasEverOwnedBomb || PlayerStats.Instance.bombCount > 0);
            
            if (bombUIPanel.activeSelf != shouldShowBomb) bombUIPanel.SetActive(shouldShowBomb);
            
            if (shouldShowBomb && bombCountTxt) 
                bombCountTxt.text = "x" + PlayerStats.Instance.bombCount;
        }

        // --- Cập nhật UI Đèn pin ---
        if (flashlightUIPanel)
        {
            bool shouldShowFlash = PlayerStats.Instance.hasFlashlight;
            if (flashlightUIPanel.activeSelf != shouldShowFlash) flashlightUIPanel.SetActive(shouldShowFlash);

            if (shouldShowFlash && batteryCountTxt)
            {
                batteryCountTxt.text = "x" + PlayerStats.Instance.batteryCount;
            }
        }
    }

    public void SetUIState(bool isOpen)
    {
        IsUIOpen = isOpen;
        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (!IsInventoryOpen) 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void OpenPanel(RectTransform panel)
    {
        if (panel == null) return;
        panel.gameObject.SetActive(true);
        SetUIState(true);
    }

    public void ClosePanel(RectTransform panel)
    {
        if (panel == null) return;
        panel.gameObject.SetActive(false);
        SetUIState(false);

        // PHÁT ÂM THANH TRƯỢT GIẤY KHI ĐÓNG PANEL
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayPaperSlideSound(transform.position);
    }

    public void OpenDeliveryUI()
    {
        Debug.Log("<color=cyan>[UIManager] Opening Delivery UI...</color>");

        // PHÁT ÂM THANH MỞ THÙNG HÀNG
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayOpenDeliveryBoxSound(transform.position);

        if (deliveryUIRoot == null || itemContainerPanel == null)
        {
            Debug.LogError("<color=red>[UIManager] Thiếu tham chiếu DeliveryUIRoot hoặc ItemContainerPanel!</color>");
            return;
        }

        // 1. CHỐT CHẶN AN TOÀN: Kiểm tra nếu bạn gán nhầm Container là Root
        if (itemContainerPanel == deliveryUIRoot.transform)
        {
            Debug.LogError("<color=red>[UIManager] SAI LẦM: Bạn đang gán ItemContainerPanel trùng với DeliveryUIRoot! Điều này sẽ xóa sạch UI. Hãy gán ItemContainerPanel là cái 'Panel' bên trong.</color>");
            return;
        }

        // 2. Xóa các item cũ trong Panel
        foreach (Transform child in itemContainerPanel)
        {
            // Bảo vệ nếu lỡ tay để nút bấm vào trong Panel
            if (btnClaimAll != null && child == btnClaimAll.transform) continue;
            Destroy(child.gameObject);
        }

        // 3. Hiện UI
        deliveryUIRoot.gameObject.SetActive(true);
        SetUIState(true);

        // 4. Sinh ra item mới
        if (deliveryItemPrefab != null && DeliveryManager.Instance != null)
        {
            foreach (var order in DeliveryManager.Instance.todaysDelivery)
            {
                GameObject itemObj = Instantiate(deliveryItemPrefab, itemContainerPanel);
                DeliveryItemUI itemUI = itemObj.GetComponent<DeliveryItemUI>();
                if (itemUI != null) itemUI.Setup(order.itemSO, order.amount, order.level);
            }
        }

        DotweenAnimationName.Instance.DoScaleUp(deliveryUIRoot, 1, 0.3f);
    }

    public void CloseDeliveryUI()
    {
        if (deliveryUIRoot == null) return;
        DotweenAnimationName.Instance.DoScaleDown(deliveryUIRoot, 0, 0.2f, true);
        SetUIState(false);
    }

    public void ValidateReferences()
    {
        if (deliveryUIRoot == null) Debug.LogError("<color=red>[UIManager] deliveryUIRoot is MISSING!</color>");
        if (itemContainerPanel == null) Debug.LogError("<color=red>[UIManager] itemContainerPanel is MISSING!</color>");
    }

    public void AnimateHealthPop()
    {
        if (healthTxt != null && DotweenAnimationName.Instance != null)
            DotweenAnimationName.Instance.DoPunchScale(healthTxt.transform, 0.5f, 0.5f);
    }

    public void AnimateHungerPop()
    {
        if (hungerTxt != null && DotweenAnimationName.Instance != null)
            DotweenAnimationName.Instance.DoPunchScale(hungerTxt.transform, 0.5f, 0.5f);
    }

    public void AnimateThirstPop()
    {
        if (thirstTxt != null && DotweenAnimationName.Instance != null)
            DotweenAnimationName.Instance.DoPunchScale(thirstTxt.transform, 0.5f, 0.5f);
    }

    public void ShowStatIncrease(string statType, float amount)
    {
        TextMeshProUGUI targetTxt = null;
        Color textColor = Color.green;

        if (statType == "HEALTH") { targetTxt = healthTxt; textColor = Color.red; }
        else if (statType == "HUNGER") { targetTxt = hungerTxt; textColor = new Color(1f, 0.5f, 0f); } // Orange
        else if (statType == "THIRST") { targetTxt = thirstTxt; textColor = Color.cyan; }

        if (targetTxt == null) return;

        // Tạo một bản sao tạm thời của text để bay lên
        GameObject go = new GameObject("FloatingText");
        go.transform.SetParent(targetTxt.transform.parent, false);
        go.transform.position = targetTxt.transform.position + new Vector3(50, 0, 0); // Lệch sang phải một chút

        TextMeshProUGUI floatingTxt = go.AddComponent<TextMeshProUGUI>();
        floatingTxt.text = "+" + Mathf.CeilToInt(amount).ToString();
        floatingTxt.fontSize = targetTxt.fontSize;
        floatingTxt.font = targetTxt.font;
        floatingTxt.color = textColor;
        floatingTxt.alignment = TextAlignmentOptions.Left;

        // Hoạt ảnh bay lên và mờ dần
        go.transform.localPosition += new Vector3(30, 0, 0);
        go.transform.DOLocalMoveY(go.transform.localPosition.y + 50f, 1f).SetUpdate(true);
        floatingTxt.DOFade(0, 1f).SetUpdate(true).OnComplete(() => Destroy(go));
    }
}