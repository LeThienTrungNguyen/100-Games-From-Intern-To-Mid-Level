using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;

    public GameObject chunkPrefab;
    public Transform player; // Kéo Player vào đây
    public int totalHeight = 10000;
    public int chunkHeight = 16;
    public int viewDistance = 2; // Số lượng chunk hiển thị phía trên và dưới player

    private List<VoxelChunk> allChunks = new List<VoxelChunk>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        int numChunks = totalHeight / chunkHeight;
        for (int i = 0; i < numChunks; i++)
        {
            Vector3 spawnPos = new Vector3(0, -i * chunkHeight, 0);
            GameObject newChunk = Instantiate(chunkPrefab, spawnPos, Quaternion.identity, transform);
            VoxelChunk chunkScript = newChunk.GetComponent<VoxelChunk>();

            chunkScript.isBottomChunk = (i == numChunks - 1);
            // chunkScript.height = chunkHeight; // Dòng này sẽ bị xóa sau khi sửa VoxelChunk
            chunkScript.InitChunk();

            // Mặc định tắt hết để tránh lag lúc đầu
            newChunk.SetActive(false);
            allChunks.Add(chunkScript);
        }
    }

    void Update()
    {
        if (player == null) return;

        // Xác định Player đang ở Chunk nào dựa trên tọa độ Y
        // Player.y = 0 -> index 0 | Player.y = -16 -> index 1
        int currentPlayerChunkIndex = Mathf.FloorToInt(Mathf.Abs(player.position.y) / chunkHeight);

        for (int i = 0; i < allChunks.Count; i++)
        {
            // Chỉ bật Chunk nếu nó nằm trong phạm vi viewDistance quanh Player
            if (i >= currentPlayerChunkIndex - viewDistance && i <= currentPlayerChunkIndex + viewDistance)
            {
                if (!allChunks[i].gameObject.activeSelf)
                    allChunks[i].gameObject.SetActive(true);
            }
            else
            {
                if (allChunks[i].gameObject.activeSelf)
                    allChunks[i].gameObject.SetActive(false);
            }
        }
    }
}