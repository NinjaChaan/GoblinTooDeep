using UnityEngine;

namespace Gameplay
{
    public abstract class InteractableObject: MonoBehaviour
    {
        public virtual bool IsInteractable { get; } = true;
        public abstract InteractButton InteractButton { get; }
        
        public abstract void Interact();
    }
}