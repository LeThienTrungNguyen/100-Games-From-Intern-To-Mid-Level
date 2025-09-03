using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public GameController gameController;
    [Min(0)] public int index;
    [SerializeField] Generator item;
    public TextMeshProUGUI Name;
    public Image Icon;
    public TextMeshProUGUI Income;
    public TextMeshProUGUI Currency;
    void Awake()
    {
        item = gameController.generators[index];
        transform.name = item.name;
        Icon.sprite = item.icon;

    }
    void FixedUpdate()
    {
        Name.text = item.name + $"[{item.ownedCount}]";
        Income.text = item.GetIncomePerSecond().ToString("0.##") + "/s";
        Currency.text = item.GetCostNext().ToString("0.##") + "";

        if (gameController.primaryCurrency < item.GetCostNext())
        {
            Currency.color = Color.red;
            GetComponent<Button>().interactable = false;
        }
        else
        {
            Currency.color = Color.green;
            GetComponent<Button>().interactable = true;
        }
    }

    public void OnClick()
    {
        gameController.primaryCurrency -= item.GetCostNext();
        item.ownedCount++;
    }
}