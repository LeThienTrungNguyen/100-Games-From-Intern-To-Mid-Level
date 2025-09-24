using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KongController : MonoBehaviour
{
    public GameObject barrelPrefab;   // Prefab của barrel
    public Transform spawnPoint;      // Vị trí tạo barrel (có thể đặt 1 empty object)

    void Start()
    {
        // Gọi hàm SpawnBarrel sau 1s, lặp lại mỗi 3s
        InvokeRepeating("SpawnBarrel", 1f, 3f);
    }

    void SpawnBarrel()
    {
        if (barrelPrefab != null)
        {
            // Tạo barrel ở vị trí spawnPoint, xoay mặc định
            Instantiate(barrelPrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Chưa gán barrelPrefab trong Inspector!");
        }
    }
}
