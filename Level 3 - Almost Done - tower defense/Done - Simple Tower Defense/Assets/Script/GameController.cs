using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    public Transform[] enemiesPrefab;  // danh sách prefab enemy
    public Transform spawnPoint;       // vị trí spawn
    public Transform[] waypoints;
    private float timeBetweenEnemies = 1f;  // thời gian giữa mỗi enemy
    public float timeBetweenWaves = 10f;      // thời gian giữa các wave
    public Transform highlightPos;
    private int currentWave = 0;
    public Vector2Int tilemapMousePos;

    private float tileSize = 0.16f;   // size mỗi ô tile
    public Tilemap pathTilemap;
    public RectTransform towerPanel;
    public Vector3 choosenPosition;
    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        UpdateTilemapMousePos();
        //if (Input.GetMouseButtonDown(0)) OpenTowerPanel();
        if (Input.GetMouseButtonDown(0))
        {
            // Nếu bấm vào UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Nếu trúng chính panel thì không đóng
                return;
            }
            else
            {
                OpenTowerPanel(!towerPanel.gameObject.activeSelf, Input.mousePosition);
            }
        }
    }

    void OpenTowerPanel(bool open, Vector3 pos)
    {
        towerPanel.gameObject.SetActive(open);
        towerPanel.position = Input.mousePosition;
        choosenPosition = (Vector2)tilemapMousePos;
    }

    void UpdateTilemapMousePos()
    {
        // Lấy vị trí chuột theo world
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Chuyển sang tọa độ tile (làm tròn về int)
        int x = Mathf.FloorToInt(mouseWorld.x / tileSize);
        int y = Mathf.FloorToInt(mouseWorld.y / tileSize);

        tilemapMousePos = new Vector2Int(x, y);

        // Nếu bạn có highlightPos để theo chuột thì di chuyển nó
        if (highlightPos != null)
        {
            highlightPos.position = new Vector3(x * tileSize + tileSize / 2f, y * tileSize + tileSize / 2f, 0f);
        }
        highlightPos.position = (Vector2)tilemapMousePos * 0.16f + Vector2.one * 0.08f;
    }

    IEnumerator SpawnWaves()

    {
        while (currentWave < enemiesPrefab.Length) // mỗi wave ứng với 1 loại enemy
        {
            Transform enemyPrefab = enemiesPrefab[currentWave];

            int enemyCount = Random.Range(10, 16); // 10-15 con
            Debug.Log($"Wave {currentWave + 1} bắt đầu, spawn {enemyCount} enemy {enemyPrefab.name}");

            for (int i = 0; i < enemyCount; i++)
            {
                Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                yield return new WaitForSeconds(timeBetweenEnemies);
            }

            currentWave++;
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("🎉 Tất cả wave đã spawn xong!");
    }

    public void SpawnTower(Transform tower)
    {
        Instantiate(tower, choosenPosition * 0.16f + Vector3.one * 0.08f, Quaternion.identity);
        OpenTowerPanel(false, Vector3.zero);
    }
}
