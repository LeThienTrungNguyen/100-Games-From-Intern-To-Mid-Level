using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Transform[] walls;
    public Transform[] boxes;
    public Transform[] targets;
    public Transform player;

    public struct GridCell
    {
        public string block1; // wall, target, null
        public string block2; // box, player, null
    }

    // Grid map: key = vị trí (x,y), value = GridCell
    public Dictionary<Vector2Int, GridCell> grid = new Dictionary<Vector2Int, GridCell>();

    void Awake()
    {
        grid.Clear();

        // Wall
        foreach (var w in walls)
        {
            Vector2Int pos = Vector2Int.RoundToInt(w.position);
            grid[pos] = new GridCell { block1 = "wall", block2 = "wall" };
        }

        // Target
        foreach (var t in targets)
        {
            Vector2Int pos = Vector2Int.RoundToInt(t.position);
            grid[pos] = new GridCell { block1 = "target", block2 = null };
        }

        // Box
        foreach (var b in boxes)
        {
            Vector2Int pos = Vector2Int.RoundToInt(b.position);
            if (grid.ContainsKey(pos))
            {
                var cell = grid[pos];
                cell.block2 = "box";
                grid[pos] = cell;
            }
            else
            {
                grid[pos] = new GridCell { block1 = null, block2 = "box" };
            }
        }

        // Player
        if (player != null)
        {
            Vector2Int pos = Vector2Int.RoundToInt(player.position);
            if (grid.ContainsKey(pos))
            {
                var cell = grid[pos];
                cell.block2 = "player";
                grid[pos] = cell;
            }
            else
            {
                grid[pos] = new GridCell { block1 = null, block2 = "player" };
            }
        }

        Debug.Log("Grid Initialized with " + grid.Count + " cells");
    }

    // Check win dựa trên block1 & block2
    public void CheckWin()
    {
        foreach (var cell in grid.Values)
        {
            if (cell.block1 == "target")
            {
                if (cell.block2 != "box")
                    return; // target chưa được box lấp đầy
            }
        }

        OnWin();
    }

    private void OnWin()
    {
        // Lấy index scene hiện tại
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        // Nếu chưa phải scene cuối, load scene tiếp theo
        if (currentSceneIndex < totalScenes - 1)
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        else
        {
            // Scene cuối, thoát game
            Debug.Log("🏁 Last scene completed. Quitting game...");
            Application.Quit();

            // Nếu chạy trong editor, để test
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
    

}
