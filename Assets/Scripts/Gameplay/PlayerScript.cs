using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class PlayerScript : MonoBehaviour
    {
        public static PlayerScript Instance;

        public float interactRange = 3f;
        
        public Transform SackAttachPoint;
        public Transform HandAttachPoint;
        
        public bool carryingSack = false;
        
        
        List<InteractableObject> interactableObjects = new List<InteractableObject>();

        public bool IsInRangeOfInteractable { get; private set;}
        public InteractButton CurrentInteractButton { get; private set; }
        
        public TopDownCharacterController Controller { get; private set; }
        public SackScript Sack { get; private set; }
        
        public void Start()
        {
            Instance = this;
            
            Controller = GetComponent<TopDownCharacterController>();
            Sack = FindAnyObjectByType<SackScript>();
            
            UpdateInteractables();
            InvokeRepeating(nameof(UpdateInteractables), 1f, 1f);
        }

        public void UpdateInteractables()
        {
            interactableObjects.Clear();
            
            foreach (var interactable in FindObjectsOfType<InteractableObject>())
            {
                interactableObjects.Add(interactable);
            }
        }
        
        public void Update()
        {
            InteractableObject closestInteractable = null;
            float closestDistance = interactRange;
            float closestDotProduct = 0f;
            foreach (var interactableObject in interactableObjects)
            {
                if (!interactableObject)
                    continue;
                
                Vector3 position = interactableObject.transform.position;
                float distance = Vector3.Distance(position, transform.position);
                
                float dotProduct = Vector3.Dot(transform.forward, (position - transform.position).normalized);
                if (dotProduct < 0.25f)
                    continue;
                
                if (distance < closestDistance || dotProduct > (closestDotProduct + 0.2f))
                {
                    closestDistance = distance;
                    closestDotProduct = dotProduct;
                    closestInteractable = interactableObject;
                    CurrentInteractButton = closestInteractable.InteractButton;
                }
            }
            
            if (closestInteractable != null && !carryingSack)
            {
                IsInRangeOfInteractable = true;
            }
            else
            {
                IsInRangeOfInteractable = false;
            }


            if (IsInRangeOfInteractable)
            {
                if (Input.GetButtonDown(closestInteractable.InteractButton.ToString()))
                {
                    closestInteractable.Interact();
                }
            }
            else if (carryingSack)
            {
                if (Input.GetButtonDown(InteractButton.Pickup.ToString()))
                {
                    Sack.DetachFromPlayer();
                }
            }
        }
    }
}