using UnityEngine;
using System.Collections.Generic;

public class LootPoolManager : MonoBehaviour
{
    public static LootPoolManager Instance;

    [Header("Settings")]
    public List<GameObject> lootPrefabs; // Danh sách các loại Prefab Loot (Stone, Iron, Gold...)
    public int initialPoolSizePerPrefab = 50;

    // Dictionary để chứa pool cho từng loại tên Prefab
    private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Khởi tạo pool cho từng Prefab trong danh sách
        foreach (GameObject prefab in lootPrefabs)
        {
            if (prefab == null) continue;
            
            string prefabName = prefab.name;
            if (!pools.ContainsKey(prefabName))
            {
                pools[prefabName] = new Queue<GameObject>();
                
                for (int i = 0; i < initialPoolSizePerPrefab; i++)
                {
                    GameObject obj = CreateNewLootObject(prefab);
                    obj.SetActive(false);
                    pools[prefabName].Enqueue(obj);
                }
            }
        }
    }

    private GameObject CreateNewLootObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, transform);
        // Lưu tên gốc để khi ReturnToPool biết trả về queue nào
        obj.name = prefab.name; 
        return obj;
    }

    public GameObject SpawnLoot(string prefabName, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(prefabName))
        {
            Debug.LogWarning($"Pool cho '{prefabName}' không tồn tại!");
            return null;
        }

        GameObject obj;
        if (pools[prefabName].Count > 0)
        {
            obj = pools[prefabName].Dequeue();
        }
        else
        {
            // Tìm prefab gốc để tạo thêm
            GameObject originalPrefab = lootPrefabs.Find(p => p.name == prefabName);
            obj = CreateNewLootObject(originalPrefab);
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        string prefabName = obj.name; // Tên đã được gán lúc Instantiate
        
        if (pools.ContainsKey(prefabName))
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pools[prefabName].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}