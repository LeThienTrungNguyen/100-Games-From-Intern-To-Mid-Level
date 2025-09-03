
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CanvasController : MonoBehaviour
{
    public RectTransform panels;
    public RectTransform panel1;
    public RectTransform panel2;
    public RectTransform GameOverPanel;
    public int currentPanel = 0;
    public void SwitchPanel(int panelCount)
    {
        panels.DOLocalMoveX(currentPanel - 2436 * panelCount, 0.5f);
    }

    public void ChooseLevel(string level)
    {
        PlayerPrefs.SetString("level", level);
        PlayerPrefs.Save();

        SceneManager.LoadScene("GamePlay");
    }
    public static void LoadScene(string scence)
    {
        SceneManager.LoadScene(scence);
    }
    public static void QuitGame()
    {
            Application.Quit(); // Thoát game khi build
    }
    public void ShowGameOverPanel()
    {
        GameOverPanel.gameObject.SetActive(true);
    }
    public void Restart()
    {
        GameController.instance.Restart();
        GameOverPanel.gameObject.SetActive(false);
    }
}
