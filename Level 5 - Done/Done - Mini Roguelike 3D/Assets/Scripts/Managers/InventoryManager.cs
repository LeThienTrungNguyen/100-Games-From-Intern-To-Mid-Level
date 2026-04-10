using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Ores Storage")]
    public int countStone;
    public int countIron;
    public int countGold;
    public int countDiamond;
    public int countGoal;
    public int totalBlocks;

    [System.Serializable]
    public class ConsumableItem
    {
        public ShopItemSO itemSO;
        public int count;
    }

    [Header("Consumables Storage")]
    public List<ConsumableItem> healItems = new List<ConsumableItem>();
    public List<ConsumableItem> foodItems = new List<ConsumableItem>();
    public List<ConsumableItem> drinkItems = new List<ConsumableItem>();

    [Header("HUD Panels (Chỉ cần kéo 3 cái Panel chính)")]
    public GameObject medkitPanel;
    public GameObject foodPanel;
    public GameObject drinkPanel;

    private Image medkitIcon;
    private TextMeshProUGUI medkitCountTxt;
    private Image foodIcon;
    private TextMeshProUGUI foodCountTxt;
    private Image drinkIcon;
    private TextMeshProUGUI drinkCountTxt;

    [Header("Usage HUD Settings")]
    private float medkitCooldownTimer = 0f;
    private ShopItemSO lastMedkitItemSO;

    private float foodCooldownTimer = 0f;
    private ShopItemSO lastFoodItemSO;

    private float drinkCooldownTimer = 0f;
    private ShopItemSO lastDrinkItemSO;

    [Header("Global Settings")]
    public float requiredHoldTime = 5f;
    public float useCooldown = 2f; 
    private float holdTimer = 0f;
    private KeyCode currentHoldingKey = KeyCode.None;

    [Header("UI Ores References")]
    public TextMeshProUGUI txtStone;
    public TextMeshProUGUI txtGoal;
    public TextMeshProUGUI txtIron;
    public TextMeshProUGUI txtGold;
    public TextMeshProUGUI txtDiamond;
    public TextMeshProUGUI txtTotal;

    [Header("UI Rects for Animation")]
    public RectTransform rectStone;
    public RectTransform rectGoal;
    public RectTransform rectIron;
    public RectTransform rectGold;
    public RectTransform rectDiamond;
    public RectTransform rectTotal;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Khởi tạo Cache cho UI Consumables
        if (medkitPanel) {
            medkitIcon = medkitPanel.GetComponentInChildren<Image>();
            medkitCountTxt = medkitPanel.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (foodPanel) {
            foodIcon = foodPanel.GetComponentInChildren<Image>();
            foodCountTxt = foodPanel.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (drinkPanel) {
            drinkIcon = drinkPanel.GetComponentInChildren<Image>();
            drinkCountTxt = drinkPanel.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        if (UIManager.Instance == null) return;

        // 1. Xử lý Cooldown và UI Cooldown cho từng loại
        HandleCooldownUI(ref medkitCooldownTimer, UIManager.Instance.medkitProgressImg, lastMedkitItemSO, KeyCode.Alpha1);
        HandleCooldownUI(ref foodCooldownTimer, UIManager.Instance.foodProgressImg, lastFoodItemSO, KeyCode.Alpha2);
        HandleCooldownUI(ref drinkCooldownTimer, UIManager.Instance.drinkProgressImg, lastDrinkItemSO, KeyCode.Alpha3);

        // 2. Xử lý giữ phím 1, 2, 3
        HandleHoldInput();

        // Đã loại bỏ việc cập nhật HUD Consumables mỗi frame ở đây
    }

    private void HandleCooldownUI(ref float timer, Image img, ShopItemSO lastItem, KeyCode key)
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            if (img != null)
            {
                img.gameObject.SetActive(true);
                if (lastItem != null) img.sprite = lastItem.icon;
                img.fillAmount = 1f - (timer / useCooldown);
            }
        }
        else if (currentHoldingKey != key)
        {
            // Chỉ ẩn UI khi đã hết cooldown và không phải phím đang được nhấn giữ
            if (img != null && img.gameObject.activeSelf) img.gameObject.SetActive(false);
        }
    }

    private string GetAnimatedStatus(string baseText)
    {
        int dots = (int)(Time.time * 3) % 4; 
        string dotStr = "";
        for (int i = 0; i < dots; i++) dotStr += ".";
        return baseText + dotStr;
    }

    private void HandleHoldInput()
    {
        if (UIManager.Instance == null) return;
        
        if (UIManager.Instance.IsUIOpen || UIManager.Instance.IsPlayerLocked)
        {
            ResetHold();
            return;
        }

        // Tự động ẩn Status Text mỗi frame (sẽ hiện lại nếu đang nhấn giữ)
        if (UIManager.Instance.medkitStatusTxt) UIManager.Instance.medkitStatusTxt.gameObject.SetActive(false);
        if (UIManager.Instance.foodStatusTxt) UIManager.Instance.foodStatusTxt.gameObject.SetActive(false);
        if (UIManager.Instance.drinkStatusTxt) UIManager.Instance.drinkStatusTxt.gameObject.SetActive(false);

        // Xác định phím đang được nhấn và UI tương ứng
        KeyCode targetKey = KeyCode.None;
        ShopItemSO selectedSO = null;
        Image targetImg = null;
        TextMeshProUGUI targetStatusTxt = null;
        string statusBaseText = "";
        float currentCooldown = 0f;

        if (Input.GetKey(KeyCode.Alpha1) && healItems.Count > 0) 
        { 
            targetKey = KeyCode.Alpha1; selectedSO = healItems[0].itemSO; 
            targetImg = UIManager.Instance.medkitProgressImg; 
            targetStatusTxt = UIManager.Instance.medkitStatusTxt;
            statusBaseText = "Healing";
            currentCooldown = medkitCooldownTimer; 
        }
        else if (Input.GetKey(KeyCode.Alpha2) && foodItems.Count > 0) 
        { 
            targetKey = KeyCode.Alpha2; selectedSO = foodItems[0].itemSO; 
            targetImg = UIManager.Instance.foodProgressImg; 
            targetStatusTxt = UIManager.Instance.foodStatusTxt;
            statusBaseText = "Eating";
            currentCooldown = foodCooldownTimer; 
        }
        else if (Input.GetKey(KeyCode.Alpha3) && drinkItems.Count > 0) 
        { 
            targetKey = KeyCode.Alpha3; selectedSO = drinkItems[0].itemSO; 
            targetImg = UIManager.Instance.drinkProgressImg; 
            targetStatusTxt = UIManager.Instance.drinkStatusTxt;
            statusBaseText = "Drinking";
            currentCooldown = drinkCooldownTimer; 
        }

        // Nếu đang trong thời gian hồi của loại item đó, không cho nhấn giữ
        if (currentCooldown > 0)
        {
            ResetHold();
            return;
        }

        if (targetKey != KeyCode.None)
        {
            if (currentHoldingKey != targetKey)
            {
                currentHoldingKey = targetKey;
                holdTimer = 0f;

                if (AudioManager.Instance != null)
                {
                    if (targetKey == KeyCode.Alpha1) AudioManager.Instance.PlayMedkitUsingSound();
                    else if (targetKey == KeyCode.Alpha2) AudioManager.Instance.PlayEatUsingSound();
                    else if (targetKey == KeyCode.Alpha3) AudioManager.Instance.PlayDrinkUsingSound();
                }
            }

            holdTimer += Time.deltaTime;

            // HIỆN STATUS TEXT VÀ HIỆU ỨNG CHỮ
            if (targetStatusTxt != null)
            {
                targetStatusTxt.gameObject.SetActive(true);
                targetStatusTxt.text = GetAnimatedStatus(statusBaseText);
            }

            if (targetImg != null)
            {
                targetImg.gameObject.SetActive(true);
                if (selectedSO != null) targetImg.sprite = selectedSO.icon;
                
                // Khi đang nhấn giữ: Hiện Icon đầy đủ (Không có hiệu ứng chạy)
                targetImg.fillAmount = 1f;
            }

            if (holdTimer >= requiredHoldTime)
            {
                if (targetKey == KeyCode.Alpha1) UseItem(healItems, "HEALTH");
                else if (targetKey == KeyCode.Alpha2) UseItem(foodItems, "HUNGER");
                else if (targetKey == KeyCode.Alpha3) UseItem(drinkItems, "THIRST");
                ResetHold();
            }
        }
        else
        {
            ResetHold();
        }
    }

    private void ResetHold()
    {
        currentHoldingKey = KeyCode.None;
        holdTimer = 0f;
        // Không ẩn UI ở đây nếu đang trong cooldown, việc ẩn UI đã được xử lý trong Update()
        if (AudioManager.Instance != null) AudioManager.Instance.StopUsageSound();
    }

    private void UseItem(List<ConsumableItem> list, string statType)
    {
        if (list == null || list.Count == 0) return;

        // Stop usage sound as soon as item is used
        if (AudioManager.Instance != null) AudioManager.Instance.StopUsageSound();

        ConsumableItem toUse = list[0];
        if (toUse == null || toUse.itemSO == null) return;

        float effect = toUse.itemSO.effectValue;
        toUse.count--;

        if (PlayerStats.Instance != null)
        {
            if (statType == "HEALTH") 
            {
                PlayerStats.Instance.ChangeHealth(effect);
                if (AudioManager.Instance != null) AudioManager.Instance.PlayMedkitFinishSound(PlayerStats.Instance.transform.position);
                if (UIManager.Instance != null) 
                {
                    UIManager.Instance.AnimateHealthPop();
                    UIManager.Instance.ShowStatIncrease("HEALTH", effect);
                }
                
                // Cập nhật Cooldown cho Medkit
                medkitCooldownTimer = useCooldown;
                lastMedkitItemSO = toUse.itemSO;
            }
            else if (statType == "HUNGER") 
            {
                PlayerStats.Instance.ChangeHunger(effect);
                if (AudioManager.Instance != null) AudioManager.Instance.PlayEatFinishSound(PlayerStats.Instance.transform.position);
                if (UIManager.Instance != null) 
                {
                    UIManager.Instance.AnimateHungerPop();
                    UIManager.Instance.ShowStatIncrease("HUNGER", effect);
                }
                
                // Cập nhật Cooldown cho Food
                foodCooldownTimer = useCooldown;
                lastFoodItemSO = toUse.itemSO;
            }
            else if (statType == "THIRST") 
            {
                PlayerStats.Instance.ChangeThirst(effect);
                if (AudioManager.Instance != null) AudioManager.Instance.PlayDrinkFinishSound(PlayerStats.Instance.transform.position);
                if (UIManager.Instance != null) 
                {
                    UIManager.Instance.AnimateThirstPop();
                    UIManager.Instance.ShowStatIncrease("THIRST", effect);
                }
                
                // Cập nhật Cooldown cho Drink
                drinkCooldownTimer = useCooldown;
                lastDrinkItemSO = toUse.itemSO;
            }

            Debug.Log($"<color=green>[Inventory] Used {toUse.itemSO.itemName}. Stat: {statType}, EffectValue: {effect}</color>");
            Debug.Log($"[Inventory] New Stats -> HP: {PlayerStats.Instance.currentHealth}, Hunger: {PlayerStats.Instance.currentHunger}, Thirst: {PlayerStats.Instance.currentThirst}");
        }
        else
        {
            Debug.LogError("[Inventory] PlayerStats.Instance is NULL!");
        }

        if (toUse.count <= 0) list.RemoveAt(0);
        
        // Cập nhật lại HUD ngay lập tức
        UpdateConsumableHUD(medkitPanel, healItems);
        UpdateConsumableHUD(foodPanel, foodItems);
        UpdateConsumableHUD(drinkPanel, drinkItems);
    }

    private void UpdateConsumableHUD(GameObject panel, List<ConsumableItem> list)
    {
        if (panel == null) return;

        bool hasItems = list.Count > 0;
        if (panel.activeSelf != hasItems) panel.SetActive(hasItems);

        if (hasItems)
        {
            Image icon = null;
            TextMeshProUGUI text = null;

            if (panel == medkitPanel) { icon = medkitIcon; text = medkitCountTxt; }
            else if (panel == foodPanel) { icon = foodIcon; text = foodCountTxt; }
            else if (panel == drinkPanel) { icon = drinkIcon; text = drinkCountTxt; }

            if (icon != null) icon.sprite = list[0].itemSO.icon;
            if (text != null) text.text = "x" + list[0].count;
        }
    }

    public void AddItem(VoxelChunk.BlockType type)
    {
        // ƯU TIÊN 1: Kiểm tra xem có đang làm Nhiệm vụ khẩn cấp không
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.TryAddQuestItem(type);
        }

        totalBlocks++;
        if (DotweenAnimationName.Instance != null) DotweenAnimationName.Instance.DoPulseEffect(rectTotal, 1f, 1.5f, 0.2f);

        switch (type)
        {
            case VoxelChunk.BlockType.Stone: countStone++; if (DotweenAnimationName.Instance != null) DotweenAnimationName.Instance.DoPulseEffect(rectStone, 1f, 1.5f, 0.2f); break;
            case VoxelChunk.BlockType.Iron: countIron++; if (DotweenAnimationName.Instance != null) DotweenAnimationName.Instance.DoPulseEffect(rectIron, 1f, 1.5f, 0.2f); break;
            case VoxelChunk.BlockType.Gold: countGold++; if (DotweenAnimationName.Instance != null) DotweenAnimationName.Instance.DoPulseEffect(rectGold, 1f, 1.5f, 0.2f); break;
            case VoxelChunk.BlockType.Diamond: countDiamond++; if (DotweenAnimationName.Instance != null) DotweenAnimationName.Instance.DoPulseEffect(rectDiamond, 1f, 1.5f, 0.2f); break;
            case VoxelChunk.BlockType.Goal: countGoal++; if (DotweenAnimationName.Instance != null) DotweenAnimationName.Instance.DoPulseEffect(rectGoal, 1f, 1.5f, 0.2f); break;
        }
        RefreshStaticUI();
        if (KPIManager.Instance != null) KPIManager.Instance.CheckKPIAchievement();
    }

    public void AddConsumable(ShopItemSO item, int amount)
    {
        if (item == null) return;
        List<ConsumableItem> targetList = null;
        string id = item.itemID.ToUpper();

        if (id.Contains("MEDKIT") || id.Contains("HEAL")) targetList = healItems;
        else if (id.Contains("FOOD")) targetList = foodItems;
        else if (id.Contains("DRINK")) targetList = drinkItems;

        if (targetList == null) return;

        ConsumableItem existing = targetList.Find(i => i.itemSO == item);
        if (existing != null) existing.count += amount;
        else
        {
            targetList.Add(new ConsumableItem { itemSO = item, count = amount });
            targetList.Sort((a, b) => a.itemSO.effectValue.CompareTo(b.itemSO.effectValue));
        }
    }

    public void RefreshStaticUI()
    {
        if (txtStone) txtStone.text = $"{countStone}";
        if (txtIron) txtIron.text = $"{countIron}";
        if (txtGold) txtGold.text = $"{countGold}";
        if (txtDiamond) txtDiamond.text = $"{countDiamond}";
        if (txtGoal) txtGoal.text = $"{countGoal}";
        if (txtTotal) txtTotal.text = $"{totalBlocks}";
    }

    public void SubtractItem(VoxelChunk.BlockType type, int amount)
    {
        switch (type)
        {
            case VoxelChunk.BlockType.Stone: countStone = Mathf.Max(0, countStone - amount); break;
            case VoxelChunk.BlockType.Iron: countIron = Mathf.Max(0, countIron - amount); break;
            case VoxelChunk.BlockType.Gold: countGold = Mathf.Max(0, countGold - amount); break;
            case VoxelChunk.BlockType.Diamond: countDiamond = Mathf.Max(0, countDiamond - amount); break;
            case VoxelChunk.BlockType.Goal: countGoal = Mathf.Max(0, countGoal - amount); break;
        }
        totalBlocks = Mathf.Max(0, totalBlocks - amount);
        RefreshStaticUI();
    }

    public void ClearInventory()
    {
        countStone = countIron = countGold = countDiamond = countGoal = totalBlocks = 0;
        RefreshStaticUI();
    }
}