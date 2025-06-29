using Gameplay;
using UnityEngine;

public class SackScript : InteractableObject
{
    public float gemPickupRange = 2f;

    public int gems = 0;

    private Rigidbody rb;
    
    public AudioSource audioSource;
    public AudioClip pickupSound;
    
    private GemAmountScript gemAmountScript;
        
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gemAmountScript = FindFirstObjectByType<GemAmountScript>();
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

    public void AddGem()
    {
        gems++;
        audioSource.PlayOneShot(pickupSound);
        gemAmountScript.UpdateGemAmount(gems);
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
        
        float randomForce = Random.Range(1f, 3f);
        rb.AddForce(PlayerScript.Instance.transform.forward * -randomForce, ForceMode.VelocityChange);
    }

    public override bool IsInteractable => !PlayerScript.Instance.carryingSack;
    public override InteractButton InteractButton => InteractButton.Pickup;
    public override void Interact()
    {
        AttachToPlayer();
    }
}
