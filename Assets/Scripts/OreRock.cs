using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class OreRock : MonoBehaviour
{
    public GameObject gemPrefab;
    public GameObject sparkVfx;
    public GameObject rubbleVfx;
    public GameObject rubbleSmolVfx;
    public int gemCount = 3;
    public int hitpoints = 5;
    public Vector2 explosionForceMinMax = new Vector2(5f, 10f);
    
    public List<Material> gemMaterials;
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        RockHit();
    }

    private void RockHit()
    {
        Instantiate(sparkVfx, transform.position, Quaternion.identity);
        Instantiate(rubbleSmolVfx, transform.position, Quaternion.identity);
        hitpoints--;
        
        if(hitpoints <= 0)
        {
            GetComponent<Collider>().enabled = false; 
            for (int i = 0; i < gemCount; i++)
            {
                GameObject gem = Instantiate(gemPrefab, transform.position, Random.rotation);
                
                Vector3 randomDirection = Random.onUnitSphere;
                randomDirection.y = Mathf.Abs(randomDirection.y); 
                float explosionForce = Random.Range(explosionForceMinMax.x, explosionForceMinMax.y);
                var rb = gem.GetComponent<Rigidbody>();
                rb.AddForce(randomDirection * explosionForce, ForceMode.Impulse);
                rb.AddTorque(Random.onUnitSphere * explosionForce, ForceMode.Impulse);
                gem.GetComponent<Renderer>().material = gemMaterials[Random.Range(0, gemMaterials.Count)];
            }
            Instantiate(rubbleVfx, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
