using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CanvasController cc;
    public static GameController instance;
    void Awake()
    {
        instance = this;
    }
    public Transform pickedItem1;
    public Transform pickedItem2;
    public void AddPickedItem(Transform target)
    {
        if (pickedItem1 == null) pickedItem1 = target;
        else { pickedItem2 = target; }

        if (pickedItem1 != null && pickedItem2 != null)
        {
            if (pickedItem1 == pickedItem2)
            {
                pickedItem1.DOLocalMoveY(0, 0.2f);
            }
            else
            {
                SwapItem(pickedItem1, pickedItem2);

            }
            ClearPickedItems();
        }
    }

    public void ClearPickedItems()
    {
        pickedItem1 = null;
        pickedItem2 = null;
    }

    float swapTime = 0.5f;

    [ContextMenu("Swap Item")]
    public void SwapItem()
    {
        if (pickedItem1.parent == pickedItem2.parent) return;
        SwapItem(pickedItem1, pickedItem2);
    }
    public bool isSwaping = false;
    public void SwapItem(Transform item1, Transform item2)
    {
        isSwaping = true;

        var parent1 = item1.parent;
        var parent2 = item2.parent;

        int index1 = item1.GetSiblingIndex();
        int index2 = item2.GetSiblingIndex();

        Vector3 pos1 = item1.position + new Vector3(0, -0.3f);
        Vector3 pos2 = item2.position + new Vector3(0, -0.3f);

        // Tween swap position
        item1.DOMove(pos2, swapTime);
        item2.DOMove(pos1, swapTime).OnComplete(() =>
        {
            // Đổi parent & index SAU KHI tween xong
            item1.SetParent(parent2);
            item2.SetParent(parent1);

            item1.SetSiblingIndex(index2);
            item2.SetSiblingIndex(index1);

            isSwaping = false;

            Debug.Log("swaped completed");
            CheckGroup(parent1);
            CheckGroup(parent2);
            if (IsAllGroupsCompleted())
            {
                //groups.GetComponent<GroupsController>().Reset();
                cc.ShowPanel();
            }
        });
    }

    void CheckGroup(Transform group)
    {
        if (group == null)
        {
            Debug.LogError("❌ CheckGroup: group is NULL!");
            return;
        }
        Debug.Log("Check Group for : " + group.name);
        bool isCompleted = true;
        for (int i = 1; i < group.childCount; i++)
        {
            if (!group.GetChild(i).name.Equals(group.GetChild(0).name)) isCompleted = false;
        }
        Debug.Log("isCompleted : " + isCompleted, group);
        if (isCompleted)
        {
            for (int i = group.childCount - 1; i >= 0; i--)
            {
                group.GetChild(i).GetComponent<SpriteRenderer>().color = Color.gray;
                group.GetChild(i).GetComponent<ItemController>().canInteract = false;

            }
            
        }
    }

    public Transform groups;
    bool IsAllGroupsCompleted()
    {
        for (int i = 0; i < groups.childCount; i++)
        {
            if (groups.GetChild(i).GetChild(0).GetComponent<ItemController>().canInteract) 
            return false;
        }
        return true;
    }
}
