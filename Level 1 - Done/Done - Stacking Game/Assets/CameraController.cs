using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public static float r,g,b;
    void Start()
    {
        r = Random.Range(0f, 256f) / 255f;
        g = Random.Range(127f, 256f) / 255f;
        b = Random.Range(0f, 256f) / 255f;

        Camera.main.backgroundColor = new Color(r, g, b);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
