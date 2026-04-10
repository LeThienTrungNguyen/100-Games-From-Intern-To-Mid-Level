using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MazeGenerator : MonoBehaviour
{
    public int width = 21;
    public int height = 21;
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject entrancePrefab;
    public GameObject exitPrefab;
    public GameObject playerPrefab; // ⬅️ Thêm player prefab

    private int[,] maze;

    void Start()
    {
        maze = GenerateMaze(width, height);
        DrawMaze(maze);
    }
    void Update()
    {
        if (win && Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }
    int[,] GenerateMaze(int w, int h)
    {
        int[,] maze = new int[w, h];

        // Fill walls
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                maze[x, y] = 1;

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int start = new Vector2Int(1, 1);
        maze[start.x, start.y] = 0;
        stack.Push(start);

        System.Random rand = new System.Random();

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = new List<Vector2Int>();

            // 4 hướng
            Vector2Int[] dirs = {
                new Vector2Int(2,0),
                new Vector2Int(-2,0),
                new Vector2Int(0,2),
                new Vector2Int(0,-2)
            };

            foreach (var dir in dirs)
            {
                Vector2Int next = current + dir;
                if (next.x > 0 && next.y > 0 && next.x < w - 1 && next.y < h - 1)
                {
                    if (maze[next.x, next.y] == 1)
                    {
                        neighbors.Add(next);
                    }
                }
            }

            if (neighbors.Count > 0)
            {
                Vector2Int chosen = neighbors[rand.Next(neighbors.Count)];
                Vector2Int wall = (current + chosen) / 2;
                maze[chosen.x, chosen.y] = 0;
                maze[wall.x, wall.y] = 0;
                stack.Push(chosen);
            }
            else
            {
                stack.Pop();
            }
        }

        return maze;
    }

    void DrawMaze(int[,] maze)
    {
        int w = maze.GetLength(0);
        int h = maze.GetLength(1);

        // Chọn entrance và exit trước
        Vector2Int entrance = GetRandomEdgeCell(maze, true);
        Vector2Int exit = GetRandomEdgeCell(maze, false);

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                Vector3 pos = new Vector3(x, y, 0);

                // Nếu là entrance/exit thì không spawn wall/floor
                if ((x == entrance.x && y == entrance.y) ||
                    (x == exit.x && y == exit.y))
                    continue;

                if (maze[x, y] == 1)
                    Instantiate(wallPrefab, pos, Quaternion.identity, transform);
                else
                    Instantiate(floorPrefab, pos, Quaternion.identity, transform);
            }
        }

        // Spawn entrance, exit
        Instantiate(entrancePrefab, new Vector3(entrance.x, entrance.y, 0), Quaternion.identity, transform);
        Instantiate(exitPrefab, new Vector3(exit.x, exit.y, 0), Quaternion.identity, transform);

        // ⬅️ Spawn player tại entrance
        Instantiate(playerPrefab, new Vector3(entrance.x, entrance.y, 0), Quaternion.identity);
    }

    Vector2Int GetRandomEdgeCell(int[,] maze, bool isEntrance)
    {
        int w = maze.GetLength(0);
        int h = maze.GetLength(1);
        System.Random rand = new System.Random();

        List<Vector2Int> edges = new List<Vector2Int>();

        // top & bottom
        for (int x = 1; x < w - 1; x++)
        {
            if (maze[x, 1] == 0) edges.Add(new Vector2Int(x, 0));
            if (maze[x, h - 2] == 0) edges.Add(new Vector2Int(x, h - 1));
        }

        // left & right
        for (int y = 1; y < h - 1; y++)
        {
            if (maze[1, y] == 0) edges.Add(new Vector2Int(0, y));
            if (maze[w - 2, y] == 0) edges.Add(new Vector2Int(w - 1, y));
        }

        if (edges.Count == 0)
            return new Vector2Int(1, 1); // fallback

        return edges[rand.Next(edges.Count)];
    }

    public RectTransform panel;
    public void Win()
    {
        panel.gameObject.SetActive(true);
    }

    public void Restart()
    {
        // Lấy tên scene hiện tại rồi load lại
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
        panel.gameObject.SetActive(false);
    }

    

    public bool win = false;
}
