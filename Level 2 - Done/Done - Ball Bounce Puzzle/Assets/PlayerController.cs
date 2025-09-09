using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform bullet;
    public float force;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        LookToMouse();
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }
    [SerializeField]Vector2 direction;
    void LookToMouse()
    {
        // Player luôn hướng về vị trí con trỏ chuột
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        direction = mouseWorldPosition - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
    void Shoot()
    {
        Transform bulletT = Instantiate(bullet, transform.position, Quaternion.identity);
        bulletT.GetComponent<Rigidbody2D>().AddForce(direction.normalized * force, ForceMode2D.Impulse);
        Destroy(bulletT.gameObject, 5f);
    }
    
}
