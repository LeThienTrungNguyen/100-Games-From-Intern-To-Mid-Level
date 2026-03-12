using UnityEngine;

public class LadderPlace : MonoBehaviour
{
    [Header("Settings")]
    public GameObject ladderPrefab; 
    public LayerMask chunkLayer;
    public float placeRange = 5f;

    void Update()
    {
        if (UIManager.Instance != null && (UIManager.Instance.IsUIOpen || UIManager.Instance.IsPlayerLocked))
            return;

        if (Input.GetMouseButtonDown(1))
        {
            TryPlaceLadder();
        }
    }

    void TryPlaceLadder()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, placeRange, chunkLayer))
        {
            VoxelChunk chunk = hit.collider.GetComponent<VoxelChunk>();
            if (chunk != null)
            {
                // 1. Xác định khối bị bắn trúng (Target Block)
                Vector3 internalPoint = hit.point - hit.normal * 0.5f;
                Vector3 localPoint = chunk.transform.InverseTransformPoint(internalPoint);

                int x = Mathf.FloorToInt(localPoint.x);
                int y = Mathf.FloorToInt(-localPoint.y + 1f);
                int z = Mathf.FloorToInt(localPoint.z);

                VoxelChunk.BlockType hitType = chunk.GetBlockTypeAt(x, y, z);

                // --- LOGIC MỚI: Thắt chặt vị trí đặt thang ---

                // KIỂM TRA 1: CHỈ ĐƯỢC PHÉP ĐẶT VÀO BORDER
                if (hitType != VoxelChunk.BlockType.Border)
                {
                    Debug.Log("<color=orange>[Ladder] Thang chỉ có thể được lắp đặt dựa vào khối chặn (Border)!</color>");
                    return;
                }

                // KIỂM TRA 2: KHÔNG ĐƯỢC ĐẶT LÊN ĐỈNH CỦA BORDER
                // hit.normal.y > 0.9f nghĩa là đang nhìn vào mặt trên của khối
                if (hit.normal.y > 0.9f)
                {
                    Debug.Log("<color=red>[Ladder] Không thể đặt thang đứng trên đỉnh khối chặn (Border)!</color>");
                    return;
                }

                // Xác định vị trí thế giới của ô trống (Air) cạnh mặt bên của Border
                Vector3 spawnPos = chunk.transform.TransformPoint(new Vector3(x + 0.5f, -y + 0.5f, z + 0.5f) + hit.normal);
                spawnPos = new Vector3(Mathf.Floor(spawnPos.x) + 0.5f, Mathf.Floor(spawnPos.y) + 0.5f, Mathf.Floor(spawnPos.z) + 0.5f);

                // KIỂM TRA 3: Ô đích phải là ô trống (Air)
                Vector3 localSpawn = chunk.transform.InverseTransformPoint(spawnPos);
                int sx = Mathf.FloorToInt(localSpawn.x);
                int sy = Mathf.FloorToInt(-localSpawn.y + 1f);
                int sz = Mathf.FloorToInt(localSpawn.z);
                
                VoxelChunk.BlockType targetSpaceType = chunk.GetBlockTypeAt(sx, sy, sz);
                if (targetSpaceType != VoxelChunk.BlockType.Air)
                {
                    // Nếu trúng thang hiện có thì cho phép gỡ bỏ
                    Collider[] colliders = Physics.OverlapSphere(spawnPos, 0.3f);
                    foreach (var col in colliders)
                    {
                        if (col.CompareTag("Ladder"))
                        {
                            Destroy(col.gameObject);
                            Debug.Log("<color=red>Đã gỡ bỏ cầu thang!</color>");
                            return;
                        }
                    }

                    Debug.Log("<color=orange>[Ladder] Vị trí này đã có khối đặc, không thể đặt thang!</color>");
                    return;
                }

                // KIỂM TRA 3: Có vật cản vật lý tại spawnPos không? (Player, hoặc vật thể khác)
                if (Physics.CheckBox(spawnPos, new Vector3(0.4f, 0.4f, 0.4f)))
                {
                    Debug.Log("<color=orange>[Ladder] Có vật cản, không thể đặt thang!</color>");
                    return;
                }

                // KIỂM TRA 3: Số lượng thang còn lại
                if (PlayerStats.Instance == null || PlayerStats.Instance.stairsCount <= 0)
                {
                    Debug.Log("<color=orange>Không còn cầu thang để đặt!</color>");
                    return;
                }

                // ĐẶT CẦU THANG
                if (ladderPrefab != null)
                {
                    // Quay mặt thang về phía khối bị hit (ngược với normal của mặt hit)
                    GameObject newLadder = Instantiate(ladderPrefab, spawnPos, Quaternion.LookRotation(-hit.normal));
                    LadderController lc = newLadder.GetComponent<LadderController>();
                    if (lc != null) lc.Init(true); // Đặt ở trạng thái Pending
                    
                    PlayerStats.Instance.stairsCount--;
                    Debug.Log("<color=green>Đã đăng ký lắp đặt thang tại ô trống. Vui lòng đợi đến ngày mai!</color>");
                }
            }
        }
    }
}
