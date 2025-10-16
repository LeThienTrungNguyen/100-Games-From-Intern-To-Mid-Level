using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitController : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
NextLevel();
        }
    }

    void NextLevel()
    {
        // Lấy index của scene hiện tại
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Tải scene kế tiếp (theo thứ tự trong Build Settings)
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
