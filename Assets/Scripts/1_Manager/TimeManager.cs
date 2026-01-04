using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TimeManager : SingletonBase<TimeManager>
{
    [Header("Settings")]
    [SerializeField] private float PAUSE_TIME_SCALE = 0f;
    [SerializeField] private float DEFAULT_TIME_SCALE = 1.0f;

    // 현재 시간 배율
    public float CurrentTimeScale => Time.timeScale;
    
    // 일시정지 여부 확인
    public bool IsPaused => Time.timeScale == PAUSE_TIME_SCALE;

    private float _savedTimeScale = 1.0f;

    protected override void Awake()
    {
        base.Awake();
        _savedTimeScale = DEFAULT_TIME_SCALE;
        GameManager.Instance.OnGamePaused.AddListener(Pause);
        GameManager.Instance.OnGameResumed.AddListener(Resume);
    }

    protected override void OnDestroy()
    {
        GameManager.Instance.OnGamePaused.RemoveListener(Pause);
        GameManager.Instance.OnGameResumed.RemoveListener(Resume);
        base.OnDestroy();
    }

    private void Start()
    {
        // 시작 시 기본 타임 스케일 적용
        SetTimeScale(DEFAULT_TIME_SCALE);
    }

    /// <summary>
    /// 게임을 일시정지합니다. (TimeScale = 0)
    /// </summary>
    public void Pause()
    {
        if (IsPaused) return;

        _savedTimeScale = Time.timeScale; // 현재 속도 저장 (슬로우 모션 중이었다면 복구하기 위해)
        Time.timeScale = PAUSE_TIME_SCALE;
        
        Debug.Log("[TimeManager] Game Paused");
    }

    /// <summary>
    /// 게임을 재개합니다. (이전 TimeScale로 복구)
    /// </summary>
    public void Resume()
    {
        if (!IsPaused) return;

        Time.timeScale = _savedTimeScale;
        
        Debug.Log($"[TimeManager] Game Resumed (Scale: {_savedTimeScale})");
    }

    /// <summary>
    /// 타임 스케일을 직접 설정합니다.
    /// </summary>
    /// <param name="scale">0 이상 권장</param>
    public void SetTimeScale(float scale)
    {
        if (scale < 0) scale = 0;
        
        Time.timeScale = scale;
        Debug.Log($"[TimeManager] TimeScale set to {scale}");
    }

    /// <summary>
    /// 일시정지 상태를 토글합니다.
    /// </summary>
    public void TogglePause()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }

    /// <summary>
    /// 잠시 동안 시간을 멈추거나 느리게 했다가 복구하는 효과 (히트 스탑 등)
    /// </summary>
    public void DoHitStop(float duration, float scale = 0f)
    {
        StartCoroutine(HitStopRoutine(duration, scale));
    }

    private IEnumerator HitStopRoutine(float duration, float scale)
    {
        float originalScale = Time.timeScale;
        Time.timeScale = scale;
        
        yield return new WaitForSecondsRealtime(duration); // Realtime을 써야 멈춘 시간에도 동작함

        Time.timeScale = originalScale;
    }
}
