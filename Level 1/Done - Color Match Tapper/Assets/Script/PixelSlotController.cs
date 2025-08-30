using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PixelSlotController : MonoBehaviour
{
    public GameController gameController;
    public int rightColorNumber;
    public int progressColorNumber;
    public bool isRightColor;
    void OnMouseEnter()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (gameController.IsMouseDown)
        {
            if (gameController.choosenColor.a == 0) return;
            transform.GetComponent<SpriteRenderer>().color = gameController.choosenColor;
            progressColorNumber = gameController.choosenNumber;
            isRightColor = IsRightColor();
        }
    }
    void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (gameController.choosenColor.a == 0) return;
        transform.GetComponent<SpriteRenderer>().color = gameController.choosenColor;
        progressColorNumber = gameController.choosenNumber;
        isRightColor = IsRightColor();
    }
    bool IsRightColor()
    {
        return rightColorNumber == progressColorNumber;
    }
}
