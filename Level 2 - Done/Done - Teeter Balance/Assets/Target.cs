
using Unity.VisualScripting;
using UnityEngine;

public class Target : MonoBehaviour
{
    GameController gameController;
    void Awake()
    {
        transform.AddComponent<BoxCollider2D>();
        GetComponent<BoxCollider2D>().isTrigger = true;
       //gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("You Win!");
            int nextSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;
            int totalScenes = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            if (nextSceneIndex < totalScenes)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.Log("No more scenes. Quitting game.");
                Application.Quit();
            }
        }
    }
}
