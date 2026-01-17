using UnityEngine;
using UnityEngine.UI;

namespace SoJoG
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class BringToFront : MonoBehaviour
    {
        private Canvas parentCanvas;
        private Canvas myCanvas;

        void Awake()
        {
            // Get the highest parent canvas
            parentCanvas = GetComponentInParent<Canvas>().rootCanvas;
            myCanvas = GetComponent<Canvas>();

            // Initialize settings
            myCanvas.overrideSorting = true;
        }

        void OnEnable()
        {
            // When enabled, set sorting order to be above everything
            if (parentCanvas != null)
            {
                myCanvas.sortingOrder = parentCanvas.sortingOrder + 1;
            }
        }
    }
}