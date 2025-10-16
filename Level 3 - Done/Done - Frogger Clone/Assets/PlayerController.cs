using UnityEngine;
using DG.Tweening;

public class PlayerInput
{
    public static KeyCode jump = KeyCode.Space;
}

public class PlayerController : MonoBehaviour
{
    GameController gameController;
    public float checkRadius;
    public LayerMask riverLayerMask;
    public LayerMask riverItemLayerMask;
    void Awake()
    {
        gameController = GameObject.FindFirstObjectByType(typeof(GameController)) as GameController;
    }
    void Update()
    {
        if (Input.GetKeyDown(PlayerInput.jump))
        {
            var item = FindNearestItem();
            if (item == null)
            {
                JumpForward();
            }
            else
            {
                JumpToItem(item);
            }
        }
    }

    void JumpToItem(Transform item)
    {
        transform.DOMove(item.position + Vector3.right*0.5f, 0.1f).OnComplete(() =>
        {
            transform.parent = item;
            CheckGameOver();
        });
    }

    void JumpForward()
    {
        transform.DOMoveY(transform.position.y + 0.5f, 0.1f).OnComplete(() =>
        {
            transform.parent = null;
            CheckGameOver();
        });
    }

    void CheckGameOver()
    {
        // Quét xem player đang chạm gì
        Collider2D[] rivers = Physics2D.OverlapCircleAll(transform.position, 0.1f, riverLayerMask);
        Collider2D[] items  = Physics2D.OverlapCircleAll(transform.position, 0.1f, riverItemLayerMask);

        if (rivers.Length > 0 && items.Length == 0)
        {
            Debug.Log("💀 GAME OVER: rơi xuống sông");
            gameController.GameOver();
            // TODO: xử lý game over (disable player, reload scene, v.v.)
        }
        else
        {
            Debug.Log("✅ SAFE: player đang đứng trên item hoặc mặt đất");
        }
    }

    Transform FindNearestItem()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, checkRadius, riverItemLayerMask);

        if (hits.Length == 0)
            return null;

        Transform currentParent = transform.parent;
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (currentParent != null && hit.transform == currentParent)
                continue;

            if (hit.transform.position.y <= transform.position.y)
                continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }

        return nearest;
    }
}
