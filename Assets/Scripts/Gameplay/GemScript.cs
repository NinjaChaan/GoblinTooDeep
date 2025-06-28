using System;
using Gameplay;
using UnityEngine;

public class GemScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(PlayerScript.Instance.carryingSack)
            {
                PlayerScript.Instance.Sack.AddGem();
                Destroy(gameObject);
            }
        }
    }
}
