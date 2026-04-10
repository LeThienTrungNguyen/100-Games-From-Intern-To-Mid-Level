using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Movement : MonoBehaviour
{
    void Start()
    {
        // Initial rule update
        Invoke("DelayedStart", 0.1f);
    }

    void DelayedStart()
    {
        RuleManager.Instance.UpdateRules();
    }

    void Update()
    {
        Vector2Int moveDir = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) moveDir = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) moveDir = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) moveDir = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) moveDir = Vector2Int.right;

        if (moveDir != Vector2Int.zero)
        {
            ProcessTurn(moveDir);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            UndoManager.Instance.Undo();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Restart Level (could just be many undos or scene reload)
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    void ProcessTurn(Vector2Int direction)
    {
        // 1. Snapshot for Undo
        UndoManager.Instance.CaptureState();

        // 2. Identify all 'YOU' objects
        var allObjects = GridManager.Instance.GetAllObjects().ToList();
        var players = allObjects.Where(o => o.isYou).ToList();

        bool anyMoved = false;

        foreach (var player in players)
        {
            Vector2Int targetPos = player.gridPos + direction;

            if (GridManager.Instance.CanMoveTo(targetPos, (Vector2)direction))
            {
                // Push objects first
                GridManager.Instance.PushObjects(targetPos, (Vector2)direction);
                
                // Move player
                player.MoveTo(targetPos);
                anyMoved = true;
            }
        }

        // 3. Luôn cập nhật Luật và Kiểm tra tương tác sau mỗi lượt bấm (dù có di chuyển hay không)
        // Điều này đảm bảo nếu bạn đang đứng trên Flag mà bấm một phím bị chặn, bạn vẫn Win.
        RuleManager.Instance.UpdateRules();
        CheckInteractions();
    }

    void CheckInteractions()
    {
        var allObjects = GridManager.Instance.GetAllObjects().ToList();

        // List to track objects to destroy
        List<DadaObject> toDestroy = new List<DadaObject>();

        foreach (var obj in allObjects)
        {
            if (!obj.gameObject.activeSelf) continue;

            var othersAtPos = GridManager.Instance.GetObjectsAt(obj.gridPos).Where(o => o != obj && o.gameObject.activeSelf).ToList();

            foreach (var other in othersAtPos)
            {
                // WIN condition: A 'YOU' object is on the same cell as a 'WIN' object
                // OR: A 'YOU' object is itself 'WIN'
                bool isDirectWin = obj.isYou && obj.isWin;
                bool isOverlapWin = obj.isYou && othersAtPos.Any(o => o.isWin);

                if (isDirectWin || isOverlapWin)
                {
                    Debug.Log($"[WIN] Win condition met by {obj.objectType} at {obj.gridPos}");
                    
                    LevelDesigner ld = FindObjectOfType<LevelDesigner>();
                    if (ld != null)
                    {
                        if (ld.currentLevelIndex < ld.levels.Count - 1)
                        {
                            ld.currentLevelIndex++;
                            ld.SpawnLevel();
                            
                            // Xóa toàn bộ lịch sử đi lùi và cập nhật lại bộ luật để sẵn sàng cho level mới
                            if (UndoManager.Instance != null) UndoManager.Instance.ClearHistory(); 
                            if (RuleManager.Instance != null) RuleManager.Instance.UpdateRules();
                            
                            return; 
                        }
                        else
                        {
                            Debug.Log("<color=cyan>[WIN] Congratulations! You have cleared ALL levels!</color>");
                            
#if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
#else
                            Application.Quit();
#endif
                            return;
                        }
                    }
                    else
                    {
                        Debug.LogError("[WIN] Win detected but LevelDesigner was not found in the scene.");
                    }
                }

                // DEFEAT condition
                if (obj.isYou && other.isDefeat)
                {
                    toDestroy.Add(obj);
                }

                // SINK condition
                if (obj.isSink || other.isSink)
                {
                    toDestroy.Add(obj);
                    toDestroy.Add(other);
                }

                // HOT/MELT interaction
                if (obj.isMelt && other.isHot)
                {
                    toDestroy.Add(obj);
                }
                if (obj.isHot && other.isMelt)
                {
                    toDestroy.Add(other);
                }
            }
        }

        foreach (var obj in toDestroy.Distinct())
        {
            GridManager.Instance.RemoveObject(obj, obj.gridPos);
            obj.gameObject.SetActive(false); // We use SetActive(false) so Undo can bring them back
        }
    }
}
