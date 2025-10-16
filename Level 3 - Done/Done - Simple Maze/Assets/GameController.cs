using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Transform start;
    public Transform end;
    public Transform player;
    void Awake()
    {
        Instantiate(player, start.position + Vector3.down*0.3f, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Quit();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
