using UnityEngine;

public class VirussController : MonoBehaviour
{
    public int x;
    public int y;
    public Color color;
    public void ChangeColor(Color color)

    {
        this.color = color;
        transform.GetComponent<SpriteRenderer>().color = this.color;
    }
}