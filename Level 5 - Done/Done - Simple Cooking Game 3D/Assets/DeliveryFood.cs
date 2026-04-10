using UnityEngine;

[CreateAssetMenu(fileName = "Food", menuName = "Game/Food")]
public class DeliveryFood : ScriptableObject
{
    public int id;
    public string foodName;
    public Sprite iconUI;
    public LayerMask layer;
}
