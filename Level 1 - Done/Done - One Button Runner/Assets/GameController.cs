using UnityEngine;
using DG.Tweening;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour
{
    //public Transform ground;
    public CanvasController CanvasController;
    public static GameController instance;
    public Transform ladderPrefab;
    public Transform groundPrefab;
    public Transform firstLadderSpawnPos;
    public Transform firstFarestGround;

    public Vector3 startPosition;

    [Space]

    // Khai báo spawnedLadder là biến thành viên
    public Transform ladderSpawnPos;
    public Transform playerLandPos;
    [Space]
    public Transform spawnedLadder;

    [Space]
    public Transform farestGround;
    public bool allowSpawnLadder = false;
    public bool isSpawningLadder = false;

    public bool canInteract = true;
    float scaleY = 0;
    public int score; 

    //
    [SerializeField] List<LadderController> spawnedLadders;
    [SerializeField] List<GroundController> spawnedGrounds;
    void Awake()
    {
        instance = this;
        spawnedLadders = new List<LadderController>();
        spawnedGrounds = new List<GroundController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canInteract) return;
        if (Input.GetKey(KeyCode.Space))
        {
            if (!isSpawningLadder)
            {
                // Khởi tạo spawnedLadder và gán cho biến thành viên

                spawnedLadder = Instantiate(ladderPrefab, ladderSpawnPos.position, Quaternion.identity);
                spawnedLadders.Add(spawnedLadder.GetComponentInChildren<LadderController>());
                // Đặt vị trí ban đầu của thang (ví dụ: ngay trên mặt đất)
                // Bạn có thể cần điều chỉnh vị trí Y tùy thuộc vào pivot của prefab thang
                //spawnedLadder.position = ground.position + new Vector3(0, ground.localScale.y / 2 + 0.05f, 0); 
                spawnedLadder.gameObject.SetActive(true);
                isSpawningLadder = true;
                scaleY = 0.1f; // Đặt scaleY ban đầu để thang có chiều cao tối thiểu
                spawnedLadder.localScale = new Vector3(0.1f, scaleY, 0.1f); // Thiết lập scale ban đầu
            }
            else
            {
                // Chỉ tăng scaleY nếu spawnedLadder đã được tạo
                if (spawnedLadder != null)
                {
                    scaleY += 0.01f; // Tăng tốc độ kéo dài
                    spawnedLadder.localScale = new Vector3(0.1f, scaleY, 0.1f);
                    // Điều chỉnh vị trí Y để thang mọc lên từ chân
                    // Bạn cần điều chỉnh giá trị này tùy thuộc vào cách thang của bạn được tạo ra
                    //spawnedLadder.position = ground.position + new Vector3(0, ground.localScale.y / 2 + scaleY / 2, 0);
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            spawnedLadder.DORotate(new Vector3(0, 0, -90f), 0.5f)
            .OnStart(() =>
            {
                canInteract = false;
                IgnoreLayerCollisionPlayerLadder(true);
            })
            .OnComplete(() =>
            {
                var movePosition = spawnedLadder.GetComponent<LadderController>().moveTarget;
                PlayerController.instance.MoveHorizontal(movePosition);
            })
            ;
            // Reset trạng thái khi nhả phím Space
            isSpawningLadder = false;


        }
    }
    public void CreateGround()
    {
        Debug.Log("CreateGround");
        Transform spawnedGround = Instantiate(groundPrefab);
        spawnedGrounds.Add(spawnedGround.GetComponentInChildren<GroundController>());
        spawnedGround.gameObject.SetActive(true);
        float distance = Random.Range(3f, 7f);
        Vector3 position = farestGround.position + new Vector3(1, 0, 0) * distance;
        float scaleX = Random.Range(0.3f, 3);
        spawnedGround.localScale = new Vector3(scaleX, 2, 1);
        spawnedGround.position = position;

        farestGround = spawnedGround;
    }
    public void IgnoreLayerCollisionPlayerLadder(bool ignore)
    {
        Physics2D.IgnoreLayerCollision(6, 3, !ignore);
    }

    public void SetNewStartGround(GroundController groundController)
    {
        groundController.isLanded = false;
        ladderSpawnPos = groundController.LadderSpawnPos;
        playerLandPos = groundController.LadderSpawnPos;
    }

    public void CreateNewGround()
    {
        Transform ground = Instantiate(groundPrefab);
        float spawnPosY = -5f;
        float scaleY = 2;
        float scaleX = 1f;
        float offsetX = 3f;
        float spawnPosX = farestGround.position.x + offsetX;
        ground.position = new Vector3(spawnPosX, spawnPosY);
        ground.localScale = new Vector3(scaleX, scaleY);
    }

    public void GameOver()
    {
        Debug.Log("Game Over");

        CanvasController.OpenPanel();
        Time.timeScale = 0;
    }

    public void Reset()
    {
        CanvasController.ClosePanel();
        PlayerController.instance.transform.position = startPosition;
        Time.timeScale = 1;
        ladderSpawnPos = firstLadderSpawnPos;
        farestGround = firstFarestGround;
        ClearSpawnedGrounds();
        ClearSpawnedLadders();

        GroundController[] groundControllers = GameObject.FindObjectsByType<GroundController>(FindObjectsSortMode.None);
        Debug.Log(groundControllers.Count());
        foreach (var groundController in groundControllers)
        {
            groundController.isLanded = false;
        }
        groundControllers[0].isLanded = true;
        score = 0;
        CanvasController.ShowScore(score);
    }

    public void ClearSpawnedGrounds()
    {
        foreach (var spawnedGround in spawnedGrounds)
        {
            DestroyImmediate(spawnedGround.gameObject);
        }
        spawnedGrounds.Clear();
    }
    public void ClearSpawnedLadders()
    {
        foreach (var spawnedLadder in spawnedLadders)
        {
            DestroyImmediate(spawnedLadder.gameObject);
        }
        spawnedLadders.Clear();
    }

    public void AddScore()
    {
        score++;
        CanvasController.ShowScore(score);
    }
}