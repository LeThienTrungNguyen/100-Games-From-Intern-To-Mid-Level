using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class VoxelChunk : MonoBehaviour
{
    public enum BlockType { Stone = 0, Goal = 1, Iron = 2, Gold = 3, Diamond = 4, Border = 5, Air = -1 , Ladder = 6 }

    // Biến static để theo dõi tổng số khối quặng còn lại trong toàn bộ thế giới
    public static int TotalMineableBlocksCount = 0;

    public int width = 17;
    public int depth = 17;
    private int height => WorldManager.Instance != null ? WorldManager.Instance.chunkHeight : 16;

    private BlockType[,,] map;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    List<Vector3> vertices = new List<Vector3>();
    List<int>[] subMeshTriangles = new List<int>[6];

    [Header("Data References")]
    public BlockDatabaseSO blockDatabase;
    public GameObject lootPrefab; // Kéo Prefab LootItem vào đây

    [HideInInspector] public int chunkPositionY; 
    [HideInInspector] public bool isBottomChunk = false;

    private Transform playerTransform;

    public void InitChunk()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
        
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        for (int i = 0; i < subMeshTriangles.Length; i++)
            subMeshTriangles[i] = new List<int>();

        GenerateData();
        BuildMesh();
    }

    void GenerateData()
    {
        map = new BlockType[width, height, depth];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (x == 0 || x == width - 1 || z == 0 || z == depth - 1)
                        map[x, y, z] = BlockType.Border;
                    else if (isBottomChunk && y == height - 1)
                        map[x, y, z] = BlockType.Border;
                    else
                    {
                        GenerateOres(x, y, z);
                        if (map[x, y, z] != BlockType.Air && map[x, y, z] != BlockType.Border)
                            TotalMineableBlocksCount++;
                    }
                }
            }
        }
    }

    void GenerateOres(int x, int y, int z)
    {
        if (blockDatabase == null || blockDatabase.allBlocks == null || blockDatabase.allBlocks.Count == 0)
        {
            map[x, y, z] = BlockType.Stone;
            return;
        }

        float r = Random.value;
        float depthFactor = (float)y / height;
        BlockType chosenType = BlockType.Stone;

        // Duyệt qua Database để tìm quặng phù hợp
        foreach (var block in blockDatabase.allBlocks)
        {
            // Bỏ qua Đá và Border trong vòng lặp tìm quặng hiếm
            if (block.blockType == BlockType.Stone || block.blockType == BlockType.Border) continue;

            if (depthFactor >= block.minDepthPercent)
            {
                float chance = block.baseSpawnChance + (depthFactor * block.chanceMultiplierPerDepth);
                if (r < chance)
                {
                    chosenType = block.blockType;
                    break;
                }
                // Nếu không trúng, không trừ r để tránh lỗi logic, chỉ tiếp tục xét loại tiếp theo
                // r -= chance; // Bỏ dòng này để logic đơn giản và chính xác hơn dựa trên thứ tự ưu tiên
            }
        }

        map[x, y, z] = chosenType;
    }

    public void BuildMesh()
    {
        vertices.Clear();
        foreach (var list in subMeshTriangles) list.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                    AddBlockMesh(x, y, z);
            }
        }

        Mesh mesh = meshFilter.mesh;
        mesh.Clear();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.subMeshCount = subMeshTriangles.Length;

        for (int i = 0; i < subMeshTriangles.Length; i++)
            mesh.SetTriangles(subMeshTriangles[i].ToArray(), i);

        mesh.RecalculateNormals();
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }

    BlockType GetBlock(int x, int y, int z)
    {
        if (x < 0 || x >= width || y < 0 || y >= height || z < 0 || z >= depth)
            return BlockType.Air;
        return map[x, y, z];
    }

    void AddBlockMesh(int x, int y, int z)
    {
        BlockType blockType = map[x, y, z];
        if (blockType == BlockType.Air) return;

        int subMeshIndex = (int)blockType;
        Vector3 pos = new Vector3(x, -y, z);

        if (y == 0) AddFace(pos, Vector3.up, subMeshIndex);
        else if (GetBlock(x, y - 1, z) == BlockType.Air) AddFace(pos, Vector3.up, subMeshIndex);

        if(y == height - 1 ) AddFace(pos, Vector3.down, subMeshIndex);
        else if (GetBlock(x, y + 1, z) == BlockType.Air) AddFace(pos, Vector3.down, subMeshIndex);
        
        if (GetBlock(x - 1, y, z) == BlockType.Air) AddFace(pos, Vector3.left, subMeshIndex);
        if (GetBlock(x + 1, y, z) == BlockType.Air) AddFace(pos, Vector3.right, subMeshIndex);
        if (GetBlock(x, y, z + 1) == BlockType.Air) AddFace(pos, Vector3.forward, subMeshIndex);
        if (GetBlock(x, y, z - 1) == BlockType.Air) AddFace(pos, Vector3.back, subMeshIndex);
    }

    void AddFace(Vector3 pos, Vector3 direction, int subMeshIndex)
    {
        int v = vertices.Count;

        if (direction == Vector3.up)
        {
            vertices.Add(pos + new Vector3(0, 1, 0)); vertices.Add(pos + new Vector3(0, 1, 1));
            vertices.Add(pos + new Vector3(1, 1, 1)); vertices.Add(pos + new Vector3(1, 1, 0));
        }
        else if (direction == Vector3.down)
        {
            vertices.Add(pos + new Vector3(0, 0, 1)); vertices.Add(pos + new Vector3(0, 0, 0));
            vertices.Add(pos + new Vector3(1, 0, 0)); vertices.Add(pos + new Vector3(1, 0, 1));
        }
        else if (direction == Vector3.left)
        {
            vertices.Add(pos + new Vector3(0, 0, 1)); vertices.Add(pos + new Vector3(0, 1, 1));
            vertices.Add(pos + new Vector3(0, 1, 0)); vertices.Add(pos + new Vector3(0, 0, 0));
        }
        else if (direction == Vector3.right)
        {
            vertices.Add(pos + new Vector3(1, 0, 0)); vertices.Add(pos + new Vector3(1, 1, 0));
            vertices.Add(pos + new Vector3(1, 1, 1)); vertices.Add(pos + new Vector3(1, 0, 1));
        }
        else if (direction == Vector3.forward)
        {
            vertices.Add(pos + new Vector3(1, 0, 1)); vertices.Add(pos + new Vector3(1, 1, 1));
            vertices.Add(pos + new Vector3(0, 1, 1)); vertices.Add(pos + new Vector3(0, 0, 1));
        }
        else if (direction == Vector3.back)
        {
            vertices.Add(pos + new Vector3(0, 0, 0)); vertices.Add(pos + new Vector3(0, 1, 0));
            vertices.Add(pos + new Vector3(1, 1, 0)); vertices.Add(pos + new Vector3(1, 0, 0));
        }

        subMeshTriangles[subMeshIndex].Add(v); subMeshTriangles[subMeshIndex].Add(v + 1); subMeshTriangles[subMeshIndex].Add(v + 2);
        subMeshTriangles[subMeshIndex].Add(v); subMeshTriangles[subMeshIndex].Add(v + 2); subMeshTriangles[subMeshIndex].Add(v + 3);
    }

    public BlockType GetBlockTypeAt(int x, int y, int z)
    {
        if (x < 0 || x >= width || y < 0 || y >= height || z < 0 || z >= depth)
            return BlockType.Air;
        return map[x, y, z];
    }

    private bool isRebuildPending = false;

    public void DestroyBlock(int x, int y, int z)
    {
        if (x < 0 || x >= width || y < 0 || y >= height || z < 0 || z >= depth) return;

        BlockType type = map[x, y, z];
        if (type == BlockType.Border || type == BlockType.Air) return;

        map[x, y, z] = BlockType.Air;
        TotalMineableBlocksCount--;

        // Lấy dữ liệu khối từ Database để tìm Prefab rơi ra
        if (blockDatabase != null)
        {
            BlockDataSO data = blockDatabase.GetData(type);
            if (data != null)
            {
                Vector3 spawnPos = transform.position + new Vector3(x + 0.5f, -y + 0.5f, z + 0.5f);

                // GỌI AUDIOMANAGER KHI VỠ (BREAK)
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayBreakSound(spawnPos);
                }

                if (data.lootItemPrefab != null)
                {
                    string prefabName = data.lootItemPrefab.name;

                    GameObject loot;
                    if (LootPoolManager.Instance != null)
                    {
                        loot = LootPoolManager.Instance.SpawnLoot(prefabName, spawnPos + Vector3.up * 0.3f, Quaternion.identity);
                    }
                    else
                    {
                        loot = Instantiate(data.lootItemPrefab, spawnPos + Vector3.up * 0.3f, Quaternion.identity);
                    }

                    LootItem lootScript = loot.GetComponent<LootItem>();
                    if (lootScript != null)
                    {
                        lootScript.Init(type, playerTransform);
                    }
                }
            }
        }

        // Thay vì BuildMesh ngay, ta đợi đến cuối frame
        RequestRebuild();
    }

    private void RequestRebuild()
    {
        if (!isRebuildPending)
        {
            isRebuildPending = true;
            StartCoroutine(Co_DelayedRebuild());
        }
    }

    private IEnumerator Co_DelayedRebuild()
    {
        yield return new WaitForEndOfFrame();
        BuildMesh();
        isRebuildPending = false;
    }

    public float GetBlockMaxHP(BlockType type)
    {
        if (blockDatabase != null)
        {
            BlockDataSO data = blockDatabase.GetData(type);
            if (data != null) return data.maxHP;
        }
        return 50f;
    }
}