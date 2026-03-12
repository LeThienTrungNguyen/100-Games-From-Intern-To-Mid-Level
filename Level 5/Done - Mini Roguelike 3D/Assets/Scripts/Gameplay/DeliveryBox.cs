using UnityEngine;
using DG.Tweening;

public class DeliveryBox : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject notificationVisual; // Kéo vật thể hiệu ứng (ví dụ Icon nổi) vào đây

    private void Start()
    {
        // Nếu bạn để trống ô này trong Inspector, nó sẽ tự lấy chính cái thùng để làm hiệu ứng
        if (notificationVisual == null) notificationVisual = this.gameObject;
        
        SetupNotificationAnimation();
    }

    private void SetupNotificationAnimation()
    {
        if (notificationVisual == null) return;

        // 1. Xoay tròn liên tục
        notificationVisual.transform.DOLocalRotate(new Vector3(0, 360, 0), 3f, RotateMode.LocalAxisAdd)
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);

        // 2. Hiệu ứng nổi bồng bềnh và Pulse Scale
        Vector3 originalScale = notificationVisual.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        float startY = notificationVisual.transform.localPosition.y;

        Sequence s = DOTween.Sequence();
        s.Append(notificationVisual.transform.DOLocalMoveY(startY + 0.3f, 1f).SetEase(Ease.InOutQuad));
        s.Join(notificationVisual.transform.DOScale(targetScale, 1f).SetEase(Ease.InOutQuad));
        s.Append(notificationVisual.transform.DOLocalMoveY(startY, 1f).SetEase(Ease.InOutQuad));
        s.Join(notificationVisual.transform.DOScale(originalScale, 1f).SetEase(Ease.InOutQuad));

        s.SetLoops(-1, LoopType.Restart);
    }

    public void Interact()
    {
        Debug.Log("<color=yellow>[DeliveryBox] Interact called!</color>");
        if (DeliveryManager.Instance != null && DeliveryManager.Instance.todaysDelivery.Count > 0)
        {
            // Mở UI Giao hàng
            if (UIManager.Instance != null)
            {
                Debug.Log("<color=green>[DeliveryBox] Opening UI...</color>");
                UIManager.Instance.OpenDeliveryUI();
            }
            else
            {
                Debug.LogError("<color=red>[DeliveryBox] UIManager.Instance is null!</color>");
            }
        }
        else
        {
            Debug.LogWarning("<color=orange>[DeliveryBox] No items in todaysDelivery to show!</color>");
        }
    }
}
