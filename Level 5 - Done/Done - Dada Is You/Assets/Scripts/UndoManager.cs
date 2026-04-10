using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UndoManager : MonoBehaviour
{
    public static UndoManager Instance { get; private set; }

    [System.Serializable]
    public class ObjectState
    {
        public DadaObject obj;
        public Vector2Int pos;
        public bool active;
    }

    [System.Serializable]
    public class TurnSnapshot
    {
        public List<ObjectState> states = new List<ObjectState>();
    }

    private Stack<TurnSnapshot> history = new Stack<TurnSnapshot>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CaptureState()
    {
        TurnSnapshot snapshot = new TurnSnapshot();
        var allObjects = GameObject.FindObjectsOfType<DadaObject>(true); // Find even inactive ones if we want, but let's stick to active for simplicity or manage a master list.

        foreach (var obj in allObjects)
        {
            snapshot.states.Add(new ObjectState
            {
                obj = obj,
                pos = obj.gridPos,
                active = obj.gameObject.activeSelf
            });
        }
        history.Push(snapshot);
    }

    public void Undo()
    {
        if (history.Count == 0) return;

        TurnSnapshot snapshot = history.Pop();
        foreach (var state in snapshot.states)
        {
            state.obj.gameObject.SetActive(state.active);
            if (state.active)
            {
                // Reset grid manager before moving
                GridManager.Instance.RemoveObject(state.obj, state.obj.gridPos);
                state.obj.gridPos = state.pos;
                state.obj.UpdateVisualPosition();
                GridManager.Instance.AddObject(state.obj, state.obj.gridPos);
            }
        }

        // Re-update rules after undo
        RuleManager.Instance.UpdateRules();
    }

    public void ClearHistory()
    {
        history.Clear();
    }
}
