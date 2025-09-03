using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingButton : MonoBehaviour
{
    public RectTransform SettingUI;

    public void ToggleSetting()
    {
        SettingUI.gameObject.SetActive(!SettingUI.gameObject.activeSelf);
    }
    public void ShowSetting()
    {
        SettingUI.gameObject.SetActive(true);
    }

    public void HideSetting()
    {
        SettingUI.gameObject.SetActive(false);
    }
 
}
