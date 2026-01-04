using UnityEngine;
using System.Collections.Generic;

public class MouseManager : SingletonBase<MouseManager>
{
    [System.Serializable]
    public struct CursorData
    {
        public Texture2D texture;
        public Vector2 hotspot;
    }

    [Header("Cursor Settings")]
    [NamedArray(typeof(Enum.CursorType))]
    [SerializeField] private List<CursorData> _cursorList = new List<CursorData>();
    private Enum.CursorType _currentType = Enum.CursorType.Default;

    protected override void Awake()
    {
        base.Awake();
        SetCursor(Enum.CursorType.Default);
    }
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;

        // Enum.CursorType의 모든 값을 가져옵니다.
        var enumValues = System.Enum.GetValues(typeof(Enum.CursorType));
        
        // 리스트가 null이면 새로 생성
        if (_cursorList == null) 
        {
            _cursorList = new List<CursorData>();
        }

        // 리스트 크기가 Enum 개수보다 작으면 부족한 만큼 추가
        if (_cursorList.Count < enumValues.Length)
        {
            int diff = enumValues.Length - _cursorList.Count;
            for (int i = 0; i < diff; i++)
            {
                _cursorList.Add(new CursorData());
            }
        }
        // 리스트 크기가 Enum 개수보다 크면 남는 만큼 제거 (선택 사항, 보통은 유지하거나 줄임)
        else if (_cursorList.Count > enumValues.Length)
        {
            _cursorList.RemoveRange(enumValues.Length, _cursorList.Count - enumValues.Length);
        }
    }
    #endif

    public void SetCursor(Enum.CursorType type)
    {
        if (_currentType == type) return; // 이미 해당 커서면 패스 (Default는 강제 초기화 용도로 허용할 수도 있음)

        CursorData data = _cursorList[(int)type];
        Cursor.SetCursor(data.texture, data.hotspot, CursorMode.Auto);
        _currentType = type;
    }

    /// <summary>
    /// 기본 커서로 되돌립니다.
    /// </summary>
    public void ResetCursor()
    {
        SetCursor(Enum.CursorType.Default);
    }
}
