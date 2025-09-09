using UnityEngine;

public class MedicineController : MonoBehaviour
{
    public Transform upperPart;
    //public Color upperColor;

    public Transform bottomPart;
    //public Color bottomColor;
    public bool isSplit = false; // true = đã tách ra, có thể rơi độc lập

    [SerializeField] private int x { get; set; }
    [SerializeField] private int y { get; set; }

    public void ChangeColor(Transform part, Color color)
    {
        part.GetComponent<SpriteRenderer>().color = color;
        part.GetComponentsInChildren<SpriteRenderer>()[1].color = color;
    }
    public Color GetColor(Transform part)
{
    return part.GetComponent<SpriteRenderer>().color;
}
}