using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    public Transform wheelCenter; // Gán transform của tâm wheel trong Inspector
    public float rotationSpeed = 100f;

    private float rotateTime; // Thời gian quay hiện tại
    private float targetRotateTime; // Thời gian mục tiêu để đổi hướng
    private int direction = 1; // 1: quay thuận, -1: quay ngược
    private enum WheelState { Rotating, Decelerating, Accelerating }
    private WheelState state = WheelState.Rotating;
    private float currentSpeed = 0f;
    private float acceleration = 0f;
    private float deceleration = 100f; // tốc độ giảm dần
    private float minSpeed = 0.1f;

    public bool active;
    void Start()
    {
        SetRandomRotateTime();
        currentSpeed = rotationSpeed;
    }

    void Update()
    {
        if (!active) return;
        if (wheelCenter != null)
        {
            switch (state)
            {
                case WheelState.Rotating:
                    transform.RotateAround(wheelCenter.position, Vector3.forward, direction * currentSpeed * Time.deltaTime);
                    rotateTime += Time.deltaTime;
                    if (rotateTime >= targetRotateTime)
                    {
                        state = WheelState.Decelerating;
                        acceleration = 0f;
                    }
                    break;
                case WheelState.Decelerating:
                    currentSpeed -= deceleration * Time.deltaTime;
                    if (currentSpeed <= minSpeed)
                    {
                        currentSpeed = minSpeed;
                        direction *= -1;
                        state = WheelState.Accelerating;
                        acceleration = 0f;
                    }
                    transform.RotateAround(wheelCenter.position, Vector3.forward, direction * currentSpeed * Time.deltaTime);
                    break;
                case WheelState.Accelerating:
                    acceleration += 200f * Time.deltaTime; // gia tốc tăng dần
                    currentSpeed += acceleration * Time.deltaTime;
                    if (currentSpeed >= rotationSpeed)
                    {
                        currentSpeed = rotationSpeed;
                        state = WheelState.Rotating;
                        SetRandomRotateTime();
                    }
                    transform.RotateAround(wheelCenter.position, Vector3.forward, direction * currentSpeed * Time.deltaTime);
                    break;
            }
        }
    }
    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }
    public void SetSpeed(float speed)
    {
        rotationSpeed = speed;
    }
    void SetRandomRotateTime()
    {
        rotateTime = 0f;
        targetRotateTime = Random.Range(5f, 10f); // Quay từ 5-10 giây
    }

}
