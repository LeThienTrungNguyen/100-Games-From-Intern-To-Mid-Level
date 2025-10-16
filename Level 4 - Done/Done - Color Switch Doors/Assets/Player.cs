using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public PlayerColor playerColor;
    public int score;
    public Transform firstCircle;
    void Awake()
    {
        ChangeRandomColor();
        lastCirclePos = firstCircle.position;
    }
    void Update()
    {
        Jump();
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    void Jump()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.up * 5;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag != transform.tag)
        {
            GameOver();
        }
        else { score++; UpdateScore(score); ChangeRandomColor(); SpawnNewCircle(); }
    }
    public RectTransform panel;
    void GameOver()
    {
        panel.gameObject.SetActive(true);
    }

    void Restart()
    {
        //reset scene
        SceneManager.LoadScene(0);
    }
    Color GetColor()
    {
        return playerColor switch
        {
            PlayerColor.Cyan => Color.cyan,
            PlayerColor.Yellow => Color.yellow,
            PlayerColor.Pink => new Color(255f / 255f, 0f, 142f / 255f),
            PlayerColor.Purple => new Color(150f / 255f, 0, 255f / 255f),
            _ => Color.cyan
        };
    }

    void ChangeRandomColor()
    {
        playerColor = Random.Range(0, 4) switch
        {
            0 => PlayerColor.Cyan,
            1 => PlayerColor.Yellow,
            2 => PlayerColor.Pink,
            3 => PlayerColor.Purple,
            _ => PlayerColor.Cyan
        };
        transform.GetComponent<SpriteRenderer>().color = GetColor();
        transform.tag = playerColor.ToString();
    }

    public TextMeshProUGUI scoreText;
    void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }
    public Transform circlePrefab;
    public Vector3 lastCirclePos;
    void SpawnNewCircle()
    {
        var c = Instantiate(circlePrefab, lastCirclePos + Vector3.up * 4, Quaternion.identity);
        lastCirclePos = c.position;
    }
}

public enum PlayerColor
{ 
    Cyan , Yellow , Pink , Purple
}
