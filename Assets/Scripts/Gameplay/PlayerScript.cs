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
        public bool carryingTorch = false;
        public bool carryingPick = true;


        List<InteractableObject> interactableObjects = new List<InteractableObject>();

        public bool IsInRangeOfInteractable { get; private set; }
        public bool IsInRangeButSack { get; private set; }
        public InteractButton CurrentInteractButton { get; private set; }

        public TopDownCharacterController Controller { get; private set; }
        public SackScript Sack { get; private set; }
        public TorchScript Torch { get; private set; }

        public GameObject PickAxe;

        public bool PickSelected = false;

        private void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            Controller = GetComponent<TopDownCharacterController>();
            Sack = FindAnyObjectByType<SackScript>();
            Torch = FindAnyObjectByType<TorchScript>();

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
            if (Input.GetButtonDown(InteractButton.SwitchTool.ToString()))
            {
                if (PickSelected)
                {
                    PickAxe.SetActive(false);
                    Torch.ToggleTorchLight(true);
                    PickSelected = false;
                }
                else
                {
                    PickSelected = true;
                    PickAxe.SetActive(true);
                    if (carryingTorch)
                    {
                        Torch.ToggleTorchLight(false);
                    }
                }
            }

            InteractableObject closestInteractable = null;
            float closestDistance = interactRange;
            float closestDotProduct = 0f;
            foreach (var interactableObject in interactableObjects)
            {
                if (!interactableObject)
                    continue;
                if (!interactableObject.gameObject.activeSelf)
                    continue;
                if (!interactableObject.IsInteractable)
                    continue;

                Vector3 position = interactableObject.transform.position;
                float distance = Vector3.Distance(position, transform.position);

                float dotProduct = Vector3.Dot(transform.forward, (position - transform.position).normalized);
                if (dotProduct < 0.25f)
                    continue;

                if (distance < closestDistance || (distance < interactRange && dotProduct > (closestDotProduct + 0.2f)))
                {
                    closestDistance = distance;
                    closestDotProduct = dotProduct;
                    closestInteractable = interactableObject;
                    CurrentInteractButton = closestInteractable.InteractButton;
                }
            }

            bool closestIsTorch = closestInteractable != null && closestInteractable is TorchScript;
            if (closestInteractable != null && (!carryingSack || closestIsTorch))
            {
                IsInRangeOfInteractable = true;
                IsInRangeButSack = false;
            }
            else
            {
                if (closestInteractable)
                {
                    IsInRangeButSack = true;
                }
                IsInRangeOfInteractable = false;
            }


            if (IsInRangeOfInteractable)
            {
                if (Input.GetButtonDown(closestInteractable.InteractButton.ToString()))
                {
                    Debug.Log("Interacting with: " + closestInteractable.name + " at range of " + closestDistance);
                    closestInteractable.Interact();
                }
            }
            else
            {
                if (carryingSack)
                {
                    if (Input.GetButtonDown(InteractButton.Pickup.ToString()))
                    {
                        Sack.DetachFromPlayer();
                    }
                }

                if (carryingTorch && !PickSelected)
                {
                    if (Input.GetButtonDown(InteractButton.Throw.ToString()))
                    {
                        Torch.Throw();
                    }
                }
            }
        }
    }
}