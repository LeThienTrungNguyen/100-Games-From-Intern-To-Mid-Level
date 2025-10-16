using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlayerController pc1;
    public PlayerController pc2;
    public bool isPc1Active;
    void Awake()
    {
        SwitchPlayer();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SwitchPlayer();
        }
    }
    [ContextMenu("Switch Player")]
    void SwitchPlayer()
    {
        isPc1Active = !isPc1Active;
        pc1.Inactive();
        pc2.Inactive();
        if (isPc1Active) { pc1.Active(); }
        else { pc2.Active(); }
    }
}
