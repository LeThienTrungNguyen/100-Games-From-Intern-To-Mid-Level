using Unity.Android.Types;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform playerMirror;
    float moveOffset = 0.5f;
    public Vector2[] directions = new Vector2[]{
        Vector2.left , Vector2.right , Vector2.up ,Vector2.down
     };
    void Awake()
    {
        playerMirror = GameObject.Find("PlayerMirror").transform;

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var direction = directions[0];
            Movement(direction);
            MovementMirror(direction);
        }
        else
        if (Input.GetKeyDown(KeyCode.D))
        {
            var direction = directions[1];
            Movement(direction);
            MovementMirror(direction);

        }
        else
        if (Input.GetKeyDown(KeyCode.W))
        {
            var direction = directions[2];
            Movement(direction);
            MovementMirror(direction);

        }
        else
        if (Input.GetKeyDown(KeyCode.S))
        {
            var direction = directions[3];
            Movement(direction);
            MovementMirror(direction);

        }
    }
    void MoveToPos(Vector2 pos, Transform mover)
    {
        mover.position = pos;
    }
    void Movement(Vector2 dir)
    {
        var pos = transform.position + (Vector3)dir * moveOffset;
        Debug.Log("Player :" + pos);
        if (HasWallOrPlayerAtPosition(pos)) return;
        MoveToPos(pos,transform);
    }
    void MovementMirror(Vector2 dir)
    {
        var pos = playerMirror.position - (Vector3)dir * moveOffset;
        Debug.Log("Player mirror:" + pos);
        if (HasWallOrPlayerAtPosition(pos)) return;
        MoveToPos(pos,playerMirror);
    }
    bool HasWallOrPlayerAtPosition(Vector2 pos)
    {
        var overlaper = Physics2D.OverlapPoint(pos);
        if (overlaper == null) return false;
        if (overlaper.CompareTag("Wall") || overlaper.CompareTag("PlayerMirror") || overlaper.CompareTag("Player")) return true;
        return false;
    }
}