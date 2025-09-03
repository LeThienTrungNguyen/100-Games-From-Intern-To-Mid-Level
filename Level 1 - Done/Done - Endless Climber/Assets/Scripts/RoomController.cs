using UnityEngine;

public class RoomController : MonoBehaviour
{
    public RoomsController roomsController;
    public bool entered = false;
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (!entered) { roomsController.RoomSpawner(); entered = true; }
    }
}