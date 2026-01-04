using UnityEngine;
using System.Diagnostics;

/// <summary>
/// UnityEngine.Debug를 래핑한 커스텀 디버그 클래스입니다.
/// UNITY_EDITOR 또는 DEVELOPMENT_BUILD가 정의된 경우에만 로그가 출력되도록 설정되어 있습니다.
/// 릴리즈 빌드에서는 로그 호출 자체가 제거되어 성능 저하를 막습니다.
/// </summary>
public static class DevDebug
{
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(object message, Object context)
    {
        UnityEngine.Debug.Log(message, context);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogFormat(string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(format, args);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogFormat(Object context, string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(context, format, args);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogWarning(object message, Object context)
    {
        UnityEngine.Debug.LogWarning(message, context);
    }

    // 에러는 릴리즈 빌드에서도 중요할 수 있으므로 Conditional을 걸지 않는 경우가 많지만,
    // 필요에 따라 [Conditional]을 추가할 수 있습니다. 여기서는 에러는 항상 출력하도록 합니다.
    public static void LogError(object message)
    {
        UnityEngine.Debug.LogError(message);
    }

    public static void LogError(object message, Object context)
    {
        UnityEngine.Debug.LogError(message, context);
    }

    public static void LogException(System.Exception exception)
    {
        UnityEngine.Debug.LogException(exception);
    }

    public static void LogException(System.Exception exception, Object context)
    {
        UnityEngine.Debug.LogException(exception, context);
    }

    // --- 그리기 관련 함수 (에디터에서만 동작) ---

    [Conditional("UNITY_EDITOR")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color = default, float duration = 0.0f, bool depthTest = true)
    {
        if (color == default) color = Color.white;
        UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
    }

    [Conditional("UNITY_EDITOR")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color = default, float duration = 0.0f, bool depthTest = true)
    {
        if (color == default) color = Color.white;
        UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
    }

    // --- 유틸리티 ---

    /// <summary>
    /// 텍스트에 색상을 입혀서 로그로 출력하기 위한 헬퍼 함수입니다.
    /// 예: Debug.Log(Debug.ColorString("Hello", Color.red));
    /// </summary>
    public static string ColorString(string text, Color color)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{text}</color>";
    }
    
    public static string ColorString(string text, string colorCode)
    {
        return $"<color={colorCode}>{text}</color>";
    }
}
