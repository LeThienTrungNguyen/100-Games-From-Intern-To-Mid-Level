using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bomb : MonoBehaviour
{
    [Header("Settings")]
    public float timeToExplode = 2.5f;
    public LayerMask chunkLayer;
    public GameObject explosionEffectPrefab;

    private int explosionRadius = 1;

    public void Activate(int radius)
    {
        explosionRadius = radius;
        StartCoroutine(Co_ExplodeSequence());
    }

    private IEnumerator Co_ExplodeSequence()
    {
        yield return new WaitForSeconds(timeToExplode);

        // 1. Hiệu ứng hình ảnh (Particle)
        if (explosionEffectPrefab != null)
        {
            GameObject fx = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            // Tự động xóa hiệu ứng sau 10 giây
            Destroy(fx, 10f);
        }

        if (AudioManager.Instance != null) AudioManager.Instance.PlayBombExplosionSound(transform.position);

        // 2. Phá hủy các khối trong vùng nổ
        Explode(explosionRadius);

        // 3. Gây sát thương cho Player nếu ở gần
        DamagePlayer(snappedCenter, explosionRadius);

        // 4. Xóa quả bom ngay lập tức
        Destroy(gameObject);
    }

    private Vector3 snappedCenter;

    private void Explode(int radius)
    {
        // 1. SNAP TÂM NỔ: Làm tròn vị trí bom về đúng tâm khối (x.5, y.5, z.5)
        Vector3 rawPos = transform.position;
        snappedCenter = new Vector3(
            Mathf.Floor(rawPos.x) + 0.5f,
            Mathf.Floor(rawPos.y) + 0.5f,
            Mathf.Floor(rawPos.z) + 0.5f
        );

        // 2. TÌM CHUNKS TRONG PHẠM VI (Mở rộng phạm vi quét Collider một chút cho chắc chắn)
        Collider[] chunksInRange = Physics.OverlapSphere(snappedCenter, radius + 1.5f, chunkLayer);
        HashSet<VoxelChunk> uniqueChunks = new HashSet<VoxelChunk>();
        
        foreach (var col in chunksInRange)
        {
            VoxelChunk c = col.GetComponent<VoxelChunk>();
            if (c != null) uniqueChunks.Add(c);
        }

        // 3. DUYỆT VÙNG NỔ DỰA TRÊN TÂM ĐÃ SNAP
        foreach (VoxelChunk chunk in uniqueChunks)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    for (int z = -radius; z <= radius; z++)
                    {
                        // Điểm đích luôn là tâm của các khối hàng xóm
                        Vector3 targetWorldPos = snappedCenter + new Vector3(x, y, z);
                        
                        Vector3 localPoint = chunk.transform.InverseTransformPoint(targetWorldPos);
                        
                        // Sử dụng FloorToInt trên tâm khối (x.5) luôn đảm bảo trúng chính xác index nguyên
                        int lx = Mathf.FloorToInt(localPoint.x);
                        int ly = Mathf.FloorToInt(-localPoint.y + 1f); // Logic Y âm của VoxelChunk
                        int lz = Mathf.FloorToInt(localPoint.z);

                        VoxelChunk.BlockType type = chunk.GetBlockTypeAt(lx, ly, lz);
                        if (type != VoxelChunk.BlockType.Air && type != VoxelChunk.BlockType.Border)
                        {
                            chunk.DestroyBlock(lx, ly, lz);
                        }
                    }
                }
            }
        }
        
        Debug.Log($"<color=red>BOM NỔ! Đã snap tâm tại {snappedCenter} và quét {uniqueChunks.Count} chunks.</color>");
    }

    private void DamagePlayer(Vector3 center, float radius)
    {
        if (PlayerStats.Instance == null) return;

        // Vị trí kiểm tra là bụng/ngực người chơi
        Vector3 playerCheckPos = PlayerStats.Instance.transform.position + Vector3.up;
        float dist = Vector3.Distance(center, playerCheckPos);

        // Phạm vi gây sát thương rộng hơn phạm vi phá khối một chút (khoảng 2m thêm)
        if (dist <= radius + 2f)
        {
            // Kiểm tra vật cản giữa tâm bom và người chơi
            Vector3 dir = (playerCheckPos - center).normalized;
            if (!Physics.Raycast(center, dir, dist, chunkLayer))
            {
                // Tính toán sát thương: Càng xa càng giảm, nhưng tối thiểu 30hp, tối đa 90hp
                float damage = Mathf.Lerp(90f, 30f, dist / (radius + 2f));
                PlayerStats.Instance.ChangeHealth(-damage);
                Debug.Log($"<color=orange>[Bomb] Player took {damage:F1} damage from explosion!</color>");
            }
            else
            {
                Debug.Log("<color=green>[Bomb] Player was shielded from explosion by a block.</color>");
            }
        }
    }

    private void TryDestroyBlockAt(Vector3 worldPos)
    {
        // Dùng OverlapSphere siêu nhỏ để tìm xem vị trí này thuộc Chunk nào
        Collider[] hitChunks = Physics.OverlapSphere(worldPos, 0.1f, chunkLayer);
        
        foreach (var col in hitChunks)
        {
            VoxelChunk chunk = col.GetComponent<VoxelChunk>();
            if (chunk != null)
            {
                // Chuyển tọa độ thế giới về tọa độ khối trong Chunk
                Vector3 localPoint = chunk.transform.InverseTransformPoint(worldPos);
                
                int lx = Mathf.FloorToInt(localPoint.x);
                int ly = Mathf.FloorToInt(-localPoint.y + 1f);
                int lz = Mathf.FloorToInt(localPoint.z);

                // Lấy loại khối để kiểm tra xem có phải Border không
                VoxelChunk.BlockType type = chunk.GetBlockTypeAt(lx, ly, lz);
                
                if (type != VoxelChunk.BlockType.Air && type != VoxelChunk.BlockType.Border)
                {
                    chunk.DestroyBlock(lx, ly, lz);
                }
            }
        }
    }
}