using Gameplay;
using UnityEngine;

public class TorchTimeScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, PlayerScript.Instance.TorchTimeLeft);
    }
}
