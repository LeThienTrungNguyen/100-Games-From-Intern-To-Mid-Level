using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Down, Up, Left, Right,
    DownLeft, DownRight, UpLeft, UpRight,
    None
}

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    Rigidbody2D rb;
    Animator animator;

    float h, v;
    Direction dir = Direction.None;
    Direction lastDir = Direction.Down; // mặc định nhìn xuống

    Dictionary<Direction, string> runAnim = new Dictionary<Direction, string>()
    {
        {Direction.Down, "PlayerRunDown"},
        {Direction.Up, "PlayerRunUp"},
        {Direction.Left, "PlayerRunLeft"},
        {Direction.Right, "PlayerRunRight"},
        {Direction.DownLeft, "PlayerRunDL"},
        {Direction.DownRight, "PlayerRunDR"},
        {Direction.UpLeft, "PlayerRunUL"},
        {Direction.UpRight, "PlayerRunUR"},
    };

    Dictionary<Direction, string> standAnim = new Dictionary<Direction, string>()
    {
        {Direction.Down, "PlayerStandDown"},
        {Direction.Up, "PlayerStandUp"},
        {Direction.Left, "PlayerStandLeft"},
        {Direction.Right, "PlayerStandRight"},
        {Direction.DownLeft, "PlayerStandDL"},
        {Direction.DownRight, "PlayerStandDR"},
        {Direction.UpLeft, "PlayerStandUL"},
        {Direction.UpRight, "PlayerStandUR"},
    };

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Lấy input
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        // Ưu tiên chặn chéo nếu muốn (chỉ di chuyển 1 hướng) 
        // hoặc bỏ để cho phép di chuyển chéo
        dir = GetDirection(h, v);

        // Gọi animation
        if (dir == Direction.None)
        {
            animator.Play(standAnim[lastDir]);
        }
        else
        {
            animator.Play(runAnim[dir]);
            lastDir = dir;
        }
    }

    void FixedUpdate()
    {
        // Di chuyển bằng Rigidbody2D
        Vector2 move = new Vector2(h, v).normalized;
        rb.velocity = move * moveSpeed;
    }

    Direction GetDirection(float h, float v)
    {
        if (h == 0 && v == 0) return Direction.None;

        if (h > 0 && v > 0) return Direction.UpRight;
        if (h < 0 && v > 0) return Direction.UpLeft;
        if (h > 0 && v < 0) return Direction.DownRight;
        if (h < 0 && v < 0) return Direction.DownLeft;

        if (h > 0) return Direction.Right;
        if (h < 0) return Direction.Left;
        if (v > 0) return Direction.Up;
        if (v < 0) return Direction.Down;

        return Direction.None;
    }
}
