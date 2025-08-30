using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public RectTransform panel;
    public TextMeshProUGUI scoreTxt;
    // Start is called before the first frame update
    public void OpenPanel()
    {
        panel.gameObject.SetActive(true);
    }

    public void ClosePanel()
    {
        panel.gameObject.SetActive(false);
    }

    public void ShowScore(int score)
    {
        scoreTxt.text = score+"";
    }
}
