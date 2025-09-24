using UnityEngine;

[RequireComponent(typeof(CarController))]
public class PlayerCar : MonoBehaviour
{
    private CarController car;
    GameController gameController;
    void Awake()
    {
        car = GetComponent<CarController>();
        gameController = FindAnyObjectByType<GameController>();
    }

    void Update()
    {
        // Lấy input từ bàn phím
        car.throttle = Input.GetAxis("Vertical");   // W/S hoặc ↑/↓
        car.steer = Input.GetAxis("Horizontal");    // A/D hoặc ←/→
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Start"))
        gameController.Win();
    }
}
