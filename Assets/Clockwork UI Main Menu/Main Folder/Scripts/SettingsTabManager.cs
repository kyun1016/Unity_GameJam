using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

namespace SoJoG
{
    public class SettingsTabManager : MonoBehaviour
    {
        [System.Serializable]
        public class Tab
        {
            [Header("Tab Name")]
            public string tabName;

            [Header("UI Elements")]
            public TextMeshProUGUI label;
            public GameObject underline;
            public GameObject contentPanel;
            public GameObject firstSelectedButton;
        }

        [Header("Tabs Setup")]
        public Tab[] tabs;
        [Tooltip("Tab index to use the first time the panel opens.")]
        public int defaultTabIndex = 0;

        [Header("Text Colors")]
        public Color selectedColor = Color.white;
        public Color normalColor = Color.gray;

        [Header("Input Actions")]
        public InputActionReference navigateLeft;
        public InputActionReference navigateRight;

        private int currentTabIndex = 0;
        private bool hasOpenedBefore = false;

        private void OnEnable()
        {
            // Select default tab the first time, else restore last one
            if (!hasOpenedBefore)
            {
                SelectTab(defaultTabIndex);
                hasOpenedBefore = true;
            }
            else
            {
                SelectTab(currentTabIndex);
            }

            navigateLeft.action.performed += OnNavigateLeft;
            navigateRight.action.performed += OnNavigateRight;

            navigateLeft.action.Enable();
            navigateRight.action.Enable();

            // Optional: force selection after frame (useful if other UI is lingering)
            StartCoroutine(ForceSelectCurrentTabButton());
        }

        private void OnDisable()
        {
            navigateLeft.action.performed -= OnNavigateLeft;
            navigateRight.action.performed -= OnNavigateRight;

            navigateLeft.action.Disable();
            navigateRight.action.Disable();
        }

        private void OnNavigateLeft(InputAction.CallbackContext ctx)
        {
            currentTabIndex = (currentTabIndex - 1 + tabs.Length) % tabs.Length;
            SelectTab(currentTabIndex);
        }

        private void OnNavigateRight(InputAction.CallbackContext ctx)
        {
            currentTabIndex = (currentTabIndex + 1) % tabs.Length;
            SelectTab(currentTabIndex);
        }

        private void SelectTab(int index)
        {
            for (int i = 0; i < tabs.Length; i++)
            {
                bool isSelected = i == index;

                tabs[i].label.color = isSelected ? selectedColor : normalColor;

                if (tabs[i].underline != null)
                    tabs[i].underline.SetActive(isSelected);

                if (tabs[i].contentPanel != null)
                    tabs[i].contentPanel.SetActive(isSelected);
            }

            currentTabIndex = index;

            // Set selection instantly for runtime tab switching
            if (tabs[index].firstSelectedButton != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(tabs[index].firstSelectedButton);
            }
        }

        private IEnumerator ForceSelectCurrentTabButton()
        {
            yield return null; // Wait 1 frame for UI to settle

            var currentTab = tabs[currentTabIndex];
            if (currentTab.firstSelectedButton != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(currentTab.firstSelectedButton);
            }
        }
    }
}


