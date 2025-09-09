using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Color[] medicineColor = new Color[] { Color.red, Color.yellow, Color.cyan };

    public Transform virusPrefab;
    public Transform medicinePrefab;
    public Color[,] Grid = new Color[8, 23];
    public float offset;

    public int virusCount = 20;
    public int maxRowVirusSpawn = 10;
    public Transform[,] gridObjects = new Transform[8, 23];

    private Dictionary<Vector2Int, Transform> cellToObject = new Dictionary<Vector2Int, Transform>();
    private Dictionary<Vector2Int, bool> isMedicine = new Dictionary<Vector2Int, bool>();

    public Transform medicineT;
    [Range(0.0001f, 1f)] public float scaleFallDown = 0.2f;
    public float fallSpeed = 6f;

    void Awake()
    {
        SpawnGrid();
        SpawnVirus();
        SpawnMedicine();
    }

    void Update()
    {
        if (medicineT != null)
            HandleMedicineMove();
    }

    void HandleMedicineMove()
    {
        var medController = medicineT.GetComponent<MedicineController>();
        int dir = 0;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) dir = -1;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) dir = 1;
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) TryRotateMedicine(medController);

        if (dir != 0)
        {
            int upperX = Mathf.RoundToInt(medController.upperPart.position.x / offset);
            int upperY = Mathf.RoundToInt(medController.upperPart.position.y / offset);
            int bottomX = Mathf.RoundToInt(medController.bottomPart.position.x / offset);
            int bottomY = Mathf.RoundToInt(medController.bottomPart.position.y / offset);

            if (upperX + dir < 0 || upperX + dir >= Grid.GetLength(0)) return;
            if (bottomX + dir < 0 || bottomX + dir >= Grid.GetLength(0)) return;
            if (Grid[upperX + dir, upperY] != Color.black || Grid[bottomX + dir, bottomY] != Color.black) return;

            medicineT.position += Vector3.right * dir * offset;
        }
    }

    void TryRotateMedicine(MedicineController medController)
    {
        int upperX = Mathf.RoundToInt(medController.upperPart.position.x / offset);
        int upperY = Mathf.RoundToInt(medController.upperPart.position.y / offset);
        int bottomX = Mathf.RoundToInt(medController.bottomPart.position.x / offset);
        int bottomY = Mathf.RoundToInt(medController.bottomPart.position.y / offset);

        int dx = bottomX - upperX;
        int dy = bottomY - upperY;
        int newDX = -dy;
        int newDY = dx;

        int newBottomX = upperX + newDX;
        int newBottomY = upperY + newDY;

        if (IsCellFree(upperX, upperY) && IsCellFree(newBottomX, newBottomY))
        {
            ApplyRotation(medController, newBottomX, newBottomY);
        }
        else if (IsCellFree(upperX + 1, upperY) && IsCellFree(newBottomX + 1, newBottomY))
        {
            medicineT.position += Vector3.right * offset;
            ApplyRotation(medController, newBottomX + 1, newBottomY);
        }
        else if (IsCellFree(upperX - 1, upperY) && IsCellFree(newBottomX - 1, newBottomY))
        {
            medicineT.position += Vector3.left * offset;
            ApplyRotation(medController, newBottomX - 1, newBottomY);
        }
    }

    void ApplyRotation(MedicineController medController, int bottomX, int bottomY)
    {
        medController.bottomPart.position = new Vector3(bottomX * offset, bottomY * offset, 0);
        medController.upperPart.Rotate(0, 0, 90);
        medController.bottomPart.Rotate(0, 0, 90);
    }

    bool IsCellFree(int x, int y)
    {
        if (x < 0 || x >= Grid.GetLength(0) || y < 0 || y >= Grid.GetLength(1)) return false;
        return Grid[x, y] == Color.black;
    }

    IEnumerator MoveMedicineDown(MedicineController medController)
    {
        float timer = 0f;
        const float baseInterval = 1f;

        while (medicineT != null)
        {
            float currentInterval = baseInterval * (Input.GetKey(KeyCode.DownArrow) ? scaleFallDown : 1f);
            timer += Time.deltaTime;
            if (timer < currentInterval) { yield return null; continue; }
            timer = 0f;

            int upperX = Mathf.RoundToInt(medController.upperPart.position.x / offset);
            int upperY = Mathf.RoundToInt(medController.upperPart.position.y / offset);
            int bottomX = Mathf.RoundToInt(medController.bottomPart.position.x / offset);
            int bottomY = Mathf.RoundToInt(medController.bottomPart.position.y / offset);

            if (upperY - 1 < 0 || bottomY - 1 < 0 ||
                !IsCellFree(upperX, upperY - 1) || !IsCellFree(bottomX, bottomY - 1))
            {
                LockMedicineToGrid(medController, upperX, upperY);
                yield break;
            }

            medicineT.position += Vector3.down * offset;
        }
    }

    void LockMedicineToGrid(MedicineController medController, int upperX, int upperY)
{
    // Lưu upper
    Color upperColor = medController.GetColor(medController.upperPart);
    Grid[upperX, upperY] = upperColor;
    gridObjects[upperX, upperY] = medController.upperPart;
    cellToObject[new Vector2Int(upperX, upperY)] = medController.upperPart;
    isMedicine[new Vector2Int(upperX, upperY)] = true;

    // Lưu bottom
    int bottomX = Mathf.RoundToInt(medController.bottomPart.position.x / offset);
    int bottomY = Mathf.RoundToInt(medController.bottomPart.position.y / offset);
    Color bottomColor = medController.GetColor(medController.bottomPart);
    Grid[bottomX, bottomY] = bottomColor;
    gridObjects[bottomX, bottomY] = medController.bottomPart;
    cellToObject[new Vector2Int(bottomX, bottomY)] = medController.bottomPart;
    isMedicine[new Vector2Int(bottomX, bottomY)] = true;

    // 🔥 Không detach, không destroy controller
    medicineT = null;

    // Check match
    bool cleared = CheckAndClearMatches();
    if (cleared)
        StartCoroutine(ApplyGravity());
    else
        SpawnMedicine();
}


    bool CheckAndClearMatches()
    {
        List<Vector2Int> cellsToClear = new List<Vector2Int>();
        int width = Grid.GetLength(0), height = Grid.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x <= width - 4; x++)
            {
                Color c = Grid[x, y];
                if (c == Color.black) continue;
                int count = 1;
                while (x + count < width && Grid[x + count, y] == c) count++;
                if (count >= 4) for (int k = 0; k < count; k++) cellsToClear.Add(new Vector2Int(x + k, y));
                if (count > 1) x += (count - 1);
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y <= height - 4; y++)
            {
                Color c = Grid[x, y];
                if (c == Color.black) continue;
                int count = 1;
                while (y + count < height && Grid[x, y + count] == c) count++;
                if (count >= 4) for (int k = 0; k < count; k++) cellsToClear.Add(new Vector2Int(x, y + k));
                if (count > 1) y += (count - 1);
            }
        }

        if (cellsToClear.Count == 0) return false;

        HashSet<Transform> processed = new HashSet<Transform>();
        foreach (var cell in cellsToClear.Distinct())
        {
            Grid[cell.x, cell.y] = Color.black;
            Transform obj = gridObjects[cell.x, cell.y];
            if (obj != null && !processed.Contains(obj))
            {
                processed.Add(obj);
                MedicineController med = obj.GetComponentInParent<MedicineController>();
                if (med != null && !med.isSplit)
                {
                    if (med.upperPart == obj && med.bottomPart != null)
                    {
                        Transform bottom = med.bottomPart;
                        bottom.SetParent(null);
                        var newMed = bottom.gameObject.AddComponent<MedicineController>();
                        newMed.upperPart = bottom;
                        newMed.bottomPart = null;
                        newMed.isSplit = true;
                        gridObjects[(int)bottom.position.x, (int)bottom.position.y] = bottom;
                        med.upperPart = null;
                    }
                    else if (med.bottomPart == obj && med.upperPart != null)
                    {
                        Transform upper = med.upperPart;
                        upper.SetParent(null);
                        var newMed = upper.gameObject.AddComponent<MedicineController>();
                        newMed.upperPart = upper;
                        newMed.bottomPart = null;
                        newMed.isSplit = true;
                        gridObjects[(int)upper.position.x, (int)upper.position.y] = upper;
                        med.bottomPart = null;
                    }
                    Destroy(obj.gameObject);
                }
                else Destroy(obj.gameObject);

                gridObjects[cell.x, cell.y] = null;
                Vector2Int v = new Vector2Int(cell.x, cell.y);
                if (cellToObject.ContainsKey(v)) cellToObject.Remove(v);
                if (isMedicine.ContainsKey(v)) isMedicine.Remove(v);

                Destroy(obj.gameObject);

            }
        }

        return true;
    }

    IEnumerator ApplyGravity()
{
    bool moved = true;
    while (moved)
    {
        moved = false;
        for (int y = 1; y < Grid.GetLength(1); y++)
        {
            for (int x = 0; x < Grid.GetLength(0); x++)
            {
                Transform obj = gridObjects[x, y];
                if (!obj) continue;
                if (obj.CompareTag("Virus")) continue;

                MedicineController med = obj.GetComponentInParent<MedicineController>();
                if (med != null && !med.isSplit)
                {
                    // Upper + Bottom cùng rơi nếu trống bên dưới
                    int ux = Mathf.RoundToInt(med.upperPart.position.x / offset);
                    int uy = Mathf.RoundToInt(med.upperPart.position.y / offset);
                    int bx = Mathf.RoundToInt(med.bottomPart.position.x / offset);
                    int by = Mathf.RoundToInt(med.bottomPart.position.y / offset);

                    bool canFall = IsCellFree(ux, uy - 1) && IsCellFree(bx, by - 1);
                    if (canFall)
                    {
                        // update grid
                        Grid[ux, uy - 1] = Grid[ux, uy]; Grid[ux, uy] = Color.black;
                        Grid[bx, by - 1] = Grid[bx, by]; Grid[bx, by] = Color.black;

                        // update gridObjects
                        gridObjects[ux, uy - 1] = med.upperPart; gridObjects[ux, uy] = null;
                        gridObjects[bx, by - 1] = med.bottomPart; gridObjects[bx, by] = null;

                        // move transform
                        StartCoroutine(SmoothFall(med.upperPart, new Vector3(ux * offset, (uy - 1) * offset, 0)));
                        StartCoroutine(SmoothFall(med.bottomPart, new Vector3(bx * offset, (by - 1) * offset, 0)));

                        moved = true;
                    }
                }
                else
                {
                    // single part falling (đối với medicine đã tách hoặc virus)
                    int px = Mathf.RoundToInt(obj.position.x / offset);
                    int py = Mathf.RoundToInt(obj.position.y / offset);
                    if (py - 1 >= 0 && Grid[px, py - 1] == Color.black && gridObjects[px, py - 1] == null)
                    {
                        Grid[px, py - 1] = Grid[px, py]; Grid[px, py] = Color.black;
                        gridObjects[px, py - 1] = obj; gridObjects[px, py] = null;
                        StartCoroutine(SmoothFall(obj, new Vector3(px * offset, (py - 1) * offset, 0)));
                        moved = true;
                    }
                }
            }
        }

        if (moved)
        {
            yield return new WaitForSeconds(0.25f);
            CheckAndClearMatches();
        }
    }

    SpawnMedicine();
}


    IEnumerator SmoothFall(Transform obj, Vector3 targetPos)
    {
        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 startPos = obj.position;

        while (elapsed < duration)
        {
            if (!obj) yield break;
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            obj.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        obj.position = targetPos;
    }

    #region Spawner
    void SpawnGrid()
    {
        for (int x = 0; x < Grid.GetLength(0); x++)
            for (int y = 0; y < Grid.GetLength(1); y++)
            {
                Grid[x, y] = Color.black;
                gridObjects[x, y] = null;
            }
        cellToObject.Clear();
        isMedicine.Clear();
    }

    void SpawnVirus()
    {
        var emptyCells = new List<Vector2Int>();
        int maxRow = Mathf.Min(maxRowVirusSpawn, Grid.GetLength(1));
        for (int x = 0; x < Grid.GetLength(0); x++)
            for (int y = 0; y < maxRow; y++)
                if (Grid[x, y] == Color.black)
                    emptyCells.Add(new Vector2Int(x, y));

        int spawnCount = Mathf.Min(virusCount, emptyCells.Count);
        for (int i = 0; i < spawnCount; i++)
        {
            int idx = Random.Range(0, emptyCells.Count);
            Vector2Int cell = emptyCells[idx];
            emptyCells.RemoveAt(idx);

            Color color = medicineColor[Random.Range(0, medicineColor.Length)];
            Grid[cell.x, cell.y] = color;

            Transform virus = Instantiate(virusPrefab, new Vector2(cell.x, cell.y) * offset, Quaternion.identity);
            var virusC = virus.gameObject.GetComponent<VirussController>() ?? virus.gameObject.AddComponent<VirussController>();
            virusC.x = cell.x;
            virusC.y = cell.y;
            virusC.ChangeColor(color);

            gridObjects[cell.x, cell.y] = virus;
            cellToObject[new Vector2Int(cell.x, cell.y)] = virus;
            isMedicine[new Vector2Int(cell.x, cell.y)] = false;
        }
    }

    void SpawnMedicine()
    {
        medicineT = Instantiate(medicinePrefab);
        medicineT.position = new Vector2(3, 22) * offset;
        medicineT.rotation = Quaternion.Euler(0, 0, 90);

        var medController = medicineT.GetComponent<MedicineController>();
        var upperPart = medController.upperPart;
        var bottomPart = medController.bottomPart;

        var upperColorIdx = Random.Range(0, medicineColor.Length);
        var bottomColorIdx = Random.Range(0, medicineColor.Length);

        medController.ChangeColor(upperPart, medicineColor[upperColorIdx]);
        medController.ChangeColor(bottomPart, medicineColor[bottomColorIdx]);

        StartCoroutine(MoveMedicineDown(medController));
    }
    #endregion
}
