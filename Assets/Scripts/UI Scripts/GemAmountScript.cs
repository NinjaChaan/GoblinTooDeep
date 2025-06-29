using Gameplay;
using UnityEngine;

public class GemAmountScript : MonoBehaviour
{
    public TMPro.TextMeshProUGUI gemAmountText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGemAmount(PlayerScript.Instance.Sack.gems);
    }
    
    public void UpdateGemAmount(int amount)
    {
        gemAmountText.text = amount.ToString();
    }
}
