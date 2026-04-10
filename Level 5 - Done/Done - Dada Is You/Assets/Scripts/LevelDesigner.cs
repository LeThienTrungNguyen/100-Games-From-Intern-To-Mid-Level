using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GridRow
{
    public string[] cells;
}

[System.Serializable]
public class LevelData
{
    public string levelName = "Level 0";
    public int width = 5;
    public int height = 5;
    public GridRow[] gridData;
}

public class LevelDesigner : MonoBehaviour
{
    [HideInInspector]
    public List<LevelData> levels = new List<LevelData>();

    [HideInInspector]
    public int currentLevelIndex = 0;

    [Header("Autotile Settings")]
    [Tooltip("Bạn hãy tạo ScriptableObject File (Chuột Phải -> Create -> Dada -> Tile Map Data) rồi ném vào danh sách này.")]
    public List<TileDataMap> autotileRules = new List<TileDataMap>();

    private void OnValidate()
    {
        if (levels == null || levels.Count == 0)
        {
            levels = new List<LevelData>();
            levels.Add(new LevelData { levelName = "Level 0" });
        }
        if (currentLevelIndex < 0) currentLevelIndex = 0;
        if (currentLevelIndex >= levels.Count) currentLevelIndex = levels.Count - 1;
    }

    public LevelData CurrentLevel 
    {
        get {
            if (levels == null || levels.Count == 0) return null;
            if (currentLevelIndex < 0 || currentLevelIndex >= levels.Count) currentLevelIndex = 0;
            return levels[currentLevelIndex];
        }
    }

    public void AddNewLevel()
    {
        levels.Add(new LevelData { levelName = "Level " + levels.Count });
        currentLevelIndex = levels.Count - 1;
        ResizeGrid(5, 5); // Khởi tạo kích thước mặc định cho level mới
    }

    public void RemoveCurrentLevel()
    {
        if (levels.Count > 1)
        {
            levels.RemoveAt(currentLevelIndex);
            if (currentLevelIndex >= levels.Count) currentLevelIndex = levels.Count - 1;
        }
    }

    public void ResizeGrid(int newW, int newH)
    {
        if (newW < 1) newW = 1;
        if (newH < 1) newH = 1;

        LevelData lvl = CurrentLevel;
        if (lvl == null) return;

        GridRow[] newGrid = new GridRow[newH];
        for (int y = 0; y < newH; y++)
        {
            newGrid[y] = new GridRow { cells = new string[newW] };
            
            if (lvl.gridData != null && y < lvl.gridData.Length)
            {
                for (int x = 0; x < newW; x++)
                {
                    if (lvl.gridData[y].cells != null && x < lvl.gridData[y].cells.Length)
                    {
                        newGrid[y].cells[x] = lvl.gridData[y].cells[x];
                    }
                    else
                    {
                        newGrid[y].cells[x] = "null";
                    }
                }
            }
            else
            {
                // Init rỗng
                for (int x = 0; x < newW; x++)
                {
                    newGrid[y].cells[x] = "null";
                }
            }
        }
        lvl.gridData = newGrid;
        lvl.width = newW;
        lvl.height = newH;
    }

    public void SpawnLevel()
    {
        // Khởi tạo bộ màu mới cho màn chơi này
        ColorThemeManager.PrepareNewLevel();

        LevelData lvl = CurrentLevel;
        if (lvl == null || lvl.gridData == null) return;

        ClearLevel();

        GameObject container = new GameObject("Level_Generated");
        container.transform.SetParent(this.transform);

        float overrideCellSize = 1.0f;
        var gm = FindObjectOfType<GridManager>();
        if (gm != null)
        {
            overrideCellSize = gm.cellSize;
            // Báo cho GridManager biết giới hạn của bản đồ hiện tại để khóa viền
            gm.width = lvl.width;
            gm.height = lvl.height;
        }

        for (int y = 0; y < lvl.height; y++)
        {
            for (int x = 0; x < lvl.width; x++)
            {
                string cellData = lvl.gridData[y].cells[x];
                if (string.IsNullOrEmpty(cellData) || cellData.ToLower() == "null" || cellData == "[Rỗng]")
                    continue;

                // Cắt theo dấu | nếu tại 1 ô có khai báo chồng đè nhiều object. Ví dụ "water|baba"
                string[] objectsToSpawn = cellData.Split('|');

                foreach (string objNameRaw in objectsToSpawn)
                {
                    string objName = objNameRaw.Trim();
                    if (string.IsNullOrEmpty(objName) || objName.ToLower() == "null" || objName == "[Rỗng]") continue;

                    // Mặc định y = 0 là dòng đầu tiên, nên khi đổ toạ độ không gian thì row 0 sẽ có Y cao nhất
                    Vector2Int pos = new Vector2Int(x, lvl.height - 1 - y);

                    SpawnDadaObject(objName, pos, container.transform, overrideCellSize);
                }
            }
        }

        // Pass 2: Khớp nối đồ họa lưới chéo (Autotiling Bitmask Mode)
        if (autotileRules != null && autotileRules.Count > 0)
        {
            ApplyAutotileRules(lvl, container.transform);
        }
        
        // Sinh mảng đen viền bao phủ khu vực ngoài vùng Level
        GenerateBlackMask(lvl, overrideCellSize, container.transform);

        // Tự động căn chỉnh Camera tập trung vào Grid vừa sinh ra
        AdjustCamera(overrideCellSize);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    private void ApplyAutotileRules(LevelData lvl, Transform container)
    {
        DadaObject[] allSpawned = container.GetComponentsInChildren<DadaObject>();
        
        foreach (DadaObject dada in allSpawned)
        {
            // Tên gốc file (bỏ đuôi số nếu có) - Lưu ý ta vừa spawn từ Tên chữ cái chuẩn ở Grid
            string objName = dada.gameObject.name; 
            if (objName.Contains("(Clone)")) objName = objName.Replace("(Clone)", "").Trim();

            // Xem trong thùng luật lệ Autotile có File Data nào tương thích với tên của Cục này không?
            TileDataMap matchData = null;
            foreach (var rule in autotileRules)
            {
                if (rule != null && objName.StartsWith(rule.targetObjectName))
                {
                    matchData = rule;
                    break;
                }
            }

            // Nếu không có Luật nào tương thích, hoặc mảng rỗng thì bỏ qua
            if (matchData == null || matchData.states == null || matchData.states.Length < 16) continue;

            // ---- TÍNH TỔNG QUAN HỆ KHÔNG GIAN ----
            int bitmask = 0;
            int cx = dada.gridPos.x;
            int cy = dada.gridPos.y;

            // Nhớ rằng DadaObject GridPos luôn có gốc Y=0 ở dưới cùng, Y tăng dần lên trên!
            // Tuy nhiên trong `lvl.gridData` thì index `y = 0` lại nằm CÙNG BÊN TRÊN CÙNG của màn hình (Mảng Data chạy từ trên xuống).
            // Công thức quy chiếu: gridData_Y = lvl.height - 1 - DadaObject_Y 

            int gridY = lvl.height - 1 - cy;
            int gridX = cx;

            // Check UP (+1). Xét gridY - 1 vì mảng lật ngược
            if (gridY - 1 >= 0 && CheckCellHasMatch(lvl, gridX, gridY - 1, matchData)) bitmask += 1;
            
            // Check RIGHT (+2).
            if (gridX + 1 < lvl.width && CheckCellHasMatch(lvl, gridX + 1, gridY, matchData)) bitmask += 2;
            
            // Check DOWN (+4).
            if (gridY + 1 < lvl.height && CheckCellHasMatch(lvl, gridX, gridY + 1, matchData)) bitmask += 4;
            
            // Check LEFT (+8).
            if (gridX - 1 >= 0 && CheckCellHasMatch(lvl, gridX - 1, gridY, matchData)) bitmask += 8;

            // Áp file Ảnh theo Bitmask tìm được
            if (matchData.states[bitmask].animationFrames != null && matchData.states[bitmask].animationFrames.Length > 0)
            {
                dada.SetCustomAnimation(matchData.states[bitmask].animationFrames);
            }
        }
    }

    private bool CheckCellHasMatch(LevelData lvl, int x, int y, TileDataMap matchData)
    {
        string rawCell = lvl.gridData[y].cells[x];
        if (string.IsNullOrEmpty(rawCell)) return false;

        string[] stacked = rawCell.Split('|');
        foreach (string obj in stacked)
        {
            if (matchData.CanConnectTo(obj.Trim())) return true;
        }
        return false;
    }

    private void GenerateBlackMask(LevelData lvl, float cellSize, Transform parent)
    {
        float w = lvl.width * cellSize;
        float h = lvl.height * cellSize;
        float halfW = w / 2f;
        float halfH = h / 2f;
        float centerX = (lvl.width - 1) * cellSize / 2f;
        float centerY = (lvl.height - 1) * cellSize / 2f;

        float thick = 1000f; // Bức tường màn đêm siêu dày
        
        Material blackMat = new Material(Shader.Find("Sprites/Default"));
        blackMat.color = new Color(0.05f, 0.05f, 0.05f, 0.85f); // Màu đen tuyền mờ mờ bụi bặm (Abyss Effect)

        CreateMaskQuad("Abyss_Left", new Vector3(centerX - halfW - thick / 2f, centerY, 0), new Vector2(thick, h + thick * 2), blackMat, parent);
        CreateMaskQuad("Abyss_Right", new Vector3(centerX + halfW + thick / 2f, centerY, 0), new Vector2(thick, h + thick * 2), blackMat, parent);
        CreateMaskQuad("Abyss_Bottom", new Vector3(centerX, centerY - halfH - thick / 2f, 0), new Vector2(w, thick), blackMat, parent);
        CreateMaskQuad("Abyss_Top", new Vector3(centerX, centerY + halfH + thick / 2f, 0), new Vector2(w, thick), blackMat, parent);
    }

    private void CreateMaskQuad(string n, Vector3 pos, Vector2 scale, Material mat, Transform parent)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = n;
        go.transform.SetParent(parent);
        go.transform.position = new Vector3(pos.x, pos.y, 5f); // Nằm chìm ở phía sâu Z=5 như phần Nền
        go.transform.localScale = new Vector3(scale.x, scale.y, 1);
        
        // Quad mặc định có gắn collider 3D, trong game 2D này chúng ta không cần nó
        DestroyImmediate(go.GetComponent<MeshCollider>());

        var mr = go.GetComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
    }

    private void AdjustCamera(float overrideCellSize)
    {
        LevelData lvl = CurrentLevel;
        if (lvl == null) return;

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("Không tìm thấy Main Camera (có gắn tag MainCamera) để tự động căn chỉnh.");
            return;
        }

        // Tọa độ trung tâm: Từ 0 đến width-1
        float centerX = (lvl.width - 1) * overrideCellSize / 2f;
        float centerY = (lvl.height - 1) * overrideCellSize / 2f;

        cam.transform.position = new Vector3(centerX, centerY, -10f); // -10 mặt định lùi ra sau
        cam.orthographic = true; // Chắc chắn game ta là 2D

        // Tính kích thước yêu cầu với một tí viền bao xung quanh (Padding)
        float padding = overrideCellSize * 1.5f; 
        
        float requiredHeight = (lvl.height * overrideCellSize) + padding;
        float requiredWidth  = (lvl.width * overrideCellSize)  + padding;

        float orthoHeight = requiredHeight / 2f;
        
        // Nếu màn hình không phải hình vuông chuẩn mà bị méo/chữ nhật, phải nhân aspect để bù
        float orthoWidth  = (requiredWidth / 2f) / cam.aspect;

        cam.orthographicSize = Mathf.Max(orthoHeight, orthoWidth);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(cam);
#endif
    }

    private void SpawnDadaObject(string originalName, Vector2Int gridPos, Transform parent, float cellSize)
    {
        GameObject go = new GameObject(originalName);
        go.transform.SetParent(parent);
        
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        DadaObject dada = go.AddComponent<DadaObject>();
        dada.gridPos = gridPos;

        // Đăng ký tức thời ngầm xuống GridManager ở Runtime để RuleManager có thể quét ngay lập tức
        if (Application.isPlaying && GridManager.Instance != null)
        {
            GridManager.Instance.AddObject(dada, gridPos);
        }

#if UNITY_EDITOR
        // Tái sử dụng logic AutoSetup thần thánh đã viết
        dada.AutoSetup();
#endif

        // Tính toán lại vị trí ngay trên Scene để trực quan xem trước
        var posTr = go.transform.position;
        posTr.x = gridPos.x * cellSize;
        posTr.y = gridPos.y * cellSize;
        go.transform.position = posTr;
    }

    public void ClearLevel()
    {
        // Xóa hoàn toàn các objects con đang rác hoặc Generated_Level từ trước để đập đi xây lại
        var parent = transform.Find("Level_Generated");
        if (parent != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(parent.gameObject);
#else
            Destroy(parent.gameObject);
#endif
        }

        // CỰC KỲ QUAN TRỌNG: Quét sạch bộ nhớ đệm GridManager để tránh lỗi `MissingReference` 
        // lúc Update game cố gắng thao tác với các DadaObject cũ đã bị Destroy ở trên
        var gm = FindObjectOfType<GridManager>();
        if (gm != null)
        {
            gm.ClearGrid();
        }
    }
}
