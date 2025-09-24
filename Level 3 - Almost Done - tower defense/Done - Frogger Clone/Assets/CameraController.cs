using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;        // Player để follow
    public float smoothSpeed = 5f;  // Độ mượt khi camera theo dõi
    [SerializeField]private Vector3 offset;         // Khoảng cách ban đầu giữa camera và player

    void Start()
    {
        if (player != null)
        {
            offset = transform.position - player.position;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Lấy vị trí camera mong muốn
        Vector3 targetPosition = transform.position;

        // Chỉ update trục Y, giữ nguyên X và Z
        targetPosition.y = player.position.y + offset.y;

        // Di chuyển mượt
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
