using UnityEngine;
using DG.Tweening;
public class ItemController : MonoBehaviour
{
    public bool canInteract = true;
    void OnMouseDown()
    {
        if (GameController.instance.isSwaping) return;
        if (!canInteract) return;
        Debug.Log(transform.name);
        if (GameController.instance.pickedItem1 != transform)
        {
            transform.DOLocalMoveY(transform.localPosition.y + 0.3f, 0.2f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                GameController.instance.AddPickedItem(transform);
            })
            ;
        }
        else
        { 
            transform.DOLocalMoveY(transform.localPosition.y - 0.3f, 0.2f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                GameController.instance.AddPickedItem(transform);
            })
            ;
        }
        
        
    }
}
