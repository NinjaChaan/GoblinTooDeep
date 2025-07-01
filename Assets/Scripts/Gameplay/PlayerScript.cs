using System;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

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

        
        private Animator animator;

        public float InitialTorchTime = 60 * 5;
        public float TorchTimeLeft = 0f;
        
        private Controls controls;
        
        private void Awake()
        {
            Instance = this;
            controls = new Controls();
        }
        void OnEnable() => controls.Player.Enable();
        void OnDisable() => controls.Player.Disable();

        public void Start()
        {
            animator = GetComponentInChildren<Animator>();
            Controller = GetComponent<TopDownCharacterController>();
            Sack = FindAnyObjectByType<SackScript>();
            Torch = FindAnyObjectByType<TorchScript>();

            UpdateInteractables();
            InvokeRepeating(nameof(UpdateInteractables), 1f, 1f);

            TorchTimeLeft = InitialTorchTime;
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
            var gamepad = Gamepad.current;
            if (gamepad == null) return;
            if (controls.Player.SwitchTool.triggered)
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
                bool interacted = false;
                switch (closestInteractable.InteractButton.ToString())
                {
                    case "Mine":
                        if(controls.Player.Mine.triggered)
                        {
                            interacted = true;
                        }
                        break;
                    case "Pickup":
                        if(controls.Player.Interact.triggered)
                        {
                            interacted = true;
                        }
                        break;
                    case "Torch":
                        if(controls.Player.DropTorch.triggered)
                        {
                            interacted = true;
                        }
                        break;
                    default:
                        break;
                }
                if (interacted)
                {
                    closestInteractable.Interact();
                    animator.SetTrigger("Pick");
                }
            }
            else
            {
                if (carryingSack)
                {
                    if (controls.Player.Interact.triggered)
                    {
                        Sack.DetachFromPlayer();
                    }
                }

                if (carryingTorch && !PickSelected)
                {
                    if (controls.Player.DropTorch.triggered)
                    {
                        Torch.Throw();
                    }
                }
            }

            TorchTimeLeft -= Time.deltaTime;
            Torch.GetComponent<TorchLightFlicker>().SetTorchStrength(TorchTimeLeft / InitialTorchTime);
            if (TorchTimeLeft <= 0f)
            {
                SceneManager.LoadScene(SceneManager.Instance.MainMenuScene);
            }
        }
    }
}