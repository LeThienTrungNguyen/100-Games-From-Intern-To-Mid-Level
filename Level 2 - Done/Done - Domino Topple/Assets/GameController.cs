using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public List<GameObject> dominoLst = new();
    public float fallAngleThreshold = 60f; // Góc nghiêng so với trục Y
    public float checkDelay = 0.5f; // Thời gian kiểm tra liên tiếp để ổn định
    public bool isReadyToPlay = false;
    void Start()
    {
        dominoLst = GameObject.FindGameObjectsWithTag("Domino").ToList();
    }
    public bool isAllFalled;
    void FixedUpdate()
    {
        if (dominoLst.Count() > 0)
        {
            isAllFalled = true;
            foreach (var d in dominoLst)
            {
                if (!d.GetComponent<Domino>().isFalled) isAllFalled = false;

            }
        }
    }
    public Transform dominoShadow;
    public Transform dominoPrefab;
    public bool isAllowPlaceDomino;
    void Awake()
    {
        isReadyToPlay = false;
    }
    void Update()
    {
        dominoShadow.gameObject.SetActive(isAllowPlaceDomino);
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AllowPlaceDomino();
        }
        Vector2 mousePos2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dominoShadow.transform.position = mousePos2D;
        if (Input.GetMouseButtonDown(0))
        {
            if (isAllowPlaceDomino) Instantiate(dominoPrefab, dominoShadow.position, Quaternion.identity);

        }
        if (Input.GetKeyDown(KeyCode.R)) ReloadScene();
        if (!isReadyToPlay)
        {
            startDomino.GetComponent<Rigidbody2D>().gravityScale = 0;
            startDomino.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
        else
        {
            startDomino.GetComponent<Rigidbody2D>().gravityScale = 1;
            startDomino.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isReadyToPlay = true;
        }
        if (isAllFalled) LoadNextScene();
    }
    [SerializeField] Transform startDomino;
    void AllowPlaceDomino()
    {
        isAllowPlaceDomino = !isAllowPlaceDomino;
    }
    void ReloadScene()
    { 
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex);
    }
    void LoadNextScene()
    { 
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // Nếu chưa phải scene cuối -> load scene tiếp theo
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Application.Quit();
            Debug.Log("This is the last scene!");
            // Có thể reload scene đầu tiên nếu muốn:
            // SceneManager.LoadScene(0);
        }
    }
}
