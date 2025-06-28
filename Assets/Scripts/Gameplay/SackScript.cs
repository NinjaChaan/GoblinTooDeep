using Gameplay;
using UnityEngine;

public class SackScript : InteractableObject
{
    public float gemPickupRange = 2f;

    public int gems = 0;

    private Rigidbody rb;
        
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        AttachToPlayer();
    }

    void Update()
    {
        if (transform.position.y < -10f)
        {
            Debug.LogWarning("SackScript: Sack has fallen out of the world!");
            AttachToPlayer();
        }
    }

    public void AttachToPlayer()
    {
        transform.parent = PlayerScript.Instance.SackAttachPoint;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        PlayerScript.Instance.carryingSack = true;
        rb.isKinematic = true;
    }
    
    public void DetachFromPlayer()
    {
        transform.parent = null;
        PlayerScript.Instance.carryingSack = false;
        rb.isKinematic = false;
    }

    public override bool IsInteractable => !PlayerScript.Instance.carryingSack;
    public override InteractButton InteractButton => InteractButton.Pickup;
    public override void Interact()
    {
        AttachToPlayer();
    }
}
