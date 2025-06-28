using UnityEngine;

namespace Gameplay
{
    public class PlayerScript : MonoBehaviour
    {
        public static PlayerScript Instance;
        
        public Transform SackAttachPoint;
        public Transform HandAttachPoint;
        
        public bool carryingSack = false;

        public void Start()
        {
            Instance = this;
        }

    }
}