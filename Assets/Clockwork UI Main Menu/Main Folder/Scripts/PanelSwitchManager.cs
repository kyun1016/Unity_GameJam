using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoJoG
{
    public class PanelSwitchManager : MonoBehaviour
    {
        [System.Serializable]
        public class Panel
        {
            public string name;
            public GameObject panelObject;
            public Button firstSelectedButton;
            [HideInInspector] public CanvasGroup canvasGroup;
            [HideInInspector] public GameObject lastSelectedObject; // Track last selected
        }

        [Header("Fade Settings")]
        public float fadeDuration = 0.3f;
        public Panel[] panels;

        private Panel currentPanel;

        void Start()
        {
            foreach (var p in panels)
            {
                if (!p.panelObject.TryGetComponent(out CanvasGroup cg))
                    cg = p.panelObject.AddComponent<CanvasGroup>();

                p.canvasGroup = cg;
                p.panelObject.SetActive(false);
                p.lastSelectedObject = p.firstSelectedButton?.gameObject; // Initialize
            }

            if (panels.Length > 0)
                ShowPanel(panels[0].name);
        }

        public void ShowPanel(string panelName)
        {
            Panel nextPanel = System.Array.Find(panels, p => p.name == panelName);
            if (nextPanel == null)
            {
                Debug.LogWarning($"Panel '{panelName}' not found.");
                return;
            }

            // Store current selection before switching
            if (currentPanel != null)
            {
                currentPanel.lastSelectedObject = EventSystem.current.currentSelectedGameObject;
            }

            StopAllCoroutines();

            if (currentPanel != null)
                StartCoroutine(FadeOut(currentPanel));

            currentPanel = nextPanel;
            currentPanel.panelObject.SetActive(true);
            StartCoroutine(FadeIn(currentPanel));
        }

        IEnumerator FadeOut(Panel panel)
        {
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                panel.canvasGroup.alpha = 1 - (t / fadeDuration);
                yield return null;
            }

            panel.canvasGroup.alpha = 0;
            panel.panelObject.SetActive(false);
        }

        IEnumerator FadeIn(Panel panel)
        {
            panel.canvasGroup.alpha = 0;
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                panel.canvasGroup.alpha = t / fadeDuration;
                yield return null;
            }

            panel.canvasGroup.alpha = 1;

            // Select either the last selected object or the default first button
            GameObject objectToSelect = panel.lastSelectedObject != null ?
                panel.lastSelectedObject :
                panel.firstSelectedButton?.gameObject;

            if (objectToSelect != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                yield return null; // Wait one frame
                EventSystem.current.SetSelectedGameObject(objectToSelect);

                // Handle sound suppression for the first selection only
                if (objectToSelect == panel.firstSelectedButton?.gameObject)
                {
                    var menuButtonSound = objectToSelect.GetComponent<MenuButtonSound>();
                    if (menuButtonSound != null)
                    {
                        menuButtonSound.suppressNextSound = true;
                    }
                }
            }
        }

        // Button triggers remain the same
        public void ShowMainMenu() => ShowPanel("MainMenu");
        public void ShowSettings() => ShowPanel("Settings");
        public void ShowCredits() => ShowPanel("Credits");
        public void ShowQuit() => ShowPanel("Quit");

        public void QuitGame()
        {
            Debug.Log("Quitting Game...");
            Application.Quit();
        }
    }

    [RequireComponent(typeof(Button))]
    public class MenuButtonSound : MonoBehaviour, ISelectHandler
    {
        public AudioClip selectionSound;
        public AudioSource audioSource;

        [HideInInspector]
        public bool suppressNextSound = false;

        public void OnSelect(BaseEventData eventData)
        {
            if (suppressNextSound)
            {
                suppressNextSound = false;
                return;
            }

            if (selectionSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(selectionSound);
            }
        }
    }
}