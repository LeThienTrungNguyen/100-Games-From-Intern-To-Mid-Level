using UnityEngine;
using System.Collections.Generic;

public class DadaObject : MonoBehaviour
{
    public ObjectType objectType;
    public Vector2Int gridPos;

    [Header("Current Properties")]
    public bool isYou;
    public bool isPush;
    public bool isStop;
    public bool isWin;
    public bool isSink;
    public bool isDefeat;
    public bool isHot;
    public bool isMelt;

    // Word properties (only relevant if this is a text object)
    public WordType wordType;
    public ObjectType representsType; // For nouns, which type it refers to
    public Property representsProperty; // For properties, which property it gives

    [Header("Animation")]
    public Sprite[] animationFrames;
    public float animationSpeed = 0.2f;
    private int currentFrameIndex = 0;
    private float animationTimer = 0f;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        // Áp dụng màu sắc cho màn chơi hiện tại
        ApplyLevelColor();
        // Ensure the visual position matches the defined gridPos at the start
        UpdateVisualPosition();
    }

    private void Update()
    {
        // Simple Sprite Animation System (Idle)
        if (animationFrames != null && animationFrames.Length > 1)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationSpeed)
            {
                animationTimer -= animationSpeed;
                currentFrameIndex = (currentFrameIndex + 1) % animationFrames.Length;
                
                if (sr != null) sr.sprite = animationFrames[currentFrameIndex];
            }
        }
    }

    /// <summary>
    /// Hàm dùng cho LevelDesigner ép ghi đè Frame.
    /// Ví dụ: Khi Autotile Wall phát hiện ra nó nối với 3 mảnh khác, nó gọi cắm nguyên băng đĩa phim [Ngã 3] vào hàm này.
    /// </summary>
    public void SetCustomAnimation(Sprite[] newFrames)
    {
        if (newFrames != null && newFrames.Length > 0)
        {
            animationFrames = newFrames;
            currentFrameIndex = 0;
            if (sr == null) sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = animationFrames[0];
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            if (sr != null) UnityEditor.EditorUtility.SetDirty(sr);
#endif
        }
    }

    private void Start()
    {
        // Register the object to the grid at its current gridPos
        GridManager.Instance.AddObject(this, gridPos);
    }

    private void OnValidate()
    {
        // When you change 'gridPos' in the Inspector, the object's transform 
        // will immediately move to the correct world position.
        GridManager gm = FindObjectOfType<GridManager>();
        if (gm != null)
        {
            transform.position = gm.GetWorldPos(gridPos);
        }
    }

    private void Reset()
    {
#if UNITY_EDITOR
        AutoSetup();
#endif
    }

    public void MoveTo(Vector2Int newPos)
    {
        GridManager.Instance.MoveObject(this, gridPos, newPos);
        gridPos = newPos;
        UpdateVisualPosition();
    }

    public void UpdateVisualPosition()
    {
        if (GridManager.Instance != null)
        {
            transform.position = GridManager.Instance.GetWorldPos(gridPos);
        }
    }

    public void ResetProperties()
    {
        isYou = false;
        isPush = false;
        isStop = false;
        isWin = false;
        isSink = false;
        isDefeat = false;
        isHot = false;
        isMelt = false;

        // Text blocks are ALWAYS pushable by default in Baba Is You
        if (IsText())
        {
            isPush = true;
        }
    }

    public bool IsText()
    {
        return objectType.ToString().StartsWith("TEXT_");
    }

    public void ApplyLevelColor()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = ColorThemeManager.GetColorForObject(objectType);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Auto Setup From Name (Tự động định nghĩa)")]
    public void AutoSetup()
    {
        string baseName = gameObject.name.ToLower().Replace(" ", "_");
        
        // Loại bỏ các hậu tố là đuôi số, ví dụ "_0_1"
        while (baseName.Length > 0 && char.IsDigit(baseName[baseName.Length - 1]))
        {
            baseName = baseName.Substring(0, baseName.Length - 1);
            if (baseName.EndsWith("_"))
            {
                baseName = baseName.Substring(0, baseName.Length - 1);
            }
        }

        // Tên sau khi loại bỏ hậu tố số (vd: "text_baba", "rock")
        // Đổi 'baba' thành 'dada' để match chuẩn với tên enum (TEXT_DADA, DADA)
        string processedName = baseName.Replace("baba", "dada");

        bool isTextObj = processedName.StartsWith("text_");

        if (isTextObj)
        {
            string contentName = processedName.Substring("text_".Length); // "dada", "is", "rock", "push"...
            
            // Tìm enum object type
            if (System.Enum.TryParse("TEXT_" + contentName.ToUpper(), out ObjectType parsedObjType))
                objectType = parsedObjType;

            // Tìm đại diện từ (WordType)
            if (contentName == "is")
            {
                wordType = WordType.OPERATOR;
            }
            else if (System.Enum.TryParse(contentName.ToUpper(), out ObjectType parsedRepresentType))
            {
                wordType = WordType.NOUN;
                representsType = parsedRepresentType;
            }
            else if (System.Enum.TryParse(contentName.ToUpper(), out Property parsedProperty))
            {
                wordType = WordType.PROPERTY;
                representsProperty = parsedProperty;
            }
        }
        else // Đối tượng thường như "dada", "rock"
        {
            if (System.Enum.TryParse(processedName.ToUpper(), out ObjectType parsedObjType))
                objectType = parsedObjType;
        }

        // 2. Tìm file Sprite trong thư mục Assets và tự gắn vào
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // Tìm chính xác tên ban đầu hoặc tên đã cắt số, hỗ trợ gõ "dada" thì tìm ảnh "baba"
            // Chỉ replace lava -> water cho đối tượng thường (vì chúng dùng chung icon), 
            // còn text_lava thì KHÔNG replace (vì chúng có icon chữ khác nhau).
            string searchName = baseName.Replace("dada", "baba");
            if (!IsText()) searchName = searchName.Replace("lava", "water");

            // Khắc phục triệt để lỗi của hàm FindAssets không nhận dạng được dấu '_' bằng cách tìm tất cả Sprite rồi tự duyệt
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Sprite", new string[] { "Assets/Sprites" });
            if (guids.Length > 0)
            {
                System.Collections.Generic.List<Sprite> loadedFrames = new System.Collections.Generic.List<Sprite>();
                
                foreach (string guid in guids)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(path).ToLower();
                    
                    // Chỉ nạp các frame thuộc Idle Animation (có dạng "ten_0_1", "ten_0_2")
                    // Hoặc tên y hệt (trường hợp object chỉ có 1 ảnh duy nhất không có frame)
                    if (fileName == searchName || fileName.StartsWith(searchName + "_0_"))
                    {
                        // Kiểm tra an toàn: Đảm bảo không bắt nhầm tường (wall) thành text_wall (nếu đoạn này vẫn lọt)
                        if (fileName == searchName || fileName.Substring(0, searchName.Length + 1) == searchName + "_") 
                        {
                            Sprite spr = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
                            if (spr != null) loadedFrames.Add(spr);
                        }
                    }
                }

                if (loadedFrames.Count > 0)
                {
                    // Sắp xếp Alphabet để frame 1 -> 2 -> 3 được chiếu đúng thứ tự
                    loadedFrames.Sort((a, b) => a.name.CompareTo(b.name));

                    animationFrames = loadedFrames.ToArray();
                    sr.sprite = animationFrames[0];
                    currentFrameIndex = 0;
                    
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.EditorUtility.SetDirty(sr);
                    
                    Debug.Log($"<color=green>Đã tự động cấu hình cho [{gameObject.name}]!</color> Enum là <b>{objectType}</b>, đã Gán Sprite và Cài đặt {animationFrames.Length} Frames Animation.");
                    
                    ApplyLevelColor();
                }
                else
                {
                    Debug.LogWarning($"<color=yellow>Cấu hình cho [{gameObject.name}]: Đã set Enum là <b>{objectType}</b> nhưng chưa tìm thấy ảnh Sprite nào hợp lệ cho khung hình <b>{searchName}_0_</b></color>");
                }
            }
            else
            {
                Debug.LogWarning($"<color=yellow>Chưa tìm thấy thư mục/kho Asset Sprites nào.</color>");
            }
        }
    }
#endif
}
