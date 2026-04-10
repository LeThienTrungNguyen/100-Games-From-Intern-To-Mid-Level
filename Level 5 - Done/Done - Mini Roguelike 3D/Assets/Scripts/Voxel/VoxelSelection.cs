using UnityEngine;

public class VoxelSelection : MonoBehaviour
{
    public LayerMask chunkLayer;

    [Header("Selection Settings")]
    public GameObject selectionCube; 
    public Material normalMaterial;   // Material dùng khi ĐÃ start day (hỗ trợ Normal Map)
    public Material warningMaterial;  // Material dùng khi CHƯA start day (cảnh báo)
    
    [Header("Break Progress Textures")]
    public Texture2D[] breakTextures; 
    
    private MeshRenderer selectionRenderer;
    private Material normalMatInstance; // Bản sao của normalMaterial để gán nứt vỡ
    private static readonly int BumpMapId = Shader.PropertyToID("_BumpMap");
    private static readonly int BumpScaleId = Shader.PropertyToID("_BumpScale");

    private Vector3Int lastBlockPos = new Vector3Int(-1, -1, -1);

    void Start()
    {
        if (selectionCube != null)
        {
            selectionRenderer = selectionCube.GetComponentInChildren<MeshRenderer>();
            
            // Tạo bản sao của normalMaterial để không làm hỏng Asset gốc
            if (normalMaterial != null)
            {
                normalMatInstance = Instantiate(normalMaterial);
                normalMatInstance.EnableKeyword("_NORMALMAP");
            }

            if (selectionCube.GetComponent<Collider>())
                selectionCube.GetComponent<Collider>().enabled = false;

            selectionCube.SetActive(false);
        }
    }

    public void SetBreakProgress(float progress)
    {
        if (normalMatInstance == null) return;

        float breakPercent = 1f - progress;
        Texture targetTex = null;

        if (breakPercent > 0.001f && breakTextures != null && breakTextures.Length > 0)
        {
            int index = Mathf.FloorToInt(breakPercent * breakTextures.Length);
            index = Mathf.Clamp(index, 0, breakTextures.Length - 1);
            targetTex = breakTextures[index];
        }

        // Luôn gán vào bản sao của Normal Material
        normalMatInstance.SetTexture(BumpMapId, targetTex);

        if (targetTex != null)
        {
            normalMatInstance.SetFloat(BumpScaleId, 1.0f);
            normalMatInstance.EnableKeyword("_NORMALMAP");
        }
        else
        {
            normalMatInstance.DisableKeyword("_NORMALMAP");
        }
    }

    void Update()
    {
        HandleSelection();
    }

    void HandleSelection()
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

                if (type != VoxelChunk.BlockType.Air && type != VoxelChunk.BlockType.Border)
                {
                    if (!selectionCube.activeSelf) selectionCube.SetActive(true);

                    // --- LOGIC CHUYỂN ĐỔI MATERIAL THEO TRẠNG THÁI NGÀY ---
                    bool isRunning = TimeManager.Instance != null && TimeManager.Instance.IsTimerRunning();
                    Material targetMat = isRunning ? normalMatInstance : warningMaterial;

                    if (selectionRenderer != null && selectionRenderer.sharedMaterial != targetMat)
                    {
                        selectionRenderer.sharedMaterial = targetMat;
                    }
                    // ------------------------------------------------------

                    if (currentPos != lastBlockPos)
                    {
                        lastBlockPos = currentPos;
                        SetBreakProgress(1f); 
                    }

                    Vector3 snappedLocalPos = new Vector3(x + 0.5f, -y + 0.5f, z + 0.5f);
                    selectionCube.transform.position = chunk.transform.TransformPoint(snappedLocalPos);
                    selectionCube.transform.localScale = Vector3.one * 1.01f;
                }
                else { HideSelection(); }
            }
            else { HideSelection(); }
        }
        else { HideSelection(); }
    }

    private void HideSelection()
    {
        if (selectionCube != null && selectionCube.activeSelf)
        {
            selectionCube.SetActive(false);
            SetBreakProgress(1f);
            lastBlockPos = new Vector3Int(-1, -1, -1);
        }
    }

    [ContextMenu("Test/Set Normal Map")]
    public void TestSetNormal() { SetBreakProgress(0.5f); }

    [ContextMenu("Test/Clear Normal Map")]
    public void TestClearNormal() { SetBreakProgress(1f); }
}