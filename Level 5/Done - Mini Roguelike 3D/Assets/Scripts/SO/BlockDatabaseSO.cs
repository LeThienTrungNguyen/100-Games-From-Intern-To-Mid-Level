using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockDatabase", menuName = "Mining/BlockDatabase")]
public class BlockDatabaseSO : ScriptableObject
{
    public List<BlockDataSO> allBlocks;

    public BlockDataSO GetData(VoxelChunk.BlockType type)
    {
        return allBlocks.Find(b => b.blockType == type);
    }
}