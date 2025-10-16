using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Transform riverprefab;
    public Transform[] itemPrefabs;

    public float lastRiverBoundY;
    public int randomOffsetSpawn;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            SpawnRiver();
        }

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Restart();
    }
    void SpawnRiver()
    {
        float sizeX = 12f;
        float sizeY = Random.Range(1, 5);
        var river = Instantiate(riverprefab);
        river.transform.localScale = new Vector3(sizeX, sizeY) * 0.5f;
        river.transform.position = new Vector3(-3, lastRiverBoundY + randomOffsetSpawn);
        lastRiverBoundY = river.transform.position.y + sizeY * 0.5f;
        randomOffsetSpawn = Random.Range(1, 3);
        Debug.Log(sizeY, river);
        int itemCount = (int)sizeY;
        Debug.Log(itemCount);
        for (int i = 0; i < itemCount; i++)
        {
            SpawnItem(river, i);
        }
    }

    void SpawnItem(Transform parent, int localY)
    {
        int index = Random.Range(0, itemPrefabs.Count());
        var item = Instantiate(itemPrefabs[index]);
        float localX = Random.Range(0, 1f) * 0.5f;
        item.parent = parent;
        item.position = parent.position + new Vector3(localX, localY * 0.5f);
        Debug.Log(parent, parent);
    }
    public RectTransform panel;
    public void GameOver()
    {
        panel.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void Restart() {
        SceneManager.LoadScene("GamePlay");
        Time.timeScale = 1;
    }
}
