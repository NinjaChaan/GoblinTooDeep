using Gameplay;
using UnityEngine;

public class TorchScript : InteractableObject
{
    private Rigidbody rb;
        
    void Start()
    {
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
        AttachToPlayer();
    }
}
