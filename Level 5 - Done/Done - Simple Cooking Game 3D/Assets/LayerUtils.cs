using UnityEngine;

public static class LayerUtils
{
    /// <summary>
    /// Kiểm tra xem layer đơn (layer1) có nằm trong LayerMask (layer2) hay không.
    /// </summary>
    /// <param name="layer1">Giá trị layer của GameObject (int).</param>
    /// <param name="layer2">LayerMask chứa nhiều layer.</param>
    /// <returns>True nếu layer1 nằm trong layer2.</returns>
    public static bool IsContainLayer(int layer1, LayerMask layer2)
    {
        return ((1 << layer1) & layer2.value) != 0;
    }
}
