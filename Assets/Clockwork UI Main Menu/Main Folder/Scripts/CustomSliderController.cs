using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SoJoG
{
    public class CustomSliderController : MonoBehaviour
    {
        [Header("References")]
        public Slider slider;
        public Button controllerButton;
        public TextMeshProUGUI valueText;
        public AudioSource audioSource;
        public AudioClip changeSound;

        [Header("Settings")]
        public float step = 0.1f;
        public float defaultValue = 0.5f;
        public string valueFormat = "0.00";

        [Header("Haptics")]
        public float hapticLow = 0.2f;
        public float hapticHigh = 0.2f;
        public float hapticDuration = 0.1f;

        private bool isSelected;
        private float hapticTimer;
        private Gamepad gamepad;

        void Start()
        {
            gamepad = Gamepad.current;
            slider.value = Mathf.Clamp(defaultValue, slider.minValue, slider.maxValue);
            UpdateValueText(slider.value);
        }

        void Update()
        {
            isSelected = EventSystem.current.currentSelectedGameObject == controllerButton.gameObject;

            if (!isSelected) return;

            float move = 0f;

            if (Keyboard.current.leftArrowKey.wasPressedThisFrame || gamepad?.dpad.left.wasPressedThisFrame == true)
                move = -step;
            else if (Keyboard.current.rightArrowKey.wasPressedThisFrame || gamepad?.dpad.right.wasPressedThisFrame == true)
                move = step;

            if (move != 0)
            {
                float newValue = Mathf.Clamp(slider.value + move, slider.minValue, slider.maxValue);
                if (newValue != slider.value)
                {
                    slider.value = newValue;
                }
            }

            // Haptic timer update
            if (hapticTimer > 0f)
            {
                hapticTimer -= Time.unscaledDeltaTime;
                if (hapticTimer <= 0f)
                {
                    gamepad?.SetMotorSpeeds(0, 0);
                }
            }
        }

        public void OnSliderValueChanged(float newValue)
        {
            PlayValueChangeSound();
            UpdateValueText(newValue);
            TriggerHaptic();
        }

        private void PlayValueChangeSound()
        {
            if (audioSource && changeSound)
                audioSource.PlayOneShot(changeSound);
        }

        private void UpdateValueText(float value)
        {
            if (valueText)
                valueText.text = value.ToString(valueFormat);
        }

        private void TriggerHaptic()
        {
            if (gamepad == null) return;

            gamepad.SetMotorSpeeds(hapticLow, hapticHigh);
            hapticTimer = hapticDuration;
        }
    }
}
