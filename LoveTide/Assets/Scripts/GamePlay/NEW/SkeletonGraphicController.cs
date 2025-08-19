using UnityEngine;
using UnityEngine.Events;
using Spine.Unity;
using Spine;

/// <summary>
/// SkeletonGraphic控制器
/// 
/// 職責:
/// 1. 管理Spine動畫在Canvas UI中的顯示
/// 2. 控制角色動畫播放和狀態切換
/// 3. 處理動畫事件和回調
/// 4. 與角色互動系統協作
/// 
/// 基於架構文檔: NurturingMode/互動系統完整設計_重製版.md
/// 專為Canvas UI優化的Spine動畫控制
/// </summary>
public class SkeletonGraphicController : MonoBehaviour
{
    [Header("== Spine組件 ==")]
    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private SkeletonDataAsset skeletonDataAsset;
    
    [Header("== 動畫配置 ==")]
    [SerializeField] private string defaultAnimation = "idle";
    [SerializeField] private bool loopDefaultAnimation = true;
    [SerializeField] private float animationSpeed = 1.0f;
    
    [Header("== 顯示模式 ==")]
    [SerializeField] private CharacterDisplayMode currentDisplayMode = CharacterDisplayMode.QVersion;
    [SerializeField] private Vector3 qVersionScale = Vector3.one * 0.8f;
    [SerializeField] private Vector3 dialogScale = Vector3.one;
    
    [Header("== 狀態管理 ==")]
    [SerializeField] private bool isInitialized = false;
    [SerializeField] private bool isPlaying = false;
    
    // 動畫事件
    public UnityEvent<string> OnAnimationStart;
    public UnityEvent<string> OnAnimationComplete;
    public UnityEvent<string, string> OnAnimationEvent; // 動畫名稱, 事件名稱
    
    // Spine相關
    private Spine.AnimationState animationState;
    private Skeleton skeleton;
    private TrackEntry currentTrackEntry;
    
    // 動畫隊列
    private System.Collections.Generic.Queue<AnimationRequest> animationQueue = 
        new System.Collections.Generic.Queue<AnimationRequest>();
    
    public bool IsInitialized => isInitialized;
    public bool IsPlaying => isPlaying;
    public CharacterDisplayMode CurrentDisplayMode => currentDisplayMode;
    public SkeletonGraphic SkeletonGraphic => skeletonGraphic;
    
    void Awake()
    {
        Initialize();
    }
    
    /// <summary>
    /// 初始化SkeletonGraphic控制器
    /// </summary>
    public void Initialize()
    {
        if (isInitialized) return;
        
        Debug.Log("[SkeletonGraphicController] 初始化SkeletonGraphic控制器");
        
        // 設置SkeletonGraphic組件
        SetupSkeletonGraphic();
        
        // 設置動畫狀態
        SetupAnimationState();
        
        // 設置事件監聽
        SetupEventListeners();
        
        // 播放默認動畫
        PlayDefaultAnimation();
        
        isInitialized = true;
        Debug.Log("[SkeletonGraphicController] SkeletonGraphic控制器初始化完成");
    }
    
    /// <summary>
    /// 設置SkeletonGraphic組件
    /// </summary>
    private void SetupSkeletonGraphic()
    {
        if (skeletonGraphic == null)
        {
            skeletonGraphic = GetComponent<SkeletonGraphic>();
        }
        
        if (skeletonGraphic == null)
        {
            Debug.LogError("[SkeletonGraphicController] 找不到SkeletonGraphic組件");
            return;
        }
        
        // 設置SkeletonDataAsset
        if (skeletonDataAsset != null)
        {
            skeletonGraphic.skeletonDataAsset = skeletonDataAsset;
        }
        
        // 初始化Skeleton
        skeletonGraphic.Initialize(false);
        
        if (skeletonGraphic.AnimationState != null)
        {
            skeletonGraphic.AnimationState.TimeScale = animationSpeed;
        }
    }
    
    /// <summary>
    /// 設置動畫狀態
    /// </summary>
    private void SetupAnimationState()
    {
        if (skeletonGraphic != null)
        {
            animationState = skeletonGraphic.AnimationState;
            skeleton = skeletonGraphic.Skeleton;
        }
        
        if (animationState == null)
        {
            Debug.LogError("[SkeletonGraphicController] 無法獲取AnimationState");
        }
    }
    
    /// <summary>
    /// 設置事件監聽
    /// </summary>
    private void SetupEventListeners()
    {
        if (animationState != null)
        {
            animationState.Start += OnSpineAnimationStart;
            animationState.Complete += OnSpineAnimationComplete;
            animationState.Event += OnSpineAnimationEvent;
        }
    }
    
    /// <summary>
    /// 播放默認動畫
    /// </summary>
    private void PlayDefaultAnimation()
    {
        if (!string.IsNullOrEmpty(defaultAnimation))
        {
            PlayAnimation(defaultAnimation, loopDefaultAnimation);
        }
    }
    
    #region 動畫播放控制
    
    /// <summary>
    /// 播放動畫
    /// </summary>
    public void PlayAnimation(string animationName, bool loop = true, int trackIndex = 0)
    {
        if (animationState == null || string.IsNullOrEmpty(animationName))
        {
            Debug.LogWarning($"[SkeletonGraphicController] 無法播放動畫: {animationName}");
            return;
        }
        
        // 檢查動畫是否存在
        if (!HasAnimation(animationName))
        {
            Debug.LogWarning($"[SkeletonGraphicController] 動畫不存在: {animationName}");
            return;
        }
        
        Debug.Log($"[SkeletonGraphicController] 播放動畫: {animationName} (循環: {loop})");
        
        currentTrackEntry = animationState.SetAnimation(trackIndex, animationName, loop);
        isPlaying = true;
        
        OnAnimationStart?.Invoke(animationName);
    }
    
    /// <summary>
    /// 排隊播放動畫
    /// </summary>
    public void QueueAnimation(string animationName, bool loop = false, float delay = 0f)
    {
        var request = new AnimationRequest
        {
            animationName = animationName,
            loop = loop,
            delay = delay,
            trackIndex = 0
        };
        
        if (currentTrackEntry == null || !isPlaying)
        {
            // 如果沒有正在播放的動畫，立即播放
            PlayAnimation(animationName, loop);
        }
        else
        {
            // 否則加入隊列
            animationQueue.Enqueue(request);
            Debug.Log($"[SkeletonGraphicController] 動畫已加入隊列: {animationName}");
        }
    }
    
    /// <summary>
    /// 添加動畫到指定軌道
    /// </summary>
    public void AddAnimation(string animationName, bool loop = false, float delay = 0f, int trackIndex = 0)
    {
        if (animationState == null || string.IsNullOrEmpty(animationName))
            return;
            
        if (!HasAnimation(animationName))
        {
            Debug.LogWarning($"[SkeletonGraphicController] 動畫不存在: {animationName}");
            return;
        }
        
        animationState.AddAnimation(trackIndex, animationName, loop, delay);
        Debug.Log($"[SkeletonGraphicController] 添加動畫: {animationName} (軌道: {trackIndex}, 延遲: {delay})");
    }
    
    /// <summary>
    /// 停止動畫
    /// </summary>
    public void StopAnimation(int trackIndex = 0)
    {
        if (animationState != null)
        {
            animationState.SetEmptyAnimation(trackIndex, 0.2f);
            isPlaying = false;
            
            Debug.Log($"[SkeletonGraphicController] 停止動畫 (軌道: {trackIndex})");
        }
    }
    
    /// <summary>
    /// 清空所有動畫
    /// </summary>
    public void ClearAllAnimations()
    {
        if (animationState != null)
        {
            animationState.ClearTracks();
            isPlaying = false;
            animationQueue.Clear();
            
            Debug.Log("[SkeletonGraphicController] 清空所有動畫");
        }
    }
    
    #endregion
    
    #region 顯示模式控制
    
    /// <summary>
    /// 設置顯示模式
    /// </summary>
    public void SetDisplayMode(CharacterDisplayMode mode)
    {
        if (currentDisplayMode == mode) return;
        
        currentDisplayMode = mode;
        
        Debug.Log($"[SkeletonGraphicController] 設置顯示模式: {mode}");
        
        switch (mode)
        {
            case CharacterDisplayMode.QVersion:
                SetQVersionMode();
                break;
                
            case CharacterDisplayMode.Dialog:
                SetDialogMode();
                break;
                
            case CharacterDisplayMode.Background:
                SetBackgroundMode();
                break;
        }
    }
    
    /// <summary>
    /// 設置Q版模式
    /// </summary>
    private void SetQVersionMode()
    {
        transform.localScale = qVersionScale;
        
        // 設置Q版動畫
        PlayAnimation("idle", true);
        
        // 調整顯示屬性
        if (skeletonGraphic != null)
        {
            skeletonGraphic.color = Color.white;
        }
    }
    
    /// <summary>
    /// 設置對話模式
    /// </summary>
    private void SetDialogMode()
    {
        transform.localScale = dialogScale;
        
        // 設置對話動畫
        PlayAnimation("talk", true);
        
        // 調整顯示屬性
        if (skeletonGraphic != null)
        {
            skeletonGraphic.color = Color.white;
        }
    }
    
    /// <summary>
    /// 設置背景模式
    /// </summary>
    private void SetBackgroundMode()
    {
        // 設置為背景顯示（通常是半透明或隱藏）
        if (skeletonGraphic != null)
        {
            skeletonGraphic.color = new Color(1, 1, 1, 0.3f);
        }
        
        // 播放背景動畫
        PlayAnimation("idle", true);
    }
    
    #endregion
    
    #region 動畫事件處理
    
    /// <summary>
    /// Spine動畫開始事件
    /// </summary>
    private void OnSpineAnimationStart(TrackEntry trackEntry)
    {
        string animationName = trackEntry.Animation?.Name ?? "unknown";
        Debug.Log($"[SkeletonGraphicController] 動畫開始: {animationName}");
        
        OnAnimationStart?.Invoke(animationName);
    }
    
    /// <summary>
    /// Spine動畫完成事件
    /// </summary>
    private void OnSpineAnimationComplete(TrackEntry trackEntry)
    {
        string animationName = trackEntry.Animation?.Name ?? "unknown";
        Debug.Log($"[SkeletonGraphicController] 動畫完成: {animationName}");
        
        isPlaying = false;
        OnAnimationComplete?.Invoke(animationName);
        
        // 處理動畫隊列
        ProcessAnimationQueue();
    }
    
    /// <summary>
    /// Spine動畫事件
    /// </summary>
    private void OnSpineAnimationEvent(TrackEntry trackEntry, Spine.Event e)
    {
        string animationName = trackEntry.Animation?.Name ?? "unknown";
        string eventName = e.Data.Name;
        
        Debug.Log($"[SkeletonGraphicController] 動畫事件: {animationName} - {eventName}");
        
        OnAnimationEvent?.Invoke(animationName, eventName);
    }
    
    /// <summary>
    /// 處理動畫隊列
    /// </summary>
    private void ProcessAnimationQueue()
    {
        if (animationQueue.Count > 0)
        {
            var nextRequest = animationQueue.Dequeue();
            PlayAnimation(nextRequest.animationName, nextRequest.loop, nextRequest.trackIndex);
        }
    }
    
    #endregion
    
    #region 工具方法
    
    /// <summary>
    /// 檢查動畫是否存在
    /// </summary>
    public bool HasAnimation(string animationName)
    {
        if (skeleton?.Data?.FindAnimation(animationName) != null)
            return true;
            
        return false;
    }
    
    /// <summary>
    /// 獲取所有動畫名稱
    /// </summary>
    public string[] GetAllAnimationNames()
    {
        if (skeleton?.Data?.Animations == null)
            return new string[0];
            
        var animations = skeleton.Data.Animations;
        string[] names = new string[animations.Count];
        
        for (int i = 0; i < animations.Count; i++)
        {
            names[i] = animations.Items[i].Name;
        }
        
        return names;
    }
    
    /// <summary>
    /// 設置動畫速度
    /// </summary>
    public void SetAnimationSpeed(float speed)
    {
        animationSpeed = speed;
        
        if (animationState != null)
        {
            animationState.TimeScale = speed;
        }
        
        Debug.Log($"[SkeletonGraphicController] 設置動畫速度: {speed}");
    }
    
    /// <summary>
    /// 設置Skin
    /// </summary>
    public void SetSkin(string skinName)
    {
        if (skeleton != null && !string.IsNullOrEmpty(skinName))
        {
            skeleton.SetSkin(skinName);
            skeleton.SetSlotsToSetupPose();
            
            Debug.Log($"[SkeletonGraphicController] 設置Skin: {skinName}");
        }
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 暫停動畫
    /// </summary>
    public void PauseAnimation()
    {
        if (animationState != null)
        {
            animationState.TimeScale = 0f;
        }
    }
    
    /// <summary>
    /// 恢復動畫
    /// </summary>
    public void ResumeAnimation()
    {
        if (animationState != null)
        {
            animationState.TimeScale = animationSpeed;
        }
    }
    
    /// <summary>
    /// 設置透明度
    /// </summary>
    public void SetAlpha(float alpha)
    {
        if (skeletonGraphic != null)
        {
            Color color = skeletonGraphic.color;
            color.a = Mathf.Clamp01(alpha);
            skeletonGraphic.color = color;
        }
    }
    
    /// <summary>
    /// 設置顏色
    /// </summary>
    public void SetColor(Color color)
    {
        if (skeletonGraphic != null)
        {
            skeletonGraphic.color = color;
        }
    }
    
    #endregion
    
    void OnDestroy()
    {
        // 清理事件監聽
        if (animationState != null)
        {
            animationState.Start -= OnSpineAnimationStart;
            animationState.Complete -= OnSpineAnimationComplete;
            animationState.Event -= OnSpineAnimationEvent;
        }
    }
}

/// <summary>
/// 動畫請求數據結構
/// </summary>
[System.Serializable]
public class AnimationRequest
{
    public string animationName;
    public bool loop;
    public float delay;
    public int trackIndex;
}