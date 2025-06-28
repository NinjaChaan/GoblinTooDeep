using System;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    
    public AudioSource audioSource;
    public AudioClip ClickSound;
    public AudioClip HoverSound;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHover()
    {
        audioSource.PlayOneShot(HoverSound);
    }

    public void OnClick()
    {
        audioSource.PlayOneShot(ClickSound);
    }
}
