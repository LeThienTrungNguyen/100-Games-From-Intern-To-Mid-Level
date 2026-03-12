using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtDescription;
    public TextMeshProUGUI txtPrice;
    public Image imgIcon;
    public Button btnBuy;
    public TextMeshProUGUI txtBtnLabel; // Kéo Text của Button vào đây

    private ShopItemSO itemData;
    private Color defaultTextColor;
    private Color defaultButtonColor;
    private Image btnImage;

    private void Awake()
    {
        if (txtBtnLabel != null) defaultTextColor = txtBtnLabel.color;
        if (btnBuy != null)
        {
            btnImage = btnBuy.GetComponent<Image>();
            if (btnImage != null) defaultButtonColor = btnImage.color;
        }
    }

    public void Setup(ShopItemSO item)
    {
        itemData = item;
        // ... (phần còn lại của Setup giữ nguyên)
        if (txtName) txtName.text = item.itemName;
        if (txtDescription) txtDescription.text = item.description;
        if (imgIcon && item.icon) imgIcon.sprite = item.icon;

        btnBuy.onClick.RemoveAllListeners();
        btnBuy.onClick.AddListener(() => ContractShop.Instance.BuyItem(itemData));

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (itemData == null || PlayerStats.Instance == null) return;

        // Logic kiểm tra điều kiện mua
        bool canBuyLogic = true;

        // Xử lý Level để tính giá:
        // Nếu là item tiêu hao (Consumable) có ID giá động, lấy level của ID đó (VD: Bomb lấy Lv của Radius Upgrade)
        // Nếu không, lấy level của chính nó (VD: Pickaxe lấy Lv của Pickaxe)
        string levelToFetch = !string.IsNullOrEmpty(itemData.dynamicPriceID) ? itemData.dynamicPriceID : itemData.itemID;
        int displayLevel = PlayerStats.Instance.GetDisplayLevel(levelToFetch);
        
        float price = itemData.GetCurrentPrice(displayLevel);
        bool isBought = PlayerStats.Instance.IsItemPurchased(itemData.itemID);
        bool isShipping = DeliveryManager.Instance != null && DeliveryManager.Instance.IsInDelivery(itemData.itemID);

        // 1. Cập nhật Tên: Tên Gốc + (Lv kế tiếp) nếu là đồ nâng cấp
        if (txtName)
        {
            if (itemData.itemType == ShopItemType.Upgradable)
            {
                // Cấp độ hiển thị đã bao gồm cả cấp độ đang Shipping (nhờ logic PlayerStats mới)
                txtName.text = $"{itemData.itemName} (Lv{displayLevel + 1})";
            }
            else
            {
                txtName.text = itemData.itemName;
            }
        }

        // 2. Xử lý hiển thị Giá
        if (txtPrice)
        {
            // Đối với Hợp đồng (OneTime), nếu đang Shipping thì coi như đã mua để khóa nút
            if (itemData.itemType == ShopItemType.OneTime && isShipping)
            {
                txtPrice.text = "SHIPPING...";
                canBuyLogic = false;
            }
            else if (itemData.itemType == ShopItemType.OneTime && isBought)
            {
                txtPrice.text = "OWNED";
                canBuyLogic = false;
            }
            else if (price < 0)
            {
                txtPrice.text = "MAXED";
                canBuyLogic = false;
            }
            else
            {
                txtPrice.text = $"{price}$";
                // Nếu là đồ nâng cấp đang shipping, canBuyLogic vẫn có thể là true để mua tiếp lv sau
            }
        }

        // 3. Reset về màu Default ban đầu
        if (txtBtnLabel != null)
        {
            if (itemData.itemType == ShopItemType.OneTime && isShipping)
                txtBtnLabel.text = "Pending";
            else if (itemData.itemType == ShopItemType.OneTime && isBought)
                txtBtnLabel.text = "Owned";
            else if (itemData.itemType == ShopItemType.Upgradable)
                txtBtnLabel.text = (price < 0) ? "Maxed" : "Upgrade";
            else
                txtBtnLabel.text = "Buy";
            
            txtBtnLabel.color = defaultTextColor;
        }
        if (btnImage != null) btnImage.color = defaultButtonColor;

        float currentMoney = PlayerStats.Instance.moneyCount;

        // KIỂM TRA ĐIỀU KIỆN HỢP ĐỒNG (Chỉ áp dụng nếu chưa sở hữu/đang shipping)
        if (canBuyLogic)
        {
            if (itemData.itemID == "CONTRACT_OFFICIAL")
                canBuyLogic = KPIManager.Instance.currentContract == ContractType.Illegal;
            else if (itemData.itemID == "CONTRACT_ADVANCED")
                canBuyLogic = KPIManager.Instance.currentContract == ContractType.Official;
        }

        // KIỂM TRA GIỚI HẠN LEVEL THEO HỢP ĐỒNG (Sử dụng displayLevel)
        if (canBuyLogic && itemData.itemType == ShopItemType.Upgradable)
        {
            int maxAllowed = PlayerStats.Instance.GetMaxAllowedLevel(itemData.itemID);
            
            if (displayLevel >= maxAllowed && (itemData.upgradePrices != null && displayLevel < itemData.upgradePrices.Length))
            {
                string nextContractName = null;
                if (KPIManager.Instance != null)
                {
                    if (KPIManager.Instance.currentContract == ContractType.Illegal)
                        nextContractName = "Official";
                    else if (KPIManager.Instance.currentContract == ContractType.Official)
                        nextContractName = "Advanced";
                }

                if (!string.IsNullOrEmpty(nextContractName))
                {
                    if (txtPrice) txtPrice.text = "UPGRADE CONTRACT";
                    if (txtBtnLabel) 
                    {
                        txtBtnLabel.text = $"Require {nextContractName}";
                        txtBtnLabel.color = Color.red; 
                    }
                    if (btnImage != null) btnImage.color = new Color(1f, 0.3f, 0.3f, defaultButtonColor.a);
                    
                    canBuyLogic = false;
                }
                else
                {
                    // Nếu không còn hợp đồng nào cao hơn để nâng cấp, thì hiển thị là MAXED
                    if (txtPrice) txtPrice.text = "MAXED";
                    if (txtBtnLabel) 
                    {
                        txtBtnLabel.text = "Maxed";
                        txtBtnLabel.color = defaultTextColor;
                    }
                    if (btnImage != null) btnImage.color = defaultButtonColor;
                    canBuyLogic = false;
                }
            }
        }
        
        // --- NEW: Kiểm tra Giấy phép sử dụng Bom & Giới hạn Hợp đồng ---
        if (itemData.itemID.Contains("BOMB") && itemData.itemID != "BOMB_LICENSE")
        {
            bool hasPermission = PlayerStats.Instance.hasBombPermission;
            bool contractAllowsUpgrade = true;

            // Nếu là nâng cấp bom, yêu cầu tối thiểu hợp đồng Official mới hiện
            if (itemData.itemID == "BOMB_RADIUS_UPGRADE")
            {
                contractAllowsUpgrade = KPIManager.Instance != null && KPIManager.Instance.currentContract != ContractType.Illegal;
            }
            
            bool isVisible = hasPermission && contractAllowsUpgrade;
            gameObject.SetActive(isVisible); 

            if (!isVisible) return; 
            // Không gán lại canBuyLogic ở đây để tránh ghi đè trạng thái MAXED
        }
        else
        {
            // Đảm bảo các item không liên quan đến Bom luôn hiện
            gameObject.SetActive(true);
        }
        // ------------------------------------------

        btnBuy.interactable = !isBought && price >= 0 && currentMoney >= price && canBuyLogic;
    }
}