using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int enemiesCount;
    void FixedUpdate()
    {
        enemiesCount = EnemiesCount();
        if (enemiesCount == 0)
        {
            Invoke("LoadNextScene", 2f); 
        }
    }

    public void LoadNextScene()
    { 
        // Lấy index của scene hiện tại
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Load scene tiếp theo (nếu còn)
        if (currentSceneIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        else
        {
            Debug.Log("Không còn scene tiếp theo!");
            // Hoặc load lại từ đầu:
            // SceneManager.LoadScene(0);
        }
    }
    public int EnemiesCount()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Count();
    }
}
