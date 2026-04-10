using UnityEngine;
using TMPro;

public class PlayerMine : MonoBehaviour
{
    public LayerMask chunkLayer;

    private Vector3Int lastTarget = new Vector3Int(-1, -1, -1);
    private float currentHP;
    private float maxHP;

    [Header("Mining Feedback")]
    public float hitInterval = 0.2f; 
    private float hitTimer = 0f;
    public VoxelSelection voxelSelection; 

    private void Start()
    {
        // Nếu chưa kéo vào Inspector, tự tìm trong Scene
        if (voxelSelection == null) 
        {
            voxelSelection = FindObjectOfType<VoxelSelection>();
            if (voxelSelection == null) Debug.LogError("[PlayerMine] KHÔNG TÌM THẤY VoxelSelection trong Scene!");
            else Debug.Log("[PlayerMine] Đã tự động tìm thấy VoxelSelection.");
        }
    }

    void Update()
    {
        if (UIManager.Instance != null && (UIManager.Instance.IsUIOpen || UIManager.Instance.IsPlayerLocked))
            return;

        bool canMine = TimeManager.Instance != null && TimeManager.Instance.IsTimerRunning();
        
        if (PlayerStats.Instance != null && !PlayerStats.Instance.canMine) canMine = false;

        if (Input.GetMouseButton(0))
        {
            if (canMine)
            {
                PerformMining();
            }
        }
        else
        {
            ResetMining();
        }
    }

    void PerformMining()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); 
        RaycastHit hit;
        float currentRange = PlayerStats.Instance != null ? PlayerStats.Instance.interactableRange : 5f;

        if (Physics.Raycast(ray, out hit, currentRange, chunkLayer))
        {
            VoxelChunk chunk = hit.collider.GetComponent<VoxelChunk>();
            if (chunk != null)
            {
                Vector3 internalPoint = hit.point - hit.normal * 0.5f;
                Vector3 localPoint = chunk.transform.InverseTransformPoint(internalPoint);

                int x = Mathf.FloorToInt(localPoint.x);
                int y = Mathf.FloorToInt(-localPoint.y + 1f);
                int z = Mathf.FloorToInt(localPoint.z);

                Vector3Int currentPos = new Vector3Int(x, y, z);
                VoxelChunk.BlockType type = chunk.GetBlockTypeAt(x, y, z);

                if (type == VoxelChunk.BlockType.Air || type == VoxelChunk.BlockType.Border)
                {
                    ResetMining();
                    return;
                }

                if (currentPos != lastTarget)
                {
                    lastTarget = currentPos;
                    maxHP = chunk.GetBlockMaxHP(type);
                    currentHP = maxHP;
                }

                float damage = PlayerStats.Instance != null ? PlayerStats.Instance.currentMiningDamage : 50f;
                if (PlayerStats.Instance != null) damage *= PlayerStats.Instance.miningSpeedMultiplier;
                
                currentHP -= damage * Time.deltaTime;

                float displayValue = Mathf.Max(0, currentHP / maxHP);

                // --- HIỆU ỨNG ÂM THANH ---
                hitTimer += Time.deltaTime;
                if (hitTimer >= hitInterval)
                {
                    hitTimer = 0f;

                    // GỌI AUDIOMANAGER ĐỂ PHÁT ÂM THANH ĐÀO & ĐÁ RƠI
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayMiningSound(hit.point, type);
                        
                        if (Random.value > 0.5f) 
                            AudioManager.Instance.PlayDebrisSound(hit.point);
                    }
                }

                // --- GỌI SANG VOXEL SELECTION ĐỂ HIỆN NỨT ---
                if (voxelSelection != null)
                {
                    voxelSelection.SetBreakProgress(displayValue);
                }

                if (currentHP <= 0)
                {
                    chunk.DestroyBlock(x, y, z);
                    if (type != VoxelChunk.BlockType.Stone)
                        DotweenAnimationName.Instance.DoShakeCamera(0.2f, 0.1f);

                    ResetMining();
                }
            }
        }
        else
        {
            ResetMining();
        }
    }

    void ResetMining()
    {
        if (lastTarget == new Vector3Int(-1, -1, -1)) return; 

        lastTarget = new Vector3Int(-1, -1, -1);
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMiningSound();
        }

        if (voxelSelection != null) voxelSelection.SetBreakProgress(1f);
    }
}