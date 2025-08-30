
using System.Collections.Generic;
using UnityEngine;

public class FruitsController : MonoBehaviour
{
    public List<Transform> fruist;
    public float spawnLoopTimer = 1f;
    public float spawnTimer;
    void Awake()
    {
        spawnTimer = spawnLoopTimer;
    }
    void Update()
    {
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        }
        else
        {
            spawnTimer = spawnLoopTimer;
            var fruit = GetRandomFruit();
            Debug.Log(fruit);
            CreateFruit(fruit);
        }
    }

    Transform GetRandomFruit()
    {
        int r = Random.Range(0, fruist.Count);
        return fruist[r];
    }
    void CreateFruit(Transform fruit)
    {
        float randomX = Random.Range(-7f, 7f);
        Vector3 spawnPosition = new Vector3(randomX, transform.position.y, transform.position.z);
        var f = Instantiate(fruit, spawnPosition, Quaternion.identity);
        f.gameObject.SetActive(true);
    }

}
