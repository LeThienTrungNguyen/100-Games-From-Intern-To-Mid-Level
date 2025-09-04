using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI moneyTextWorld;
    public TextMeshProUGUI moneyTextUI;
    public TextMeshProUGUI total;
    public Button betButton;
    public RectTransform panel;
    public TMP_InputField ballCostIF;
    public TMP_InputField ballNumberIF;

    [Header("Ball Settings")]
    public Transform ballPrefab;
    public List<Transform> scoreZones;
    public float money = 100f; // tiền khởi tạo
    public float ballCost = 1f;
    public int ballsPerClick = 10;
    public bool allIn = false;
    public bool canSpawn = true;
    public bool isSpawningBalls = false;
    public float spawnInterval = 0.05f;

    private float betMoney;
    private List<Transform> ballsList = new List<Transform>();

    void Start()
    {
        // Gắn listener để xử lý input an toàn
        ballCostIF.onValueChanged.AddListener(OnBallCostInputChanged);
        ballNumberIF.onValueChanged.AddListener(OnBallNumberInputChanged);

        // Gán giá trị ban đầu
        ballCostIF.text = ballCost.ToString();
        ballNumberIF.text = ballsPerClick.ToString();
        UpdateUI();
        RecalculateBet();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)){
            ClearBalls();
         }
        if (Input.GetKeyDown(KeyCode.E))
        {
            money += 1f;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && canSpawn)
        {
            panel.gameObject.SetActive(!panel.gameObject.activeSelf);
        }
        if(Input.GetKeyDown(KeyCode.B) && canSpawn)
        {
            Bet();
        }
        // Chỉ cho spawn khi không còn bóng nào
        canSpawn = !isSpawningBalls && CountBalls() == 0;

        // Cập nhật nút bet
        betButton.interactable = betMoney <= money;
        total.color = betMoney > money ? Color.red : Color.white;

        UpdateUI();
    }

    // =========================
    // XỬ LÝ INPUT
    // =========================

    void OnBallCostInputChanged(string value)
    {
        if (!float.TryParse(value, out float result) || result <= 0)
        {
            result = 1f;
        }
        ballCost = result;
        ballCostIF.text = ballCost.ToString();
        RecalculateBet();
    }

    void OnBallNumberInputChanged(string value)
    {
        if (!int.TryParse(value, out int result) || result <= 0)
        {
            result = 1;
        }
        ballsPerClick = result;
        ballNumberIF.text = ballsPerClick.ToString();
        RecalculateBet();
    }

    void RecalculateBet()
    {
        betMoney = ballCost * ballsPerClick;
        total.text = "Total: " + betMoney.ToString("F2") + "$";
    }

    // =========================
    // BET
    // =========================
    public void Bet()
    {
        if (betMoney > money) return;

        panel.gameObject.SetActive(false);

        // ❌ Trừ toàn bộ tiền cược một lần duy nhất
        money -= betMoney;

        StartCoroutine(SpawnBalls());
        UpdateUI();
    }

    IEnumerator SpawnBalls()
    {
        int ballsToSpawn = allIn ? Mathf.FloorToInt(money / ballCost) : ballsPerClick;
        isSpawningBalls = true;

        for (int i = 0; i < ballsToSpawn; i++)
        {
            CreateBall(); // ❌ không trừ tiền nữa
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawningBalls = false;
    }

    void CreateBall()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(-9.49f, 9.26f),
            Random.Range(5.57f, 12f)
        );

        var ball = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
        ball.GetComponent<Rigidbody2D>().sharedMaterial = RandomBouncyMaterial();
        ballsList.Add(ball);
    }

    PhysicsMaterial2D RandomBouncyMaterial()
    {
        PhysicsMaterial2D mat = new PhysicsMaterial2D();
        mat.bounciness = Random.Range(0.4f, 0.9f);
        mat.friction = 0f;
        return mat;
    }

    [ContextMenu("Clear Balls")]
    public void ClearBalls()
    {
        foreach (var ball in ballsList)
        {
            if (ball != null) Destroy(ball.gameObject);
        }
        ballsList.Clear();
    }

    int CountBalls()
    {
        int count = 0;
        foreach (var ball in ballsList)
        {
            if (ball != null) count++;
        }
        return count;
    }

    void UpdateUI()
    {
        moneyTextUI.text = money.ToString("F2") + "$";
        moneyTextWorld.text = money.ToString("F2") + "$";
        total.text = "Total: " + betMoney.ToString("F2") + "$";
    }
}
