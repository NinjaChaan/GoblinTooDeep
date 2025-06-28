using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpiderHole : MonoBehaviour
{
    public GameObject spiderPrefab;
    public float initialSpawnDelayMin = 5f;
    public float initialSpawnDelayMax = 30f;
    public float minSpawningDelay = 30f;
    public float maxSpawningDelay = 60f;

    public int maxSpidersSpawned = 10;
    
    private int _currentSpiders = 0;
    
    void Start()
    {
        float initialDelay = Random.Range(initialSpawnDelayMin, initialSpawnDelayMax);
        Invoke(nameof(SpawnSpider), initialDelay);
    }
    
    private void SpawnSpider()
    {
        if (_currentSpiders >= maxSpidersSpawned)
        {
            return;
        }
        _currentSpiders++;
        
        var spider = Instantiate(spiderPrefab, transform.position, Quaternion.identity);
        spider.GetComponent<Spider>().spiderHole = transform;
        
        float delay = Random.Range(minSpawningDelay, maxSpawningDelay);
        Invoke(nameof(SpawnSpider), delay);
    }
}
