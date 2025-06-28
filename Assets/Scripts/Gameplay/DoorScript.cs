using System;
using DefaultNamespace;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public SceneDescriptor NextScene;

    private void Start()
    {
        NextScene = GameProgressManager.Instance.GetNextScene();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameProgressManager.Instance.CurrentLevel++;
            SceneManager.LoadScene(NextScene);
        }
    }
}
