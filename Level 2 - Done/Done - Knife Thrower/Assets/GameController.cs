using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI KnifeNumberText;
    public TextMeshProUGUI LevelText;
    int level = 1;
    public int knifeNumber = 100;
    public Transform knifePrefab;
    public Transform currentKnife;
    float minScale = 1f;
    float maxScale = 5f;
    int minBaseKnifeNumber = 1;
    int maxBaseKnifeNumber = 15;
    float maxBaseSpeed = 200f;
    float minBaseSpeed = 20f;
    public WheelController wheelController;
    public Color[] wheelColor;
    public Color[] camerabackgroundColor;

    public bool canThrow = true;
    void Awake()
    {
        BeginLevel();
        SpawnKnife();
    }

    void Update()
    {
        if (!canThrow) return;
        if (Input.GetMouseButtonDown(0) && currentKnife != null)
        {
            Vector3 origin = currentKnife.position;
            Vector3 direction = currentKnife.up;
            Debug.DrawRay(origin, direction * 100f, Color.red, 2f); // Vẽ ray ra màn hình
            Debug.Log($"Ray2D origin: {origin}, direction: {direction}");
            RaycastHit2D[] hit2Ds = Physics2D.RaycastAll(origin, direction, Mathf.Infinity);
            Debug.Log($"Raycast2D hit: {hit2Ds[1].collider != null}");

            // tạo biến trả về collider chứa tag = "Wheel"
            RaycastHit2D hit2D = System.Array.Find(hit2Ds, hit => hit.collider != null && hit.collider.CompareTag("Wheel"));


            Debug.Log($"Raycast2D hit collider tag: {(hit2D.collider.CompareTag("Wheel") ? hit2D.collider.tag : "No collider")}");
            if (hit2D.collider != null)
            {
                Debug.Log($"Hit2D collider: {hit2D.collider.name}, tag: {hit2D.collider.tag}, point: {hit2D.point}");
                // phóng dao tới wheel
                currentKnife.DOMove(hit2D.point + Vector2.down * transform.localScale.y / 2, 0.1f)
                .OnStart(() =>
                {
                    spawnedKnives.Add(currentKnife);
                })
                .OnComplete(() =>
                {
                    currentKnife.SetParent(hit2D.collider.transform);
                    currentKnife = null;
                    knifeNumber--;
                    if (knifeNumber > 0)
                    {
                        SpawnKnife();
                    }
                    else NextLevel();
                    // ghi lại spawned knife
                    UpdateTexts();
                });
            }
        }
    }
    [SerializeField] float scale;
    [SerializeField] int knifeBaseNumber;
    [SerializeField] float wheelSpeed;
    void BeginLevel()
    {
        // set level =  1
        level = 1;
        // đặt lại màu nền và màu wheel
        ChangeRandomCameraBackgroundColor();
        ChangeRandomWheelColor();
        // set các giá trị
        scale = maxScale;
        knifeBaseNumber = GetBaseKnifeNumberByScale(minBaseKnifeNumber, (int)scale);
        wheelSpeed = GetBaseSpeedByScale(minBaseSpeed, (int)scale);
        SetWheelControllerValues(scale, wheelSpeed);
        // bắt đầu hoạt động
        wheelController.active = true;
        // đặt lại level
        RenewLevel();
        UpdateTexts();
    }
    // hàm set giá trị cho wheelcontroller
    void SetWheelControllerValues(float scale, float wheelSpeed)
    {
        wheelController.SetScale(scale);
        wheelController.SetSpeed(wheelSpeed);
    }
    // hàm set số dao
    void SetKnifeBaseNumber(int number)
    {
        knifeBaseNumber = number;
    }
    [ContextMenu("Next Level")]
    void NextLevel()
    {
        // tăng level
        level++;
        // đặt lại màu nền và màu wheel
        ChangeRandomCameraBackgroundColor();
        ChangeRandomWheelColor();
        int nextLevelType = Random.Range(0, 7);
        // 1.Tăng tốc độ 10
        // 2. Tăng kích thước 0.5
        // 3. tăng số dao 1
        // 4 tăng tốc độ 10 và kích thước 0.5

        // 5 tăng số dao 1 và kích thước 0.5
        // 6 tăng số dao 1 và tốc độ 10
        // 7 tốc độ 10, kích thước 0.5, số dao 1
        switch (nextLevelType)
        {
            case 0:
                wheelSpeed += 20f;
                Debug.Log("Tăng tốc độ 20");
                break;
            case 1:
                scale -= 0.5f;
                Debug.Log("Giảm kích thước 0.5");
                break;
            case 2:
                knifeBaseNumber += 1;
                Debug.Log("Tăng số dao 1");
                break;
            case 3:
                wheelSpeed += 20f;
                scale -= 0.5f;
                Debug.Log("Tăng tốc độ 20f và giảm kích thước 0.5");
                break;
            case 4:
                knifeBaseNumber += 1;
                scale -= 0.5f;
                Debug.Log("Tăng số dao 1 và giảm kích thước 0.5");
                break;
            case 5:
                knifeBaseNumber += 1;
                wheelSpeed += 20f;
                Debug.Log("Tăng số dao 1 và tăng tốc độ 20f");
                break;
            case 6:
                knifeBaseNumber += 1;
                wheelSpeed += 20f;
                scale -= 0.5f;
                Debug.Log("Tăng số dao 1 và tăng tốc độ 20 và giảm kích thước 0.5");
                break;
        }
        // kiểm tra các giá trị đã đạt max chưa , nếu đạt thì không tăng nữa (>=max)
        if (scale < minScale) scale = minScale;
        if (knifeBaseNumber > maxBaseKnifeNumber * scale) knifeBaseNumber = maxBaseKnifeNumber * ((int)scale);
        if (wheelSpeed > maxBaseSpeed * scale) wheelSpeed = maxBaseSpeed * scale;
        // set lại các giá trị
        RenewLevel();
        SetWheelControllerValues(scale, wheelSpeed);
        SetKnifeBaseNumber(knifeBaseNumber);

        // tạo lại dao đầu tiên
        SpawnKnife();

        UpdateTexts();
    }
    [SerializeField] List<Transform> spawnedKnives = new List<Transform>();

    void RenewLevel()
    {
        // đặt lại số dao
        knifeNumber = knifeBaseNumber;
        // xóa tất cả số dao đã spawn
        foreach (var knife in spawnedKnives)
        {
            if (knife != null)
                Destroy(knife.gameObject);
        }
        spawnedKnives.Clear();
    }

    int GetBaseKnifeNumberByScale(int baseKnifeNumber, int scale)
    {
        return baseKnifeNumber * scale;
    }
    float GetBaseSpeedByScale(float baseSpeed, int scale)
    {
        return baseSpeed * scale;
    }

    void SpawnKnife()
    {
        currentKnife = Instantiate(knifePrefab);
    }
    void ChangeRandomWheelColor()
    {
        int index = Random.Range(0, wheelColor.Length);
        wheelController.GetComponent<SpriteRenderer>().color = wheelColor[index];
    }

    void ChangeRandomCameraBackgroundColor()
    {
        int index = Random.Range(0, camerabackgroundColor.Length);
        Camera.main.backgroundColor = camerabackgroundColor[index];
    }

    void UpdateTexts()
    {
        KnifeNumberText.text = knifeNumber.ToString();
        LevelText.text = "Level " + level.ToString();
    }
    // biến PanelGameOver = RectTransform
    public RectTransform PanelGameOver;
    // viết các hàm sau : ShowPanelGameOver, HidePanelGameOver, GameOver,restart , quit
    void ShowPanelGameOver()
    {
        canThrow = false;

        PanelGameOver.gameObject.SetActive(true);
        PanelGameOver.localScale = Vector3.zero;
        PanelGameOver.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack)

        ;
    }
    void HidePanelGameOver()
    {
        PanelGameOver.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            PanelGameOver.gameObject.SetActive(false);
        }
        )
        .OnComplete(() =>
        {

            canThrow = true;
        });
    }
    public void GameOver()
    {
        wheelController.active = false;
        ShowPanelGameOver();
    }
    public void Restart()
    {
        HidePanelGameOver();
        BeginLevel();
    }
    public void Quit()
    {
        Application.Quit();
    }

}
