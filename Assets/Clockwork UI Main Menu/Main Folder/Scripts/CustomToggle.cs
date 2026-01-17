using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace SoJoG
{
    public class CustomToggle : MonoBehaviour
    {
        [Header("Visual References")]
        [SerializeField] private GameObject onObject;
        [SerializeField] private GameObject offObject;
        [SerializeField] private TMP_Text onText;
        [SerializeField] private TMP_Text offText;

        [Header("Color Settings")]
        [SerializeField] private Color onColor = Color.white;
        [SerializeField] private Color offColor = Color.gray;

        [Header("Initial State")]
        [SerializeField] private bool isOnByDefault = false;

        [Header("Toggle Events")]
        [SerializeField] private UnityEvent OnToggleOn;
        [SerializeField] private UnityEvent OnToggleOff;

        private bool isOn;

        private void Awake()
        {
            isOn = isOnByDefault;
            UpdateVisuals();
            TriggerInitialEvent();
        }

        private void TriggerInitialEvent()
        {
            if (isOn) OnToggleOn?.Invoke();
            else OnToggleOff?.Invoke();
        }

        public void Toggle()
        {
            isOn = !isOn;
            UpdateVisuals();
            TriggerEvents();
        }

        public void SetToggleState(bool state)
        {
            if (isOn != state)
            {
                isOn = state;
                UpdateVisuals();
                TriggerEvents();
            }
        }

        private void UpdateVisuals()
        {
            if (onObject != null) onObject.SetActive(isOn);
            if (offObject != null) offObject.SetActive(!isOn);

            if (onText != null) onText.color = isOn ? onColor : offColor;
            if (offText != null) offText.color = !isOn ? onColor : offColor;
        }

        private void TriggerEvents()
        {
            if (isOn) OnToggleOn?.Invoke();
            else OnToggleOff?.Invoke();
        }

        public bool IsOn => isOn;
    }
}