using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : SingletonBase<SoundManager>
{

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Sound Data")]
    public AudioClip _bgmClip;
    [NamedArray(typeof(Enum.SFX))]
    [SerializeField] private List<AudioClip> _sfxList = new List<AudioClip>();

    [Header("Volume Settings")]
    [Range(0f, 1f)] [SerializeField] private float _masterVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float _bgmVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float _sfxVolume = 1f;
    

    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            _masterVolume = Mathf.Clamp01(value);
            UpdateVolume();
        }
    }

    public float BGMVolume
    {
        get => _bgmVolume;
        set
        {
            _bgmVolume = Mathf.Clamp01(value);
            UpdateVolume();
        }
    }

    public float SFXVolume
    {
        get => _sfxVolume;
        set
        {
            _sfxVolume = Mathf.Clamp01(value);
            UpdateVolume();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;

        // Enum.SFX의 모든 값을 가져옵니다.
        var enumValues = System.Enum.GetValues(typeof(Enum.SFX));
                // 리스트가 null이면 새로 생성
        if (_sfxList == null) 
        {
            _sfxList = new List<AudioClip>();
        }

        // 리스트 크기가 Enum 개수보다 작으면 부족한 만큼 추가
        if (_sfxList.Count < enumValues.Length)
        {
            int diff = enumValues.Length - _sfxList.Count;
            for (int i = 0; i < diff; i++)
            {
                _sfxList.Add(null);
            }
        }
        // 리스트 크기가 Enum 개수보다 크면 남는 만큼 제거 (선택 사항, 보통은 유지하거나 줄임)
        else if (_sfxList.Count > enumValues.Length)
        {
            _sfxList.RemoveRange(enumValues.Length, _sfxList.Count - enumValues.Length);
        }
    }
#endif

    protected override void Awake()
    {
        base.Awake();
        
        // AudioSource가 할당되지 않았다면 자동으로 생성 및 설정
        if (_bgmSource == null)
        {
            GameObject bgmObj = new GameObject("BGM_Source");
            bgmObj.transform.SetParent(transform);
            _bgmSource = bgmObj.AddComponent<AudioSource>();
            _bgmSource.loop = true;
            _bgmSource.playOnAwake = false;
        }

        if (_sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFX_Source");
            sfxObj.transform.SetParent(transform);
            _sfxSource = sfxObj.AddComponent<AudioSource>();
            _sfxSource.loop = false;
            _sfxSource.playOnAwake = false;
        }

        UpdateVolume();
        PlayBGM();
    }

    private void UpdateVolume()
    {
        if (_bgmSource != null) _bgmSource.volume = _bgmVolume * _masterVolume;
        if (_sfxSource != null) _sfxSource.volume = _sfxVolume * _masterVolume;
    }

    // --- BGM ---
    public void PlayBGM()
    {
        PlayBGM(_bgmClip);
    }

    public void PlayBGM(AudioClip clip, bool loop = true, float fadeDuration = 0f)
    {
        if (clip == null) return;
        if (_bgmSource.clip == clip && _bgmSource.isPlaying) return;

        if (fadeDuration > 0)
        {
            StartCoroutine(FadeBGMRoutine(clip, loop, fadeDuration));
        }
        else
        {
            _bgmSource.clip = clip;
            _bgmSource.loop = loop;
            _bgmSource.Play();
        }
    }

    public void StopBGM(float fadeDuration = 0f)
    {
        if (fadeDuration > 0)
        {
            StartCoroutine(FadeOutBGMRoutine(fadeDuration));
        }
        else
        {
            _bgmSource.Stop();
        }
    }

    private IEnumerator FadeBGMRoutine(AudioClip newClip, bool loop, float duration)
    {
        // Fade Out
        float startVolume = _bgmSource.volume;
        float halfDuration = duration * 0.5f;

        if (_bgmSource.isPlaying)
        {
            for (float t = 0; t < halfDuration; t += Time.deltaTime)
            {
                _bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / halfDuration);
                yield return null;
            }
            _bgmSource.volume = 0f;
            _bgmSource.Stop();
        }

        // Change Clip
        _bgmSource.clip = newClip;
        _bgmSource.loop = loop;
        _bgmSource.Play();

        // Fade In
        float targetVolume = _bgmVolume * _masterVolume;
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            _bgmSource.volume = Mathf.Lerp(0f, targetVolume, t / halfDuration);
            yield return null;
        }
        _bgmSource.volume = targetVolume;
    }

    private IEnumerator FadeOutBGMRoutine(float duration)
    {
        float startVolume = _bgmSource.volume;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            _bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }
        _bgmSource.volume = 0f;
        _bgmSource.Stop();
        
        // 볼륨 복구 (다음 재생을 위해)
        UpdateVolume();
    }

    // --- SFX ---

    /// <summary>
    /// Enum 타입으로 SFX를 재생합니다.
    /// </summary>
    public void PlaySFX(Enum.SFX type)
    {
        PlaySFX(type, 1.0f);
    }
    public void PlaySFX(Enum.SFX type, float pitch = 1.0f)
    {
        if (_sfxList[(int)type] != null)
        {
            PlaySFX(_sfxList[(int)type], pitch);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] SFX Type '{type}' not found in dictionary.");
        }
    }

    public void PlaySFX(AudioClip clip, float pitch = 1.0f)
    {
        if (clip == null) return;
        
        _sfxSource.pitch = pitch;
        _sfxSource.PlayOneShot(clip);
    }
    
    /// <summary>
    /// 3D 공간에서 사운드를 재생합니다. (AudioSource.PlayClipAtPoint 활용)
    /// </summary>
    public void PlaySFX(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;
        // PlayClipAtPoint는 임시 오브젝트를 생성하므로 볼륨을 직접 넣어줘야 함
        AudioSource.PlayClipAtPoint(clip, position, _sfxVolume * _masterVolume);
    }
}
