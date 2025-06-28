using System;
using Gameplay;
using UnityEngine;

public class GemScript : MonoBehaviour
{
    public bool isCarried = false;

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
