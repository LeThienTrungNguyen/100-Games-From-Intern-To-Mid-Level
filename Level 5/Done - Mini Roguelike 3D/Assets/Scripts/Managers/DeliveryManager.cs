using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeliveryItem
{
    public ShopItemSO itemSO;
    public int amount;
    public int level;
}

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance;

    [Header("Settings")]
    public GameObject deliveryBoxPrefab;
    public Transform spawnPoint; 

    [Header("Current Orders")]
    public List<DeliveryItem> pendingOrders = new List<DeliveryItem>();
    public List<DeliveryItem> todaysDelivery = new List<DeliveryItem>();
    
    private GameObject currentBox;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddOrder(ShopItemSO item, int amount, int level = 0)
    {
        if (item == null) return;

        // Kiểm tra xem đã có đơn hàng cho món này chưa
        DeliveryItem existingOrder = pendingOrders.Find(o => o.itemSO.itemID == item.itemID);

        if (existingOrder != null)
        {
            if (item.itemType == ShopItemType.Upgradable)
            {
                existingOrder.level = level; // Cập nhật lên level cao nhất vừa mua
                Debug.Log($"<color=cyan>[Delivery] Đã cập nhật đơn hàng {item.itemName} lên Lv{level}</color>");
            }
            else if (item.itemType == ShopItemType.Consumable)
            {
                existingOrder.amount += amount; // Cộng dồn số lượng
                Debug.Log($"<color=cyan>[Delivery] Đã cộng dồn đơn hàng {item.itemName}: {existingOrder.amount}</color>");
            }
            // OneTime thường không mua 2 lần nhưng nếu có thì ta bỏ qua hoặc giữ nguyên
            return;
        }

        pendingOrders.Add(new DeliveryItem { itemSO = item, amount = amount, level = level });
        Debug.Log($"<color=cyan>[Delivery] Đã thêm đơn hàng mới: {item.itemName} x{amount}</color>");
    }

    public bool IsInDelivery(string itemID)
    {
        if (string.IsNullOrEmpty(itemID)) return false;
        foreach (var order in pendingOrders) if (order.itemSO != null && order.itemSO.itemID == itemID) return true;
        foreach (var order in todaysDelivery) if (order.itemSO != null && order.itemSO.itemID == itemID) return true;
        return false;
    }

    public void PrepareDeliveryForNextDay()
    {
        if (pendingOrders.Count > 0)
        {
            todaysDelivery.Clear();
            foreach(var item in pendingOrders) {
                if (item != null && item.itemSO != null) todaysDelivery.Add(item);
            }
            pendingOrders.Clear();
        }
    }

    public void SpawnDeliveryBox()
    {
        if (todaysDelivery.Count > 0)
        {
            if (currentBox != null) Destroy(currentBox);
            if (spawnPoint != null && deliveryBoxPrefab != null)
            {
                currentBox = Instantiate(deliveryBoxPrefab, spawnPoint.position, spawnPoint.rotation);
                
                // PHÁT ÂM THANH THÙNG HÀNG TỚI
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayDeliveryBoxArrivedSound(spawnPoint.position);
            }
        }
    }

    public void ClaimAll()
    {
        Debug.Log("<color=green>[Delivery] Claiming all items...</color>");

        // PHÁT ÂM THANH NHẬN HÀNG
        if (AudioManager.Instance != null && spawnPoint != null)
            AudioManager.Instance.PlayClaimSound(spawnPoint.position);

        foreach (var order in todaysDelivery)
        {
            if (order != null && order.itemSO != null) ApplyItemToPlayer(order);
        }
        todaysDelivery.Clear();
        if (currentBox != null) Destroy(currentBox);
        
        if (ContractShop.Instance != null) ContractShop.Instance.UpdateShopUI();
        
        // Đóng UI sau khi nhận hết hàng
        if (UIManager.Instance != null) UIManager.Instance.CloseDeliveryUI();
    }

    private void ApplyItemToPlayer(DeliveryItem order)
    {
        if (PlayerStats.Instance == null || order.itemSO == null) return;

        ShopItemSO item = order.itemSO;
        switch (item.itemType)
        {
            case ShopItemType.OneTime:
                PlayerStats.Instance.SetItemPurchased(item.itemID, true);
                
                if (item.itemID == "FLASHLIGHT") {
                    PlayerStats.Instance.hasFlashlight = true;
                    PlayerStats.Instance.currentBattery = item.effectValue; // Giả định effectValue là pin ban đầu
                }
                // Logic đặc biệt cho Hợp đồng
                else if (item.itemID == "CONTRACT_OFFICIAL") {
                    KPIManager.Instance.currentContract = ContractType.Official;
                    if (Mailbox.Instance != null) Mailbox.Instance.ReceiveNewMail(MailType.ToOfficial);
                } 
                else if (item.itemID == "CONTRACT_ADVANCED") {
                    KPIManager.Instance.currentContract = ContractType.Advanced;
                    if (Mailbox.Instance != null) Mailbox.Instance.ReceiveNewMail(MailType.ToAdvanced);
                }
                // Logic cho Giấy phép bom
                else if (item.itemID == "BOMB_LICENSE") {
                    PlayerStats.Instance.hasBombPermission = true;
                    if (Mailbox.Instance != null) Mailbox.Instance.ReceiveNewMail(MailType.BombLicenseSuccess);
                }
                break;

            case ShopItemType.Consumable:
                if (item.itemID == "BATTERY") 
                {
                    PlayerStats.Instance.batteryCount += order.amount;
                    if (PlayerStats.Instance.currentBattery <= 0) PlayerStats.Instance.currentBattery = 20f; 
                }
                else if (item.itemID == "MINING_BOMB") PlayerStats.Instance.bombCount += order.amount;
                else if (item.itemID == "STAIRS") PlayerStats.Instance.stairsCount += order.amount;
                else
                {
                    // Đưa Medkit, Food, Drink vào Inventory để dùng sau
                    if (InventoryManager.Instance != null)
                    {
                        InventoryManager.Instance.AddConsumable(item, order.amount);
                    }
                }
                break;

            case ShopItemType.Upgradable:
                PlayerStats.Instance.SetItemLevel(item.itemID, order.level);
                break;
        }
    }
}
