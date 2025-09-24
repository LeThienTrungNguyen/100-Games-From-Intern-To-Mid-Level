using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        transform.parent.GetComponent<Collider2D>().isTrigger = true;
    }
    void OnTriggerExit2D(Collider2D collision)
    {
                transform.parent.GetComponent<Collider2D>().isTrigger = false;

    }
}
