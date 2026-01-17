using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoJoG
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectAutoScroll : MonoBehaviour
    {
        public float scrollSpeed = 10f;

        private List<Selectable> m_Selectables = new List<Selectable>();
        private ScrollRect m_ScrollRect;

        private Vector2 m_TargetScrollPosition = Vector2.up;

        void Awake()
        {
            m_ScrollRect = GetComponent<ScrollRect>();
        }

        void Start()
        {
            RefreshSelectables();
            ScrollToSelected(true);
        }

        void Update()
        {
            if (m_Selectables.Count == 0) return;

            GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
            if (selectedObj != null && selectedObj.GetComponent<Selectable>() != null)
            {
                ScrollToSelected(false);
            }

            // Smooth scroll
            m_ScrollRect.normalizedPosition = Vector2.Lerp(
                m_ScrollRect.normalizedPosition,
                m_TargetScrollPosition,
                Time.unscaledDeltaTime * scrollSpeed
            );
        }

        void RefreshSelectables()
        {
            m_Selectables.Clear();
            m_ScrollRect.content.GetComponentsInChildren(true, m_Selectables);
        }

        void ScrollToSelected(bool quickScroll)
        {
            GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
            if (selectedObj == null) return;

            Selectable selected = selectedObj.GetComponent<Selectable>();
            if (selected == null || !m_Selectables.Contains(selected)) return;

            int index = m_Selectables.IndexOf(selected);
            if (index < 0) return;

            float t = 1f - (index / (float)(m_Selectables.Count - 1));
            Vector2 target = new Vector2(0, Mathf.Clamp01(t));

            if (quickScroll)
            {
                m_ScrollRect.normalizedPosition = target;
                m_TargetScrollPosition = target;
            }
            else
            {
                m_TargetScrollPosition = target;
            }
        }
    }

}

