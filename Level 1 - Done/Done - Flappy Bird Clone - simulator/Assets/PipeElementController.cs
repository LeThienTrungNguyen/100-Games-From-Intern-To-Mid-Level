using UnityEngine;

public class PipeElementController : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision);
        if (collision.transform.CompareTag("Player"))
        {
            GameController.instance.GameOver();
        }
    }
}
