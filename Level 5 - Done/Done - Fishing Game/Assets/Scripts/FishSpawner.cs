using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 1.0f;

    [Header("Colors")]
    public Color[] coolColors = { Color.cyan, Color.green, Color.blue, new Color(0.5f, 0f, 0.5f) };
    public Color[] warmColors = { Color.red, new Color(1f, 0.5f, 0f), Color.yellow, new Color(1f, 0f, 0.5f) };

    private PlayerFish player;
    private float timer;
    private Camera mainCam;

    void Start()
    {
        player = FindObjectOfType<PlayerFish>();
        mainCam = Camera.main;
    }

    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerFish>();
            return;
        }

        timer += Time.deltaTime;
        
        float currentInterval = player.isGodMode ? 0.15f : spawnInterval;

        if (timer >= currentInterval)
        {
            SpawnFish();
            timer = 0f;
        }
    }

    void SpawnFish()
    {
        // Kích thước ảo của người chơi
        float pVirtualSize = player.virtualSize;
        float randomVirtualSize;
        bool isCool = (Random.value < 0.7f);

        // Tạo kích thước ảo cho cá địch dựa trên virtualSize của Player
        // Cá Cool: Nhỏ hơn player (từ 30% đến 90%) -> Ăn được
        // Cá Warm: Lớn hơn player (từ 110% đến 300%) -> Nguy hiểm
        if (isCool) randomVirtualSize = pVirtualSize * Random.Range(0.3f, 0.9f);
        else randomVirtualSize = pVirtualSize * Random.Range(1.1f, 3.0f);

        // Đảm bảo không quá nhỏ để không bị lỗi collider
        randomVirtualSize = Mathf.Max(randomVirtualSize, 0.2f);

        // Vị trí spawn cố định ở mép màn hình (khoảng 12 đơn vị)
        float spawnPosX = 12f;
        float spawnRangeY = 7f;

        float x = (Random.value > 0.5f) ? spawnPosX : -spawnPosX;
        float y = Random.Range(-spawnRangeY, spawnRangeY);
        Vector3 spawnPos = new Vector3(x, y, 0);

        GameObject newFish = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        EnemyFish fishScript = newFish.GetComponent<EnemyFish>();

        if (fishScript != null)
        {
            SetFishAppearance(newFish, randomVirtualSize, isCool);
            
            // Tốc độ cá là độc lập, từ 3-6 đơn vị/giây
            float baseSpeed = Random.Range(3f, 6f);

            fishScript.SetProperties(randomVirtualSize, baseSpeed, player);
        }
    }

    void SetFishAppearance(GameObject fish, float size, bool isCool)
    {
        SpriteRenderer sr = fish.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color[] palette = isCool ? coolColors : warmColors;
            sr.color = palette[Random.Range(0, palette.Length)];
        }
    }
}
