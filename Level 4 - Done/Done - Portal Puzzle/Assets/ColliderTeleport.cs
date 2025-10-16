using UnityEngine;

public class ColliderTeleport : MonoBehaviour
{
    public Transform anotherPortal;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.position = anotherPortal.position + transform.right * 0.3f;
            //
            collision.GetComponent<Rigidbody2D>().velocity = new Vector3(collision.GetComponent<Rigidbody2D>().velocity.x, -collision.GetComponent<Rigidbody2D>().velocity.y);
        }
    }

    void TeleportPlayer(Transform portalIn, Transform portalOut, Rigidbody2D rb)
    {
        // Lưu velocity hiện tại
        Vector2 velocity = rb.velocity;

        // Góc quay chênh lệch giữa 2 portal
        float angleDelta = portalOut.eulerAngles.z - portalIn.eulerAngles.z;

        // Xoay vector velocity theo góc chênh lệch
        Vector2 newVelocity = RotateVector(velocity, angleDelta);

        // Đặt player tại vị trí portal ra (dịch một chút để tránh kẹt collider)
        rb.position = portalOut.position + portalOut.up * 0.5f;

        // Cập nhật vận tốc mới
        rb.velocity = newVelocity;
    }

    Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }

}
