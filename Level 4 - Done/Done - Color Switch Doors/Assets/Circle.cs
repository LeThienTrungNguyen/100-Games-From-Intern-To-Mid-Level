using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public float speed = 100f; // tốc độ xoay (độ/giây)

    void Update()
    {
        // Xoay quanh trục Z (0,0,1)
        transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }
}
