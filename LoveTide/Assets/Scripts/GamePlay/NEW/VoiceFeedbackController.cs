using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 語音反饋控制器
/// 
/// 職責:
/// 1. 管理角色語音播放和同步
/// 2. 控制語音與對話文字的同步顯示
/// 3. 處理語音播放狀態和事件
/// 4. 與對話系統和音頻系統協作
/// 
/// 基於架構文檔: SharedSystems/對話系統架構.md
/// 實現語音播放的統一管理和同步控制
/// </summary>
public class VoiceFeedbackController : MonoBehaviour
{
    [Header("== 音頻組件 ==")]
    [SerializeField] private AudioSource voiceAudioSource;
    [SerializeField] private AudioClip[] voiceClips;
    
    [Header("== 語音配置 ==")]
    [SerializeField] private float voiceVolume = 1.0f;
    [SerializeField] private bool enableVoiceSync = true;
    [SerializeField] private bool autoPlayNext = false;
    
    [Header("== 當前狀態 ==")]
    [SerializeField] private bool isInitialized = false;
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private int currentClipIndex = -1;
    
    // 語音事件
    public UnityEvent<string> OnVoiceStarted;
    public UnityEvent<string> OnVoiceCompleted;
    public UnityEvent OnVoiceStopped;
    public UnityEvent<float> OnVoiceProgress; // 播放進度 0-1
    
    // 語音數據
    private Dictionary<string, AudioClip> voiceClipDict = new Dictionary<string, AudioClip>();
    private Queue<VoicePlayRequest> voiceQueue = new Queue<VoicePlayRequest>();
    private VoicePlayRequest currentRequest;
    
    // 同步控制
    private float voiceStartTime;
    private float currentVoiceDuration;
    
    public bool IsInitialized => isInitialized;
    public bool IsPlaying => isPlaying;
    public float CurrentProgress => GetCurrentProgress();
    
    /// <summary>
    /// 初始化語音反饋控制器
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[VoiceFeedbackController] 初始化語音反饋控制器");
        
        // 設置音頻組件
        SetupAudioSource();
        
        // 載入語音資源
        LoadVoiceClips();
        
        // 初始化狀態
        InitializeState();
        
        isInitialized = true;
        Debug.Log("[VoiceFeedbackController] 語音反饋控制器初始化完成");
    }
    
    void Update()
    {
        if (isPlaying && enableVoiceSync)
        {
            // 更新播放進度
            UpdateVoiceProgress();
            
            // 檢查播放完成
            CheckVoiceCompletion();
        }
        
        // 處理語音隊列
        ProcessVoiceQueue();
    }
    
    /// <summary>
    /// 設置音頻組件
    /// </summary>
    private void SetupAudioSource()
    {
        if (voiceAudioSource == null)
        {
            voiceAudioSource = GetComponent<AudioSource>();
            if (voiceAudioSource == null)
            {
                voiceAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // 配置AudioSource
        voiceAudioSource.playOnAwake = false;
        voiceAudioSource.loop = false;
        voiceAudioSource.volume = voiceVolume;
        
        Debug.Log("[VoiceFeedbackController] AudioSource設置完成");
    }
    
    /// <summary>
    /// 載入語音資源
    /// </summary>
    private void LoadVoiceClips()
    {
        voiceClipDict.Clear();
        
        // 載入預設的語音片段
        if (voiceClips != null)
        {
            foreach (var clip in voiceClips)
            {
                if (clip != null)
                {
                    voiceClipDict[clip.name] = clip;
                    Debug.Log($"[VoiceFeedbackController] 載入語音: {clip.name}");
                }
            }
        }
        
        // 從Resources資料夾載入語音
        LoadVoiceClipsFromResources();
    }
    
    /// <summary>
    /// 從Resources載入語音
    /// </summary>
    private void LoadVoiceClipsFromResources()
    {
        // 載入Yuka的語音文件
        AudioClip[] yukaVoices = Resources.LoadAll<AudioClip>("Audio/Voice/Yuka");
        foreach (var clip in yukaVoices)
        {
            if (!voiceClipDict.ContainsKey(clip.name))
            {
                voiceClipDict[clip.name] = clip;
                Debug.Log($"[VoiceFeedbackController] 從Resources載入語音: {clip.name}");
            }
        }
        
        Debug.Log($"[VoiceFeedbackController] 總共載入 {voiceClipDict.Count} 個語音片段");
    }
    
    /// <summary>
    /// 初始化狀態
    /// </summary>
    private void InitializeState()
    {
        isPlaying = false;
        currentClipIndex = -1;
        currentRequest = null;
        voiceQueue.Clear();
        
        voiceStartTime = 0f;
        currentVoiceDuration = 0f;
    }
    
    #region 語音播放控制
    
    /// <summary>
    /// 播放語音
    /// </summary>
    public void PlayVoice(string voiceKey, bool immediate = false)
    {
        var request = new VoicePlayRequest
        {
            voiceKey = voiceKey,
            immediate = immediate,
            startTime = Time.time
        };
        
        if (immediate || !isPlaying)
        {
            PlayVoiceImmediate(request);
        }
        else
        {
            voiceQueue.Enqueue(request);
            Debug.Log($"[VoiceFeedbackController] 語音已加入隊列: {voiceKey}");
        }
    }
    
    /// <summary>
    /// 立即播放語音
    /// </summary>
    private void PlayVoiceImmediate(VoicePlayRequest request)
    {
        if (!voiceClipDict.ContainsKey(request.voiceKey))
        {
            Debug.LogWarning($"[VoiceFeedbackController] 找不到語音: {request.voiceKey}");
            return;
        }
        
        // 停止當前播放
        if (isPlaying)
        {
            StopVoice();
        }
        
        AudioClip clip = voiceClipDict[request.voiceKey];
        currentRequest = request;
        
        // 播放語音
        voiceAudioSource.clip = clip;
        voiceAudioSource.volume = voiceVolume;
        voiceAudioSource.Play();
        
        // 記錄播放信息
        isPlaying = true;
        voiceStartTime = Time.time;
        currentVoiceDuration = clip.length;
        
        Debug.Log($"[VoiceFeedbackController] 開始播放語音: {request.voiceKey} (時長: {clip.length:F2}s)");
        
        // 觸發開始事件
        OnVoiceStarted?.Invoke(request.voiceKey);
    }
    
    /// <summary>
    /// 停止語音播放
    /// </summary>
    public void StopVoice()
    {
        if (isPlaying)
        {
            voiceAudioSource.Stop();
            isPlaying = false;
            
            Debug.Log("[VoiceFeedbackController] 語音播放已停止");
            OnVoiceStopped?.Invoke();
        }
        
        currentRequest = null;
    }
    
    /// <summary>
    /// 暫停語音播放
    /// </summary>
    public void PauseVoice()
    {
        if (isPlaying && voiceAudioSource.isPlaying)
        {
            voiceAudioSource.Pause();
            Debug.Log("[VoiceFeedbackController] 語音播放已暫停");
        }
    }
    
    /// <summary>
    /// 恢復語音播放
    /// </summary>
    public void ResumeVoice()
    {
        if (isPlaying && !voiceAudioSource.isPlaying)
        {
            voiceAudioSource.UnPause();
            Debug.Log("[VoiceFeedbackController] 語音播放已恢復");
        }
    }
    
    /// <summary>
    /// 清除語音隊列
    /// </summary>
    public void ClearVoiceQueue()
    {
        voiceQueue.Clear();
        Debug.Log("[VoiceFeedbackController] 語音隊列已清除");
    }
    
    #endregion
    
    #region 播放進度和同步
    
    /// <summary>
    /// 更新語音播放進度
    /// </summary>
    private void UpdateVoiceProgress()
    {
        if (isPlaying && currentVoiceDuration > 0)
        {
            float elapsed = Time.time - voiceStartTime;
            float progress = Mathf.Clamp01(elapsed / currentVoiceDuration);
            
            OnVoiceProgress?.Invoke(progress);
        }
    }
    
    /// <summary>
    /// 檢查語音播放完成
    /// </summary>
    private void CheckVoiceCompletion()
    {
        if (isPlaying && !voiceAudioSource.isPlaying)
        {
            // 語音播放完成
            string completedVoice = currentRequest?.voiceKey ?? "unknown";
            
            isPlaying = false;
            currentRequest = null;
            
            Debug.Log($"[VoiceFeedbackController] 語音播放完成: {completedVoice}");
            OnVoiceCompleted?.Invoke(completedVoice);
        }
    }
    
    /// <summary>
    /// 處理語音隊列
    /// </summary>
    private void ProcessVoiceQueue()
    {
        if (!isPlaying && voiceQueue.Count > 0)
        {
            VoicePlayRequest nextRequest = voiceQueue.Dequeue();
            PlayVoiceImmediate(nextRequest);
        }
    }
    
    /// <summary>
    /// 獲取當前播放進度
    /// </summary>
    private float GetCurrentProgress()
    {
        if (!isPlaying || currentVoiceDuration <= 0)
            return 0f;
            
        float elapsed = Time.time - voiceStartTime;
        return Mathf.Clamp01(elapsed / currentVoiceDuration);
    }
    
    #endregion
    
    #region 語音資源管理
    
    /// <summary>
    /// 添加語音片段
    /// </summary>
    public void AddVoiceClip(string key, AudioClip clip)
    {
        if (clip != null && !string.IsNullOrEmpty(key))
        {
            voiceClipDict[key] = clip;
            Debug.Log($"[VoiceFeedbackController] 添加語音片段: {key}");
        }
    }
    
    /// <summary>
    /// 移除語音片段
    /// </summary>
    public void RemoveVoiceClip(string key)
    {
        if (voiceClipDict.ContainsKey(key))
        {
            voiceClipDict.Remove(key);
            Debug.Log($"[VoiceFeedbackController] 移除語音片段: {key}");
        }
    }
    
    /// <summary>
    /// 檢查語音是否存在
    /// </summary>
    public bool HasVoiceClip(string key)
    {
        return voiceClipDict.ContainsKey(key);
    }
    
    /// <summary>
    /// 預載入語音組
    /// </summary>
    public void PreloadVoiceGroup(string[] voiceKeys)
    {
        foreach (string key in voiceKeys)
        {
            if (!voiceClipDict.ContainsKey(key))
            {
                // 嘗試從Resources載入
                AudioClip clip = Resources.Load<AudioClip>($"Audio/Voice/{key}");
                if (clip != null)
                {
                    AddVoiceClip(key, clip);
                }
            }
        }
    }
    
    #endregion
    
    #region 設置和配置
    
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
        
        Debug.Log($"[VoiceFeedbackController] 語音音量設置為: {voiceVolume:F2}");
    }
    
    /// <summary>
    /// 設置語音同步
    /// </summary>
    public void SetVoiceSync(bool enable)
    {
        enableVoiceSync = enable;
        Debug.Log($"[VoiceFeedbackController] 語音同步: {(enable ? "啟用" : "禁用")}");
    }
    
    /// <summary>
    /// 設置自動播放下一個
    /// </summary>
    public void SetAutoPlayNext(bool enable)
    {
        autoPlayNext = enable;
        Debug.Log($"[VoiceFeedbackController] 自動播放下一個: {(enable ? "啟用" : "禁用")}");
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 獲取當前播放的語音名稱
    /// </summary>
    public string GetCurrentVoiceName()
    {
        return currentRequest?.voiceKey ?? "";
    }
    
    /// <summary>
    /// 獲取剩餘播放時間
    /// </summary>
    public float GetRemainingTime()
    {
        if (!isPlaying || currentVoiceDuration <= 0)
            return 0f;
            
        float elapsed = Time.time - voiceStartTime;
        return Mathf.Max(0f, currentVoiceDuration - elapsed);
    }
    
    /// <summary>
    /// 獲取隊列中的語音數量
    /// </summary>
    public int GetQueueCount()
    {
        return voiceQueue.Count;
    }
    
    /// <summary>
    /// 跳過當前語音
    /// </summary>
    public void SkipCurrentVoice()
    {
        if (isPlaying)
        {
            StopVoice();
            Debug.Log("[VoiceFeedbackController] 跳過當前語音");
        }
    }
    
    #endregion
}

/// <summary>
/// 語音播放請求
/// </summary>
[System.Serializable]
public class VoicePlayRequest
{
    public string voiceKey;
    public bool immediate;
    public float startTime;
    public float delay;
    
    public VoicePlayRequest()
    {
        immediate = false;
        startTime = 0f;
        delay = 0f;
    }
}