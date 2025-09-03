using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoleHolesController : MonoBehaviour
{
    public List<Transform> holes;
    public float timerChoosenHole = 0.5f;
    // Start is called before the first frame update
    void Awake()
    {
        SetIsUpFalseAll();
    }

    // Update is called once per frame
    void Update()
    {
        //return;
        if (timerChoosenHole <= 0)
        {
            timerChoosenHole = 1.5f;
            var moleController = GetRandomHole();
            if (moleController != null) { moleController.isShowUp = true; }
        }
        else { timerChoosenHole -= Time.deltaTime; }
    }

    MoleController GetRandomHole()
    {
        int r = Random.Range(0, holes.Count);
        Debug.Log(holes[r], holes[r]);
        return holes[r].GetComponentInChildren<MoleController>();
    }

    void SetIsUpFalseAll()
    {
        foreach (Transform h in holes)
        {
            var moleController = h.GetComponentInChildren<MoleController>();
            moleController.isShowUp = false;
        }
    }
}
