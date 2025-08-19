using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 互動管理器
/// 
/// 職責:
/// 1. 統一管理所有互動系統
/// 2. 檢查互動解鎖條件
/// 3. 處理互動結果和數值變化
/// 4. 協調靜態和動態互動系統
/// 
/// 基於架構文檔: NurturingMode/互動系統完整設計_重製版.md
/// 與CoreSystems數據流架構.md的事件驅動設計對應
/// </summary>
public class InteractionManager : MonoBehaviour
{
    [Header("== 互動系統引用 ==")]
    [SerializeField] private StaticObjectInteraction staticObjectInteraction;
    [SerializeField] private SceneInteractionController sceneInteractionController;
    
    [Header("== Canvas引用 ==")]
    [SerializeField] private Canvas dynamicCharacterCanvas;
    [SerializeField] private Canvas staticInteractionCanvas;
    [SerializeField] private Canvas dialogCanvas;
    [SerializeField] private Canvas menuCanvas;
    
    [Header("== 互動條件檢查 ==")]
    [SerializeField] private InteractionConditionChecker conditionChecker;
    [SerializeField] private InteractionResultProcessor resultProcessor;
    
    [Header("== 狀態管理 ==")]
    [SerializeField] private bool isInitialized = false;
    [SerializeField] private bool interactionsEnabled = true;
    
    // 互動事件
    public UnityEvent<InteractionType, GameObject> OnInteractionTriggered;
    public UnityEvent OnDialogModeEntered;
    public UnityEvent OnSceneModeEntered;
    public UnityEvent<InteractionResult> OnInteractionCompleted;
    
    // 互動狀態追踪
    private Dictionary<InteractionType, bool> interactionStates;
    private InteractionType currentInteraction = InteractionType.None;
    
    public bool IsInitialized => isInitialized;
    public InteractionType CurrentInteraction => currentInteraction;
    public bool InteractionsEnabled => interactionsEnabled;
    
    /// <summary>
    /// 初始化互動管理器
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[InteractionManager] 初始化互動管理器");
        
        // 初始化互動狀態字典
        InitializeInteractionStates();
        
        // 查找並綁定子系統
        FindAndBindSubSystems();
        
        // 初始化子系統
        InitializeSubSystems();
        
        // 設置事件監聽
        SetupEventListeners();
        
        isInitialized = true;
        Debug.Log("[InteractionManager] 互動管理器初始化完成");
    }
    
    /// <summary>
    /// 初始化互動狀態字典
    /// </summary>
    private void InitializeInteractionStates()
    {
        interactionStates = new Dictionary<InteractionType, bool>
        {
            { InteractionType.CharacterTalk, true },
            { InteractionType.Work, true },
            { InteractionType.CatInteraction, true },
            { InteractionType.GoOut, true },
            { InteractionType.SceneTransition, true },
            { InteractionType.Rest, true },
            { InteractionType.DrinkingInvite, true }
        };
    }
    
    /// <summary>
    /// 查找並綁定子系統
    /// </summary>
    private void FindAndBindSubSystems()
    {
        // 查找StaticObjectInteraction
        if (staticObjectInteraction == null)
        {
            staticObjectInteraction = FindObjectOfType<StaticObjectInteraction>();
        }
        
        // 查找SceneInteractionController
        if (sceneInteractionController == null)
        {
            sceneInteractionController = FindObjectOfType<SceneInteractionController>();
        }
        
        // 查找ConditionChecker
        if (conditionChecker == null)
        {
            conditionChecker = GetComponent<InteractionConditionChecker>();
            if (conditionChecker == null)
            {
                conditionChecker = gameObject.AddComponent<InteractionConditionChecker>();
            }
        }
        
        // 查找ResultProcessor
        if (resultProcessor == null)
        {
            resultProcessor = GetComponent<InteractionResultProcessor>();
            if (resultProcessor == null)
            {
                resultProcessor = gameObject.AddComponent<InteractionResultProcessor>();
            }
        }
    }
    
    /// <summary>
    /// 初始化子系統
    /// </summary>
    private void InitializeSubSystems()
    {
        // 初始化條件檢查器
        conditionChecker?.Initialize(this);
        
        // 初始化結果處理器
        resultProcessor?.Initialize(this);
        
        // 初始化靜態互動系統
        staticObjectInteraction?.Initialize();
        
        // 初始化場景互動控制器
        sceneInteractionController?.Initialize();
    }
    
    /// <summary>
    /// 設置事件監聽
    /// </summary>
    private void SetupEventListeners()
    {
        // 監聽靜態物件互動
        if (staticObjectInteraction != null)
        {
            staticObjectInteraction.OnInteractionTriggered.AddListener(HandleStaticInteraction);
        }
        
        // 監聽場景角色互動
        if (sceneInteractionController != null)
        {
            sceneInteractionController.OnCharacterInteractionTriggered.AddListener(HandleCharacterInteraction);
            sceneInteractionController.OnDialogModeRequested.AddListener(HandleDialogModeRequest);
        }
    }
    
    #region 互動觸發處理
    
    /// <summary>
    /// 處理靜態物件互動
    /// </summary>
    private void HandleStaticInteraction(InteractionType interactionType, GameObject interactionObject)
    {
        Debug.Log($"[InteractionManager] 處理靜態互動: {interactionType} - {interactionObject.name}");
        
        if (!CanTriggerInteraction(interactionType))
        {
            Debug.LogWarning($"[InteractionManager] 互動 {interactionType} 當前不可用");
            return;
        }
        
        ProcessInteraction(interactionType, interactionObject);
    }
    
    /// <summary>
    /// 處理角色互動
    /// </summary>
    private void HandleCharacterInteraction(InteractionType interactionType, GameObject character)
    {
        Debug.Log($"[InteractionManager] 處理角色互動: {interactionType} - {character.name}");
        
        if (!CanTriggerInteraction(interactionType))
        {
            Debug.LogWarning($"[InteractionManager] 角色互動 {interactionType} 當前不可用");
            return;
        }
        
        ProcessInteraction(interactionType, character);
    }
    
    /// <summary>
    /// 處理進入對話模式請求
    /// </summary>
    private void HandleDialogModeRequest()
    {
        Debug.Log("[InteractionManager] 處理進入對話模式請求");
        EnterDialogMode();
    }
    
    #endregion
    
    #region 互動處理邏輯
    
    /// <summary>
    /// 處理互動
    /// </summary>
    private void ProcessInteraction(InteractionType interactionType, GameObject targetObject)
    {
        // 檢查互動條件
        if (!conditionChecker.CheckInteractionCondition(interactionType))
        {
            Debug.LogWarning($"[InteractionManager] 互動 {interactionType} 不滿足觸發條件");
            return;
        }
        
        // 設置當前互動
        currentInteraction = interactionType;
        
        // 觸發互動事件
        OnInteractionTriggered?.Invoke(interactionType, targetObject);
        
        // 根據互動類型執行相應邏輯
        switch (interactionType)
        {
            case InteractionType.CharacterTalk:
                ProcessCharacterTalkInteraction(targetObject);
                break;
                
            case InteractionType.Work:
                ProcessWorkInteraction(targetObject);
                break;
                
            case InteractionType.CatInteraction:
                ProcessCatInteraction(targetObject);
                break;
                
            case InteractionType.GoOut:
                ProcessGoOutInteraction(targetObject);
                break;
                
            case InteractionType.SceneTransition:
                ProcessSceneTransitionInteraction(targetObject);
                break;
                
            case InteractionType.Rest:
                ProcessRestInteraction(targetObject);
                break;
                
            case InteractionType.DrinkingInvite:
                ProcessDrinkingInviteInteraction(targetObject);
                break;
                
            default:
                Debug.LogWarning($"[InteractionManager] 未實現的互動類型: {interactionType}");
                break;
        }
    }
    
    /// <summary>
    /// 處理角色對話互動
    /// </summary>
    private void ProcessCharacterTalkInteraction(GameObject character)
    {
        Debug.Log($"[InteractionManager] 開始角色對話: {character.name}");
        
        // 進入對話模式
        EnterDialogMode();
        
        // 處理互動結果
        var result = new InteractionResult
        {
            interactionType = InteractionType.CharacterTalk,
            success = true,
            message = "開始對話",
            changedValues = new Dictionary<string, object>()
        };
        
        CompleteInteraction(result);
    }
    
    /// <summary>
    /// 處理工作互動
    /// </summary>
    private void ProcessWorkInteraction(GameObject workObject)
    {
        Debug.Log($"[InteractionManager] 開始工作互動: {workObject.name}");
        
        // 使用結果處理器處理工作邏輯
        var result = resultProcessor.ProcessWorkInteraction();
        CompleteInteraction(result);
    }
    
    /// <summary>
    /// 處理貓咪互動
    /// </summary>
    private void ProcessCatInteraction(GameObject catObject)
    {
        Debug.Log($"[InteractionManager] 開始貓咪互動: {catObject.name}");
        
        // 使用結果處理器處理貓咪互動邏輯
        var result = resultProcessor.ProcessCatInteraction();
        CompleteInteraction(result);
    }
    
    /// <summary>
    /// 處理外出互動
    /// </summary>
    private void ProcessGoOutInteraction(GameObject exitObject)
    {
        Debug.Log($"[InteractionManager] 開始外出互動: {exitObject.name}");
        
        // 使用結果處理器處理外出邏輯
        var result = resultProcessor.ProcessGoOutInteraction();
        CompleteInteraction(result);
    }
    
    /// <summary>
    /// 處理場景切換互動
    /// </summary>
    private void ProcessSceneTransitionInteraction(GameObject transitionObject)
    {
        Debug.Log($"[InteractionManager] 開始場景切換: {transitionObject.name}");
        
        // 使用結果處理器處理場景切換邏輯
        var result = resultProcessor.ProcessSceneTransitionInteraction();
        CompleteInteraction(result);
    }
    
    /// <summary>
    /// 處理休息互動
    /// </summary>
    private void ProcessRestInteraction(GameObject restObject)
    {
        Debug.Log($"[InteractionManager] 開始休息互動: {restObject.name}");
        
        // 使用結果處理器處理休息邏輯
        var result = resultProcessor.ProcessRestInteraction();
        CompleteInteraction(result);
    }
    
    /// <summary>
    /// 處理邀請喝酒互動
    /// </summary>
    private void ProcessDrinkingInviteInteraction(GameObject drinkObject)
    {
        Debug.Log($"[InteractionManager] 開始邀請喝酒互動: {drinkObject.name}");
        
        // 使用結果處理器處理邀請喝酒邏輯
        var result = resultProcessor.ProcessDrinkingInviteInteraction();
        CompleteInteraction(result);
    }
    
    #endregion
    
    #region 模式切換
    
    /// <summary>
    /// 進入對話模式
    /// </summary>
    public void EnterDialogMode()
    {
        Debug.Log("[InteractionManager] 進入對話模式");
        
        // 啟用對話Canvas
        if (dialogCanvas != null)
            dialogCanvas.gameObject.SetActive(true);
        
        // 啟用選單Canvas  
        if (menuCanvas != null)
            menuCanvas.gameObject.SetActive(true);
        
        // 禁用場景互動
        SetInteractionEnabled(InteractionType.Work, false);
        SetInteractionEnabled(InteractionType.CatInteraction, false);
        SetInteractionEnabled(InteractionType.GoOut, false);
        
        OnDialogModeEntered?.Invoke();
    }
    
    /// <summary>
    /// 退出對話模式，返回場景模式
    /// </summary>
    public void ExitDialogMode()
    {
        Debug.Log("[InteractionManager] 退出對話模式");
        
        // 禁用對話Canvas
        if (dialogCanvas != null)
            dialogCanvas.gameObject.SetActive(false);
        
        // 禁用選單Canvas
        if (menuCanvas != null)
            menuCanvas.gameObject.SetActive(false);
        
        // 重新啟用場景互動
        SetInteractionEnabled(InteractionType.Work, true);
        SetInteractionEnabled(InteractionType.CatInteraction, true);
        SetInteractionEnabled(InteractionType.GoOut, true);
        
        // 重置當前互動
        currentInteraction = InteractionType.None;
        
        OnSceneModeEntered?.Invoke();
    }
    
    #endregion
    
    #region 互動條件和狀態管理
    
    /// <summary>
    /// 檢查是否可以觸發互動
    /// </summary>
    private bool CanTriggerInteraction(InteractionType interactionType)
    {
        // 檢查互動是否啟用
        if (!interactionsEnabled)
            return false;
        
        // 檢查特定互動是否啟用
        if (!interactionStates.ContainsKey(interactionType))
            return false;
        
        return interactionStates[interactionType];
    }
    
    /// <summary>
    /// 設置互動啟用狀態
    /// </summary>
    public void SetInteractionEnabled(InteractionType interactionType, bool enabled)
    {
        if (interactionStates.ContainsKey(interactionType))
        {
            interactionStates[interactionType] = enabled;
            Debug.Log($"[InteractionManager] 設置互動 {interactionType} 為 {(enabled ? "啟用" : "禁用")}");
        }
    }
    
    /// <summary>
    /// 完成互動處理
    /// </summary>
    private void CompleteInteraction(InteractionResult result)
    {
        Debug.Log($"[InteractionManager] 完成互動: {result.interactionType}, 成功: {result.success}");
        
        // 觸發完成事件
        OnInteractionCompleted?.Invoke(result);
        
        // 如果不是對話互動，重置當前互動
        if (result.interactionType != InteractionType.CharacterTalk)
        {
            currentInteraction = InteractionType.None;
        }
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 啟用所有互動
    /// </summary>
    public void EnableAllInteractions()
    {
        interactionsEnabled = true;
        foreach (var key in new List<InteractionType>(interactionStates.Keys))
        {
            interactionStates[key] = true;
        }
        Debug.Log("[InteractionManager] 所有互動已啟用");
    }
    
    /// <summary>
    /// 禁用所有互動
    /// </summary>
    public void DisableAllInteractions()
    {
        interactionsEnabled = false;
        Debug.Log("[InteractionManager] 所有互動已禁用");
    }
    
    /// <summary>
    /// 手動觸發互動
    /// </summary>
    public void TriggerInteraction(InteractionType interactionType, GameObject targetObject = null)
    {
        ProcessInteraction(interactionType, targetObject);
    }
    
    #endregion
}

// 互動類型枚舉
public enum InteractionType
{
    None,
    CharacterTalk,
    Work,
    CatInteraction,
    GoOut,
    SceneTransition,
    Rest,
    DrinkingInvite
}

// 互動結果數據結構
[System.Serializable]
public class InteractionResult
{
    public InteractionType interactionType;
    public bool success;
    public string message;
    public Dictionary<string, object> changedValues;
    
    public InteractionResult()
    {
        changedValues = new Dictionary<string, object>();
    }
}