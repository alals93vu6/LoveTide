using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 新版遊戲管理器 - 基於Canvas + SkeletonGraphic架構
/// 
/// 職責:
/// 1. 統籌遊戲整體流程
/// 2. 管理場景切換、狀態控制、存檔載入
/// 3. 協調時間系統、數值系統、演員系統
/// 4. 整合InteractionManager和NewUIManager
/// 
/// 設計原則:
/// - 中央調度器模式
/// - 事件驅動架構 
/// - 與現有系統協調工作
/// </summary>
public class NewGameManager : MonoBehaviour
{
    [Header("== 核心系統引用 ==")]
    [SerializeField] private TimeManagerTest timeManager;
    [SerializeField] private NumericalRecords numericalRecords;
    [SerializeField] private ActorManagerTest actorManager;
    [SerializeField] private InteractionManager interactionManager;
    [SerializeField] private NewUIManager newUIManager;
    
    [Header("== 遊戲狀態管理 ==")]
    [SerializeField] private SceneStateManager sceneStateManager;
    [SerializeField] private GameDataManager gameDataManager;
    
    [Header("== 當前遊戲狀態 ==")]
    public GameState currentGameState = GameState.MainMenu;
    public SceneMode currentSceneMode = SceneMode.Tavern;
    public InteractionMode currentInteractionMode = InteractionMode.SceneOverview;
    
    // 遊戲狀態事件
    public UnityEvent<GameState> OnGameStateChanged;
    public UnityEvent<SceneMode> OnSceneModeChanged;
    public UnityEvent<InteractionMode> OnInteractionModeChanged;
    
    // 單例模式
    public static NewGameManager Instance { get; private set; }
    
    void Awake()
    {
        // 單例設置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGameManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        StartCoroutine(InitializeGameSystems());
    }
    
    /// <summary>
    /// 初始化遊戲管理器
    /// </summary>
    private void InitializeGameManager()
    {
        Debug.Log("[NewGameManager] 初始化遊戲管理器");
        
        // 確保所有組件都已設置
        if (sceneStateManager == null)
            sceneStateManager = GetComponent<SceneStateManager>();
            
        if (gameDataManager == null)
            gameDataManager = GetComponent<GameDataManager>();
    }
    
    /// <summary>
    /// 初始化所有遊戲系統
    /// </summary>
    private IEnumerator InitializeGameSystems()
    {
        Debug.Log("[NewGameManager] 開始初始化所有遊戲系統");
        
        // 1. 初始化數據系統
        if (gameDataManager != null)
        {
            gameDataManager.Initialize();
            yield return new WaitUntil(() => gameDataManager.IsInitialized);
        }
        
        // 2. 初始化場景狀態系統
        if (sceneStateManager != null)
        {
            sceneStateManager.Initialize();
            yield return new WaitUntil(() => sceneStateManager.IsInitialized);
        }
        
        // 3. 初始化UI管理器
        if (newUIManager != null)
        {
            newUIManager.Initialize();
            yield return new WaitUntil(() => newUIManager.IsInitialized);
        }
        
        // 4. 初始化互動管理器
        if (interactionManager != null)
        {
            interactionManager.Initialize();
            yield return new WaitUntil(() => interactionManager.IsInitialized);
        }
        
        // 5. 設置事件監聽
        SetupEventListeners();
        
        Debug.Log("[NewGameManager] 所有系統初始化完成");
        
        // 開始遊戲
        ChangeGameState(GameState.Gameplay);
    }
    
    /// <summary>
    /// 設置事件監聽
    /// </summary>
    private void SetupEventListeners()
    {
        // 時間系統事件
        if (timeManager != null)
        {
            // 假設TimeManager有時間變化事件
            // timeManager.OnTimeChanged.AddListener(OnTimeChanged);
        }
        
        // 數值系統事件
        if (numericalRecords != null)
        {
            // 假設NumericalRecords有數值變化事件
            // numericalRecords.OnValueChanged.AddListener(OnNumericalValueChanged);
        }
        
        // 互動系統事件
        if (interactionManager != null)
        {
            interactionManager.OnInteractionTriggered.AddListener(OnInteractionTriggered);
            interactionManager.OnDialogModeEntered.AddListener(OnDialogModeEntered);
            interactionManager.OnSceneModeEntered.AddListener(OnSceneModeEntered);
        }
    }
    
    #region 遊戲狀態管理
    
    /// <summary>
    /// 改變遊戲狀態
    /// </summary>
    public void ChangeGameState(GameState newState)
    {
        if (currentGameState == newState) return;
        
        GameState previousState = currentGameState;
        currentGameState = newState;
        
        Debug.Log($"[NewGameManager] 遊戲狀態變更: {previousState} → {newState}");
        
        // 處理狀態轉換邏輯
        HandleGameStateTransition(previousState, newState);
        
        // 觸發事件
        OnGameStateChanged?.Invoke(newState);
    }
    
    /// <summary>
    /// 改變場景模式
    /// </summary>
    public void ChangeSceneMode(SceneMode newMode)
    {
        if (currentSceneMode == newMode) return;
        
        SceneMode previousMode = currentSceneMode;
        currentSceneMode = newMode;
        
        Debug.Log($"[NewGameManager] 場景模式變更: {previousMode} → {newMode}");
        
        // 通知場景狀態管理器
        sceneStateManager?.ChangeSceneMode(newMode);
        
        // 觸發事件
        OnSceneModeChanged?.Invoke(newMode);
    }
    
    /// <summary>
    /// 改變互動模式
    /// </summary>
    public void ChangeInteractionMode(InteractionMode newMode)
    {
        if (currentInteractionMode == newMode) return;
        
        InteractionMode previousMode = currentInteractionMode;
        currentInteractionMode = newMode;
        
        Debug.Log($"[NewGameManager] 互動模式變更: {previousMode} → {newMode}");
        
        // 通知UI管理器
        newUIManager?.SetInteractionMode(newMode);
        
        // 觸發事件
        OnInteractionModeChanged?.Invoke(newMode);
    }
    
    /// <summary>
    /// 處理遊戲狀態轉換
    /// </summary>
    private void HandleGameStateTransition(GameState from, GameState to)
    {
        switch (to)
        {
            case GameState.MainMenu:
                // 進入主選單邏輯
                break;
                
            case GameState.Gameplay:
                // 進入遊戲邏輯
                ChangeSceneMode(SceneMode.Tavern);
                ChangeInteractionMode(InteractionMode.SceneOverview);
                break;
                
            case GameState.Dialog:
                // 進入對話邏輯
                ChangeInteractionMode(InteractionMode.CharacterDialog);
                break;
                
            case GameState.Paused:
                // 暫停邏輯
                Time.timeScale = 0f;
                break;
                
            default:
                if (from == GameState.Paused)
                {
                    Time.timeScale = 1f;
                }
                break;
        }
    }
    
    #endregion
    
    #region 事件處理
    
    /// <summary>
    /// 處理互動觸發事件
    /// </summary>
    private void OnInteractionTriggered(InteractionType interactionType, GameObject target)
    {
        Debug.Log($"[NewGameManager] 互動觸發: {interactionType} - {target.name}");
        
        // 根據互動類型處理邏輯
        switch (interactionType)
        {
            case InteractionType.CharacterTalk:
                ChangeGameState(GameState.Dialog);
                break;
                
            case InteractionType.Work:
                // 處理工作邏輯
                ProcessWorkInteraction();
                break;
                
            case InteractionType.CatInteraction:
                // 處理貓咪互動邏輯
                ProcessCatInteraction();
                break;
                
            case InteractionType.GoOut:
                // 處理外出邏輯
                ProcessGoOutInteraction();
                break;
        }
    }
    
    /// <summary>
    /// 進入對話模式
    /// </summary>
    private void OnDialogModeEntered()
    {
        ChangeInteractionMode(InteractionMode.CharacterDialog);
    }
    
    /// <summary>
    /// 進入場景模式
    /// </summary>
    private void OnSceneModeEntered()
    {
        ChangeInteractionMode(InteractionMode.SceneOverview);
    }
    
    #endregion
    
    #region 互動處理邏輯
    
    private void ProcessWorkInteraction()
    {
        Debug.Log("[NewGameManager] 處理工作互動");
        // 實現工作邏輯
        // 1. 檢查時間條件
        // 2. 更新數值
        // 3. 播放相應反饋
    }
    
    private void ProcessCatInteraction()
    {
        Debug.Log("[NewGameManager] 處理貓咪互動");
        // 實現貓咪互動邏輯
        // 1. 檢查是否有魚
        // 2. 播放互動動畫
        // 3. 更新好感度
    }
    
    private void ProcessGoOutInteraction()
    {
        Debug.Log("[NewGameManager] 處理外出互動");
        // 實現外出邏輯
        // 1. 檢查外出條件
        // 2. 切換場景
        // 3. 更新時間
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 暫停遊戲
    /// </summary>
    public void PauseGame()
    {
        ChangeGameState(GameState.Paused);
    }
    
    /// <summary>
    /// 繼續遊戲
    /// </summary>
    public void ResumeGame()
    {
        ChangeGameState(GameState.Gameplay);
    }
    
    /// <summary>
    /// 獲取當前遊戲數據
    /// </summary>
    public GameData GetCurrentGameData()
    {
        return gameDataManager?.GetCurrentGameData();
    }
    
    /// <summary>
    /// 保存遊戲
    /// </summary>
    public void SaveGame()
    {
        gameDataManager?.SaveGame();
    }
    
    /// <summary>
    /// 載入遊戲
    /// </summary>
    public void LoadGame()
    {
        gameDataManager?.LoadGame();
    }
    
    #endregion
}

// 遊戲狀態枚舉
public enum GameState
{
    MainMenu,
    Gameplay,
    Dialog,
    Paused,
    Loading,
    Saving
}

// 場景模式枚舉
public enum SceneMode
{
    Tavern,     // 酒吧場景
    Dormitory   // 宿舍場景
}

// 互動模式枚舉
public enum InteractionMode
{
    SceneOverview,      // Q版場景總覽
    CharacterDialog     // 角色對話模式
}

// InteractionType 枚舉定義已移至 InteractionManager.cs 以避免重複定義