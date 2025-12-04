using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [HideInInspector] public PlayerController controller;
    [SerializeField] private GameObject currentCounter;

    private readonly Dictionary<Renderer, Material[]> originalMaterials = new();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) HandleInteract();
        if (Input.GetKeyDown(KeyCode.F)) HandleCutting();
    }

    #region --- INTERACTION LOGIC ---

    public void HandleInteract()
    {
        if (currentCounter == null)
        {
            Debug.Log("No counter selected → Try picking normally.");
            if (!controller.isPicking || controller.isBringingPlate) PickObject();
            return;
        }

        // Nếu đang cầm plate, thử đặt xuống ClearCounter trống
        if (controller.isBringingPlate && controller.isPicking)
        {
            var delivery = currentCounter.GetComponentInParent<DeliveryCounter_Visual>();
            if (delivery)
            {
                Debug.Log("You interact with delivery");
                delivery.CheckCompletedDeliveryInOrders();
                return;
            }
            var clear = currentCounter.GetComponentInParent<ClearCounter_Visual>();
            if (clear && clear.PutDownObject == null)
            {
                PutObjectDown(controller.PickingObject, clear);
                return;
            }
        }

        // Nếu không cầm gì hoặc đang cầm plate → nhặt
        if (!controller.isPicking || controller.isBringingPlate)
        {
            // Nếu đang cầm thì thử ném vào thùng rác
            if (controller.isPicking && TryThrowToTrash()) return;
            PickObject();
            return;
        }

        // Đang cầm object thường → đặt xuống tuỳ counter
        TryPutDownToCounter();
    }

    void HandleCutting()
    {
        var cut = currentCounter?.GetComponentInParent<CuttingCounter_Visual>();
        if (cut != null) cut.CuttingHandle();
    }

    #endregion

    #region --- PICK & PUT ---
    void LateUpdate()
    {
        GetDeliveryObjOnHand();
    }
    void PickObject()
    {
        Transform pick = TryGetPickupFromCounter();
        if (!pick)
        {
            Debug.LogWarning("Không tìm thấy object để nhặt!");
            return;
        }

        controller.PickingObject = pick;
        controller.isPicking = true;
        Debug.Log($"Picked object: {pick.name}");
        
    }

    Transform TryGetPickupFromCounter()
    {
        if (controller.isBringingPlate) return TryAttachHeldPlate();

        // Thứ tự ưu tiên
        var container = currentCounter.GetComponentInParent<ContainerCounter_Visual>();
        var clear = currentCounter.GetComponentInParent<ClearCounter_Visual>();
        var cutting = currentCounter.GetComponentInParent<CuttingCounter_Visual>();
        var stove = currentCounter.GetComponentInParent<StoveCounter_Visual>();
        var plates = currentCounter.GetComponentInParent<PlatesCounter_Visual>();

        Transform pick = null;

        if (container?.containerObject) // ContainerCounter → spawn mới
            pick = Instantiate(container.containerObject, controller.handPosition.position, Quaternion.identity, controller.handPosition);
        else if (clear?.PutDownObject) // ClearCounter → lấy object sẵn
            pick = TakeFromCounter(clear);
        else if (cutting?.PutDownObject)
            pick = TakeFromCounter(cutting);
        else if (stove?.PutDownObject)
            pick = TakeFromCounter(stove);
        else if (plates && plates.plateLst.Count > 0)
        {
            pick = plates.plateLst[^1];
            plates.plateLst.RemoveAt(plates.plateLst.Count - 1);
            controller.isBringingPlate = true;
        }

        if (pick)
        {
            pick.SetParent(controller.handPosition);
            pick.localPosition = Vector3.zero;
        }

        return pick;
    }

    bool TryPutDownToCounter()
    {
        var clear = currentCounter.GetComponentInParent<ClearCounter_Visual>();
        var cut = currentCounter.GetComponentInParent<CuttingCounter_Visual>();
        var stove = currentCounter.GetComponentInParent<StoveCounter_Visual>();
        var trash = currentCounter.GetComponentInParent<TrashCounter_Visual>();
        var obj = controller.PickingObject;

        if (trash) return TryThrowToTrash();

        if (clear)
        {
            if (clear.PutDownObject == null)
                PutObjectDown(obj, clear);
            else if (TryAttachToPlate(obj, clear.PutDownObject, clear))
                ResetHand();
            return true;
        }

        if (cut && CanCut(obj))
        {
            PutObjectDown(obj, cut);
            return true;
        }

        if (stove && CanCook(obj))
        {
            PutObjectDown(obj, stove);
            return true;
        }

        return false;
    }

    bool TryThrowToTrash()
    {
        var trash = currentCounter?.GetComponentInParent<TrashCounter_Visual>();
        if (!trash || !controller.PickingObject) return false;
        Destroy(controller.PickingObject.gameObject);
        ResetHand();
        Debug.Log("Object thrown into trash.");
        return true;
    }

    void PutObjectDown(Transform obj, MonoBehaviour counter)
    {
        if (counter is ClearCounter_Visual clear)
        {
            if (clear.PutDownObject != null) return;
            MoveToCounter(obj, clear.PutDownPosition, clear);
        }
        else if (counter is CuttingCounter_Visual cut)
        {
            if (cut.PutDownObject != null) return;
            MoveToCounter(obj, cut.PutDownPosition, cut);
            if (CanCut(obj)) cut.SetCuttingMaxCount();
        }
        else if (counter is StoveCounter_Visual stove)
        {
            if (stove.PutDownObject != null) return;
            MoveToCounter(obj, stove.PutDownPosition, stove);
        }

        ResetHand();
    }


    void MoveToCounter(Transform obj, Transform pos, MonoBehaviour counter)
    {
        obj.position = pos.position;
        obj.parent = counter.transform.parent;

        if (counter is ClearCounter_Visual clear)
            clear.PutDownObject = obj;
        else if (counter is CuttingCounter_Visual cut)
            cut.PutDownObject = obj;
        else if (counter is StoveCounter_Visual stove)
            stove.PutDownObject = obj;
    }



    Transform TakeFromCounter(MonoBehaviour counter)
    {
        Transform obj = null;

        if (counter is ClearCounter_Visual clear)
        {
            obj = clear.PutDownObject;
            clear.PutDownObject = null;
            controller.isBringingPlate = LayerUtils.IsContainLayer(obj.gameObject.layer, LayerMask.GetMask("Plate"));
        }
        else if (counter is CuttingCounter_Visual cut)
        {
            obj = cut.PutDownObject;
            cut.PutDownObject = null;
        }
        else if (counter is StoveCounter_Visual stove)
        {
            obj = stove.PutDownObject;
            stove.PutDownObject = null;
        }

        return obj;
    }





    void ResetHand()
    {
        controller.PickingObject = null;
        controller.isPicking = false;
        controller.isBringingPlate = false;
        controller.deliveryObjLst.Clear();
    }

    void GetDeliveryObjOnHand()
    {
        if (controller == null) return;
        if (controller.PickingObject == null) return;
        if (!LayerUtils.IsContainLayer(controller.PickingObject.gameObject.layer, LayerMask.GetMask("Plate"))) return;
        var KitchenObjects = GetComponentsInChildren<DeliveriableKitchenObject>();
        if (KitchenObjects.Count() > 0)
        {
            foreach (var obj in KitchenObjects)
            {
                Debug.Log($"{obj.name} , {obj.gameObject.layer}");
                if (controller.deliveryObjLst.Contains(1<<obj.gameObject.layer)) continue;
                controller.deliveryObjLst.Add(1<<obj.gameObject.layer);

            }
        }
        else
        {
            controller.deliveryObjLst.Clear();
        }
        Debug.Log(KitchenObjects.Count());
    }

    #endregion

    #region --- PLATE ATTACHING ---

    Transform TryAttachHeldPlate()
    {
        MonoBehaviour counter = null;

        if (currentCounter != null)
        {
            var stove = currentCounter.GetComponentInParent<StoveCounter_Visual>();
            var cutting = currentCounter.GetComponentInParent<CuttingCounter_Visual>();
            var clear = currentCounter.GetComponentInParent<ClearCounter_Visual>();

            if (stove != null) counter = stove;
            else if (cutting != null) counter = cutting;
            else if (clear != null) counter = clear;
        }

        if (counter == null) return null;

        Transform food = null;

        if (counter is StoveCounter_Visual stoveCounter)
            food = stoveCounter.PutDownObject;
        else if (counter is CuttingCounter_Visual cuttingCounter)
            food = cuttingCounter.PutDownObject;
        else if (counter is ClearCounter_Visual clearCounter)
            food = clearCounter.PutDownObject;

        if (food != null)
            SetDeliveryObjToPlate(food, controller.PickingObject, counter);

        return null;
    }


    bool TryAttachToPlate(Transform food, Transform plate, MonoBehaviour counter)
    {
        if (!LayerUtils.IsContainLayer(food.gameObject.layer, controller.deliveriableLayer)) return false;
        return SetDeliveryObjToPlate(food, plate, counter);
    }

    bool SetDeliveryObjToPlate(Transform food, Transform plate, MonoBehaviour counter)
    {
        if (!food || !plate) return false;

        int layer = food.gameObject.layer;
        for (int i = 1; i <= 5; i++)
        {
            Transform slot = plate.GetChild(i);
            if (slot.childCount > 0 && slot.GetChild(0).gameObject.layer == layer)
                return false;
        }

        for (int i = 1; i <= 5; i++)
        {
            Transform slot = plate.GetChild(i);
            if (food.gameObject.layer == slot.gameObject.layer)
            {
                food.SetParent(slot);
                food.localPosition = Vector3.zero;
                ClearCounterRef(counter, food);

                // ✅ NEW: Thêm layer của món ăn vào danh sách deliveryObjLst
                if (controller != null)
                {
                    LayerMask foodLayerMask = 1 << food.gameObject.layer;
                    if (!controller.deliveryObjLst.Contains(foodLayerMask))
                    {
                        controller.deliveryObjLst.Add(foodLayerMask);
                        Debug.Log($"[PlayerInteract] Added food layer {LayerMask.LayerToName(food.gameObject.layer)} to deliveryObjLst.");
                    }
                }

                return true;
            }
        }

        return false;
    }

    void ClearCounterRef(MonoBehaviour counter, Transform obj)
    {
        if (counter is ClearCounter_Visual clear && clear.PutDownObject == obj) clear.PutDownObject = null;
        if (counter is CuttingCounter_Visual cut && cut.PutDownObject == obj) cut.PutDownObject = null;
        if (counter is StoveCounter_Visual stove && stove.PutDownObject == obj) stove.PutDownObject = null;
    }

    bool CanCut(Transform obj) =>
        LayerUtils.IsContainLayer(obj.gameObject.layer, controller.cutableLayer) ||
        LayerUtils.IsContainLayer(obj.gameObject.layer, controller.cuttedLayer);

    bool CanCook(Transform obj) =>
        LayerUtils.IsContainLayer(obj.gameObject.layer, controller.cookableLayer);

    #endregion

    #region --- HIGHLIGHTING ---

    public void CheckCounterForwardAndHandleHighlight()
    {
        if (!controller) return;

        Ray ray = new(controller.transform.position + Vector3.up * 0.5f, controller.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, controller.checkDistance, controller.counterLayer))
        {
            if (currentCounter != hit.collider.gameObject)
            {
                RemoveHighlight();
                currentCounter = hit.collider.gameObject;
                AddHighlight(currentCounter);
            }
            Debug.DrawLine(ray.origin, hit.point, Color.green);
        }
        else
        {
            RemoveHighlight();
            currentCounter = null;
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * controller.checkDistance, Color.red);
        }
    }

    void AddHighlight(GameObject obj)
    {
        if (!obj || !controller.counterSelectedMaterial) return;

        foreach (var rend in obj.GetComponentsInChildren<Renderer>())
        {
            if (!rend || originalMaterials.ContainsKey(rend)) continue;

            Material[] original = rend.sharedMaterials;
            originalMaterials[rend] = original;
            Material[] newMats = new Material[original.Length + 1];
            original.CopyTo(newMats, 0);
            newMats[^1] = controller.counterSelectedMaterial;
            rend.materials = newMats;
        }
    }

    void RemoveHighlight()
    {
        if (!currentCounter) return;

        foreach (var rend in currentCounter.GetComponentsInChildren<Renderer>())
        {
            if (!rend) continue;

            if (originalMaterials.TryGetValue(rend, out Material[] original))
            {
                rend.materials = original;
                originalMaterials.Remove(rend);
            }
        }
    }

    #endregion
}
