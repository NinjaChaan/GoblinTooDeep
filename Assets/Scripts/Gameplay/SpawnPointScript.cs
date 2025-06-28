using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
    void Start()
    {
        TopDownCharacterController character = FindAnyObjectByType<TopDownCharacterController>();

        if (character != null)
        {
            character.transform.position = transform.position;
        }
        else
        {
            Debug.LogWarning("SpawnPointScript: No TopDownCharacterController found in the scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
