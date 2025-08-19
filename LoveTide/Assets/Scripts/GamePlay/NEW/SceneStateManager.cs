using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 場景狀態管理器
/// 
/// 職責:
/// 1. 管理場景狀態切換 (酒吧 ⇄ 宿舍)
/// 2. 控制場景視覺切換
/// 3. 管理場景特定的互動物件啟用/禁用
/// 4. 協調場景相關的時間和條件檢查
/// 
/// 基於架構文檔: NurturingMode/互動系統完整設計_重製版.md
/// </summary>
public class SceneStateManager : MonoBehaviour
{
    [Header("== 場景狀態配置 ==")]
    [SerializeField] private SceneMode currentScene = SceneMode.Tavern;
    [SerializeField] private bool isInitialized = false;
    
    [Header("== 場景切換配置 ==")]
    [SerializeField] private float sceneTransitionDuration = 0.5f;
    [SerializeField] private bool isTransitioning = false;
    
    // 場景狀態事件
    public UnityEvent<SceneMode> OnSceneChanged;
    public UnityEvent OnSceneTransitionStart;
    public UnityEvent OnSceneTransitionComplete;
    
    // 場景切換條件檢查
    public System.Func<SceneMode, bool> SceneTransitionConditionChecker;
    
    public bool IsInitialized => isInitialized;
    public SceneMode CurrentScene => currentScene;
    public bool IsTransitioning => isTransitioning;
    
    /// <summary>
    /// 初始化場景狀態管理器
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[SceneStateManager] 初始化場景狀態管理器");
        
        // 設置初始場景狀態
        SetupInitialSceneState();
        
        isInitialized = true;
        Debug.Log($"[SceneStateManager] 初始化完成，當前場景: {currentScene}");
    }
    
    /// <summary>
    /// 設置初始場景狀態
    /// </summary>
    private void SetupInitialSceneState()
    {
        // 根據遊戲進度或存檔數據設置初始場景
        // 默認從酒吧開始
        currentScene = SceneMode.Tavern;
        
        // 觸發場景設置事件
        OnSceneChanged?.Invoke(currentScene);
    }
    
    /// <summary>
    /// 改變場景模式
    /// </summary>
    public void ChangeSceneMode(SceneMode newScene)
    {
        if (currentScene == newScene)
        {
            Debug.Log($"[SceneStateManager] 已經在場景 {newScene}，無需切換");
            return;
        }
        
        if (isTransitioning)
        {
            Debug.LogWarning("[SceneStateManager] 場景正在切換中，忽略切換請求");
            return;
        }
        
        // 檢查場景切換條件
        if (!CanTransitionToScene(newScene))
        {
            Debug.LogWarning($"[SceneStateManager] 不滿足切換到場景 {newScene} 的條件");
            return;
        }
        
        StartCoroutine(TransitionToScene(newScene));
    }
    
    /// <summary>
    /// 檢查是否可以切換到指定場景
    /// </summary>
    private bool CanTransitionToScene(SceneMode targetScene)
    {
        // 使用外部條件檢查器
        if (SceneTransitionConditionChecker != null)
        {
            return SceneTransitionConditionChecker(targetScene);
        }
        
        // 默認條件檢查
        switch (targetScene)
        {
            case SceneMode.Tavern:
                // 酒吧通常沒有特殊條件
                return true;
                
            case SceneMode.Dormitory:
                // 宿舍可能需要檢查時間條件
                return CheckDormitoryAccessCondition();
                
            default:
                return false;
        }
    }
    
    /// <summary>
    /// 檢查宿舍訪問條件
    /// </summary>
    private bool CheckDormitoryAccessCondition()
    {
        // 實現宿舍訪問條件邏輯
        // 例如：檢查時間、好感度等
        
        // 暫時返回true，實際邏輯需要根據遊戲設計實現
        return true;
    }
    
    /// <summary>
    /// 場景切換協程
    /// </summary>
    private IEnumerator TransitionToScene(SceneMode targetScene)
    {
        isTransitioning = true;
        SceneMode previousScene = currentScene;
        
        Debug.Log($"[SceneStateManager] 開始場景切換: {previousScene} → {targetScene}");
        
        // 觸發場景切換開始事件
        OnSceneTransitionStart?.Invoke();
        
        // 1. 場景退出處理
        yield return StartCoroutine(HandleSceneExit(previousScene));
        
        // 2. 切換場景狀態
        currentScene = targetScene;
        
        // 3. 場景進入處理
        yield return StartCoroutine(HandleSceneEnter(targetScene));
        
        // 4. 等待切換動畫完成
        yield return new WaitForSeconds(sceneTransitionDuration);
        
        isTransitioning = false;
        
        Debug.Log($"[SceneStateManager] 場景切換完成: {targetScene}");
        
        // 觸發場景變更事件
        OnSceneChanged?.Invoke(targetScene);
        OnSceneTransitionComplete?.Invoke();
    }
    
    /// <summary>
    /// 處理場景退出
    /// </summary>
    private IEnumerator HandleSceneExit(SceneMode exitingScene)
    {
        Debug.Log($"[SceneStateManager] 處理場景退出: {exitingScene}");
        
        switch (exitingScene)
        {
            case SceneMode.Tavern:
                yield return HandleTavernExit();
                break;
                
            case SceneMode.Dormitory:
                yield return HandleDormitoryExit();
                break;
        }
    }
    
    /// <summary>
    /// 處理場景進入
    /// </summary>
    private IEnumerator HandleSceneEnter(SceneMode enteringScene)
    {
        Debug.Log($"[SceneStateManager] 處理場景進入: {enteringScene}");
        
        switch (enteringScene)
        {
            case SceneMode.Tavern:
                yield return HandleTavernEnter();
                break;
                
            case SceneMode.Dormitory:
                yield return HandleDormitoryEnter();
                break;
        }
    }
    
    #region 場景特定處理
    
    /// <summary>
    /// 酒吧場景退出處理
    /// </summary>
    private IEnumerator HandleTavernExit()
    {
        // 禁用酒吧特定的互動物件
        // 保存酒吧場景狀態
        // 播放退出動畫
        
        yield return null; // 暫時空實現
    }
    
    /// <summary>
    /// 酒吧場景進入處理
    /// </summary>
    private IEnumerator HandleTavernEnter()
    {
        // 啟用酒吧特定的互動物件
        // 恢復酒吧場景狀態
        // 播放進入動畫
        // 設置Yuka在酒吧的位置和狀態
        
        yield return null; // 暫時空實現
    }
    
    /// <summary>
    /// 宿舍場景退出處理
    /// </summary>
    private IEnumerator HandleDormitoryExit()
    {
        // 禁用宿舍特定的互動物件
        // 保存宿舍場景狀態
        // 播放退出動畫
        
        yield return null; // 暫時空實現
    }
    
    /// <summary>
    /// 宿舍場景進入處理
    /// </summary>
    private IEnumerator HandleDormitoryEnter()
    {
        // 啟用宿舍特定的互動物件
        // 恢復宿舍場景狀態
        // 播放進入動畫
        // 設置Yuka在宿舍的位置和狀態
        
        yield return null; // 暫時空實現
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 切換到酒吧場景
    /// </summary>
    public void TransitionToTavern()
    {
        ChangeSceneMode(SceneMode.Tavern);
    }
    
    /// <summary>
    /// 切換到宿舍場景
    /// </summary>
    public void TransitionToDormitory()
    {
        ChangeSceneMode(SceneMode.Dormitory);
    }
    
    /// <summary>
    /// 獲取當前場景的配置信息
    /// </summary>
    public SceneConfig GetCurrentSceneConfig()
    {
        return GetSceneConfig(currentScene);
    }
    
    /// <summary>
    /// 獲取指定場景的配置信息
    /// </summary>
    public SceneConfig GetSceneConfig(SceneMode scene)
    {
        switch (scene)
        {
            case SceneMode.Tavern:
                return new SceneConfig
                {
                    sceneName = "酒吧",
                    allowedInteractions = new[] { "Work", "CatInteraction", "CharacterTalk" },
                    backgroundMusicKey = "tavern_bgm",
                    ambientSoundKey = "tavern_ambient"
                };
                
            case SceneMode.Dormitory:
                return new SceneConfig
                {
                    sceneName = "宿舍",
                    allowedInteractions = new[] { "Rest", "DrinkingInvite", "CharacterTalk" },
                    backgroundMusicKey = "dormitory_bgm",
                    ambientSoundKey = "dormitory_ambient"
                };
                
            default:
                return new SceneConfig { sceneName = "未知場景" };
        }
    }
    
    /// <summary>
    /// 設置場景切換條件檢查器
    /// </summary>
    public void SetSceneTransitionConditionChecker(System.Func<SceneMode, bool> conditionChecker)
    {
        SceneTransitionConditionChecker = conditionChecker;
    }
    
    #endregion
}

/// <summary>
/// 場景配置數據結構
/// </summary>
[System.Serializable]
public class SceneConfig
{
    public string sceneName;
    public string[] allowedInteractions;
    public string backgroundMusicKey;
    public string ambientSoundKey;
}