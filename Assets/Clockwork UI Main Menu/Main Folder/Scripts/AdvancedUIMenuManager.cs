using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace SoJoG
{
    [RequireComponent(typeof(AudioSource))]
    public class AdvancedUIMenuManager : MonoBehaviour
    {
        [Header("UI Element Names")]
        public string underlineName = "Underline";
        public string flameLocationName = "Selection Flame Location";
        public Transform panelsRoot;

        [Header("Description UI")]
        public TMP_Text descriptionTextUI;

        [System.Serializable]
        public class MenuPanel
        {
            public string panelName;
            public GameObject panelObject;
            public List<MenuButton> buttons = new List<MenuButton>();

            [System.Serializable]
            public class Category
            {
                public string name;
                public Button firstSelected;
                public List<MenuButton> buttons = new List<MenuButton>();
            }

            public List<Category> categories = new List<Category>();
        }

        [System.Serializable]
        public class MenuButton
        {
            public Button button;
            public GameObject underline;
            public Transform selectionFlameLocation;
            public string description;

            public bool hasDescription = true; // ✅ New toggle to control description update

            [HideInInspector] public int panelIndex;
            [HideInInspector] public int categoryIndex = -1;
        }

        [Header("UI Components")]
        public List<MenuPanel> panels = new List<MenuPanel>();
        public RectTransform selectionFlame;

        [Header("Flame Animation Settings")]
        public float flameTweenDuration = 0.2f;
        public float flameSelectionDelay = 0.1f;
        public Ease flameTweenEase = Ease.OutQuint;

        [Header("Audio")]
        public AudioClip buttonSelectSound;
        public AudioClip buttonPressSound;
        [Range(0, 1)] public float volume = 1f;

        private AudioSource audioSource;
        private Coroutine currentFlameMovement;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = volume;

            if (panelsRoot != null && panels.Count == 0)
            {
                DetectPanelsAndButtons();
            }

            InitializeButtons();
        }

        public void DetectPanelsAndButtons()
        {
            panels.Clear();
            if (panelsRoot == null)
            {
                Debug.LogWarning("PanelsRoot is not assigned! Cannot detect panels.");
                return;
            }

            foreach (Transform panelTransform in panelsRoot)
            {
                var panel = new MenuPanel
                {
                    panelName = panelTransform.name,
                    panelObject = panelTransform.gameObject,
                    buttons = new List<MenuButton>()
                };

                Button[] buttonsInPanel = panelTransform.GetComponentsInChildren<Button>(true);
                foreach (var btn in buttonsInPanel)
                {
                    Transform underlineT = btn.transform.FindDeepChild(underlineName);
                    Transform flameLocationT = btn.transform.FindDeepChild(flameLocationName);

                    var menuButton = new MenuButton
                    {
                        button = btn,
                        underline = underlineT != null ? underlineT.gameObject : null,
                        selectionFlameLocation = flameLocationT,
                        description = ""
                    };
                    panel.buttons.Add(menuButton);
                }

                panels.Add(panel);
            }
        }

        public void InitializeButtons()
        {
            for (int i = 0; i < panels.Count; i++)
            {
                if (panels[i] == null) continue;

                foreach (var button in panels[i].buttons)
                {
                    if (button != null)
                    {
                        button.panelIndex = i;
                        SetupButtonEvents(button);
                    }
                }
            }
        }

        private void SetupButtonEvents(MenuButton menuButton)
        {
            if (menuButton?.button == null) return;

            var existingTrigger = menuButton.button.GetComponent<EventTrigger>();
            if (existingTrigger != null)
            {
                DestroyImmediate(existingTrigger);
            }

            var trigger = menuButton.button.gameObject.AddComponent<EventTrigger>();

            var selectEntry = new EventTrigger.Entry();
            selectEntry.eventID = EventTriggerType.Select;
            selectEntry.callback.AddListener((data) => OnButtonSelected(menuButton));
            trigger.triggers.Add(selectEntry);

            menuButton.button.onClick.AddListener(() => OnButtonPressed(menuButton));

            if (menuButton.underline != null)
            {
                menuButton.underline.SetActive(false);
            }
        }

        private void OnButtonSelected(MenuButton menuButton, bool suppressSound = false)
        {
            if (menuButton?.button == null) return;

            if (!suppressSound && buttonSelectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(buttonSelectSound);
            }

            if (menuButton.underline != null)
            {
                menuButton.underline.SetActive(true);
            }

            if (descriptionTextUI != null && menuButton.hasDescription)
            {
                descriptionTextUI.text = menuButton.description ?? "";
            }

            if (currentFlameMovement != null)
            {
                StopCoroutine(currentFlameMovement);
            }
            currentFlameMovement = StartCoroutine(MoveFlameDelayed(menuButton));

            HideOtherUnderlines(menuButton);
        }

        private IEnumerator MoveFlameDelayed(MenuButton menuButton)
        {
            yield return new WaitForSeconds(flameSelectionDelay);

            if (menuButton?.selectionFlameLocation != null && selectionFlame != null)
            {
                selectionFlame.DOMove(menuButton.selectionFlameLocation.position, flameTweenDuration)
                    .SetEase(flameTweenEase);
            }
        }

        private void HideOtherUnderlines(MenuButton currentButton)
        {
            if (currentButton?.panelIndex < 0 || currentButton.panelIndex >= panels.Count) return;
            var panel = panels[currentButton.panelIndex];
            if (panel == null) return;

            foreach (var button in panel.buttons)
            {
                if (button != null && button != currentButton && button.underline != null)
                {
                    button.underline.SetActive(false);
                }
            }
        }

        private void OnButtonPressed(MenuButton menuButton)
        {
            if (menuButton?.button == null) return;

            if (buttonPressSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(buttonPressSound);
            }
        }
    }

    [CustomEditor(typeof(AdvancedUIMenuManager))]
    public class AdvancedUIMenuManagerEditor : Editor
    {
        private SerializedProperty panelsProp;
        private GUIStyle boxStyle;
        private GUIStyle foldoutStyle;
        private GUIStyle headerStyle;

        private bool[] panelFoldouts;
        private bool[][] buttonFoldouts;

        private void OnEnable()
        {
            panelsProp = serializedObject.FindProperty("panels");
            UpdateFoldoutArrays();
        }

        private void UpdateFoldoutArrays()
        {
            int panelCount = panelsProp != null ? panelsProp.arraySize : 0;

            if (panelFoldouts == null || panelFoldouts.Length != panelCount)
            {
                panelFoldouts = new bool[panelCount];
            }

            if (buttonFoldouts == null || buttonFoldouts.Length != panelCount)
            {
                buttonFoldouts = new bool[panelCount][];
            }

            for (int i = 0; i < panelCount; i++)
            {
                var panelProp = panelsProp.GetArrayElementAtIndex(i);
                if (panelProp == null) continue;

                var buttonsProp = panelProp.FindPropertyRelative("buttons");
                int buttonCount = buttonsProp != null ? buttonsProp.arraySize : 0;

                if (buttonFoldouts[i] == null || buttonFoldouts[i].Length != buttonCount)
                {
                    buttonFoldouts[i] = new bool[buttonCount];
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            InitStyles();

            GUILayout.Space(10);
            GUILayout.Label("Advanced UI Menu Manager", headerStyle);
            GUILayout.Space(8);

            GUILayout.Space(15);

            if (panelsProp == null)
            {
                EditorGUILayout.HelpBox("Panels list not found.", MessageType.Error);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            UpdateFoldoutArrays();

            for (int i = 0; i < panelsProp.arraySize; i++)
            {
                var panelProp = panelsProp.GetArrayElementAtIndex(i);
                if (panelProp == null) continue;

                var panelNameProp = panelProp.FindPropertyRelative("panelName");
                string panelTitle = string.IsNullOrEmpty(panelNameProp.stringValue) ? $"Panel {i + 1}" : panelNameProp.stringValue;

                EditorGUILayout.BeginVertical(boxStyle);
                panelFoldouts[i] = EditorGUILayout.Foldout(panelFoldouts[i], panelTitle, true, foldoutStyle);
                if (panelFoldouts[i])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(panelNameProp);
                    EditorGUILayout.PropertyField(panelProp.FindPropertyRelative("panelObject"));

                    SerializedProperty buttonsProp = panelProp.FindPropertyRelative("buttons");
                    if (buttonsProp != null)
                    {
                        EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
                        for (int b = 0; b < buttonsProp.arraySize; b++)
                        {
                            if (b >= buttonFoldouts[i].Length) continue;

                            var buttonProp = buttonsProp.GetArrayElementAtIndex(b);
                            if (buttonProp == null) continue;

                            var buttonNameProp = buttonProp.FindPropertyRelative("button");
                            string buttonName = buttonNameProp.objectReferenceValue != null ? ((Button)buttonNameProp.objectReferenceValue).gameObject.name : $"Button {b + 1}";

                            buttonFoldouts[i][b] = EditorGUILayout.Foldout(buttonFoldouts[i][b], buttonName, true, foldoutStyle);
                            if (buttonFoldouts[i][b])
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(buttonProp.FindPropertyRelative("button"));
                                EditorGUILayout.PropertyField(buttonProp.FindPropertyRelative("underline"));
                                EditorGUILayout.PropertyField(buttonProp.FindPropertyRelative("selectionFlameLocation"));
                                EditorGUILayout.PropertyField(buttonProp.FindPropertyRelative("description"));
                                EditorGUILayout.PropertyField(buttonProp.FindPropertyRelative("hasDescription")); // ✅ new field shown
                                EditorGUI.indentLevel--;
                            }
                        }
                    }

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Auto Detect Panels and Buttons", GUILayout.Height(30)))
            {
                ((AdvancedUIMenuManager)target).DetectPanelsAndButtons();
                serializedObject.Update();
                UpdateFoldoutArrays();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("UI Element Names", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("underlineName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("flameLocationName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("panelsRoot"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Flame Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("selectionFlame"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("flameTweenDuration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("flameSelectionDelay"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("flameTweenEase"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Audio Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("buttonSelectSound"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("buttonPressSound"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("volume"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Description UI", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("descriptionTextUI"));

            serializedObject.ApplyModifiedProperties();
        }

        private void InitStyles()
        {
            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(10, 10, 10, 10),
                    margin = new RectOffset(5, 5, 5, 5)
                };
            }

            if (foldoutStyle == null)
            {
                foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 12
                };
            }

            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 16,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }
        }
    }

    public static class TransformExtensions
    {
        public static Transform FindDeepChild(this Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;

                var result = child.FindDeepChild(name);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
