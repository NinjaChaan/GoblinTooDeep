using Gameplay;
using UnityEngine;

public class TorchScript : InteractableObject
{
    private Rigidbody rb;
    private TorchLightFlicker torchLightFlicker;
        
    void Start()
    {
        torchLightFlicker = GetComponentInChildren<TorchLightFlicker>();
        rb = GetComponent<Rigidbody>();
        AttachToPlayer();
    }

    void Update()
    {
        
    }

    public void AttachToPlayer()
    {
        transform.parent = PlayerScript.Instance.HandAttachPoint;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        PlayerScript.Instance.carryingTorch = true;
        rb.isKinematic = true;

        if (PlayerScript.Instance.PickSelected)
        {
            gameObject.SetActive(false);
            torchLightFlicker.SetLightOnOff(false);
        }
    }
    
    public void Throw()
    {
        transform.parent = null;
        PlayerScript.Instance.carryingTorch = false;
        rb.isKinematic = false;
        rb.AddForce((PlayerScript.Instance.transform.forward + (Vector3.up*0.1f)) * 5f, ForceMode.Impulse);
    }

    public override bool IsInteractable => !PlayerScript.Instance.carryingTorch;
    public override InteractButton InteractButton => InteractButton.Pickup;
    public override void Interact()
    {
        // ToggleTorchLight(true);
        AttachToPlayer();
    }
    
    public void ToggleTorchLight(bool isOn)
    {
        torchLightFlicker.SetLightOnOff(isOn);
        gameObject.SetActive(isOn);
    }
}
