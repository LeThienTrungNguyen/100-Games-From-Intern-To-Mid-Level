using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public GameController gc;
    public float mspeed;
    const float startMspeed = 0.5f;
    public bool ready;
    public float mul;
    void FixedUpdate()
    {
        if (!gc.ready) return;
        mspeed = startMspeed+ gc.score/mul;
        mspeed = Mathf.Clamp(mspeed, 0f, 2.5f);
        transform.position += Vector3.up * Time.deltaTime * mspeed;
    }
}
