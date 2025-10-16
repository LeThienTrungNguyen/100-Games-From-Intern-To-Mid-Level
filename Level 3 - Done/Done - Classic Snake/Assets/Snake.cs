using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
    Vector2 _direction = Vector2.right;
    public List<Transform> _segments = new();

    public Transform _segmentPrefab;
    public float speed;
    void Start()
    {
        _segments.Add(this.transform);

        // tìm food trong scene và gán snake reference
        Food food = FindObjectOfType<Food>();
        if (food != null)
        {
            food.snake = this;
        }

        InvokeRepeating(nameof(Move), 1f, 1f / speed);
    }
    void Reset()
    {
        Time.timeScale = 1;
        // Lấy scene hiện tại
        Scene currentScene = SceneManager.GetActiveScene();

        // Reload lại scene
        SceneManager.LoadScene(currentScene.name);
    }
    public bool canInput;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Reset();
        if (!canInput) return;
        // Kiểm tra đầu vào từ người chơi
        if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down)
        {
            _direction = Vector2.up; canInput = false;
        }
        else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up)
        {
            _direction = Vector2.down; canInput = false;
        }
        else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right)
        {
            _direction = Vector2.left; canInput = false;
        }
        else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left)
        {
            _direction = Vector2.right; canInput = false;
        }

    }
    public BoxCollider2D gridArea2D;
    void Move()
    {
        // Di chuyển thân rắn
        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }

        // Di chuyển đầu rắn
        float x = Mathf.Round(this.transform.position.x) + _direction.x;
        float y = Mathf.Round(this.transform.position.y) + _direction.y;
        this.transform.position = new Vector2(x, y);

        if (this.transform.position.x > gridArea2D.bounds.max.x)
        {
            this.transform.position = new Vector2(gridArea2D.bounds.min.x, this.transform.position.y);
        }
        if (this.transform.position.x < gridArea2D.bounds.min.x)
        {
            this.transform.position = new Vector2(gridArea2D.bounds.max.x, this.transform.position.y);
        }
        if (this.transform.position.y > gridArea2D.bounds.max.y)
        {
            this.transform.position = new Vector2(this.transform.position.x, gridArea2D.bounds.min.y);
        }
        if (this.transform.position.y < gridArea2D.bounds.min.y)
        {
            this.transform.position = new Vector2(this.transform.position.x, gridArea2D.bounds.max.y);
        }


        canInput = true;
    }

    void Grow()
    {
        Transform segment = Instantiate(this._segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position - (Vector3)_direction;
        _segments.Add(segment);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // Xử lý va chạm
        if (other.tag == "Food")
        {
            Grow();
        }
        else if (other.tag == "Wall" || other.tag == "Snake")
        {
            panel.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }
    public RectTransform panel;

}
