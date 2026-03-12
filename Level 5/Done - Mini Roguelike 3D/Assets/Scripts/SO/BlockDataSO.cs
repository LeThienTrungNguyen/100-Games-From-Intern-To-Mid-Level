using UnityEngine;

[CreateAssetMenu(fileName = "NewBlockData", menuName = "Mining/BlockData")]
public class BlockDataSO : ScriptableObject
{
    public VoxelChunk.BlockType blockType;
    public string blockName;
    public float maxHP = 50f;
    public GameObject lootItemPrefab;

    [Header("Spawn Settings")]
    [Range(0, 1)] public float minDepthPercent = 0f; // Tầng bắt đầu xuất hiện (0-1)
    [Range(0, 1)] public float baseSpawnChance = 0.1f; // Tỉ lệ xuất hiện cơ bản
    public float chanceMultiplierPerDepth = 0.05f; // Tỉ lệ tăng thêm khi xuống sâu
}