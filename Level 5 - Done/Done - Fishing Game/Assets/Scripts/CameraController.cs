using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerFish player;
    
    [Header("Infinite Scale Settings")]
    public float baseCameraSize = 5f;
    public float basePlayerSize = 1f;
    public float zoomThresholdMultiplier = 1.5f; 
    public float smoothSpeed = 2f;

    private Camera cam;
    private float targetSize;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (baseCameraSize <= 0) baseCameraSize = cam.orthographicSize;
        targetSize = baseCameraSize;
    }

    void Update()
    {
        if (player == null) 
        {
            player = FindObjectOfType<PlayerFish>();
            if (player == null) return;
        }

        // Logic Zoom vô hạn theo cấp số nhân
        float sizeRatio = player.fishSize / basePlayerSize;
        if (sizeRatio > 1f)
        {
            int level = Mathf.FloorToInt(Mathf.Log(sizeRatio) / Mathf.Log(zoomThresholdMultiplier));
            targetSize = baseCameraSize * Mathf.Pow(zoomThresholdMultiplier, level);
        }
        else
        {
            targetSize = baseCameraSize;
        }

        // Zoom mượt mà
        if (Mathf.Abs(cam.orthographicSize - targetSize) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * smoothSpeed);
        }
    }
}
