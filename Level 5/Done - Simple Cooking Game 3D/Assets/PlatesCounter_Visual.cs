using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter_Visual : MonoBehaviour
{
    PlayerController controller;
    public List<Transform> plateLst;
    public Transform platePrefab;
    public float spawnOffset;
    public Transform plateSpawnPos;
    void Awake()
    {
        controller = FindAnyObjectByType(typeof(PlayerController)) as PlayerController;
        InvokeRepeating(nameof(SpawnPlate), 3f,3f);
    }
    void SpawnPlate()
    {
        if (plateLst.Count >= 4) return;
        var plate = Instantiate(platePrefab, plateSpawnPos.position + Vector3.up * (plateLst.Count + 1) * spawnOffset, Quaternion.identity);
        plateLst.Add(plate);
    }

}
