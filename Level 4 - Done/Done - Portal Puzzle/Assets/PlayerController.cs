using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // tốc độ di chuyển
    [Header("Portal")]
    public Transform portal1;
    public Transform portal2;
    private Rigidbody2D rb;
    public bool canMouse0Input;
    public bool canMouse1Input;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Movement();
        if (Input.GetMouseButtonDown(0) && canMouse0Input)
        {
            SpawnPortal(0);
        }
        else if (Input.GetMouseButtonDown(1) && canMouse1Input)
        {
            SpawnPortal(1);
        }
    }

    void Movement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal"); // nhận input A/D hoặc ← →
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void SpawnPortal(int mouseInput)
    {
        Transform portal = null;
        if (mouseInput == 0)
        {
            portal = portal1;
        }
        else if (mouseInput == 1)
        {
            portal = portal2;
        }

        if (portal == null) return;
        portal.gameObject.SetActive(!portal.gameObject.activeSelf);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position,  Mathf.Infinity, LayerMask.GetMask("Wall"));
        portal.transform.position = hit.point;
        Vector2 normal = hit.normal;

        float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
        portal.rotation = Quaternion.Euler(0, 0, angle);
        portal.gameObject.SetActive(true);
        Debug.DrawRay(hit.point, normal * 0.5f, Color.green, 2f);
    }
}
