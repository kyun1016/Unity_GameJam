using UnityEngine;
using UnityEngine.EventSystems;

namespace SoJoG
{
    public class ButtonSelectionActivator : MonoBehaviour
    {
        [Header("Target Object To Toggle")]
        public GameObject targetObject;

        private GameObject lastSelected;

        void Start()
        {
            if (targetObject != null)
                targetObject.SetActive(false);
        }

        void Update()
        {
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

            if (currentSelected != lastSelected)
            {
                if (currentSelected == gameObject)
                {
                    targetObject?.SetActive(true);
                }
                else
                {
                    targetObject?.SetActive(false);
                }

                lastSelected = currentSelected;
            }
        }
    }
}
