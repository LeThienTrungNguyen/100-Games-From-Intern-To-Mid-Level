using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class ActiveOrderUI
{
    public Order order;
    public RectTransform ui;
}
public class OrdersManager : MonoBehaviour
{
    public Order[] templates;
    public RectTransform orderUIPrefab;

    // Danh sách các Order đang hoạt động cùng với UI của chúng
    public  List<ActiveOrderUI> activeOrders = new List<ActiveOrderUI>();

    void Awake()
    {
        InvokeRepeating(nameof(CreateOrder), 1f, 2f);
    }

    void CreateOrder()
    {
        if (activeOrders.Count >= 4) return;

        // Chọn order ngẫu nhiên từ templates
        int r = Random.Range(0, templates.Length);
        var order = templates[r];

        // Khởi tạo UI
        var orderUIRect = Instantiate(orderUIPrefab, transform);
        orderUIRect.gameObject.SetActive(true);

        // Gán icon cho từng món ăn
        for (int i = 0; i < order.foods.Length; i++)
        {
            orderUIRect.GetChild(i).GetComponent<Image>().sprite = order.foods[i].iconUI;
            orderUIRect.GetChild(i).gameObject.SetActive(true);
        }
        // Ẩn các slot dư
        for (int i = order.foods.Length; i < 5; i++)
        {
            orderUIRect.GetChild(i).gameObject.SetActive(false);
        }

        // 🟢 Lưu cặp dữ liệu (Order + UI)
        activeOrders.Add(new ActiveOrderUI { order = order, ui = orderUIRect });
    }

    // 🗑️ Hàm xoá order
    public void RemoveOrder(Order order)
    {
        // Tìm cặp tương ứng
        var target = activeOrders.FirstOrDefault(x => x.order == order);
        if (target != null)
        {
            Destroy(target.ui.gameObject); // xoá UI
            activeOrders.Remove(target);   // xoá logic
        }
    }

    // 🗑️ Hoặc có thể xoá theo index

    public void RemoveOrderAt(int index)
    {
        if (index < 0 || index >= activeOrders.Count) return;
        Destroy(activeOrders[index].ui.gameObject);
        activeOrders.RemoveAt(index);
    }
    [ContextMenu("Remove Order")]
    public void RemoveOrderTest()
    {
        RemoveOrderAt(0);
    }
}

