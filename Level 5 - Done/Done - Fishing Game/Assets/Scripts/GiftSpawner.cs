using UnityEngine;

public class GiftSpawner : MonoBehaviour
{
    public GameObject giftPrefab;
    public float minSpawnTime = 5f;
    public float maxSpawnTime = 10f;
    
    private float nextSpawnTime;
    private float timer;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        SetRandomSpawnTime();
    }

    void Update()
    {
        // Chỉ chạy timer nếu trên màn hình CHƯA có hộp quà nào
        if (!IsGiftOnScreen())
        {
            timer += Time.deltaTime;
            if (timer >= nextSpawnTime)
            {
                SpawnGift();
                timer = 0f;
                SetRandomSpawnTime();
            }
        }
        else
        {
            // Nếu có hộp quà rồi thì reset timer (hoặc giữ nguyên tùy ý bạn)
            timer = 0f;
        }
    }

    void SetRandomSpawnTime()
    {
        nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }

    bool IsGiftOnScreen()
    {
        // Kiểm tra xem có đối tượng nào có script GiftBox trong Scene không
        return FindObjectOfType<GiftBox>() != null;
    }

    void SpawnGift()
    {
        if (giftPrefab == null) return;

        // Tính toán vùng an toàn bên trong Camera
        float camHeight = mainCam.orthographicSize - 1f;
        float camWidth = (camHeight * mainCam.aspect) - 1f;

        float randomX = Random.Range(-camWidth, camWidth);
        float randomY = Random.Range(-camHeight, camHeight);
        
        Vector3 spawnPos = new Vector3(randomX, randomY, 0);
        GameObject newGift = Instantiate(giftPrefab, spawnPos, Quaternion.identity);
        
        // ĐIỀU CHỈNH SCALE: Hộp quà to theo Camera (mặc định cam size là 5)
        float scaleFactor = mainCam.orthographicSize / 5f;
        newGift.transform.localScale = Vector3.one * scaleFactor;
        
        Debug.Log("<color=green>GIFT SPAWNED! Scale: " + scaleFactor + "</color>");
    }
}
