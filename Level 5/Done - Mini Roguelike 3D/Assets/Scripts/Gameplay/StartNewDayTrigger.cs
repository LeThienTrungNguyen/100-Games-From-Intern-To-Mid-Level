using UnityEngine;

public class StartNewDayTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu là Player bước vào vùng Trigger
        if (other.CompareTag("Player"))
        {
            if (TimeManager.Instance != null && !TimeManager.Instance.IsTimerRunning())
            {
                TimeManager.Instance.StartDay();
                
                // Tự ẩn chính mình ngay lập tức
                gameObject.SetActive(false);
                
                Debug.Log("<color=green>Player đã bước vào hầm mỏ. Bắt đầu tính giờ và ẩn Trigger!</color>");
            }
        }
    }
}