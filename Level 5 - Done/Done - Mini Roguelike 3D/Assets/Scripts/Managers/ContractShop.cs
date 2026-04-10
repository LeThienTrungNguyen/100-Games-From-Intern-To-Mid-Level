using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ContractShop : MonoBehaviour
{
    public static ContractShop Instance;

    [Header("UI References")]
    [Header("UI References")]
    public GameObject shop; // Container tổng chứa toàn bộ UI Shop
    public GameObject shopPanel;
    public TextMeshProUGUI txtPlayerMoney;

    [Header("UI Dynamic Items")]
    public GameObject shopItemPrefab;
    
    [Header("Category Parents")]
    public Transform contractParent;
    public Transform generalItemParent;
    public Transform upgradableItemParent;

    private System.Collections.Generic.List<ShopItemUI> instantiatedItems = new System.Collections.Generic.List<ShopItemUI>();

    [Header("Shop Content Lists")]
    public System.Collections.Generic.List<ShopItemSO> contractList;
    public System.Collections.Generic.List<ShopItemSO> generalItemList;
    public System.Collections.Generic.List<ShopItemSO> upgradableItemList;

    public bool hasPendingContractRequest = false;
    public bool hasPendingBombMail = false;

    private void Awake() => Instance = this;

    void Update()
    {
        // Thêm kiểm tra IsPlayerLocked để tránh mở shop lúc Intro hoặc EndDay
        if (Input.GetKeyDown(KeyCode.B) && !Mailbox.IsReadingMail && !UIManager.Instance.IsPlayerLocked)
        {
            ToggleShop();
        }
    }

    public void ToggleShop()
    {
        if (shop == null) return;

        bool newState = !shop.activeSelf;

        if (newState) { 
            shop.SetActive(newState); 
            DotweenAnimationName.Instance.DoScaleUp(shop.transform, 1, 0.5f); 
            PopulateAllCategories(); // Tạo các item cho từng khu vực
        } else {
            DotweenAnimationName.Instance.DoScaleDown(shop.transform, 0, 0.2f, true);
        }
        UIManager.Instance.SetUIState(newState);
        if (newState) UpdateShopUI();
    }

    private void PopulateAllCategories()
    {
        // Xóa sạch các item cũ
        foreach (Transform child in contractParent) { Destroy(child.gameObject); }
        foreach (Transform child in generalItemParent) { Destroy(child.gameObject); }
        foreach (Transform child in upgradableItemParent) { Destroy(child.gameObject); }
        instantiatedItems.Clear();

        // 1. Populate Contracts
        PopulateCategory(contractList, contractParent);
        
        // 2. Populate General Items
        PopulateCategory(generalItemList, generalItemParent);

        // 3. Populate Upgradable Items
        PopulateCategory(upgradableItemList, upgradableItemParent);
    }

    private void PopulateCategory(System.Collections.Generic.List<ShopItemSO> list, Transform parent)
    {
        if (parent == null) { Debug.LogError($"<color=red>Shop Error: Parent transform is null!</color>"); return; }
        if (list == null || list.Count == 0) { Debug.LogWarning($"<color=yellow>Shop Warning: Item list for {parent.name} is empty.</color>"); return; }

        foreach (var itemSO in list)
        {
            // Kiểm tra trạng thái đã mua
            bool alreadyPurchased = itemSO.itemType == ShopItemType.OneTime && PlayerStats.Instance.IsItemPurchased(itemSO.itemID);
            
            if (alreadyPurchased)
            {
                Debug.Log($"<color=white>Shop: Bỏ qua '{itemSO.itemName}' vì đã sở hữu.</color>");
                continue;
            }

            GameObject newItem = Instantiate(shopItemPrefab, parent);
            newItem.name = itemSO.itemName; // Đổi tên GameObject theo tên vật phẩm
            ShopItemUI ui = newItem.GetComponent<ShopItemUI>();
            if (ui != null)
            {
                ui.Setup(itemSO);
                instantiatedItems.Add(ui);
            }
        }
    }

    public void UpdateShopUI()
    {
        if (PlayerStats.Instance == null) return;

        float currentMoney = PlayerStats.Instance.moneyCount;
        txtPlayerMoney.text = $"Ví tiền: {currentMoney:F2}$";

        // Cập nhật trạng thái nút bấm cho tất cả các item đang hiển thị
        foreach (var itemUI in instantiatedItems)
        {
            if (itemUI != null) itemUI.RefreshUI();
        }
    }

    public void BuyItem(ShopItemSO item)
    {
        if (item == null || PlayerStats.Instance == null) return;

        // Sử dụng GetDisplayLevel để tính giá dựa trên level cao nhất (đang chờ hoặc thực tế)
        int currentDisplayLevel = PlayerStats.Instance.GetDisplayLevel(item.itemID);
        
        // --- CHỐT CHẶN BẢO VỆ: Kiểm tra giới hạn hợp đồng ---
        if (item.itemType == ShopItemType.Upgradable)
        {
            int maxAllowed = PlayerStats.Instance.GetMaxAllowedLevel(item.itemID);
            if (currentDisplayLevel >= maxAllowed)
            {
                Debug.LogWarning($"<color=red>Giao dịch bị từ chối: Cần nâng cấp hợp đồng để mua {item.itemName} Lv{currentDisplayLevel + 1}!</color>");
                UpdateShopUI(); // Refresh lại UI để đảm bảo nút bị khóa
                return;
            }
        }
        // ---------------------------------------------------

        float price = item.GetCurrentPrice(currentDisplayLevel);

        if (price < 0) { Debug.Log("Hết cấp độ nâng cấp!"); return; }
        if (item.itemType == ShopItemType.OneTime && PlayerStats.Instance.IsItemPurchased(item.itemID)) return;

        if (PlayerStats.Instance.SpendMoney(price))
        {
            // CHUYỂN TOÀN BỘ VÀO HỆ THỐNG GIAO HÀNG
            if (DeliveryManager.Instance != null)
            {
                int orderLevel = (item.itemType == ShopItemType.Upgradable) ? currentDisplayLevel + 1 : 0;
                int purchaseAmount = item.amountPerPurchase > 0 ? item.amountPerPurchase : 1;
                
                DeliveryManager.Instance.AddOrder(item, purchaseAmount, orderLevel);
                Debug.Log($"<color=green>[Shop] Đã đặt mua: {item.itemName} x{purchaseAmount}. Giá: {price}$</color>");
            }

            UpdateShopUI();
        }
        else
        {
            Debug.Log("<color=red>Không đủ tiền!</color>");
        }
    }

    // Xóa hàm ExecuteItemEffect cũ vì logic đã chuyển sang DeliveryManager

}