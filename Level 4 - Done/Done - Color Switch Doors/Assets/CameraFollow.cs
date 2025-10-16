using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        // smooth damp to target y position + offset
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, target.position.y + 3f, Time.deltaTime * 2), transform.position.z);
    }
}
