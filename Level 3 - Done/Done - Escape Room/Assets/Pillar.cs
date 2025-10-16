using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    public int value;

    public Piece LastPiece()
    {
        if (transform.childCount == 0) return null;
        else
        return transform.GetChild(transform.childCount - 1).GetComponent<Piece>();
    }
}
