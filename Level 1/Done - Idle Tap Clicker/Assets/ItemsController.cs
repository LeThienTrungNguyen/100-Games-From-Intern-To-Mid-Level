using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ItemsController : MonoBehaviour
{
    public List<ItemController> itemControllers;


    void Awake()
    {
        itemControllers = GetComponentsInChildren<ItemController>().ToList();

    }

}