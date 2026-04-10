using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public Transform camTransform; // Kéo Camera của Player vào đây

    void Update()
    {
        if (UIManager.Instance != null && (UIManager.Instance.IsUIOpen || UIManager.Instance.IsPlayerLocked))
            return;

        HandleInteractionKeys();
    }

    void HandleInteractionKeys()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float currentRange = PlayerStats.Instance != null ? PlayerStats.Instance.interactableRange : 3f;
            Ray ray = new Ray(camTransform.position, camTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, currentRange))
            {
                Debug.Log($"<color=cyan>[Interact] Hit: {hit.collider.name} with Tag: {hit.collider.tag}</color>");

                if (hit.collider.CompareTag("Mailbox"))
                {
                    if (!UIManager.Instance.IsUIOpen)
                    {
                        Mailbox.Instance.OpenMail();
                        Debug.Log("<color=green>Đã tương tác với Hộp thư!</color>");
                    }
                    else Mailbox.Instance.CloseMail();
                }
                else if (hit.collider.CompareTag("DeliveryBox"))
                {
                    DeliveryBox dBox = hit.collider.GetComponent<DeliveryBox>();
                    if (dBox != null) dBox.Interact();
                }
            }
            else
            {
                if (Mailbox.IsReadingMail) Mailbox.Instance.CloseMail();
                else Debug.Log("<color=red>Nhấn E nhưng không nhìn vào vật thể tương tác nào.</color>");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        float currentRange = PlayerStats.Instance != null ? PlayerStats.Instance.interactableRange : 3f;
        Gizmos.DrawWireSphere(transform.position, currentRange);
    }
}