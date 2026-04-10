using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Sensitivity")]
    public float mouseSensitivity = 100f;

    [Header("References")]
    public Transform cameraTransform; // Kéo Camera con vào đây

    private float xRotation = 0f;

    void Start()
    {
        // Khóa con trỏ chuột vào giữa màn hình và ẩn nó đi
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // KIỂM TRA BIẾN CHUNG TẠI ĐÂY
        if (UIManager.Instance != null && (UIManager.Instance.IsUIOpen || UIManager.Instance.IsPlayerLocked))
            return;
        // 1. Lấy dữ liệu đầu vào từ chuột
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 2. Xử lý xoay theo trục X (Nhìn lên/xuống)
        xRotation -= mouseY;
        // Giới hạn góc nhìn để không bị lộn ngược (Clamp)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Áp dụng xoay cho Camera (Local Rotation)
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 3. Xử lý xoay theo trục Y (Xoay thân nhân vật sang trái/phải)
        transform.Rotate(Vector3.up * mouseX);
    }
}