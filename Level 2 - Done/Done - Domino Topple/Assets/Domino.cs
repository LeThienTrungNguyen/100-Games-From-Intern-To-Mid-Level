using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Domino : MonoBehaviour
{
    public bool isFalled;

    void FixedUpdate()
{
    // Lấy góc nghiêng trên trục X và Z
    Vector3 euler = transform.eulerAngles;

    // Chuyển góc > 180 sang -180..180
    float x = (euler.x > 180) ? euler.x - 360 : euler.x;
    float z = (euler.z > 180) ? euler.z - 360 : euler.z;

    // Nếu nghiêng > threshold, coi là đổ
    if (Mathf.Abs(x) > 45f || Mathf.Abs(z) > 45f)
        isFalled = true;
    else
        isFalled = false;
}

}
