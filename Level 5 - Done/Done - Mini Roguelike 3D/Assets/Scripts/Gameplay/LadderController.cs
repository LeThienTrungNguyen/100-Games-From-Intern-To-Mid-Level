using UnityEngine;

public class LadderController : MonoBehaviour
{
    public bool isPending = true;
    public Material pendingMaterial;
    public Material activeMaterial;
    public MeshRenderer meshRenderer;
    public Collider ladderCollider;

    private void Awake()
    {
        if (meshRenderer == null) meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (ladderCollider == null) ladderCollider = GetComponent<Collider>();
    }

    public void Init(bool pending)
    {
        isPending = pending;
        RefreshState();
    }

    public void Activate()
    {
        isPending = false;
        RefreshState();
        Debug.Log("<color=green>Cầu thang đã được lắp đặt hoàn tất!</color>");
    }

    private void RefreshState()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material = isPending ? pendingMaterial : activeMaterial;
        }
        
        // Vẫn bật Collider để Player có thể Raycast trúng để phá, 
        // nhưng PlayerMovement sẽ kiểm tra biến isPending để quyết định có cho leo hay không.
    }
}
