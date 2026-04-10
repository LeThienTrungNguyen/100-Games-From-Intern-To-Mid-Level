using UnityEngine;

public class LootItem : MonoBehaviour
{
    [System.Serializable]
    public struct ItemVisual
    {
        public VoxelChunk.BlockType type;
        public Material material;
        public Mesh mesh; // Nếu bạn muốn hình dáng quặng khác nhau (tùy chọn)
    }

    public ItemVisual[] visuals;
    public VoxelChunk.BlockType itemType;

    private Transform player;
    private bool isFollowing = false;
    public float moveSpeed = 8f;

    public void Init(VoxelChunk.BlockType type, Transform playerTransform)
    {
        itemType = type;
        player = playerTransform;
        Physics.IgnoreLayerCollision(7, 8);
        
        MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
        if (renderer == null)
        {
            Debug.LogError($"LootItem {gameObject.name} không có MeshRenderer!");
            return;
        }

        Material targetMaterial = null;
        foreach (var v in visuals) if (v.type == type) { targetMaterial = v.material; break; }
        if (targetMaterial != null) renderer.sharedMaterial = targetMaterial;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(new Vector3(Random.Range(-1f, 1f), 2f, Random.Range(-1f, 1f)), ForceMode.Impulse);
        }

        Invoke("StartFollowing", 0.6f);
    }

    void StartFollowing()
    {
        isFollowing = true;
        if (GetComponent<Rigidbody>() != null) GetComponent<Rigidbody>().useGravity = false;
    }

    void Update()
    {
        if (isFollowing && player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, player.position) < 0.7f)
            {
                Collect();
            }
        }
    }

    void Collect()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(itemType);
            
            // GỌI AUDIOMANAGER KHI NHẶT (PICKUP)
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPickupSound(transform.position);
            }

            if (LootPoolManager.Instance != null)
                LootPoolManager.Instance.ReturnToPool(gameObject);
            else
                Destroy(gameObject);
        }
    }
}