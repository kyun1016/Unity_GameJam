using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GameManager : SingletonBase<GameManager>
{
    // 일시정지/재개 시 발생할 이벤트 (UI 표시 등에 활용)
    public UnityEvent OnGamePaused = new UnityEvent();
    public UnityEvent OnGameResumed = new UnityEvent();

    public bool IsPaused;

    protected override void Awake()
    {
        base.Awake();
        // 추가적인 초기화 코드가 여기에 올 수 있습니다.
        DevDebug.Log("GameManager Awake called.");
        IsPaused = false;
    }

    public void TogglePause()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }
    
    public void Pause()
    {
        if (IsPaused) return;
        IsPaused = true;
        OnGamePaused?.Invoke();
    }

    /// <summary>
    /// 게임을 재개합니다. (이전 TimeScale로 복구)
    /// </summary>
    public void Resume()
    {
        if (!IsPaused) return;
        IsPaused = false;
        OnGameResumed?.Invoke();
    }
}
