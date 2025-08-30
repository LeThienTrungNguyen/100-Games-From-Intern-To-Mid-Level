using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatformsController : MonoBehaviour
{
    public Transform platformPrefab;
    public Transform parent;
    public Transform startPlatform;
    public Transform lastPlatform;
    public List<Transform> spawnedTransform;
    public float spawnOffsetMin = 1f,spawnOffsetMax = 3.38f;
    public float spawnMinX = -5.2f, spawnMaxX = 5.2f;
    public int previousPlatformNumbers;
    // Start is called before the first frame update
    void Awake()
    {
        lastPlatform = startPlatform;
    }
    void Start()
    {
        GenerateStartPlatforms(previousPlatformNumbers);
    }
    public void GenerateStartPlatforms(int number)
    {
        for (int i = 0; i < number; i++)
        {
            var platform = Instantiate(platformPrefab, parent); platform.gameObject.SetActive(true);
            var spawnOffset = Random.Range(spawnOffsetMin, spawnOffsetMax);
            var spawnX = Random.Range(spawnMinX, spawnMaxX);
            platform.position = new Vector3(spawnX, lastPlatform.position.y + spawnOffset);
            lastPlatform = platform;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
