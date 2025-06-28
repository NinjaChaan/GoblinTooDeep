using System;
using DefaultNamespace;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public SceneDescriptor NextScene;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(NextScene);
        }
    }
}
