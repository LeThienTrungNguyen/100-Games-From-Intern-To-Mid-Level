using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    public int cuttingMaxCount;
    public Transform CuttedObject;

    public Vector3 CheeseSlices_Pos;
    public Vector3 Bread_Pos;
    public Vector3 MeatPattyCooked_Pos;
    public Vector3 CabbageSliced_Pos;
    public Vector3 TomatoSlices_Pos;
    void Awake()
    {
        if (!LayerUtils.IsContainLayer(gameObject.layer, LayerMask.NameToLayer("Plate"))) return;
        CheeseSlices_Pos = transform.Find("CheeseSlices_Pos").position;
        Bread_Pos = transform.Find("Bread_Pos").position;
        MeatPattyCooked_Pos = transform.Find("MeatPattyCooked_Pos").position;
        CabbageSliced_Pos = transform.Find("CabbageSliced_Pos").position;
        TomatoSlices_Pos = transform.Find("TomatoSlices_Pos").position;
    }
}
