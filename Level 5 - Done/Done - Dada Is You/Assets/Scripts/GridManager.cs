using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public float cellSize = 1.0f;
    public int width = 0;
    public int height = 0;

    private Dictionary<Vector2Int, List<DadaObject>> grid = new Dictionary<Vector2Int, List<DadaObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool IsOutOfBounds(Vector2Int pos)
    {
        // Nếu width hoặc height = 0 tức là Grid vô hạn (dành cho tự test)
        if (width <= 0 || height <= 0) return false;
        
        return pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height;
    }

    public Vector2Int GetGridPos(Vector3 worldPos)
    {
        float size = Mathf.Max(0.01f, cellSize);
        return new Vector2Int(Mathf.RoundToInt(worldPos.x / size), Mathf.RoundToInt(worldPos.y / size));
    }

    public Vector3 GetWorldPos(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0);
    }

    public void AddObject(DadaObject obj, Vector2Int pos)
    {
        if (!grid.ContainsKey(pos))
        {
            grid[pos] = new List<DadaObject>();
        }
        if (!grid[pos].Contains(obj))
        {
            grid[pos].Add(obj);
        }
    }

    public void RemoveObject(DadaObject obj, Vector2Int pos)
    {
        if (grid.ContainsKey(pos))
        {
            grid[pos].Remove(obj);
        }
    }

    public void MoveObject(DadaObject obj, Vector2Int oldPos, Vector2Int newPos)
    {
        RemoveObject(obj, oldPos);
        AddObject(obj, newPos);
    }

    public List<DadaObject> GetObjectsAt(Vector2Int pos)
    {
        if (grid.ContainsKey(pos))
            return grid[pos];
        return new List<DadaObject>();
    }

    public bool IsCellStopped(Vector2Int pos)
    {
        if (IsOutOfBounds(pos)) return true;

        var objects = GetObjectsAt(pos);
        foreach (var obj in objects)
        {
            if (obj.isStop) return true;
        }
        return false;
    }

    // Logic for recursive pushing
    public bool CanMoveTo(Vector2Int pos, Vector2 direction)
    {
        // Kiểm tra đi ra ngoài ma trận lưới
        if (IsOutOfBounds(pos)) return false;

        var objects = GetObjectsAt(pos);
        
        bool blocked = false;
        bool pushableFound = false;

        foreach (var obj in objects)
        {
            if (obj.isStop) return false;
            if (obj.isPush) pushableFound = true;
        }

        if (pushableFound)
        {
            Vector2Int nextPos = pos + Vector2Int.RoundToInt(direction);
            return CanMoveTo(nextPos, direction);
        }

        return true;
    }

    public void PushObjects(Vector2Int pos, Vector2 direction)
    {
        var objects = GetObjectsAt(pos);
        List<DadaObject> pushables = new List<DadaObject>();

        foreach (var obj in objects)
        {
            if (obj.isPush) pushables.Add(obj);
        }

        if (pushables.Count > 0)
        {
            Vector2Int nextPos = pos + Vector2Int.RoundToInt(direction);
            // Push objects at next pos first (recursion)
            PushObjects(nextPos, direction);

            // Now move these objects
            foreach (var p in pushables)
            {
                p.MoveTo(nextPos);
            }
        }
    }

    public IEnumerable<DadaObject> GetAllObjects()
    {
        foreach (var list in grid.Values)
        {
            foreach (var obj in list)
            {
                yield return obj;
            }
        }
    }

    public void ClearGrid()
    {
        grid.Clear();
    }
}
