using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupsController : MonoBehaviour
{
    [Header("List of Item Prefabs")]
    public List<Transform> itemList;

    private List<Transform> spawnedItems = new();
    private const int maxRetries = 50;

    void Start()
    {
        Reset();
    }
    [ContextMenu("Reset")]
    public void Reset()
    {
        if (itemList.Count < transform.childCount/2)
        {
            Debug.LogError("❌ itemList must have at least as many elements as number of groups!");
            return;
        }
        CreateShuffledGroups();
    }
    
    void CreateShuffledGroups()
    {
        Debug.Log("🧱 Creating shuffled groups...");
        ClearGroups();
        spawnedItems.Clear();

        int groupCount = transform.childCount;

        // Step 1: Tạo mỗi group có 3 item giống nhau
        for (int i = 0; i < groupCount; i++)
        {
            var prefab = itemList[i % itemList.Count];

            for (int j = 0; j < 3; j++)
            {
                var item = Instantiate(prefab);
                item.gameObject.SetActive(true);
                spawnedItems.Add(item);
            }
        }

        // Step 2: Shuffle tất cả item
        spawnedItems = spawnedItems.OrderBy(x => Random.value).ToList();

        // Step 3: Phân phối trở lại vào các group
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            int groupIndex = i / 3;
            Transform group = transform.GetChild(groupIndex);
            var item = spawnedItems[i];

            item.SetParent(group);
            float offset = (i % 3) * 1.3f;
            item.localPosition = new Vector2(offset, 0);
        }

        Debug.Log("✅ Shuffled groups created successfully.");
    }

    void ClearGroups()
    {
        Debug.Log("🧹 Clearing old groups...");
        for (int i = 0; i < transform.childCount; i++)
        {
            var group = transform.GetChild(i);
            for (int j = group.childCount - 1; j >= 0; j--)
            {
                Destroy(group.GetChild(j).gameObject);
            }
        }
    }
}
