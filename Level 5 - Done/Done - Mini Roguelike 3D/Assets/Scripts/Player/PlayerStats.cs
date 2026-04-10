using UnityEngine;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    
    [Header("Basic Stats")]
    public float interactableRange = 5f;
    private float baseMiningDamage = 3.125f; 
    public float currentMiningDamage; 

    [Header("Survival Stats")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public bool isDead = false;
    public float maxHunger = 100f;
    public float currentHunger = 100f;
    public float maxThirst = 100f;
    public float currentThirst = 100f;
    public float currentHungerDrainRate; 
    public float currentThirstDrainRate; 
    public int stairsCount = 0; 

    [Header("Flashlight & Battery")]
    public bool hasFlashlight = false;
    public float maxBattery = 100f;
    public float currentBattery = 0f;
    public float batteryDrainRate = 2f; 
    public int batteryCount = 0;

    [Header("Bomb")]
    public bool hasBombPermission = false;
    public bool hasEverOwnedBomb = false;
    public int bombCount = 0;
    public int baseBombRadius = 1; 
    public int currentBombRadius; 

    [Header("Currency")]
    public float moneyCount = 0f;

    [Header("Dynamic Multipliers")]
    public float speedMultiplier = 1.0f;
    public float miningSpeedMultiplier = 1.0f;
    public bool canMine = true;

    [Header("Upgrades (Debug/Inspector)")]
    public int pickaxeLevel = 0;
    public int bombLevel = 0;

    // Events cho UI Tối ưu
    public System.Action OnStatsChanged;
    public System.Action OnMoneyChanged;
    public System.Action OnInventoryStatsChanged; // Dành cho Bom, Pin, Cầu thang

    // Sử dụng Dictionary nội bộ và đảm bảo khởi tạo
    private Dictionary<string, int> itemLevels = new Dictionary<string, int>();
    private Dictionary<string, bool> purchasedItems = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        RefreshDynamicStats();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Cho phép cập nhật thông số ngay lập tức khi chỉnh trong Inspector
        if (itemLevels == null) itemLevels = new Dictionary<string, int>();
        RefreshDynamicStats();
    }
#endif

    private void Update()
    {
        if (bombCount > 0 && !hasEverOwnedBomb) hasEverOwnedBomb = true;
    }

    public void RefreshDynamicStats()
    {
        // Đồng bộ hóa giá trị từ Inspector vào hệ thống Dictionary
        if (itemLevels == null) itemLevels = new Dictionary<string, int>();
        itemLevels["PICKAXE_UPGRADE"] = pickaxeLevel;
        itemLevels["BOMB_RADIUS_UPGRADE"] = bombLevel;

        currentMiningDamage = GetCurrentMiningDamage();
        currentBombRadius = GetCurrentBombRadius();
        Debug.Log($"<color=yellow>[Stats] Stats Refreshed. Damage: {currentMiningDamage}, Bomb Radius: {currentBombRadius}</color>");
    }

    public int GetMaxAllowedLevel(string id)
    {
        if (KPIManager.Instance == null) return 10;
        ContractType contract = KPIManager.Instance.currentContract;

        if (id == "PICKAXE_UPGRADE")
        {
            if (contract == ContractType.Illegal) return 3; 
            if (contract == ContractType.Official) return 7;
            return 10;
        }
        if (id == "BOMB_RADIUS_UPGRADE")
        {
            if (contract == ContractType.Illegal) return -1;
            if (contract == ContractType.Official) return 1;
            return 3;
        }
        return 10;
    }

    public float GetCurrentMiningDamage()
    {
        int level = GetItemLevel("PICKAXE_UPGRADE");
        int maxAllowed = GetMaxAllowedLevel("PICKAXE_UPGRADE");
        level = Mathf.Min(level, maxAllowed);
        return baseMiningDamage * Mathf.Pow(2, level);
    }

    public int GetCurrentBombRadius()
    {
        int level = GetItemLevel("BOMB_RADIUS_UPGRADE");
        int maxAllowed = GetMaxAllowedLevel("BOMB_RADIUS_UPGRADE");
        level = Mathf.Min(level, maxAllowed);
        return baseBombRadius + level;
    }

    public void SetItemLevel(string id, int level)
    {
        itemLevels[id] = level;
        Debug.Log($"<color=green>[Stats] {id} set to Lv{level}</color>");
        RefreshDynamicStats(); // Cập nhật lại stats ngay khi nâng cấp
    }

    public int GetItemLevel(string id)
    {
        return itemLevels.ContainsKey(id) ? itemLevels[id] : 0;
    }

    public int GetDisplayLevel(string id)
    {
        int current = GetItemLevel(id);
        int deliveryLevel = 0;
        if (DeliveryManager.Instance != null)
        {
            foreach (var order in DeliveryManager.Instance.pendingOrders)
                if (order.itemSO.itemID == id && order.itemSO.itemType == ShopItemType.Upgradable) deliveryLevel = Mathf.Max(deliveryLevel, order.level);
            
            foreach (var order in DeliveryManager.Instance.todaysDelivery)
                if (order.itemSO.itemID == id && order.itemSO.itemType == ShopItemType.Upgradable) deliveryLevel = Mathf.Max(deliveryLevel, order.level);
        }
        return Mathf.Max(current, deliveryLevel);
    }

    public bool IsItemPurchased(string id) 
    {
        if (purchasedItems.ContainsKey(id) && purchasedItems[id]) return true;
        if (id == "FLASHLIGHT" && hasFlashlight) return true;
        // Loại bỏ IsInDelivery ở đây để trạng thái Owned chỉ có sau khi Claim hàng
        return false;
    }

    public void SetItemPurchased(string id, bool status) 
    {
        purchasedItems[id] = status;
        if (id == "FLASHLIGHT") hasFlashlight = status;
    }

    public void AddMoney(float amount)
    {
        moneyCount += amount;
        OnMoneyChanged?.Invoke();
    }

    public bool SpendMoney(float amount)
    {
        if (moneyCount >= amount)
        {
            moneyCount -= amount;
            OnMoneyChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void ChangeHealth(float amount) 
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnStatsChanged?.Invoke();
    }

    public void ChangeHunger(float amount) 
    {
        currentHunger = Mathf.Clamp(currentHunger + amount, 0, maxHunger);
        OnStatsChanged?.Invoke();
    }

    public void ChangeThirst(float amount) 
    {
        currentThirst = Mathf.Clamp(currentThirst + amount, 0, maxThirst);
        OnStatsChanged?.Invoke();
    }

    public void InvokeInventoryStatsChanged() => OnInventoryStatsChanged?.Invoke();
}
