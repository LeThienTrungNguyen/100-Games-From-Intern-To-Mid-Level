using UnityEngine;

public class VoxelPicker : MonoBehaviour
{
    public LayerMask chunkLayer; // Gán Layer của Chunk vào đây để tránh Raycast trúng thứ khác

    void Update()
    {
        IdentifyBlock();
    }

    void IdentifyBlock()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, PlayerStats.Instance.interactableRange, chunkLayer))
        {
            VoxelChunk chunk = hit.collider.GetComponent<VoxelChunk>();
            if (chunk != null)
            {
                // Bước quan trọng: Lùi điểm va chạm vào trong lòng khối 0.5 đơn vị
                Vector3 internalPoint = hit.point - hit.normal * 0.5f;

                // Chuyển đổi tọa độ World về tọa độ Local của Chunk (nếu Chunk bị di chuyển)
                Vector3 localPoint = chunk.transform.InverseTransformPoint(internalPoint);

                // Ép kiểu về số nguyên để tìm Index trong mảng map[x, y, z]
                int x = Mathf.FloorToInt(localPoint.x);
                int y = Mathf.FloorToInt(-localPoint.y + 1f); // Đảo ngược y vì map dùng chỉ số dương 0-199
                int z = Mathf.FloorToInt(localPoint.z);

                // Lấy loại khối từ hàm GetBlock đã viết trong VoxelChunk
                VoxelChunk.BlockType type = chunk.GetBlockTypeAt(x, y, z);

                //Debug.Log($"Đang nhìn vào: {type} tại tọa độ mảng: [{x}, {y}, {z}]");

                if (type == VoxelChunk.BlockType.Air)
                {
                    // Vẽ một tia đỏ từ vị trí va chạm theo hướng pháp tuyến
                    Debug.DrawRay(hit.point, hit.normal * 2f, Color.red, 0.5f);
                    Debug.Log($"<color=red>LỖI:</color> Chạm vào Collider tại {hit.point} nhưng mảng là Air. Normal: {hit.normal}");
                }
            }
        }
    }
}