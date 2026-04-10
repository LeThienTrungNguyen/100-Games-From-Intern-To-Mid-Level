using UnityEngine;

public enum ShopItemType { 
    OneTime,    // Mua 1 lần (Hợp đồng, Kỹ năng vĩnh viễn)
    Consumable, // Mua nhiều lần (Thuốc, Quặng, Vật phẩm tiêu hao)
    Upgradable  // Nâng cấp theo cấp độ (Tốc độ chạy Lv1 -> Lv2)
}

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/ShopItem")]
public class ShopItemSO : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string itemID;      // ID duy nhất để lưu trữ dữ liệu
    public string itemName;
    [TextArea(3, 10)]
    public string description;
    public Sprite icon;
    public ShopItemType itemType;

    [Header("Cấu hình Giá")]
    public float basePrice;    // Giá gốc (Dùng cho OneTime và Consumable)
    public float[] upgradePrices; // Mảng giá cho từng cấp (Cấp 0 là giá mua cấp 1, Cấp 1 là cấp 2...)
    [Tooltip("ID của item nâng cấp dùng để tính giá động cho item tiêu hao (Ví dụ: BOMB_RADIUS_UPGRADE cho Bomb Pack)")]
    public string dynamicPriceID; 

    [Header("Thông số Hiệu ứng")]
    public float effectValue;  // Giá trị hiệu ứng (Tốc độ đào, % hồi máu...)
    public int amountPerPurchase = 1; // Số lượng nhận được mỗi lần mua (Dành cho Consumable)

    /// <summary>
    /// Lấy giá hiện tại dựa trên cấp độ hiện có
    /// </summary>
    public float GetCurrentPrice(int currentLevel = 0)
    {
        // 1. Nếu là đồ nâng cấp (Upgradable), lấy giá theo level hiện tại
        if (itemType == ShopItemType.Upgradable)
        {
            if (upgradePrices != null && currentLevel < upgradePrices.Length)
                return upgradePrices[currentLevel];
            return -1; // Đạt cấp tối đa
        }
        
        // 2. Nếu là đồ tiêu hao (Consumable) và có ID giá động (như Bomb Pack tăng giá theo Radius Upgrade)
        if (itemType == ShopItemType.Consumable && !string.IsNullOrEmpty(dynamicPriceID))
        {
            // Nếu có mảng giá nâng cấp, lấy theo level của dynamicPriceID
            if (upgradePrices != null && upgradePrices.Length > 0)
            {
                int level = currentLevel; // Giá trị này sẽ được truyền từ ShopItemUI (là level của dynamicPriceID)
                if (level < upgradePrices.Length)
                    return upgradePrices[level];
                else
                    return upgradePrices[upgradePrices.Length - 1]; // Lấy giá cấp cuối nếu vượt quá
            }
        }

        return basePrice;
    }

    /// <summary>
    /// Kiểm tra xem còn cấp độ để nâng cấp không
    /// </summary>
    public bool CanUpgradeFurther(int currentLevel)
    {
        if (itemType != ShopItemType.Upgradable) return false;
        return upgradePrices != null && currentLevel < upgradePrices.Length;
    }
}