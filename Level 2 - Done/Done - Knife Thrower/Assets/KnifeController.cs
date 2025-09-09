using UnityEngine;

public class KnifeController : MonoBehaviour
{
    public GameController gameController;
    void Awake()
    {
        //tìm game controller trong scene
        gameController = FindObjectOfType<GameController>();
    }
    // Hàm này sẽ được gọi khi knife va chạm với một collider khác
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Knife"))
        {
            // Xử lý khi knife va chạm với một knife khác
            Debug.Log($"Knife {gameObject.name} va chạm với Knife khác: {other.gameObject.name}");
            // Thêm xử lý gameover hoặc hiệu ứng tại đây nếu cần
            if(gameController !=null)gameController.GameOver();
        }
    }
}
