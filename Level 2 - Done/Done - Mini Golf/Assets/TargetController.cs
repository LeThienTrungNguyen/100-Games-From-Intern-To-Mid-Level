using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Load next scene index after 2seconds
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Hit the target");
            Invoke("LoadScene", 1f);
        }
    }

    void LoadScene()
    {
        // nếu index là last index thì quit game

        int index = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int totalScenes = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        if (index < totalScenes - 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(index + 1);
        }
        else
        {
            Application.Quit();
        }
        
    }
}
