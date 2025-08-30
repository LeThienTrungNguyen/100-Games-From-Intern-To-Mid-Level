using System.Collections.Generic;
using UnityEngine;

public class RoomsController : MonoBehaviour
{
    public int roomSpawnerIndex = -1;
    public List<RoomController> rooms;
    public RoomController starterRoom;
    void Awake()
    {
        RoomSpawner();
    }
    [ContextMenu("Create Room")]
    public void RoomSpawner()
    {
        RoomController room;
        float xPos = 0 + 16 * (roomSpawnerIndex == -1 ? 0 : roomSpawnerIndex);
        if (roomSpawnerIndex == -1)
        {
            room = starterRoom;
            roomSpawnerIndex = 1;
        }
        else
        {
            int index = Random.Range(0, rooms.Count);
            room = rooms[index];
            roomSpawnerIndex++;
        }

        var roomT = Instantiate(room);
        roomT.gameObject.SetActive(true);
        roomT.transform.position = new Vector3(xPos, 0, 0);


    }
}
