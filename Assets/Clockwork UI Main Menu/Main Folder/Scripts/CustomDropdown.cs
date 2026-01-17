using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

namespace SoJoG
{
    public class CustomDropdown : MonoBehaviour
    {
        [Header("UI Text")]
        public TMP_Text mainLabelText; // If using legacy Text

        [Header("Main References")]
        public Button mainButton;
        public GameObject optionsContainer;
        public Button defaultSelectedOption;
        public RectTransform dropdownArrow;

        [Header("External References")]
        public GameObject settingsObject; // The object with InputActionPerformer

        [Header("Settings")]
        public bool startHidden = true;

        private bool isOpen = false;
        private Selectable[] allUISelectables;
        private Button lastSelectedOption;
        private MonoBehaviour inputActionPerformer;
        private InputAction cancelAction;

        private List<ScrollRectAutoScroll> disabledAutoScrolls = new List<ScrollRectAutoScroll>();

        private void Start()
        {
            if (startHidden)
                optionsContainer.SetActive(false);

            mainButton.onClick.AddListener(ToggleOptions);

            allUISelectables = GameObject.FindObjectsByType<Selectable>(FindObjectsSortMode.None);

            Button[] optionButtons = optionsContainer.GetComponentsInChildren<Button>();
            foreach (Button btn in optionButtons)
            {
                btn.onClick.AddListener(() => OnOptionSelected(btn));
            }

            if (defaultSelectedOption != null)
                lastSelectedOption = defaultSelectedOption;

            if (settingsObject != null)
                inputActionPerformer = settingsObject.GetComponent("InputActionPerformer") as MonoBehaviour;

            // Cancel input (ESC or B)
            cancelAction = new InputAction("Cancel", binding: "<Gamepad>/buttonEast");
            cancelAction.AddBinding("<Keyboard>/escape");
            cancelAction.performed += ctx =>
            {
                if (isOpen)
                    HideOptions();
            };
            cancelAction.Enable();
        }

        private void OnDestroy()
        {
            cancelAction?.Disable();
            cancelAction?.Dispose();
        }

        public void ToggleOptions()
        {
            if (isOpen)
                HideOptions();
            else
                ShowOptions();
        }

        public void ShowOptions()
        {
            isOpen = true;
            optionsContainer.SetActive(true);

            if (lastSelectedOption != null)
                EventSystem.current.SetSelectedGameObject(lastSelectedOption.gameObject);

            if (dropdownArrow != null)
                dropdownArrow.Rotate(0, 0, 180);

            LockNavigationExcept(optionsContainer);

            if (inputActionPerformer != null)
                inputActionPerformer.enabled = false;

            // Disable all active ScrollRectAutoScroll scripts
            disabledAutoScrolls.Clear();
#pragma warning disable CS0618 // Type or member is obsolete
            foreach (var scroll in FindObjectsOfType<ScrollRectAutoScroll>())
#pragma warning restore CS0618 // Type or member is obsolete
            {
                if (scroll.enabled)
                {
                    scroll.enabled = false;
                    disabledAutoScrolls.Add(scroll);
                }
            }
        }

        public void HideOptions()
        {
            isOpen = false;
            optionsContainer.SetActive(false);

            if (dropdownArrow != null)
                dropdownArrow.Rotate(0, 0, 180);

            UnlockNavigation();

            if (mainButton != null)
                EventSystem.current.SetSelectedGameObject(mainButton.gameObject);

            if (inputActionPerformer != null)
                inputActionPerformer.enabled = true;

            // Re-enable only the ones we disabled before
            foreach (var scroll in disabledAutoScrolls)
            {
                if (scroll != null)
                    scroll.enabled = true;
            }
            disabledAutoScrolls.Clear();
        }

        private void OnOptionSelected(Button selectedButton)
        {
            lastSelectedOption = selectedButton;

            // ✅ Update main label with TMP text
            TMP_Text label = selectedButton.GetComponentInChildren<TMP_Text>();
            if (label != null && mainLabelText != null)
            {
                mainLabelText.text = label.text;
            }

            OptionEventTrigger trigger = selectedButton.GetComponent<OptionEventTrigger>();
            if (trigger != null)
                trigger.onSelect.Invoke();

            HideOptions();
        }


        private void LockNavigationExcept(GameObject allowedContainer)
        {
            foreach (Selectable ui in allUISelectables)
            {
                if (!ui.transform.IsChildOf(allowedContainer.transform) && ui.gameObject != mainButton.gameObject)
                    ui.interactable = false;
            }

            if (mainButton != null)
                mainButton.interactable = false;
        }

        private void UnlockNavigation()
        {
            foreach (Selectable ui in allUISelectables)
                ui.interactable = true;

            if (mainButton != null)
                mainButton.interactable = true;
        }
    }
}
