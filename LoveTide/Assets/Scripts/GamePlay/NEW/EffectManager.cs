using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 特效管理器
/// 
/// 職責:
/// 1. 管理視覺特效的播放和控制
/// 2. 提供粒子系統的統一接口
/// 3. 處理UI特效和場景特效
/// 4. 與互動系統協作提供反饋
/// 
/// 基於架構文檔: 提供統一的特效管理和播放
/// 支援粒子系統、動畫特效和UI特效
/// </summary>
public class EffectManager : MonoBehaviour
{
    [Header("== 特效配置 ==")]
    [SerializeField] private EffectDatabase effectDatabase;
    [SerializeField] private Transform effectContainer;
    [SerializeField] private int maxActiveEffects = 20;
    
    [Header("== 預製體池 ==")]
    [SerializeField] private int poolSize = 10;
    [SerializeField] private GameObject[] effectPrefabs;
    
    [Header("== 狀態管理 ==")]
    [SerializeField] private bool isInitialized = false;
    [SerializeField] private bool effectsEnabled = true;
    
    // 特效事件
    public UnityEvent<string> OnEffectStarted;
    public UnityEvent<string> OnEffectCompleted;
    public UnityEvent<int> OnActiveEffectCountChanged;
    
    // 特效字典和池
    private Dictionary<string, GameObject> effectPrefabDict = new Dictionary<string, GameObject>();
    private Dictionary<string, Queue<GameObject>> effectPools = new Dictionary<string, Queue<GameObject>>();
    private List<ActiveEffect> activeEffects = new List<ActiveEffect>();
    
    // 單例模式
    public static EffectManager Instance { get; private set; }
    
    public bool IsInitialized => isInitialized;
    public bool EffectsEnabled => effectsEnabled;
    public int ActiveEffectCount => activeEffects.Count;
    
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
    
    void Update()
    {
        UpdateActiveEffects();
    }
    
    /// <summary>
    /// 初始化特效管理器
    /// </summary>
    public void Initialize()
    {
        if (isInitialized) return;
        
        Debug.Log("[EffectManager] 初始化特效管理器");
        
        // 設置容器
        SetupEffectContainer();
        
        // 載入特效資源
        LoadEffectResources();
        
        // 初始化對象池
        InitializeEffectPools();
        
        isInitialized = true;
        Debug.Log("[EffectManager] 特效管理器初始化完成");
    }
    
    /// <summary>
    /// 設置特效容器
    /// </summary>
    private void SetupEffectContainer()
    {
        if (effectContainer == null)
        {
            GameObject containerObj = new GameObject("EffectContainer");
            containerObj.transform.SetParent(transform);
            effectContainer = containerObj.transform;
        }
    }
    
    /// <summary>
    /// 載入特效資源
    /// </summary>
    private void LoadEffectResources()
    {
        // 從資料庫載入
        if (effectDatabase != null)
        {
            LoadFromDatabase();
        }
        
        // 從預製體陣列載入
        LoadFromPrefabArray();
        
        // 從Resources載入
        LoadFromResources();
    }
    
    /// <summary>
    /// 從資料庫載入
    /// </summary>
    private void LoadFromDatabase()
    {
        foreach (var effect in effectDatabase.effectPrefabs)
        {
            if (effect.prefab != null)
            {
                effectPrefabDict[effect.key] = effect.prefab;
            }
        }
        
        Debug.Log($"[EffectManager] 從資料庫載入 {effectDatabase.effectPrefabs.Length} 個特效");
    }
    
    /// <summary>
    /// 從預製體陣列載入
    /// </summary>
    private void LoadFromPrefabArray()
    {
        if (effectPrefabs != null)
        {
            foreach (var prefab in effectPrefabs)
            {
                if (prefab != null && !effectPrefabDict.ContainsKey(prefab.name))
                {
                    effectPrefabDict[prefab.name] = prefab;
                }
            }
            
            Debug.Log($"[EffectManager] 從預製體陣列載入 {effectPrefabs.Length} 個特效");
        }
    }
    
    /// <summary>
    /// 從Resources載入
    /// </summary>
    private void LoadFromResources()
    {
        // 載入粒子特效
        GameObject[] particleEffects = Resources.LoadAll<GameObject>("Effects/Particles");
        foreach (var effect in particleEffects)
        {
            if (!effectPrefabDict.ContainsKey(effect.name))
            {
                effectPrefabDict[effect.name] = effect;
            }
        }
        
        // 載入UI特效
        GameObject[] uiEffects = Resources.LoadAll<GameObject>("Effects/UI");
        foreach (var effect in uiEffects)
        {
            if (!effectPrefabDict.ContainsKey(effect.name))
            {
                effectPrefabDict[effect.name] = effect;
            }
        }
        
        Debug.Log($"[EffectManager] 從Resources載入特效，總計: {effectPrefabDict.Count}");
    }
    
    /// <summary>
    /// 初始化特效對象池
    /// </summary>
    private void InitializeEffectPools()
    {
        foreach (var kvp in effectPrefabDict)
        {
            string effectKey = kvp.Key;
            GameObject prefab = kvp.Value;
            
            Queue<GameObject> pool = new Queue<GameObject>();
            
            // 預先創建對象池
            for (int i = 0; i < poolSize; i++)
            {
                GameObject poolObj = Instantiate(prefab, effectContainer);
                poolObj.SetActive(false);
                poolObj.name = $"{effectKey}_Pool_{i}";
                
                pool.Enqueue(poolObj);
            }
            
            effectPools[effectKey] = pool;
        }
        
        Debug.Log($"[EffectManager] 初始化 {effectPools.Count} 個特效對象池");
    }
    
    #region 特效播放控制
    
    /// <summary>
    /// 播放特效
    /// </summary>
    public GameObject PlayEffect(string effectKey, Vector3 position, Transform parent = null, float duration = -1f)
    {
        if (!effectsEnabled)
        {
            Debug.Log($"[EffectManager] 特效已禁用，忽略播放: {effectKey}");
            return null;
        }
        
        if (!effectPrefabDict.ContainsKey(effectKey))
        {
            Debug.LogWarning($"[EffectManager] 找不到特效: {effectKey}");
            return null;
        }
        
        // 檢查活躍特效數量限制
        if (activeEffects.Count >= maxActiveEffects)
        {
            Debug.LogWarning("[EffectManager] 達到最大活躍特效數量限制");
            return null;
        }
        
        GameObject effectObj = GetEffectFromPool(effectKey);
        if (effectObj == null)
        {
            Debug.LogWarning($"[EffectManager] 無法從對象池獲取特效: {effectKey}");
            return null;
        }
        
        // 設置位置和父物件
        effectObj.transform.position = position;
        if (parent != null)
        {
            effectObj.transform.SetParent(parent);
        }
        else
        {
            effectObj.transform.SetParent(effectContainer);
        }
        
        // 激活特效
        effectObj.SetActive(true);
        
        // 計算持續時間
        float effectDuration = duration > 0 ? duration : GetEffectDuration(effectObj);
        
        // 添加到活躍特效列表
        ActiveEffect activeEffect = new ActiveEffect
        {
            effectObject = effectObj,
            effectKey = effectKey,
            startTime = Time.time,
            duration = effectDuration,
            isLoop = IsLoopingEffect(effectObj)
        };
        
        activeEffects.Add(activeEffect);
        
        Debug.Log($"[EffectManager] 播放特效: {effectKey} 在位置 {position}");
        
        OnEffectStarted?.Invoke(effectKey);
        OnActiveEffectCountChanged?.Invoke(activeEffects.Count);
        
        return effectObj;
    }
    
    /// <summary>
    /// 播放UI特效
    /// </summary>
    public GameObject PlayUIEffect(string effectKey, Transform uiParent, Vector2 localPosition, float duration = -1f)
    {
        GameObject effectObj = PlayEffect(effectKey, Vector3.zero, uiParent, duration);
        
        if (effectObj != null)
        {
            RectTransform rectTransform = effectObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = localPosition;
            }
        }
        
        return effectObj;
    }
    
    /// <summary>
    /// 播放跟隨特效
    /// </summary>
    public GameObject PlayFollowEffect(string effectKey, Transform target, Vector3 offset, float duration = -1f)
    {
        GameObject effectObj = PlayEffect(effectKey, target.position + offset, null, duration);
        
        if (effectObj != null)
        {
            FollowTarget followComponent = effectObj.GetComponent<FollowTarget>();
            if (followComponent == null)
            {
                followComponent = effectObj.AddComponent<FollowTarget>();
            }
            
            followComponent.SetTarget(target, offset);
        }
        
        return effectObj;
    }
    
    /// <summary>
    /// 停止特效
    /// </summary>
    public void StopEffect(GameObject effectObj)
    {
        if (effectObj == null) return;
        
        // 從活躍列表中移除
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (activeEffects[i].effectObject == effectObj)
            {
                string effectKey = activeEffects[i].effectKey;
                activeEffects.RemoveAt(i);
                
                OnEffectCompleted?.Invoke(effectKey);
                OnActiveEffectCountChanged?.Invoke(activeEffects.Count);
                break;
            }
        }
        
        // 回收到對象池
        RecycleEffect(effectObj);
    }
    
    /// <summary>
    /// 停止所有特效
    /// </summary>
    public void StopAllEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            StopEffect(activeEffects[i].effectObject);
        }
        
        Debug.Log("[EffectManager] 停止所有特效");
    }
    
    /// <summary>
    /// 停止指定類型的所有特效
    /// </summary>
    public void StopEffectsByKey(string effectKey)
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (activeEffects[i].effectKey == effectKey)
            {
                StopEffect(activeEffects[i].effectObject);
            }
        }
        
        Debug.Log($"[EffectManager] 停止所有 {effectKey} 特效");
    }
    
    #endregion
    
    #region 對象池管理
    
    /// <summary>
    /// 從對象池獲取特效
    /// </summary>
    private GameObject GetEffectFromPool(string effectKey)
    {
        if (effectPools.ContainsKey(effectKey) && effectPools[effectKey].Count > 0)
        {
            return effectPools[effectKey].Dequeue();
        }
        
        // 對象池空了，創建新的
        if (effectPrefabDict.ContainsKey(effectKey))
        {
            GameObject newEffect = Instantiate(effectPrefabDict[effectKey], effectContainer);
            newEffect.SetActive(false);
            newEffect.name = $"{effectKey}_Runtime";
            return newEffect;
        }
        
        return null;
    }
    
    /// <summary>
    /// 回收特效到對象池
    /// </summary>
    private void RecycleEffect(GameObject effectObj)
    {
        if (effectObj == null) return;
        
        // 停止所有粒子系統
        ParticleSystem[] particles = effectObj.GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in particles)
        {
            particle.Stop();
            particle.Clear();
        }
        
        // 重置變換
        effectObj.transform.SetParent(effectContainer);
        effectObj.transform.localPosition = Vector3.zero;
        effectObj.transform.localRotation = Quaternion.identity;
        effectObj.transform.localScale = Vector3.one;
        
        // 禁用物件
        effectObj.SetActive(false);
        
        // 找到對應的對象池
        string effectKey = FindEffectKey(effectObj);
        if (!string.IsNullOrEmpty(effectKey) && effectPools.ContainsKey(effectKey))
        {
            effectPools[effectKey].Enqueue(effectObj);
        }
        else
        {
            // 如果找不到對應池，直接銷毀
            Destroy(effectObj);
        }
    }
    
    /// <summary>
    /// 找到特效的Key
    /// </summary>
    private string FindEffectKey(GameObject effectObj)
    {
        foreach (var kvp in effectPrefabDict)
        {
            if (effectObj.name.Contains(kvp.Key))
            {
                return kvp.Key;
            }
        }
        return null;
    }
    
    #endregion
    
    #region 特效檢測工具
    
    /// <summary>
    /// 獲取特效持續時間
    /// </summary>
    private float GetEffectDuration(GameObject effectObj)
    {
        float maxDuration = 1f; // 默認1秒
        
        // 檢查粒子系統
        ParticleSystem[] particles = effectObj.GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in particles)
        {
            float particleDuration = particle.main.duration + particle.main.startLifetime.constantMax;
            if (particleDuration > maxDuration)
            {
                maxDuration = particleDuration;
            }
        }
        
        // 檢查動畫
        Animator animator = effectObj.GetComponent<Animator>();
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                if (clip.length > maxDuration)
                {
                    maxDuration = clip.length;
                }
            }
        }
        
        // 檢查音頻
        AudioSource audioSource = effectObj.GetComponent<AudioSource>();
        if (audioSource != null && audioSource.clip != null)
        {
            if (audioSource.clip.length > maxDuration)
            {
                maxDuration = audioSource.clip.length;
            }
        }
        
        return maxDuration;
    }
    
    /// <summary>
    /// 檢查是否為循環特效
    /// </summary>
    private bool IsLoopingEffect(GameObject effectObj)
    {
        // 檢查粒子系統
        ParticleSystem[] particles = effectObj.GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in particles)
        {
            if (particle.main.loop)
            {
                return true;
            }
        }
        
        // 檢查動畫
        Animator animator = effectObj.GetComponent<Animator>();
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.loop)
            {
                return true;
            }
        }
        
        // 檢查音頻
        AudioSource audioSource = effectObj.GetComponent<AudioSource>();
        if (audioSource != null && audioSource.loop)
        {
            return true;
        }
        
        return false;
    }
    
    #endregion
    
    #region 活躍特效管理
    
    /// <summary>
    /// 更新活躍特效
    /// </summary>
    private void UpdateActiveEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            ActiveEffect effect = activeEffects[i];
            
            // 檢查特效是否應該結束
            if (ShouldEffectEnd(effect))
            {
                StopEffect(effect.effectObject);
            }
        }
    }
    
    /// <summary>
    /// 檢查特效是否應該結束
    /// </summary>
    private bool ShouldEffectEnd(ActiveEffect effect)
    {
        // 如果物件被銷毀了
        if (effect.effectObject == null)
            return true;
        
        // 如果是循環特效，不自動結束
        if (effect.isLoop)
            return false;
        
        // 檢查時間是否到了
        float elapsedTime = Time.time - effect.startTime;
        if (elapsedTime >= effect.duration)
            return true;
        
        // 檢查粒子系統是否還在播放
        ParticleSystem[] particles = effect.effectObject.GetComponentsInChildren<ParticleSystem>();
        bool anyParticleAlive = false;
        
        foreach (var particle in particles)
        {
            if (particle.IsAlive())
            {
                anyParticleAlive = true;
                break;
            }
        }
        
        // 如果沒有粒子還活著，結束特效
        if (particles.Length > 0 && !anyParticleAlive)
            return true;
        
        return false;
    }
    
    #endregion
    
    #region 特效控制
    
    /// <summary>
    /// 啟用/禁用特效
    /// </summary>
    public void SetEffectsEnabled(bool enabled)
    {
        effectsEnabled = enabled;
        
        if (!enabled)
        {
            StopAllEffects();
        }
        
        Debug.Log($"[EffectManager] 特效系統: {(enabled ? "啟用" : "禁用")}");
    }
    
    /// <summary>
    /// 設置最大活躍特效數量
    /// </summary>
    public void SetMaxActiveEffects(int maxCount)
    {
        maxActiveEffects = Mathf.Max(1, maxCount);
        
        // 如果當前活躍特效超過限制，停止最舊的特效
        while (activeEffects.Count > maxActiveEffects)
        {
            StopEffect(activeEffects[0].effectObject);
        }
        
        Debug.Log($"[EffectManager] 設置最大活躍特效數量: {maxActiveEffects}");
    }
    
    /// <summary>
    /// 預熱特效對象池
    /// </summary>
    public void PrewarmEffectPool(string effectKey, int count)
    {
        if (!effectPools.ContainsKey(effectKey))
        {
            effectPools[effectKey] = new Queue<GameObject>();
        }
        
        for (int i = 0; i < count; i++)
        {
            if (effectPrefabDict.ContainsKey(effectKey))
            {
                GameObject poolObj = Instantiate(effectPrefabDict[effectKey], effectContainer);
                poolObj.SetActive(false);
                poolObj.name = $"{effectKey}_Prewarm_{i}";
                
                effectPools[effectKey].Enqueue(poolObj);
            }
        }
        
        Debug.Log($"[EffectManager] 預熱 {effectKey} 對象池: {count} 個物件");
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 檢查特效是否存在
    /// </summary>
    public bool HasEffect(string effectKey)
    {
        return effectPrefabDict.ContainsKey(effectKey);
    }
    
    /// <summary>
    /// 獲取所有可用特效名稱
    /// </summary>
    public string[] GetAvailableEffects()
    {
        return new List<string>(effectPrefabDict.Keys).ToArray();
    }
    
    /// <summary>
    /// 獲取特效統計信息
    /// </summary>
    public EffectStats GetEffectStats()
    {
        return new EffectStats
        {
            totalEffectTypes = effectPrefabDict.Count,
            activeEffectCount = activeEffects.Count,
            maxActiveEffects = maxActiveEffects,
            totalPoolSize = GetTotalPoolSize()
        };
    }
    
    /// <summary>
    /// 獲取總對象池大小
    /// </summary>
    private int GetTotalPoolSize()
    {
        int total = 0;
        foreach (var pool in effectPools.Values)
        {
            total += pool.Count;
        }
        return total;
    }
    
    #endregion
}

/// <summary>
/// 活躍特效數據
/// </summary>
[System.Serializable]
public class ActiveEffect
{
    public GameObject effectObject;
    public string effectKey;
    public float startTime;
    public float duration;
    public bool isLoop;
}

/// <summary>
/// 特效數據
/// </summary>
[System.Serializable]
public class EffectData
{
    public string key;
    public GameObject prefab;
}

/// <summary>
/// 特效資料庫
/// </summary>
[CreateAssetMenu(fileName = "EffectDatabase", menuName = "LoveTide/EffectDatabase")]
public class EffectDatabase : ScriptableObject
{
    [Header("特效預製體")]
    public EffectData[] effectPrefabs;
}

/// <summary>
/// 特效統計
/// </summary>
[System.Serializable]
public class EffectStats
{
    public int totalEffectTypes;
    public int activeEffectCount;
    public int maxActiveEffects;
    public int totalPoolSize;
}

/// <summary>
/// 跟隨目標組件
/// </summary>
public class FollowTarget : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;
    
    public void SetTarget(Transform newTarget, Vector3 newOffset)
    {
        target = newTarget;
        offset = newOffset;
    }
    
    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}