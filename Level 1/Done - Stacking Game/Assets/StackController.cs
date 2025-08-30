using DG.Tweening;
using UnityEngine;

public class StackController : MonoBehaviour
{
    public static StackController instance;
    public Transform stack;
    public Transform parent;
    public bool isStackPutted = true;
    public static bool isThrowing = false;
    public float moveSpeed;
    public const float minX = -3f;
    public const float maxX = 3f;
    public const int maxStackedMoveCamera = 4;
    public int stacked = 0;
    public int resetCollisionCount = 2;

    void Awake()
    {
        instance = this;

    }
    public bool firstTower = true;
    void Update()
    {
        if (isStackPutted || (FloorCount() == 0 && firstTower && handlingTower == null))
        {
            CreateStack();
            isStackPutted = false;
            firstTower = false;
        }
    }
    public Transform handlingTower;
    void CreateStack()
    {
        var s = Instantiate(stack);
        s.gameObject.SetActive(true);
        handlingTower = s;
        float r = Random.Range(0f, 256f) / 256f;
        float g = Random.Range(0f, 256f) / 256f;
        float b = Random.Range(0f, 256f) / 256f;
        while (g == CameraController.g)
        {
            g = Random.Range(0f, 256f) / 256f;
        }
        s.GetComponent<SpriteRenderer>().color = new Color(r, b, g);
    }
    public void OnCollisionHandler()
    {
        resetCollisionCount--;
        if (resetCollisionCount <= 0)
        {
            ClearTowers();
            Camera.main.transform.DOMoveY(-1.75f, 1.5f);
            stacked = 0;
            resetCollisionCount = 2;
            
            GameController.score = 0;
        }
    }


    void ClearTowers()
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
        firstTower = true;
        
    }

    public int FloorCount()
    {
        Debug.Log("parent :"+parent.childCount, parent);
        return parent.childCount;
    }
}
