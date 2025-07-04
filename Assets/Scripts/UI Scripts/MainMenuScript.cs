using System;
using UnityEngine;
public class MainMenuScript : MonoBehaviour
{
    string[] joystickButtons = {
        "joystick button 0", "joystick button 1", "joystick button 2",
        "joystick button 3", "joystick button 4", "joystick button 5",
        "joystick button 6", "joystick button 7", "joystick button 8",
        "joystick button 9", "joystick button 10", "joystick button 11",
        "joystick button 12", "joystick button 13", "joystick button 14",
        "joystick button 15", "joystick button 16", "joystick button 17",
        "joystick button 18", "joystick button 19"
    };

    bool AnyJoystickButtonDown()
    {
        foreach (string button in joystickButtons)
        {
            if (Input.GetKeyDown(button))
                return true;
        }
        return false;
    }
    private void Update()
    {
        if (Input.anyKeyDown || AnyJoystickButtonDown())
        {
            Play();
        }
    }

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.Instance.TutorialScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
