using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SoJoG
{
    public class MenuHapticFeedback : MonoBehaviour
    {
        [Header("Haptic Settings")]
        public float lowFrequency = 0.2f;
        public float highFrequency = 0.2f;
        public float hapticDuration = 0.1f;

        private Gamepad gamepad;
        private float timer;

        void Start()
        {
            gamepad = Gamepad.current;
        }

        void Update()
        {
            if (timer > 0f)
            {
                timer -= Time.unscaledDeltaTime;
                if (timer <= 0f && gamepad != null)
                    gamepad.SetMotorSpeeds(0, 0);
            }

            // Auto-detect UI Button presses (mouse or controller)
            if (Mouse.current.leftButton.wasPressedThisFrame || Gamepad.current?.buttonSouth.wasPressedThisFrame == true)
            {
                var selected = EventSystem.current.currentSelectedGameObject;
                if (selected && selected.GetComponent<Button>())
                {
                    TriggerSoftHaptic();
                }
            }
        }

        public void TriggerSoftHaptic()
        {
            if (gamepad == null) return;

            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            timer = hapticDuration;
        }

        // Call from Slider OnValueChanged event
        public void OnSliderChanged()
        {
            TriggerSoftHaptic();
        }
    }
}
