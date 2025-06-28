using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Jump"))
        {
            SceneManager.LoadScene(SceneManager.Instance.FirstRoom);
        }
    }
}
