using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{

    public Canvas canvas;
    public RectTransform Panel;
    public void ShowPanel()
    {
        Panel.gameObject.SetActive(true);
    }

    public void HidePanel()
    { 
        Panel.gameObject.SetActive(false);
    }
    public void TryAgain()
    {
        GameController.instance.groups.GetComponent<GroupsController>().Reset();
        HidePanel();
    }
}
