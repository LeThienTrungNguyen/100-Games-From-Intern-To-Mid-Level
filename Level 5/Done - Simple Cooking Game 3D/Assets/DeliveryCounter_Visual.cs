using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeliveryCounter_Visual : MonoBehaviour
{
    PlayerController controller;
    OrdersManager ordersManager;
    // Start is called before the first frame update
    void Awake()
    {
        controller = FindAnyObjectByType(typeof(PlayerController)) as PlayerController;
        ordersManager = FindAnyObjectByType(typeof(OrdersManager)) as OrdersManager;

    }

    public void CheckCompletedDeliveryInOrders()
    {
        // kiểm tra xem biến controller.deliveryObject có bằng bất kì orders nào đang có không
        foreach (var activeOrderUI in ordersManager.activeOrders)
        {
            if (IsExactlyEqual(controller.deliveryObjLst, activeOrderUI.order))
            {
                Debug.Log("Delivery Food is in Order");
                ordersManager.RemoveOrder(activeOrderUI.order);
                Destroy(controller.PickingObject.gameObject);
                break;
            }
        }
        Debug.Log("Delivery Food is not in Order");
    }

    public bool IsExactlyEqual(List<LayerMask> l1, Order order)
    {
        if (order.foods.Count() != l1.Count())
        {
            return false;
        }
        for (int i = 0; i < l1.Count(); i++)
        {
            if (!Contain(l1[i], order)) return false;
        }

        return true;
    }

    public bool Contain(LayerMask l , Order order)
    {
        foreach (var food in order.foods)
        {
            if (food.layer == l) return true;
        }
        return false;
    }
}
