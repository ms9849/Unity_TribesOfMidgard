using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private Dictionary<string, AudioClip> SoundDict;  // SFX와 BGM을 저장할 Dictionary
    [SerializeField] private AudioSource[] SFXSource;  // SFX 재생용 AudioSource
    [SerializeField] private AudioSource[] BGMSource;  // BGM 재생용 AudioSource

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] AudioClips; // 오디오 클립 배열

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        SoundDict = new Dictionary<string, AudioClip>();
        foreach (AudioClip Clip in AudioClips)
        {
            SoundDict[Clip.name] = Clip;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // index번 BGMSource로 clipName을 재생합니다.
    public void PlayBGM(string ClipName, int Index)
    {
        if (!IsValidIndex(BGMSource, Index) || !TryGetClip(ClipName, out AudioClip Clip))
            return;

        BGMSource[Index].clip = Clip;
        BGMSource[Index].loop = true;
        BGMSource[Index].Play();
    }

    // index번 BGMSource를 정지합니다.
    public void StopBGM(int Index)
    {
        if (!IsValidIndex(BGMSource, Index))
            return;

        BGMSource[Index].Stop();
    }

    // index번 SFXSource로 clipName을 재생합니다.
    public void PlaySFX(string ClipName, int Index)
    {
        if (!IsValidIndex(SFXSource, Index) || !TryGetClip(ClipName, out AudioClip Clip))
            return;

        SFXSource[Index].clip = Clip;
        SFXSource[Index].Play();
    }

    public void PlaySFX(string ClipName, int Index, float Volume)
    {
        if (!IsValidIndex(SFXSource, Index) || !TryGetClip(ClipName, out AudioClip Clip))
            return;

        SFXSource[Index].clip = Clip;
        SFXSource[Index].Play();

        SetSFXVolume(Index, Volume);
    }

    // index번 SFXSource를 정지합니다.
    public void StopSFX(int Index)
    {
        if (!IsValidIndex(SFXSource, Index))
            return;

        SFXSource[Index].Stop();
    }

    // index번 BGMSource의 볼륨을 설정합니다.
    public void SetBGMVolume(int Index, float Volume)
    {
        if (!IsValidIndex(BGMSource, Index))
            return;

        BGMSource[Index].volume = Mathf.Clamp01(Volume);
    }

    // index번 SFXSource의 볼륨을 설정합니다.
    public void SetSFXVolume(int Index, float Volume)
    {
        if (!IsValidIndex(SFXSource, Index))
            return;

        SFXSource[Index].volume = Mathf.Clamp01(Volume);
    }

    // Sources 배열에서 index가 유효한지 확인합니다.
    private bool IsValidIndex(AudioSource[] Sources, int Index)
    {
        if (Sources == null || Index < 0 || Index >= Sources.Length)
        {
            Debug.LogWarning($"SoundManager: 유효하지 않은 인덱스입니다. ({Index})");
            return false;
        }
        return true;
    }

    // SoundDict에서 clipName에 해당하는 클립을 찾습니다.
    private bool TryGetClip(string ClipName, out AudioClip Clip)
    {
        if (!SoundDict.TryGetValue(ClipName, out Clip))
        {
            Debug.LogWarning($"SoundManager: '{ClipName}' 클립을 찾을 수 없습니다.");
            return false;
        }
        return true;
    }
}
