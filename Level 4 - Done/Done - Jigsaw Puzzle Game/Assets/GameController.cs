using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public enum CameraAlignment
{
    Left,
    Right
}
public class GameController : MonoBehaviour
{
    int boardWidth = 5;
    int boardHeight = 5;

    Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.down, Vector2Int.left, Vector2Int.right, Vector2Int.up
    };

    public Transform PuzzleSquarePrefab;
    public Transform PuzzleSquareShadowPrefab;
    public Transform parent;

    [System.Serializable]
    public struct GridCell
    {
        public bool occupied;
        public Transform owner;
    }

    [SerializeField]GridCell[,] grid;


    public CameraAlignment cameraAlignment = CameraAlignment.Right;
    [SerializeField] Transform choosenShape;
    [SerializeField] Vector2 mousePos;
    bool _isPickupShape;
    void Awake()
    {
        CreateOrRecreateBoard();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) OnClickDown();
        if (Input.GetMouseButton(0) && _isPickupShape)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            choosenShape.position = mousePos;
        }
        if (Input.GetMouseButtonUp(0)) OnClickUp();
    }
    void OnClickUp()
    {
        if (choosenShape == null) return; 
        _isPickupShape = false;

        List<Transform> snapablePieces;
        List<Vector2> snapablePoses;
        if (CanSnapShape(choosenShape, out snapablePieces, out snapablePoses))
        {
            SnapShape(snapablePieces, snapablePoses);
        }
        choosenShape = null;
        if (CheckWin()) Debug.Log("You Win");
    }

    bool CheckWin()
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (!grid[i, j].occupied) return false;
            }
        }
        return true;
    }
    void SnapShape(List<Transform> snapablePieces, List<Vector2> snapablePoses)
    {
        snapablePieces[0].position = snapablePoses[0];
        for (int i = 0; i < snapablePieces.Count; i++)
        {
            Vector2 pos = snapablePoses[i];
            int x = Mathf.RoundToInt(pos.x);
            int y = Mathf.RoundToInt(pos.y);

            // Snap về đúng tọa độ integer
            SnapPiece(snapablePieces[i], new Vector2(x, y));

            // Đánh dấu grid là bị chiếm
            SetValidPosition(x, y, snapablePieces[i].parent,true);
        }

    }
    void SnapPiece(Transform piece, Vector2 pos)
    {
        piece.position = pos;
    }

    void SetValidPosition(int i, int j, Transform attacher,bool valid = true)
    {
        if (attacher == null) valid = false;
        grid[i, j].occupied = valid;
        grid[i, j].owner = attacher;
    }

    bool CanSnapShape(Transform shape, out List<Transform> snapablePieces, out List<Vector2> snapablePoses)
    {
        snapablePieces = new(); snapablePoses = new();
        foreach (Transform piece in shape)
        {
            Vector2Int snapablePos = -Vector2Int.one;
            if (!CanSnapPiece(piece, out snapablePos)) return false;

            snapablePieces.Add(piece);
            snapablePoses.Add(snapablePos);
        }
        return true;
    }
    bool CanSnapPiece(Transform piece, out Vector2Int snapablePos)
    {
        snapablePos = -Vector2Int.one;
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (Vector2.Distance(new Vector2(i, j), (Vector2)piece.position) < 0.5f && ValidBoardPosition(i, j))
                {
                    snapablePos = new Vector2Int(i, j);
                    return true;
                }
            }
        }
        return false;
    }

    void OnClickDown()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPoint = new Vector2Int(Mathf.RoundToInt(worldPoint.x),Mathf.RoundToInt(worldPoint.y));
        Collider2D hit = Physics2D.OverlapPointAll(worldPoint).Count() > 0 ? Physics2D.OverlapPointAll(worldPoint)[0] : null;

        if (hit == null) return;
        choosenShape = hit.transform.parent;
        
        if (choosenShape != null)
        {
            _isPickupShape = true;
            //if (!ValidBoardPosition(gridPoint.x, gridPoint.y)) return;
            if (gridPoint.x < 0 || gridPoint.x >= boardWidth || gridPoint.y < 0 || gridPoint.y >= boardHeight)
                return;
            if (choosenShape == grid[gridPoint.x, gridPoint.y].owner) { 
                // Gỡ chiếm chỗ trên grid cho các mảnh của shape này
                foreach (Transform pos in choosenShape)
                {
                    int x = Mathf.RoundToInt(pos.position.x);
                    int y = Mathf.RoundToInt(pos.position.y);

                    if (x >= 0 && x < boardWidth && y >= 0 && y < boardHeight)
                    {
                        SetValidPosition(x, y, null,false);
                    }
                }
            }

            
        }
    }


    void CreateOrRecreateBoard()
    {
        grid = new GridCell[boardWidth, boardHeight];
        // khởi tạo các ô trống
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                Instantiate(PuzzleSquarePrefab, new Vector2(i, j), Quaternion.identity, parent);
                Instantiate(PuzzleSquareShadowPrefab, new Vector2(i, j), Quaternion.identity, parent);
                grid[i, j].occupied = false;
                grid[i, j].owner = null;
            }
        }

        int shapeIndex = 0;
        List<Transform> allShapes = new List<Transform>(); // 🆕

        // lặp tới khi board đầy
        while (true)
        {
            Vector2Int? nextStart = FindNextValidCell();
            if (nextStart == null)
            {
                
                break;
            }

            // tạo parent cho shape
            GameObject shapeParent = new GameObject($"Shape_{shapeIndex}");
            shapeParent.transform.parent = parent;

            CreateShape(nextStart.Value, shapeParent.transform);
            allShapes.Add(shapeParent.transform); // 🆕

            shapeIndex++;
        }

        AdjustCameraToFitBoard();
        RandomizeShapePositions(allShapes);
        ResetGrid();
        // 🆕 Random vị trí các shape trong vùng camera

    }
    void ResetGrid()
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                grid[i, j].occupied = false;
                grid[i, j].owner = null;
            }
        }
    }
    Vector2Int? FindNextValidCell()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (!grid[x, y].occupied)
                    return new Vector2Int(x, y);
            }
        }
        return null;
    }

    void CreateShape(Vector2Int startPos, Transform shapeParent)
    {
        int pieceCount = Random.Range(1, 7);
        List<Vector2Int> shape = new List<Vector2Int>();

        shape.Add(startPos);
        grid[startPos.x, startPos.y].occupied = true;
        grid[startPos.x, startPos.y].owner = shapeParent;
        DrawSquareColor(startPos, Random.ColorHSV());
        shapeParent.position = (Vector2)startPos;
        AttachToShape(startPos, shapeParent);

        for (int i = 1; i < pieceCount; i++)
        {
            Vector2Int basePos = shape[Random.Range(0, shape.Count)];

            List<Vector2Int> validPositions;
            if (Is4AdjustInvalid(basePos, out validPositions)) continue;

            Vector2Int nextPos = GetRandomDirection(validPositions);

            grid[nextPos.x, nextPos.y].occupied = true;
            grid[nextPos.x, nextPos.y].owner = shapeParent;
            shape.Add(nextPos);
            DrawSquareColor(nextPos, shapeParent.GetComponentInChildren<SpriteRenderer>().color);
            AttachToShape(nextPos, shapeParent);
        }
    }

    bool Is4AdjustInvalid(Vector2Int position, out List<Vector2Int> listValidPositions)
    {
        listValidPositions = new List<Vector2Int>();
        foreach (Vector2Int dir in directions)
        {
            Vector2Int newPos = position + dir;
            if (ValidBoardPosition(newPos.x, newPos.y))
            {
                listValidPositions.Add(newPos);
            }
        }
        return listValidPositions.Count == 0;
    }

    Vector2Int GetRandomDirection(List<Vector2Int> validDirections)
    {
        int random = Random.Range(0, validDirections.Count);
        return validDirections[random];
    }

    bool ValidBoardPosition(int x, int y)
    {
        if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight)
            return false;

        if (grid[x, y].occupied)
            return false;

        return true;
    }

    void DrawSquareColor(Vector2Int position, Color color)
    {
        Collider2D col = Physics2D.OverlapPoint(position);
        if (col != null)
        {
            col.GetComponent<SpriteRenderer>().color = color;
        }
    }

    void AttachToShape(Vector2Int position, Transform shapeParent)
    {
        Collider2D col = Physics2D.OverlapPoint(position);
        if (col != null)
        {
            col.transform.SetParent(shapeParent);
        }
    }

    void AdjustCameraToFitBoard()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float boardWidthWorld = boardWidth;
        float boardHeightWorld = boardHeight;

        float centerX = (boardWidthWorld - 1) / 2f + 0.5f;
        float centerY = (boardHeightWorld - 1) / 2f;

        // Tính kích thước camera để bao trọn board
        float aspect = (float)Screen.width / Screen.height;
        float sizeY = boardHeightWorld / 2f + 0.5f;
        float sizeX = (boardWidthWorld / 2f + 0.5f) / aspect;
        cam.orthographicSize = Mathf.Max(sizeY, sizeX);

        // Tính chiều rộng camera (theo đơn vị world)
        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * aspect;

        // Mặc định: camera nằm chính giữa board
        Vector3 camPos = new Vector3(centerX, centerY, -10f);

        // Dịch camera tùy theo lựa chọn
        switch (cameraAlignment)
        {
            case CameraAlignment.Left:
                camPos.x = centerX - (camWidth / 2f - boardWidthWorld / 2f);
                break;
            case CameraAlignment.Right:
                camPos.x = centerX + (camWidth / 2f - boardWidthWorld / 2f);
                break;
            default:
                break;
        }

        cam.transform.position = camPos;
    }

    void RandomizeShapePositions(List<Transform> allShapes)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float camHeight = cam.orthographicSize * 2f;
        float camWidth = camHeight * cam.aspect;
        Vector2 camCenter = cam.transform.position;

        // biên camera
        float camLeft = camCenter.x - camWidth / 2f ;
        float camRight = camCenter.x + camWidth / 2f ;
        float camTop = camCenter.y + camHeight / 2f;
        float camBottom = camCenter.y - camHeight / 2f;

        // biên board
        float boardLeft = 0;
        float boardRight = boardWidth;
        float boardTop = boardHeight;

        // offset board theo vị trí thật trong world
        float boardCenterX = (boardWidth - 1) / 2f ;
        float boardCenterY = (boardHeight - 1) / 2f ;

        Vector3 boardWorldCenter = new Vector3(boardCenterX, boardCenterY, 0);

        // Tùy camera alignment
        Rect spawnArea;
        switch (cameraAlignment)
        {
            case CameraAlignment.Right:
                // shape ở bên phải board
                spawnArea = new Rect(boardRight + 1f, camBottom + 1f, camRight - (boardRight + 2f), camHeight - 2f);
                break;
            case CameraAlignment.Left:
                // shape ở bên trái board
                spawnArea = new Rect(camLeft + 1f, camBottom + 1f, boardLeft - camLeft - 2f, camHeight - 2f);
                break;
            default:
                // shape phía dưới board
                spawnArea = new Rect(boardLeft, camBottom + 1f, boardWidth, camHeight - boardHeight - 2f);
                break;
        }

        foreach (Transform shape in allShapes)
        {
            if (shape == null) continue;

            float randomX = Random.Range(spawnArea.xMin, spawnArea.xMax);
            float randomY = Random.Range(spawnArea.yMin, spawnArea.yMax);

            shape.position = new Vector3(randomX, randomY, 0);
        }
    }

}
