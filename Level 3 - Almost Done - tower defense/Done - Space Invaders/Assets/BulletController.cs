using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 5f;

    void Start()
    {
        Destroy(gameObject, 10f); // tự hủy sau 10 giây
    }

    void Update()
    {
        // 🔴 Nếu là đạn Enemy thì đi xuống, còn lại đi lên
        Vector3 dir = (CompareTag("Enemy")) ? Vector3.down : Vector3.up;
        transform.Translate(dir * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other);

        // Nếu đạn của player chạm Enemy
        if (other.CompareTag("Enemy") && !CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }

        // Nếu đạn Enemy chạm Player
        if (other.CompareTag("Player") && CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
