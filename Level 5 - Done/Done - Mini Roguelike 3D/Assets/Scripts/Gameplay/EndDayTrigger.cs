using UnityEngine;

public class EndDayTrigger : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject visual; // Kéo model hoặc hiệu ứng của Trigger vào đây

    private bool hasTriggered = false;

    private void Start()
    {
        SetActiveTrigger(false);
    }

    public void SetActiveTrigger(bool isActive)
    {
        if (visual != null) visual.SetActive(isActive);
        gameObject.SetActive(isActive);
        hasTriggered = false; // Reset trạng thái khi bắt đầu ngày mới
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        // Kiểm tra nếu là người chơi chạm vào (giả định Player có tag "Player")
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            Debug.Log("<color=green>Người chơi đã chạm vào End Day Trigger!</color>");
            
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnDayEnd();
            }
        }
    }
}
