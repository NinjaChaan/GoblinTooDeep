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
        public Sprite dropSackHint;

        public void Update()
        {
            if (PlayerScript.Instance.IsInRangeOfInteractable || PlayerScript.Instance.IsInRangeButSack)
            {
                if (!image.enabled)
                {
                    image.enabled = true;
                }

                if (PlayerScript.Instance.IsInRangeButSack)
                {
                    image.sprite = dropSackHint;
                }
                else
                {
                    if (PlayerScript.Instance.CurrentInteractButton == InteractButton.Mine)
                        image.sprite = PlayerScript.Instance.Controller.CurrentControlType == ControllingType.Keyboard
                            ? keyboardMineButtonHint
                            : gamepadMineButtonHint;
                    else
                        image.sprite = PlayerScript.Instance.Controller.CurrentControlType == ControllingType.Keyboard
                            ? keyboardPickupButtonHint
                            : gamepadPickupButtonHint;
                }
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