using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliveryItemUI : MonoBehaviour
{
    public Image imgIcon;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtAmount;

    public void Setup(ShopItemSO item, int amount, int level)
    {
        if (item == null) {
            Debug.LogError("[DeliveryItemUI] ShopItemSO is null!");
            return;
        }

        if (imgIcon != null)
        {
            if (item.icon != null)
            {
                imgIcon.sprite = item.icon;
                Debug.Log($"[DeliveryItemUI] Set icon for {item.itemName}");
            }
            else
            {
                Debug.LogWarning($"[DeliveryItemUI] Icon is missing on ShopItemSO: {item.itemName}");
            }
        }
        else
        {
            Debug.LogError($"[DeliveryItemUI] imgIcon (Image component) is not assigned in the Inspector for {gameObject.name}!");
        }
        
        if (txtName != null)
        {
            if (item.itemType == ShopItemType.Upgradable)
                txtName.text = $"{item.itemName} (Lv{level})";
            else
                txtName.text = item.itemName;
        }

        if (txtAmount)
        {
            if (item.itemType == ShopItemType.Upgradable || item.itemType == ShopItemType.OneTime)
                txtAmount.text = ""; // Không hiện số lượng cho đồ nâng cấp/hợp đồng
            else
                txtAmount.text = $"x{amount}";
        }
    }
}
