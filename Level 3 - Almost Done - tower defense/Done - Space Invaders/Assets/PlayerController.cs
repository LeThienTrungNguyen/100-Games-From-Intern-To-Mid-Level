using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector2 mousePos2D;
    public Transform bulletPrefab;
    public ShipsManager shipsManager;

    void Update()
    {
        Movement();
        Shoot();
    }

    void Movement()
    {
        mousePos2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos2D.x = Mathf.Clamp(mousePos2D.x, -7.75f, 7.75f);
        mousePos2D = new Vector2(mousePos2D.x, -4.730667f);

        transform.position = mousePos2D;
    }

    void Shoot()
    {
        if (Input.GetMouseButton(0))
        {
            Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Gọi GameOver khi player chạm Enemy
            shipsManager.GameOver();
        }
    }
}
