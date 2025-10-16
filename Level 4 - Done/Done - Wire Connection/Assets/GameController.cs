using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("References")]
    public GameObject linePrefab;

    [Header("Runtime")]
    public Vector2Int mousePosGrid;
    public bool isPathCreating;
    public List<Path> paths = new List<Path>();
    [SerializeField] private Path currentPath;
    public Transform startPathPoint;

    void Awake() => paths = new List<Path>();

    void Update()
    {
        mousePosGrid = GetMousePosGrid();

        if (Input.GetMouseButtonDown(0))
            TryStartPath();

        if (Input.GetMouseButton(0) && isPathCreating && currentPath != null)
            UpdateCreatingPath();

        if (Input.GetMouseButtonUp(0))
        {
            // nếu thả chuột mà path chỉ có 1 line thì xóa path
            if (currentPath != null && currentPath.points.Count == 1)
            {
                Destroy(currentPath.points[0].gameObject);
                paths.Remove(currentPath);
                currentPath = null;
            }
            FinishCreatePath();

        }
    }

    // =========================
    // 🔹 1. Xử lý bắt đầu tạo Path
    // =========================
    void TryStartPath()
    {
        Transform hitDot = GetDotUnderMouse();
        if (hitDot == null) return;

        Path existingPath = GetPathByStartPoint(hitDot);
        if (existingPath != null)
        {
            // Xóa toàn bộ path cũ (bao gồm line object trong scene)
            foreach (var point in existingPath.points)
            {
                if (point != null)
                    Destroy(point.gameObject);
            }
            paths.Remove(existingPath);
            Debug.Log("Đã xóa path cũ tại dot: " + hitDot.name);
        }

        // Tạo path mới
        currentPath = new Path { color = GetColorByDot(hitDot) };
        currentPath.startPoint = hitDot;
        paths.Add(currentPath);
        isPathCreating = true;
        startPathPoint = currentPath.startPoint;

        Debug.Log("Tạo path mới tại: " + mousePosGrid);
    }


    // =========================
    // 🔹 2. Xử lý khi đang giữ chuột để vẽ Path
    // =========================
    void UpdateCreatingPath()
    {
        Transform hitDot = GetDotUnderMouse();

        // Nếu đang đi qua dot khác màu -> chặn line
        if (hitDot != null && hitDot != startPathPoint)
        {
            // Nếu dot không cùng màu -> huỷ path
            if (!IsSameColor(hitDot, startPathPoint))
            {
                Debug.Log("Chạm dot khác màu, huỷ path!");
                FinishCreatePath();
                return;
            }
        }

        // Nếu chưa có line ở vị trí grid thì vẽ thêm
        if (!IsGridInAnyPath(mousePosGrid))
        {
            CreateLineAt(mousePosGrid);
        }

        // Nếu đến đúng dot cùng màu -> kết thúc path
        if (CanFinishPath(hitDot))
        {
            currentPath.endPoint = hitDot;
            currentPath.isComplete = currentPath.IsComplete();
            FinishCreatePath();

        }
    }


    // =========================
    // 🔹 3. Hoàn tất Path
    // =========================
    void FinishCreatePath()
    {
        isPathCreating = false;
        currentPath = null;
        startPathPoint = null;
    }

    // =========================
    // 🔹 4. Tạo line và cập nhật Path
    // =========================
    void CreateLineAt(Vector2Int gridPos)
    {
        var lineObj = Instantiate(linePrefab, (Vector2)gridPos, Quaternion.identity);
        currentPath.AddPoint(lineObj.transform);
        RotatePath(currentPath);
        ColorPath(currentPath);
    }

    // =========================
    // 🔹 5. Kiểm tra điều kiện kết thúc path
    // =========================
    bool CanFinishPath(Transform hitDot)
    {
        if (hitDot == null || !hitDot.CompareTag("Dot")) return false;
        if (hitDot == startPathPoint) return false;
        if (IsTransformInAnyPath(hitDot)) return false;
        if (!IsSameColor(hitDot, startPathPoint)) return false;
        return true;
    }

    // =========================
    // 🔹 6. Các hàm xử lý line
    // =========================
    void RotatePath(Path path)
    {
        if (path.points.Count == 0) return;

        var srStart = path.points[0].Find("Child0").GetComponentInChildren<SpriteRenderer>();
        var srEnd = path.points[path.points.Count - 1].Find("Child1").GetComponentInChildren<SpriteRenderer>();

        // Line giữa
        for (int i = 1; i < path.points.Count - 1; i++)
        {
            var cur = path.points[i];
            var prev = path.points[i - 1];
            var next = path.points[i + 1];

            RotateChild(cur.Find("Child0"), cur.position, prev.position);
            RotateChild(cur.Find("Child1"), cur.position, next.position);
            SetAlpha(cur.Find("Child0"), 1f);
            SetAlpha(cur.Find("Child1"), 1f);
        }

        // Nếu chỉ có 1 line
        if (path.points.Count < 2)
        {
            SetAlpha(path.points[0].Find("Child0"), 0f);
            SetAlpha(path.points[0].Find("Child1"), 0f);
            return;
        }

        // Đầu và cuối
        SetAlpha(srStart, 0f);
        SetAlpha(srEnd, 0f);

        LookAt2D(path.points[0].Find("Child1"), path.points[0].position, path.points[1].position);
        LookAt2D(path.points[^1].Find("Child0"), path.points[^1].position, path.points[^2].position);
    }

    void ColorPath(Path path)
    {
        if (path.points.Count == 0) return;

        for (int i = 0; i < path.points.Count; i++)
        {
            var point = path.points[i];
            var color = path.color;

            point.Find("Child0").GetComponentInChildren<SpriteRenderer>().color = color;
            point.Find("Child1").GetComponentInChildren<SpriteRenderer>().color = color;
            point.Find("Dot Center").GetComponent<SpriteRenderer>().color = color;
        }

        // Clear hai đầu
        path.points[0].Find("Child0").GetComponentInChildren<SpriteRenderer>().color = Color.clear;
        path.points[^1].Find("Child1").GetComponentInChildren<SpriteRenderer>().color = Color.clear;
    }

    // =========================
    // 🔹 7. Helper functions
    // =========================
    void RotateChild(Transform child, Vector3 from, Vector3 to)
        => LookAt2D(child, from, to);

    void SetAlpha(Transform target, float alpha)
    {
        var sr = target.GetComponentInChildren<SpriteRenderer>();
        var c = sr.color;
        sr.color = new Color(c.r, c.g, c.b, alpha);
    }

    void SetAlpha(SpriteRenderer sr, float alpha)
    {
        var c = sr.color;
        sr.color = new Color(c.r, c.g, c.b, alpha);
    }

    Transform GetDotUnderMouse()
    {
        Vector2 worldPos = new Vector2(mousePosGrid.x + 0.5f, mousePosGrid.y + 0.5f);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        return (hit != null && hit.CompareTag("Dot")) ? hit.transform : null;
    }

    bool IsSameColor(Transform a, Transform b)
    {
        var cA = a.GetComponent<SpriteRenderer>().color;
        var cB = b.GetComponent<SpriteRenderer>().color;
        return cA.Equals(cB);
    }

    bool IsTransformInAnyPath(Transform dot)
    {
        foreach (var path in paths)
            if (path.points.Contains(dot))
                return true;
        return false;
    }

    bool IsGridInAnyPath(Vector2 point)
    {
        foreach (var path in paths)
            foreach (var p in path.points)
                if (p.position.x == point.x && p.position.y == point.y)
                    return true;
        return false;
    }

    void LookAt2D(Transform target, Vector3 fromPos, Vector3 toPos)
    {
        Vector3 dir = toPos - fromPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        target.rotation = Quaternion.Euler(0, 0, angle);
    }

    Vector2Int GetMousePosGrid()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2Int(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));
    }

    Path GetPathByTransform(Transform t)
    {
        foreach (var path in paths)
            if (path.points.Contains(t))
                return path;
        return null;
    }
    Path GetPathByStartPoint(Transform dot)
    {
        foreach (var path in paths)
        {
            if (path.startPoint == dot)
                return path;
        }
        return null;
    }

    Color GetColorByDot(Transform dot)
    {
        return dot.GetComponent<SpriteRenderer>()?.color ?? Color.clear;
    }

    bool IsLocationHasTransform(Vector2Int location) {
        // dùng foreach
        foreach (var path in paths)
        {
            foreach (var point in path.points)
            { 
                if((int)point.position.x == location.x && (int)point.position.y == location.y)
                    return true;
            }
        }
        return false;
    }
    [SerializeField]bool isGameComplete = true;
    void FixedUpdate()
    {
        isGameComplete = true;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (!IsLocationHasTransform(new Vector2Int(i, j)))
                {
                    isGameComplete = false;
                    Debug.Log($"Vị trí ({i},{j}) chưa có line");
                }
                else
                {
                    Debug.Log($"Vị trí ({i},{j}) đã có line");
                }
            }
        }
        if (isGameComplete)
        { 
            Debug.Log("Game Complete!");
        }
    }
}

// =========================
// 🔹 Class Path
// =========================
[System.Serializable]
public class Path
{
    public Color color;
    public List<Transform> points = new List<Transform>();
    public Transform startPoint;
    public Transform endPoint;// khi end có giá trị thì Check isComplete = IsComplete();

    public bool isComplete;
    public void AddPoint(Transform point) => points.Add(point);
    public bool IsComplete()
    {
        // trả về nếu start và end không null và cùng màu
        return startPoint != null && endPoint != null && startPoint.GetComponent<SpriteRenderer>().color == endPoint.GetComponent<SpriteRenderer>().color;
    }
}
