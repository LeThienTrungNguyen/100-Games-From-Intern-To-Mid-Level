using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        Vector2Int dir = Vector2Int.zero;

        // Di chuyển
        if (Input.GetKeyDown(KeyCode.A)) dir = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D)) dir = Vector2Int.right;
        else if (Input.GetKeyDown(KeyCode.W)) dir = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector2Int.down;

        if (dir != Vector2Int.zero)
            TryMove(dir);

        // Reset scene hiện tại
        if (Input.GetKeyDown(KeyCode.R))
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }

        // Quay về scene đầu tiên
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(0);
        }
    }


    void TryMove(Vector2Int dir)
    {
        Vector2Int curPos = Vector2Int.RoundToInt(transform.position);
        Vector2Int nextPos = curPos + dir;

        var grid = gameController.grid;

        // Nếu next ô có vật thể trên nền
        if (grid.ContainsKey(nextPos))
        {
            var cell = grid[nextPos];

            if (cell.block2 == "box")
            {
                // thử push box
                Vector2Int afterPos = nextPos + dir;

                if (!grid.ContainsKey(afterPos))
                {
                    grid[afterPos] = new GameController.GridCell { block1 = null, block2 = "box" };

                    // Update transform box
                    Transform boxTransform = FindBoxAt(nextPos);
                    if (boxTransform != null)
                        boxTransform.position = (Vector2)afterPos;

                    MovePlayer(curPos, nextPos);
                    grid[nextPos] = new GameController.GridCell { block1 = cell.block1, block2 = null };
                    gameController.CheckWin();
                    return;
                }

                var afterCell = grid[afterPos];

                // Cho phép push nếu afterCell.block2 == null hoặc afterCell.block1 == target
                if (afterCell.block2 == null || afterCell.block1 == "target")
                {
                    // Update transform box
                    Transform boxTransform = FindBoxAt(nextPos);
                    if (boxTransform != null)
                        boxTransform.position = (Vector2)afterPos;

                    // Update box
                    afterCell.block2 = "box";
                    grid[afterPos] = afterCell;

                    // Update box cũ
                    cell.block2 = null;
                    grid[nextPos] = cell;

                    // Move player
                    MovePlayer(curPos, nextPos);

                    // Check win
                    gameController.CheckWin();
                }

                // nếu bị chặn thì return
                return;
            }
            else if (cell.block2 == "player" || cell.block1 == "wall")
            {
                // blocked
                return;
            }
            else
            {
                // trống hoặc target
                MovePlayer(curPos, nextPos);
            }
        }
        else
        {
            // ô trống hoàn toàn
            MovePlayer(curPos, nextPos);
        }
    }

    // Hàm tìm box ở vị trí grid
    Transform FindBoxAt(Vector2Int pos)
    {
        foreach (var box in gameController.boxes)
        {
            if (Vector2Int.RoundToInt(box.position) == pos)
                return box;
        }
        return null;
    }


    void MovePlayer(Vector2Int curPos, Vector2Int nextPos)
    {
        var grid = gameController.grid;

        // Update grid
        if (grid.ContainsKey(curPos))
        {
            var curCell = grid[curPos];
            curCell.block2 = null;
            grid[curPos] = curCell;
        }

        if (grid.ContainsKey(nextPos))
        {
            var nextCell = grid[nextPos];
            nextCell.block2 = "player";
            grid[nextPos] = nextCell;
        }
        else
        {
            grid[nextPos] = new GameController.GridCell { block1 = null, block2 = "player" };
        }

        // Update world position
        transform.position = (Vector2)nextPos;
    }

}
