using TMPro;
using UnityEngine;

public class BuyMoreTorchUIScript : MonoBehaviour
{
    private TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (BuyTorchMoreScript.IsPlayerInRange)
        {
            text.enabled = true;
            text.text = $"Buy More Torch Time \\n( {FindAnyObjectByType<BuyTorchMoreScript>().torchCost} HRT )";
        }
        else
        {
            text.enabled = false;
        }
    }
}
