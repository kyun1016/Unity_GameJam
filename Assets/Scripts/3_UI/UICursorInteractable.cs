using UnityEngine;
using UnityEngine.EventSystems;

public class UICursorInteractable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Cursor Types")]
    [SerializeField] private Enum.CursorType _hoverCursor = Enum.CursorType.Hover;
    [SerializeField] private Enum.CursorType _clickCursor = Enum.CursorType.Click;

    private bool _isHovering = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovering = true;
        CursorManager.Instance.SetCursor(_hoverCursor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovering = false;
        CursorManager.Instance.ResetCursor();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CursorManager.Instance.SetCursor(_clickCursor);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isHovering)
        {
            CursorManager.Instance.SetCursor(_hoverCursor);
        }
        else
        {
            CursorManager.Instance.ResetCursor();
        }
    }

    private void OnDisable()
    {
        // 비활성화 될 때 커서가 꼬이지 않도록 초기화
        if (_isHovering)
        {
            _isHovering = false;
            if (CursorManager.Instance != null)
                CursorManager.Instance.ResetCursor();
        }
    }
}
