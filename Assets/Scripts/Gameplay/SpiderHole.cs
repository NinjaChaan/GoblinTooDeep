using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpiderHole : MonoBehaviour
{
    public GameObject spiderPrefab;
    public float initialSpawnDelayMin = 5f;
    public float initialSpawnDelayMax = 30f;
    public float spawningDelay = 30f;

    public int maxSpidersSpawned = 10;
    
    private int _currentSpiders = 0;
    
    void Start()
    {
        float initialDelay = Random.Range(initialSpawnDelayMin, initialSpawnDelayMax);
        InvokeRepeating(nameof(SpawnSpider), initialDelay, spawningDelay);
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
    }
}
