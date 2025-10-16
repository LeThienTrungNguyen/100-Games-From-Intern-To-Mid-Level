using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float jumpForce = 5f;

    Rigidbody rb;
    Camera cam;

    float yaw = 0f;
    float pitch = 0f;

    bool wantJump = false;

    void Start()
    {
        canMove = true;
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;

        if (cam == null)
            Debug.LogWarning("Main Camera not found. Make sure it's tagged MainCamera and is child of Player.");

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public bool canMove = false;
    void Update()
    {
        if (!canMove) return;
        // === Xử lý xoay camera ===
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mx;
        pitch -= my;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        if (cam != null)
            cam.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        if (Input.GetButtonDown("Jump"))
            wantJump = true;

        // === Xử lý click chuột ===
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                string tag = hit.collider.tag;
                Debug.Log("Clicked on tag: " + tag,hit.transform);

                switch (tag)
                {
                    case "Paint":
                        DragDown(hit.collider.gameObject);
                        break;

                    case "Box":
                        DragBack(hit.collider.gameObject);
                        break;

                    case "Pillar":
                        Play(hit.collider.gameObject);
                        break;
                    case "Lock":
                        Unlock(hit.collider.gameObject);
                        break;
                }
            }
        }
    }
    void Rotate(GameObject target)
    {
        target.GetComponent<Wheel>().Rotate();
    }
    void FixedUpdate()
    {
        // Xoay ngang
        Quaternion target = Quaternion.Euler(0f, yaw, 0f);
        rb.MoveRotation(target);

        // Di chuyển
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = transform.right * h + transform.forward * v;
        Vector3 vel = moveDir * moveSpeed;
        vel.y = rb.velocity.y;
        rb.velocity = vel;

        // Nhảy
        if (wantJump)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            wantJump = false;
        }
    }
    void Unlock(GameObject target)
    {
        target.GetComponentInChildren<Lock>().isReady = true;
        target.transform.root.GetComponent<Collider>().enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    // === Các hàm xử lý ===
    void DragDown(GameObject target)
    {
        Debug.Log("DragDown on " + target.name);
        var pos = target.transform.position;
        pos.y = 11.73f;
        target.transform.position = pos;
        // TODO: logic của bạn
    }

    void DragBack(GameObject target)
    {
        Debug.Log("DragBack on " + target.name);
        var pos = target.transform.position;
        pos.x += -0.1f;
        target.transform.position = pos;
        // TODO: logic của bạn
    }

    void Play(GameObject target)
    {
        Debug.Log("Play on " + target.name);
        target.GetComponent<TowerOfHanoi>().isReady = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // TODO: logic của bạn
    }
}
