using UnityEngine;

public class CameraController : MonoBehaviour
{
    void Awake()
    {
        // Đặt mouse ở giữa màn hình
        Vector3 centerScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        // Di chuyển mouse về giữa màn hình (chỉ hoạt động trên một số nền tảng)
        Cursor.SetCursor(null, centerScreen, CursorMode.Auto);

        // Chỉnh tỉ lệ màn hình về 16:10
        Camera cam = Camera.main;
        if (cam != null)
        {
            float targetAspect = 16f / 10f;
            float windowAspect = (float)Screen.width / (float)Screen.height;
            float scaleHeight = windowAspect / targetAspect;

            if (scaleHeight < 1.0f)
            {
                Rect rect = cam.rect;
                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;
                cam.rect = rect;
            }
            else
            {
                float scaleWidth = 1.0f / scaleHeight;
                Rect rect = cam.rect;
                rect.width = scaleWidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scaleWidth) / 2.0f;
                rect.y = 0;
                cam.rect = rect;
            }
        }
    }
}
