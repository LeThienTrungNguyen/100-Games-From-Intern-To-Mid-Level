using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public Rigidbody2D hook;
    public Transform linkPrefab;
    public int links = 7;

    public Weight weight;
    void Start()
    {
        GenerateRope();
    }
    void GenerateRope()
    {
        Rigidbody2D previousRb = hook;
        for (int i = 0; i < links; i++)
        {
            Transform link = Instantiate(linkPrefab, transform);
            HingeJoint2D joint = link.GetComponent<HingeJoint2D>();
            joint.connectedBody = previousRb;

            if (i < links - 1)
            { 
                previousRb = joint.GetComponent<Rigidbody2D>();
            }
            else
            {
                weight.ConnectRopeEnd(joint.GetComponent<Rigidbody2D>());
                
            }
        }
    }
}
