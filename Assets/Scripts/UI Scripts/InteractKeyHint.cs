using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Scripts
{
    public class InteractKeyHint : MonoBehaviour
    {
        public Image image;
        public Sprite keyboardMineButtonHint;
        public Sprite gamepadMineButtonHint;
        public Sprite keyboardPickupButtonHint;
        public Sprite gamepadPickupButtonHint;

        public void Update()
        {
            if (PlayerScript.Instance.IsInRangeOfInteractable && !PlayerScript.Instance.carryingSack)
            {
                if (!image.enabled)
                {
                    image.enabled = true;
                }
                
                if (PlayerScript.Instance.CurrentInteractButton == InteractButton.Mine)
                    image.sprite = PlayerScript.Instance.Controller.CurrentControlType == ControllingType.Keyboard
                        ? keyboardMineButtonHint
                        : gamepadMineButtonHint;
                else
                    image.sprite = PlayerScript.Instance.Controller.CurrentControlType == ControllingType.Keyboard
                        ? keyboardPickupButtonHint
                        : gamepadPickupButtonHint;
            }
            else
            {
                if (image.enabled)
                {
                    image.enabled = false;
                }
            }
        }
    }
}