using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatformController : MonoBehaviour
{
    public PlayerController pc;
    BoxCollider2D bc;
    [SerializeField] bool isLanded;
    public GameController gameController;
    void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        bc.isTrigger = true;
        Debug.Log(transform.position);
        Debug.Log(transform.GetComponent<BoxCollider2D>().bounds.max.y);

    }
    void FixedUpdate()
    {
        var boundY = transform.GetComponent<BoxCollider2D>().bounds.max.y;
        var pcScaleY = pc.transform.localScale.y;
        var pcColliderSizeY = pc.GetComponent<BoxCollider2D>().size.y;
        var pcY = pc.transform.position.y;
        if (boundY + pcScaleY <= pcY)
        {
            bc.isTrigger = false;
        }
        else { bc.isTrigger = true; }

        if (transform.position.y + 15 < Camera.main.transform.position.y)
        {
            gameObject.SetActive(false);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isLanded) return;
        transform.parent.GetComponent<OneWayPlatformsController>().GenerateStartPlatforms(1);
        isLanded = true;
        if (!gameController.ready) gameController.ready = true;
    }
    
}
