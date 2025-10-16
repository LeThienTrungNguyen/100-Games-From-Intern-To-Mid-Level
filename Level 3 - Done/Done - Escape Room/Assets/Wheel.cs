using UnityEngine;

public class Wheel : MonoBehaviour
{
    [Range(0, 9)] public int number = 10;

    [ContextMenu("Rotate")]
    public void Rotate()
    {
        if (number >= 9) { number = 0; }
        else { number++; }
        transform.rotation = Quaternion.Euler(Vector3.right * (number-1) * -36f);
        CheckIsUnlocked();
    }

    void CheckIsUnlocked()
    {
        transform.parent.parent.GetComponent<Lock>().CheckLock();
    }
}