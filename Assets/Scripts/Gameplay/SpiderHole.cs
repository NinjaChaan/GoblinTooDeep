using System;
using System.Collections;
using UnityEngine;

public class SpiderHole : MonoBehaviour
{
    public GameObject spiderPrefab; 
    
    void Start()
    {
        var spider = Instantiate(spiderPrefab, transform.position, Quaternion.identity);
        spider.GetComponent<Spider>().spiderHole = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
