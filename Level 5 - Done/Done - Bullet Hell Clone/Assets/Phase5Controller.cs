using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase5Controller : MonoBehaviour
{
    
    void OnEnable()
    {
        transform.GetComponentInParent<BossController>().EnablePhaseAll();
    }
}
