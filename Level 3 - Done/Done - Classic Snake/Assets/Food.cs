using UnityEngine;

public class Food : MonoBehaviour
{
    public Collider2D _gridArea;
    public Snake snake; // tham chiếu đến Snake

    void RandomizePosition()
    {
        Bounds bounds = this._gridArea.bounds;

        Vector2 newPosition;
        bool validPosition = false;

        // Lặp đến khi tìm được vị trí hợp lệ
        do
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);

            newPosition = new Vector2(Mathf.Round(x), Mathf.Round(y));

            validPosition = true;

            // kiểm tra xem có trùng với segment nào không
            foreach (Transform segment in snake._segments)
            {
                if ((Vector2)segment.position == newPosition)
                {
                    validPosition = false;
                    break;
                }
            }

        } while (!validPosition);

        this.transform.position = newPosition;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Snake"))
        {
            RandomizePosition();
        }
    }
}
