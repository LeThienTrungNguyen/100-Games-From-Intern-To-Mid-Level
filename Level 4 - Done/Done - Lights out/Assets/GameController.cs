
using UnityEngine;

public class GameController : MonoBehaviour
{

    public Transform gridElementPrefabs;
    public bool[,] grid;
    void Awake()
    {
        grid = new bool[5, 5];
        CreateBoard();
        CheckBoard();
    }
    public Vector2Int mousePosGrid;
    public Vector2 mousePos;
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosGrid = new Vector2Int(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.y));
        if (mousePos.x >= -0.5f && mousePos.x < 4.5f && mousePos.y >= -0.5f && mousePos.y < 4.5f)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseClick(mousePos, mousePosGrid);
            }
        }


    }
    void OnMouseClick(Vector3 worldPos, Vector2Int gridPos)
    {
        var hit = Physics2D.OverlapPoint(worldPos);
        var hitYPlus = Physics2D.OverlapPoint(new Vector3(worldPos.x, worldPos.y + 1));
        var hitYMinus = Physics2D.OverlapPoint(new Vector3(worldPos.x, worldPos.y - 1));
        var hitXPlus = Physics2D.OverlapPoint(new Vector3(worldPos.x + 1, worldPos.y));
        var hitXMinus = Physics2D.OverlapPoint(new Vector3(worldPos.x - 1, worldPos.y));
        if (hit != null)
        {
            ChangeStatus(gridPos.x, gridPos.y, hit.transform);
        }
        if (hitYPlus != null)
        {
            ChangeStatus(gridPos.x, gridPos.y + 1, hitYPlus.transform);
        }
        if (hitYMinus != null)
        {
            ChangeStatus(gridPos.x, gridPos.y - 1, hitYMinus.transform);
        }
        if (hitXPlus != null)
        {
            ChangeStatus(gridPos.x + 1, gridPos.y, hitXPlus.transform);
        }
        if (hitXMinus != null)
        {
            ChangeStatus(gridPos.x - 1, gridPos.y, hitXMinus.transform);
        }

        CheckBoard();
    }

    void CreateBoard()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                grid[i, j] = false;
            }
        }



        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                var element = Instantiate(gridElementPrefabs, new Vector3(i, j, 0), Quaternion.identity, GameObject.Find("Board").transform);
                ChangeColorByLightStatus(element, grid[i, j]);
            }
        }
        for (int i = 0; i < 1000; i++)
        {
            float x = Random.Range(-0.5f, 4.5f);
            float y = Random.Range(-0.5f, 4.5f);
            int iX = Mathf.RoundToInt(x);
            int iY = Mathf.RoundToInt(y);

            OnMouseClick(new Vector3(x, y), new Vector2Int(iX, iY));
        }

        Debug.Log("Set all square to gray !!!");
    }
    Color onColor = Color.white;
    Color offColor = Color.gray;
    void ChangeStatus(int x, int y, Transform element)
    {
        grid[x, y] = !grid[x, y];
        ChangeColorByLightStatus(element, grid[x, y]);
    }
    void ChangeColorByLightStatus(Transform element, bool isOn)
    {
        if (isOn)
        {
            element.GetComponent<SpriteRenderer>().color = onColor;
        }
        else
        {
            element.GetComponent<SpriteRenderer>().color = offColor;
        }
    }

    void CheckBoard()
    {
        if (IsAllLightOff()) Debug.Log("Game Win");
    }

    bool IsAllLightOff()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (grid[i, j]) return false;
            }
        }
        return true;
    }
}
