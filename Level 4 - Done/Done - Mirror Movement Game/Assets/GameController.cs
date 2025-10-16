using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Transform target1;
    public Transform target2;
    public bool isTarget1Filled;
    public bool isTarget2Filled;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isTarget1Filled = IsTargetFilled(target1);
        isTarget2Filled = IsTargetFilled(target2);

        if (isTarget1Filled && isTarget2Filled)
        {
            NextLevel();
        }
    }

    void NextLevel()
    {
        // Lấy scene hiện tại và tổng số scene trong Build Settings
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        // Nếu chưa phải màn cuối => chuyển qua màn tiếp theo
        if (currentIndex < totalScenes - 1)
        {
            SceneManager.LoadScene(currentIndex + 1);
        }
        else
        {
            // Nếu là màn cuối => thoát game
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    bool IsTargetFilled(Transform target)
    {
        return Physics2D.OverlapPoint(target.position);
    }
}
