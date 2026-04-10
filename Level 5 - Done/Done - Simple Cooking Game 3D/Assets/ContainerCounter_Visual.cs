using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter_Visual : MonoBehaviour,IInteractable
{
    public Transform containerObject;

    public void Interact(PlayerController player)
    {
        var obj = Instantiate(containerObject, player.handPosition.position, Quaternion.identity, player.transform);
    }
}
