using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

/// <summary>
/// 音頻管理器
/// 
/// 職責:
/// 1. 統一管理BGM、SFX、語音的播放
/// 2. 控制音量和音頻混合
/// 3. 處理音頻資源的載入和釋放
/// 4. 與設置系統協作管理音頻設定
/// 
/// 基於架構文檔: 提供音頻系統的統一管理
/// 支援多種音頻類型和動態音量控制
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("== 音頻混合器 ==")]
    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup bgmMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup voiceMixerGroup;
    
    [Header("== 音頻源配置 ==")]
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource[] sfxAudioSources;
    [SerializeField] private AudioSource voiceAudioSource;
    [SerializeField] private int maxSFXSources = 10;
    
    [Header("== 音量配置 ==")]
    [SerializeField] private float masterVolume = 1.0f;
    [SerializeField] private float bgmVolume = 0.8f;
    [SerializeField] private float sfxVolume = 1.0f;
    [SerializeField] private float voiceVolume = 1.0f;
    
    [Header("== 音頻資源 ==")]
    [SerializeField] private AudioClipDatabase audioDatabase;
    
    [Header("== 狀態管理 ==")]
    [SerializeField] private bool isInitialized = false;
    [SerializeField] private bool isMuted = false;
    
    // 音頻事件
    public UnityEvent<string> OnBGMStarted;
    public UnityEvent<string> OnBGMStopped;
    public UnityEvent<string> OnSFXPlayed;
    public UnityEvent<float> OnVolumeChanged;
    
    // 音頻字典
    private Dictionary<string, AudioClip> bgmClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> voiceClips = new Dictionary<string, AudioClip>();
    
    // SFX源管理
    private Queue<AudioSource> availableSFXSources = new Queue<AudioSource>();
    private List<AudioSource> usedSFXSources = new List<AudioSource>();
    
    // BGM控制
    private string currentBGM = "";
    private Coroutine bgmFadeCoroutine;
    
    // 單例模式
    public static AudioManager Instance { get; private set; }
    
    public bool IsInitialized => isInitialized;
    public bool IsMuted => isMuted;
    public float MasterVolume => masterVolume;
    public float BGMVolume => bgmVolume;
    public float SFXVolume => sfxVolume;
    public float VoiceVolume => voiceVolume;
    
    void Awake()
    {
        // 單例設置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 初始化音頻管理器
    /// </summary>
    public void Initialize()
    {
        if (isInitialized) return;
        
        Debug.Log("[AudioManager] 初始化音頻管理器");
        
        // 設置音頻源
        SetupAudioSources();
        
        // 載入音頻資源
        LoadAudioResources();
        
        // 設置音量
        ApplyVolumeSettings();
        
        isInitialized = true;
        Debug.Log("[AudioManager] 音頻管理器初始化完成");
    }
    
    /// <summary>
    /// 設置音頻源
    /// </summary>
    private void SetupAudioSources()
    {
        // 設置BGM音頻源
        if (bgmAudioSource == null)
        {
            GameObject bgmObj = new GameObject("BGM_AudioSource");
            bgmObj.transform.SetParent(transform);
            bgmAudioSource = bgmObj.AddComponent<AudioSource>();
        }
        
        bgmAudioSource.outputAudioMixerGroup = bgmMixerGroup;
        bgmAudioSource.loop = true;
        bgmAudioSource.playOnAwake = false;
        
        // 設置語音音頻源
        if (voiceAudioSource == null)
        {
            GameObject voiceObj = new GameObject("Voice_AudioSource");
            voiceObj.transform.SetParent(transform);
            voiceAudioSource = voiceObj.AddComponent<AudioSource>();
        }
        
        voiceAudioSource.outputAudioMixerGroup = voiceMixerGroup;
        voiceAudioSource.loop = false;
        voiceAudioSource.playOnAwake = false;
        
        // 設置SFX音頻源池
        SetupSFXAudioSources();
    }
    
    /// <summary>
    /// 設置SFX音頻源池
    /// </summary>
    private void SetupSFXAudioSources()
    {
        // 清除現有的SFX源
        availableSFXSources.Clear();
        usedSFXSources.Clear();
        
        // 創建SFX音頻源池
        for (int i = 0; i < maxSFXSources; i++)
        {
            GameObject sfxObj = new GameObject($"SFX_AudioSource_{i}");
            sfxObj.transform.SetParent(transform);
            AudioSource sfxSource = sfxObj.AddComponent<AudioSource>();
            
            sfxSource.outputAudioMixerGroup = sfxMixerGroup;
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            
            availableSFXSources.Enqueue(sfxSource);
        }
        
        Debug.Log($"[AudioManager] 創建了 {maxSFXSources} 個SFX音頻源");
    }
    
    /// <summary>
    /// 載入音頻資源
    /// </summary>
    private void LoadAudioResources()
    {
        // 從AudioDatabase載入
        if (audioDatabase != null)
        {
            LoadFromDatabase();
        }
        
        // 從Resources載入
        LoadFromResources();
    }
    
    /// <summary>
    /// 從資料庫載入
    /// </summary>
    private void LoadFromDatabase()
    {
        foreach (var bgm in audioDatabase.bgmClips)
        {
            if (bgm.clip != null)
            {
                bgmClips[bgm.key] = bgm.clip;
            }
        }
        
        foreach (var sfx in audioDatabase.sfxClips)
        {
            if (sfx.clip != null)
            {
                sfxClips[sfx.key] = sfx.clip;
            }
        }
        
        foreach (var voice in audioDatabase.voiceClips)
        {
            if (voice.clip != null)
            {
                voiceClips[voice.key] = voice.clip;
            }
        }
        
        Debug.Log($"[AudioManager] 從資料庫載入音頻: BGM {bgmClips.Count}, SFX {sfxClips.Count}, Voice {voiceClips.Count}");
    }
    
    /// <summary>
    /// 從Resources載入
    /// </summary>
    private void LoadFromResources()
    {
        // 載入BGM
        LoadAudioClipsFromPath("Audio/BGM", bgmClips);
        
        // 載入SFX
        LoadAudioClipsFromPath("Audio/SFX", sfxClips);
        
        // 載入語音
        LoadAudioClipsFromPath("Audio/Voice", voiceClips);
    }
    
    /// <summary>
    /// 從指定路徑載入音頻
    /// </summary>
    private void LoadAudioClipsFromPath(string path, Dictionary<string, AudioClip> targetDict)
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>(path);
        
        foreach (AudioClip clip in clips)
        {
            if (!targetDict.ContainsKey(clip.name))
            {
                targetDict[clip.name] = clip;
            }
        }
        
        Debug.Log($"[AudioManager] 從 {path} 載入 {clips.Length} 個音頻檔案");
    }
    
    #region BGM控制
    
    /// <summary>
    /// 播放BGM
    /// </summary>
    public void PlayBGM(string bgmKey, bool fadeIn = true, float fadeTime = 1f)
    {
        if (!bgmClips.ContainsKey(bgmKey))
        {
            Debug.LogWarning($"[AudioManager] 找不到BGM: {bgmKey}");
            return;
        }
        
        AudioClip newBGM = bgmClips[bgmKey];
        
        if (currentBGM == bgmKey && bgmAudioSource.isPlaying)
        {
            Debug.Log($"[AudioManager] BGM {bgmKey} 已在播放");
            return;
        }
        
        Debug.Log($"[AudioManager] 播放BGM: {bgmKey}");
        
        if (fadeIn && bgmAudioSource.isPlaying)
        {
            // 淡出當前BGM並淡入新BGM
            if (bgmFadeCoroutine != null)
            {
                StopCoroutine(bgmFadeCoroutine);
            }
            bgmFadeCoroutine = StartCoroutine(CrossfadeBGM(newBGM, fadeTime));
        }
        else
        {
            // 直接播放
            bgmAudioSource.clip = newBGM;
            bgmAudioSource.volume = bgmVolume;
            bgmAudioSource.Play();
        }
        
        currentBGM = bgmKey;
        OnBGMStarted?.Invoke(bgmKey);
    }
    
    /// <summary>
    /// 停止BGM
    /// </summary>
    public void StopBGM(bool fadeOut = true, float fadeTime = 1f)
    {
        if (!bgmAudioSource.isPlaying) return;
        
        Debug.Log("[AudioManager] 停止BGM");
        
        if (fadeOut)
        {
            if (bgmFadeCoroutine != null)
            {
                StopCoroutine(bgmFadeCoroutine);
            }
            bgmFadeCoroutine = StartCoroutine(FadeOutBGM(fadeTime));
        }
        else
        {
            bgmAudioSource.Stop();
            OnBGMStopped?.Invoke(currentBGM);
            currentBGM = "";
        }
    }
    
    /// <summary>
    /// BGM交叉淡入淡出
    /// </summary>
    private IEnumerator CrossfadeBGM(AudioClip newClip, float fadeTime)
    {
        float startVolume = bgmAudioSource.volume;
        
        // 淡出當前BGM
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmAudioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
            yield return null;
        }
        
        // 切換BGM
        bgmAudioSource.clip = newClip;
        bgmAudioSource.Play();
        
        // 淡入新BGM
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmAudioSource.volume = Mathf.Lerp(0, bgmVolume, t / fadeTime);
            yield return null;
        }
        
        bgmAudioSource.volume = bgmVolume;
        bgmFadeCoroutine = null;
    }
    
    /// <summary>
    /// BGM淡出
    /// </summary>
    private IEnumerator FadeOutBGM(float fadeTime)
    {
        float startVolume = bgmAudioSource.volume;
        
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmAudioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
            yield return null;
        }
        
        bgmAudioSource.Stop();
        bgmAudioSource.volume = bgmVolume;
        
        OnBGMStopped?.Invoke(currentBGM);
        currentBGM = "";
        bgmFadeCoroutine = null;
    }
    
    #endregion
    
    #region SFX控制
    
    /// <summary>
    /// 播放SFX
    /// </summary>
    public void PlaySFX(string sfxKey, float volume = 1f, float pitch = 1f)
    {
        if (!sfxClips.ContainsKey(sfxKey))
        {
            Debug.LogWarning($"[AudioManager] 找不到SFX: {sfxKey}");
            return;
        }
        
        AudioSource sfxSource = GetAvailableSFXSource();
        if (sfxSource == null)
        {
            Debug.LogWarning("[AudioManager] 沒有可用的SFX音頻源");
            return;
        }
        
        AudioClip sfxClip = sfxClips[sfxKey];
        
        sfxSource.clip = sfxClip;
        sfxSource.volume = sfxVolume * volume;
        sfxSource.pitch = pitch;
        sfxSource.Play();
        
        // 添加到使用中列表
        usedSFXSources.Add(sfxSource);
        
        // 開始協程來回收音頻源
        StartCoroutine(RecycleSFXSource(sfxSource, sfxClip.length / pitch));
        
        OnSFXPlayed?.Invoke(sfxKey);
    }
    
    /// <summary>
    /// 獲取可用的SFX音頻源
    /// </summary>
    private AudioSource GetAvailableSFXSource()
    {
        // 檢查並回收已完成的音頻源
        RecycleCompletedSFXSources();
        
        if (availableSFXSources.Count > 0)
        {
            return availableSFXSources.Dequeue();
        }
        
        return null;
    }
    
    /// <summary>
    /// 回收已完成的SFX音頻源
    /// </summary>
    private void RecycleCompletedSFXSources()
    {
        for (int i = usedSFXSources.Count - 1; i >= 0; i--)
        {
            AudioSource source = usedSFXSources[i];
            if (!source.isPlaying)
            {
                usedSFXSources.RemoveAt(i);
                availableSFXSources.Enqueue(source);
            }
        }
    }
    
    /// <summary>
    /// 回收SFX音頻源協程
    /// </summary>
    private IEnumerator RecycleSFXSource(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay + 0.1f); // 稍微多等一點時間
        
        if (usedSFXSources.Contains(source))
        {
            usedSFXSources.Remove(source);
            availableSFXSources.Enqueue(source);
        }
    }
    
    /// <summary>
    /// 停止所有SFX
    /// </summary>
    public void StopAllSFX()
    {
        foreach (AudioSource source in usedSFXSources)
        {
            source.Stop();
        }
        
        // 回收所有音頻源
        while (usedSFXSources.Count > 0)
        {
            AudioSource source = usedSFXSources[0];
            usedSFXSources.RemoveAt(0);
            availableSFXSources.Enqueue(source);
        }
        
        Debug.Log("[AudioManager] 停止所有SFX");
    }
    
    #endregion
    
    #region 語音控制
    
    /// <summary>
    /// 播放語音
    /// </summary>
    public void PlayVoice(string voiceKey, float volume = 1f)
    {
        if (!voiceClips.ContainsKey(voiceKey))
        {
            Debug.LogWarning($"[AudioManager] 找不到語音: {voiceKey}");
            return;
        }
        
        AudioClip voiceClip = voiceClips[voiceKey];
        
        voiceAudioSource.clip = voiceClip;
        voiceAudioSource.volume = voiceVolume * volume;
        voiceAudioSource.Play();
        
        Debug.Log($"[AudioManager] 播放語音: {voiceKey}");
    }
    
    /// <summary>
    /// 停止語音
    /// </summary>
    public void StopVoice()
    {
        if (voiceAudioSource.isPlaying)
        {
            voiceAudioSource.Stop();
            Debug.Log("[AudioManager] 停止語音");
        }
    }
    
    #endregion
    
    #region 音量控制
    
    /// <summary>
    /// 設置主音量
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        
        Debug.Log($"[AudioManager] 設置主音量: {masterVolume:F2}");
        OnVolumeChanged?.Invoke(masterVolume);
    }
    
    /// <summary>
    /// 設置BGM音量
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = bgmVolume;
        }
        
        Debug.Log($"[AudioManager] 設置BGM音量: {bgmVolume:F2}");
    }
    
    /// <summary>
    /// 設置SFX音量
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        
        Debug.Log($"[AudioManager] 設置SFX音量: {sfxVolume:F2}");
    }
    
    /// <summary>
    /// 設置語音音量
    /// </summary>
    public void SetVoiceVolume(float volume)
    {
        voiceVolume = Mathf.Clamp01(volume);
        if (voiceAudioSource != null)
        {
            voiceAudioSource.volume = voiceVolume;
        }
        
        Debug.Log($"[AudioManager] 設置語音音量: {voiceVolume:F2}");
    }
    
    /// <summary>
    /// 應用音量設置
    /// </summary>
    private void ApplyVolumeSettings()
    {
        if (masterMixerGroup != null)
        {
            float dbValue = masterVolume > 0 ? 20f * Mathf.Log10(masterVolume) : -80f;
            masterMixerGroup.audioMixer.SetFloat("MasterVolume", dbValue);
        }
    }
    
    /// <summary>
    /// 靜音/取消靜音
    /// </summary>
    public void SetMuted(bool muted)
    {
        isMuted = muted;
        
        if (bgmAudioSource != null)
        {
            bgmAudioSource.mute = muted;
        }
        
        if (voiceAudioSource != null)
        {
            voiceAudioSource.mute = muted;
        }
        
        foreach (AudioSource source in usedSFXSources)
        {
            source.mute = muted;
        }
        
        Debug.Log($"[AudioManager] {(muted ? "靜音" : "取消靜音")}");
    }
    
    #endregion
    
    #region 音頻資源管理
    
    /// <summary>
    /// 添加音頻片段
    /// </summary>
    public void AddAudioClip(AudioType audioType, string key, AudioClip clip)
    {
        if (clip == null || string.IsNullOrEmpty(key)) return;
        
        switch (audioType)
        {
            case AudioType.BGM:
                bgmClips[key] = clip;
                break;
            case AudioType.SFX:
                sfxClips[key] = clip;
                break;
            case AudioType.Voice:
                voiceClips[key] = clip;
                break;
        }
        
        Debug.Log($"[AudioManager] 添加音頻片段: {audioType} - {key}");
    }
    
    /// <summary>
    /// 移除音頻片段
    /// </summary>
    public void RemoveAudioClip(AudioType audioType, string key)
    {
        switch (audioType)
        {
            case AudioType.BGM:
                bgmClips.Remove(key);
                break;
            case AudioType.SFX:
                sfxClips.Remove(key);
                break;
            case AudioType.Voice:
                voiceClips.Remove(key);
                break;
        }
        
        Debug.Log($"[AudioManager] 移除音頻片段: {audioType} - {key}");
    }
    
    /// <summary>
    /// 檢查音頻是否存在
    /// </summary>
    public bool HasAudioClip(AudioType audioType, string key)
    {
        switch (audioType)
        {
            case AudioType.BGM:
                return bgmClips.ContainsKey(key);
            case AudioType.SFX:
                return sfxClips.ContainsKey(key);
            case AudioType.Voice:
                return voiceClips.ContainsKey(key);
            default:
                return false;
        }
    }
    
    #endregion
}

/// <summary>
/// 音頻類型
/// </summary>
public enum AudioType
{
    BGM,
    SFX,
    Voice
}

/// <summary>
/// 音頻片段資料
/// </summary>
[System.Serializable]
public class AudioClipData
{
    public string key;
    public AudioClip clip;
}

/// <summary>
/// 音頻資料庫
/// </summary>
[CreateAssetMenu(fileName = "AudioClipDatabase", menuName = "LoveTide/AudioClipDatabase")]
public class AudioClipDatabase : ScriptableObject
{
    [Header("BGM")]
    public AudioClipData[] bgmClips;
    
    [Header("SFX")]
    public AudioClipData[] sfxClips;
    
    [Header("Voice")]
    public AudioClipData[] voiceClips;
}