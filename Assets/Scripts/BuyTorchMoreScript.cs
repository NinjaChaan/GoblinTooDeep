using System;
using Gameplay;
using UnityEngine;

public class BuyTorchMoreScript : InteractableObject
{
    public static bool IsPlayerInRange = false;
    
    
    public void Update()
    {
        if (Vector3.Distance(transform.position, PlayerScript.Instance.transform.position) <
            PlayerScript.Instance.interactRange)
        {
            IsPlayerInRange = true;
        }
        else
        {
            IsPlayerInRange = false;
        }
    }

    public override bool IsInteractable => PlayerScript.Instance.Sack.gems >= 20;
    public override InteractButton InteractButton => InteractButton.Pickup;
    
    public override void Interact()
    {
        if (PlayerScript.Instance.Sack.gems >= 20)
        {
            PlayerScript.Instance.Sack.gems -= 20;
            PlayerScript.Instance.TorchTimeLeft = PlayerScript.Instance.InitialTorchTime;
        }
    }
}
