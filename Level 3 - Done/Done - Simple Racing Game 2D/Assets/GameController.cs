
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Transform[] AI_cars;
    public Vector2 start;
    public Vector2 end;

    public RectTransform winP;
    public RectTransform gameOverP;
    // Start is called before the first frame update
    void Awake()
    {
        float minx = start.x, miny = start.y, maxx = end.x, maxy = end.y;
        foreach (Transform ai in AI_cars)
        {
            var spawnX = Random.Range(minx, maxx);
            var spawnY = Random.Range(miny, maxy);
            ai.position = new Vector2(spawnX, spawnY);
        }
    }

    [SerializeField]bool win = false;
    [SerializeField]bool gameOver = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!win && !gameOver) return;
            Restart(); 

        }
    }

    public void Win()
    {
        winP.gameObject.SetActive(true);
        win = true;
        Time.timeScale = 0;
    }
    public void GameOver()
    {
        gameOverP.gameObject.SetActive(true);
        gameOver = true;
        Time.timeScale = 0;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
