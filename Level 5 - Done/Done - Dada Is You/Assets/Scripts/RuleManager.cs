using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RuleManager : MonoBehaviour
{
    public static RuleManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateRules()
    {
        // 1. Reset all object properties
        var allObjects = GridManager.Instance.GetAllObjects().ToList();
        foreach (var obj in allObjects)
        {
            obj.ResetProperties();
        }

        // 2. Scan for rules
        // For simplicity, we find all 'TEXT_IS' blocks and check their neighbors
        var isBlocks = allObjects.Where(o => o.objectType == ObjectType.TEXT_IS).ToList();

        foreach (var isBlock in isBlocks)
        {
            // Check Horizontal: [Left] [IS] [Right]
            CheckAndApplyRule(isBlock.gridPos + Vector2Int.left, isBlock.gridPos + Vector2Int.right, allObjects);
            
            // Check Vertical: [Up] [IS] [Down]
            CheckAndApplyRule(isBlock.gridPos + Vector2Int.up, isBlock.gridPos + Vector2Int.down, allObjects);
        }
    }

    private void CheckAndApplyRule(Vector2Int nounPos, Vector2Int valuePos, List<DadaObject> allObjects)
    {
        var nounsAtPos = GridManager.Instance.GetObjectsAt(nounPos).Where(o => o.IsText() && o.wordType == WordType.NOUN).ToList();
        var valuesAtPos = GridManager.Instance.GetObjectsAt(valuePos).Where(o => o.IsText()).ToList();

        if (nounsAtPos.Count == 0 || valuesAtPos.Count == 0) return;

        foreach (var nounBlock in nounsAtPos)
        {
            foreach (var valueBlock in valuesAtPos)
            {
                Debug.Log($"[RuleFound] Found potential rule: {nounBlock.objectType} - IS - {valueBlock.objectType}");
                
                if (valueBlock.wordType == WordType.PROPERTY)
                {
                    Debug.Log($"[RuleApply] Applying Property {valueBlock.representsProperty} to {nounBlock.representsType}");
                    ApplyPropertyRule(nounBlock.representsType, valueBlock.representsProperty, allObjects);
                }
            }
        }
    }

    private void ApplyPropertyRule(ObjectType targetType, Property property, List<DadaObject> allObjects)
    {
        var targets = allObjects.Where(o => o.objectType == targetType).ToList();
        if (targets.Count > 0)
        {
            Debug.Log($"[RuleApply] Setting {property} for {targets.Count} objects of type {targetType}");
        }

        foreach (var target in targets)
        {
            switch (property)
            {
                case Property.YOU: target.isYou = true; break;
                case Property.PUSH: target.isPush = true; break;
                case Property.STOP: target.isStop = true; break;
                case Property.WIN: target.isWin = true; break;
                case Property.SINK: target.isSink = true; break;
                case Property.HOT: target.isHot = true; break;
                case Property.MELT: target.isMelt = true; break;
            }
        }
    }
}
