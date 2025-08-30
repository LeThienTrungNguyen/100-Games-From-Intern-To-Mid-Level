using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CardType Type;
    public Sprite Image;
    public bool isShowing = false;
}

public enum CardType
{
    type1, type2, type3,
    type4,type5,type6,type7,type8,type9,type10,none
}
