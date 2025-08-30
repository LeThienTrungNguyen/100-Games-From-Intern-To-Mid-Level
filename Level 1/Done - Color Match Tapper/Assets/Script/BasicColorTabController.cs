using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicColorTabController : MonoBehaviour
{
    public RectTransform content;
    public RectTransform colorExample;
    public List<Color32> colorList = new();
    public NumbersMapConverter nmc;

    public void AddColorList(List<Color32> colors)
    {
        colorList = new();
        for (int i = 0; i < colors.Count; i++)
        {
            colors[i] = new Color32(colors[i].r, colors[i].g, colors[i].b, 255);
            colorList.Add(colors[i]);
        }
        AddColorListUI();
    }

    public void AddColorListUI()
    {
        ClearContent();
        for (int i = 0; i < colorList.Count; i++)
        {
            RectTransform colorSlot = Instantiate(colorExample, content);
            colorSlot.gameObject.SetActive(true);
            colorSlot.GetComponent<Image>().color = colorList[i];
            string numberTxt = (i + 1) + "";
            colorSlot.GetComponentInChildren<TextMeshProUGUI>().text = numberTxt;
            float brightness = 0.299f * colorSlot.GetComponent<Image>().color.r + 0.587f * colorSlot.GetComponent<Image>().color.g + 0.114f * colorSlot.GetComponent<Image>().color.b;
            if (brightness < 0.5f) colorSlot.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            else { colorSlot.GetComponentInChildren<TextMeshProUGUI>().color = Color.black; }
        }
    }

    void ClearContent()
    { 
        
    }
}
