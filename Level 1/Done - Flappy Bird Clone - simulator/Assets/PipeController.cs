using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeController : MonoBehaviour
{
    public float speed;
    public float destroyTime;
    // Update is called once per frame
    void Update()
    {
        if (!GameController.instance.gameStarted) return;
        
        Destroy(gameObject, destroyTime);
        transform.position -= Vector3.right * Time.deltaTime * speed;
    }
}
