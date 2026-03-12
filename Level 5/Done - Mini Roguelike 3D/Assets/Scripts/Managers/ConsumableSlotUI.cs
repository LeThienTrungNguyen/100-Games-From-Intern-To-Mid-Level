using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public enum ConsumableType { Heal, Food, Drink }

public class ConsumableSlotUI : MonoBehaviour
{
    [Header("Settings")]
    public ConsumableType type;
    public KeyCode hotkey;

    [Header("References")]
    public Image imgIcon;
    public TextMeshProUGUI txtCount;
    public GameObject panel; // Chính là cái Panel bao quanh Slot này

    private void Update()
    {
        if (InventoryManager.Instance == null) return;

        // 1. Lấy danh sách tương ứng từ Inventory
        List<InventoryManager.ConsumableItem> targetList = null;
        switch (type)
        {
            case ConsumableType.Heal: targetList = InventoryManager.Instance.healItems; break;
            case ConsumableType.Food: targetList = InventoryManager.Instance.foodItems; break;
            case ConsumableType.Drink: targetList = InventoryManager.Instance.drinkItems; break;
        }

        if (targetList == null) return;

        // 2. Cập nhật trạng thái Ẩn/Hiện
        bool hasItems = targetList.Count > 0;
        if (panel != null && panel.activeSelf != hasItems) panel.SetActive(hasItems);

        if (hasItems)
        {
            // 3. Tự động đổi Icon theo món đầu tiên (nhỏ nhất)
            if (imgIcon != null)
            {
                Sprite currentIcon = targetList[0].itemSO.icon;
                if (imgIcon.sprite != currentIcon) imgIcon.sprite = currentIcon;
            }

            // 4. Cập nhật số lượng của món đó
            if (txtCount != null)
            {
                txtCount.text = "x" + targetList[0].count;
            }
        }
    }
}