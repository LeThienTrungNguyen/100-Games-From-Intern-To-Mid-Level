using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct TileStateCombo
{
    [Tooltip("Bitmask (0-15) đại diện cho trạng thái kết nối xung quanh.")]
    public string stateName; // Hiển thị trên Unity Inspector cho dễ hiểu (vd: "15: Nối 4 hướng")
    public Sprite[] animationFrames;
}

[CreateAssetMenu(fileName = "NewTileDataMap", menuName = "Dada Is You/Tile Data Map")]
public class TileDataMap : ScriptableObject
{
    [Header("Tile Settings")]
    [Tooltip("Tên của Object áp dụng (vd: 'wall', 'pipe', 'grass').")]
    public string targetObjectName = "wall";

    [Header("Connection Rules")]
    [Tooltip("NẾU BẬT (Tích CÓ): Wall chỉ tự nối mạch với MỘT Object có cùng Target Object Name (vd: Wall đứng cạnh Wall thì mới nối).\n\nNẾU TẮT (Tích KHÔNG): Nối với tất cả mọi object, miễn là chỗ đó không phải là khoảng trống/rỗng.")]
    public bool connectToSameNameOnly = true;

    [Tooltip("Danh sách Tên Ngoại Lệ (Chỉ có tác dụng nếu bật ConnectToSameNameOnly). Ví dụ điền thêm 'door' thì Wall khi đụng Door cũng sẽ vươn tay ra nối.")]
    public List<string> connectableExtraNames = new List<string>();

    [Header("16 States Configuration (Bitmask: Up=1, Right=2, Down=4, Left=8)")]
    public TileStateCombo[] states = new TileStateCombo[16];

    private void OnValidate()
    {
        // Tự động gán nhãn cho 16 ô để người dùng khỏi nhầm lẫn khi kéo và thả ảnh trong Inspector
        if (states == null || states.Length != 16)
        {
            states = new TileStateCombo[16];
        }

        states[0].stateName = "0: Đứng Cô Đơn (Ko nối)";
        states[1].stateName = "1: Nối Lên TRÊN";
        states[2].stateName = "2: Nối Sang PHẢI";
        states[3].stateName = "3: Mép Góc (TRÊN + PHẢI)";
        states[4].stateName = "4: Nối Xuống DƯỚI";
        states[5].stateName = "5: Đường Thẳng (TRÊN + DƯỚI)";
        states[6].stateName = "6: Mép Góc (DƯỚI + PHẢI)";
        states[7].stateName = "7: Ngã 3 (TRÊN + PHẢI + DƯỚI)";
        states[8].stateName = "8: Nối Sang TRÁI";
        states[9].stateName = "9: Mép Góc (TRÊN + TRÁI)";
        states[10].stateName = "10: Đường Thẳng (TRÁI + PHẢI)";
        states[11].stateName = "11: Ngã 3 (TRÊN + TRÁI + PHẢI)";
        states[12].stateName = "12: Mép Góc (DƯỚI + TRÁI)";
        states[13].stateName = "13: Ngã 3 (TRÊN + DƯỚI + TRÁI)";
        states[14].stateName = "14: Ngã 3 (DƯỚI + TRÁI + PHẢI)";
        states[15].stateName = "15: Nối Đủ 4 Hướng (Giữa)";
    }

    /// <summary>
    /// Kiểm tra xem mảnh ghép liền kề có phải là mục tiêu hợp lệ để nối tới không.
    /// </summary>
    public bool CanConnectTo(string neighborName)
    {
        if (string.IsNullOrEmpty(neighborName) || neighborName.ToLower() == "null" || neighborName == "[rỗng]") return false;

        if (connectToSameNameOnly)
        {
            if (neighborName == targetObjectName) return true;
            return connectableExtraNames.Contains(neighborName);
        }
        else
        {
            // Nối bừa với bất kỳ cục gạch cản đường nào tồn tại (ko phải không khí)
            return true;
        }
    }
}
