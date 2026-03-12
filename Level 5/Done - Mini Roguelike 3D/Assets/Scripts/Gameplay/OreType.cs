using UnityEngine;

[System.Serializable]
public struct OreType
{
    public string name;
    public Transform prefab;
    public float rarity;     // Ngưỡng Perlin (0.0 -> 1.0), càng cao càng hiếm
    public int maxHeight;    // Độ cao tối đa có thể xuất hiện
    public int minHeight;    // Độ cao tối thiểu
    public float scale;     // Độ lớn của vỉa quặng
}