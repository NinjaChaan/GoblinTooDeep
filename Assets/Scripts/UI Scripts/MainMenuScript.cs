using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        // if (Input.GetButtonUp("Submit"))
        // {
        //     SceneManager.LoadScene(SceneManager.Instance.FirstRoom);
        // }
    }

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.Instance.FirstRoom);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
