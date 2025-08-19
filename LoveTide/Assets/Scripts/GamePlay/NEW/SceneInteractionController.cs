using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Spine.Unity;

/// <summary>
/// 場景互動控制器
/// 
/// 職責:
/// 1. 管理動態角色(Yuka)的互動系統
/// 2. 控制SkeletonGraphic的顯示和動畫
/// 3. 處理角色點擊互動和對話模式切換
/// 4. 協調Q版總覽和角色對話模式的切換
/// 
/// 基於架構文檔: NurturingMode/互動系統完整設計_重製版.md
/// 實現Canvas-based的動態角色互動
/// </summary>
public class SceneInteractionController : MonoBehaviour
{
    [Header("== 動態角色配置 ==")]
    [SerializeField] private List<CharacterInteractionData> characters = new List<CharacterInteractionData>();
    
    [Header("== Canvas引用 ==")]
    [SerializeField] private Canvas dynamicCharacterCanvas; // Order: 50
    [SerializeField] private Canvas dialogCanvas; // Order: 70
    
    [Header("== 當前狀態 ==")]
    [SerializeField] private InteractionMode currentMode = InteractionMode.SceneOverview;
    [SerializeField] private CharacterInteractionData currentActiveCharacter;
    [SerializeField] private bool isInitialized = false;
    
    // 互動事件
    public UnityEvent<InteractionType, GameObject> OnCharacterInteractionTriggered;
    public UnityEvent OnDialogModeRequested;
    public UnityEvent<CharacterInteractionData> OnCharacterSelected;
    
    public bool IsInitialized => isInitialized;
    public InteractionMode CurrentMode => currentMode;
    public CharacterInteractionData CurrentActiveCharacter => currentActiveCharacter;
    
    /// <summary>
    /// 初始化場景互動控制器
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[SceneInteractionController] 初始化場景互動控制器");
        
        // 查找Canvas引用
        FindCanvasReferences();
        
        // 設置角色互動
        SetupCharacterInteractions();
        
        // 設置初始模式
        SetInteractionMode(InteractionMode.SceneOverview);
        
        isInitialized = true;
        Debug.Log("[SceneInteractionController] 場景互動控制器初始化完成");
    }
    
    /// <summary>
    /// 查找Canvas引用
    /// </summary>
    private void FindCanvasReferences()
    {
        if (dynamicCharacterCanvas == null)
        {
            dynamicCharacterCanvas = GameObject.Find("DynamicCharacterCanvas")?.GetComponent<Canvas>();
        }
        
        if (dialogCanvas == null)
        {
            dialogCanvas = GameObject.Find("DialogCanvas")?.GetComponent<Canvas>();
        }
        
        if (dynamicCharacterCanvas == null)
        {
            Debug.LogWarning("[SceneInteractionController] 找不到DynamicCharacterCanvas");
        }
        
        if (dialogCanvas == null)
        {
            Debug.LogWarning("[SceneInteractionController] 找不到DialogCanvas");
        }
    }
    
    /// <summary>
    /// 設置角色互動
    /// </summary>
    private void SetupCharacterInteractions()
    {
        // 如果沒有手動配置角色，自動查找
        if (characters.Count == 0)
        {
            AutoDiscoverCharacters();
        }
        
        // 初始化每個角色的互動組件
        foreach (var character in characters)
        {
            if (character != null)
            {
                SetupCharacterComponents(character);
            }
        }
    }
    
    /// <summary>
    /// 自動發現場景中的角色
    /// </summary>
    private void AutoDiscoverCharacters()
    {
        // 查找所有的SkeletonGraphic組件
        SkeletonGraphic[] skeletonGraphics = FindObjectsOfType<SkeletonGraphic>();
        
        foreach (var skeletonGraphic in skeletonGraphics)
        {
            // 檢查是否為角色(可以根據名稱或標籤判斷)
            if (skeletonGraphic.name.Contains("Yuka") || skeletonGraphic.name.Contains("Character"))
            {
                var characterData = new CharacterInteractionData
                {
                    characterName = skeletonGraphic.name,
                    skeletonGraphic = skeletonGraphic,
                    gameObject = skeletonGraphic.gameObject,
                    interactionType = InteractionType.CharacterTalk
                };
                
                characters.Add(characterData);
                Debug.Log($"[SceneInteractionController] 發現角色: {characterData.characterName}");
            }
        }
    }
    
    /// <summary>
    /// 設置角色組件
    /// </summary>
    private void SetupCharacterComponents(CharacterInteractionData character)
    {
        if (character.gameObject == null) return;
        
        // 添加或獲取CharacterInteractionComponent
        var interactionComponent = character.gameObject.GetComponent<CharacterInteractionComponent>();
        if (interactionComponent == null)
        {
            interactionComponent = character.gameObject.AddComponent<CharacterInteractionComponent>();
        }
        
        // 設置組件引用
        character.interactionComponent = interactionComponent;
        
        // 初始化組件
        interactionComponent.Initialize(character);
        
        // 綁定事件
        interactionComponent.OnCharacterClicked.AddListener(HandleCharacterClicked);
    }
    
    /// <summary>
    /// 處理角色點擊
    /// </summary>
    private void HandleCharacterClicked(CharacterInteractionData character)
    {
        Debug.Log($"[SceneInteractionController] 角色被點擊: {character.characterName}");
        
        currentActiveCharacter = character;
        
        // 觸發角色選擇事件
        OnCharacterSelected?.Invoke(character);
        
        // 根據當前模式處理點擊
        switch (currentMode)
        {
            case InteractionMode.SceneOverview:
                // Q版總覽模式 - 進入對話模式
                RequestDialogMode();
                break;
                
            case InteractionMode.CharacterDialog:
                // 已在對話模式 - 觸發對話互動
                TriggerCharacterInteraction(character);
                break;
        }
    }
    
    /// <summary>
    /// 請求進入對話模式
    /// </summary>
    private void RequestDialogMode()
    {
        Debug.Log("[SceneInteractionController] 請求進入對話模式");
        OnDialogModeRequested?.Invoke();
    }
    
    /// <summary>
    /// 觸發角色互動
    /// </summary>
    private void TriggerCharacterInteraction(CharacterInteractionData character)
    {
        Debug.Log($"[SceneInteractionController] 觸發角色互動: {character.characterName}");
        
        // 觸發互動事件
        OnCharacterInteractionTriggered?.Invoke(character.interactionType, character.gameObject);
    }
    
    #region 模式管理
    
    /// <summary>
    /// 設置互動模式
    /// </summary>
    public void SetInteractionMode(InteractionMode mode)
    {
        if (currentMode == mode) return;
        
        InteractionMode previousMode = currentMode;
        currentMode = mode;
        
        Debug.Log($"[SceneInteractionController] 互動模式變更: {previousMode} → {mode}");
        
        // 處理模式切換
        HandleModeTransition(previousMode, mode);
    }
    
    /// <summary>
    /// 處理模式切換
    /// </summary>
    private void HandleModeTransition(InteractionMode from, InteractionMode to)
    {
        switch (to)
        {
            case InteractionMode.SceneOverview:
                EnterSceneOverviewMode();
                break;
                
            case InteractionMode.CharacterDialog:
                EnterCharacterDialogMode();
                break;
        }
    }
    
    /// <summary>
    /// 進入場景總覽模式
    /// </summary>
    private void EnterSceneOverviewMode()
    {
        Debug.Log("[SceneInteractionController] 進入場景總覽模式");
        
        // 啟用動態角色Canvas
        if (dynamicCharacterCanvas != null)
        {
            dynamicCharacterCanvas.gameObject.SetActive(true);
        }
        
        // 禁用對話Canvas
        if (dialogCanvas != null)
        {
            dialogCanvas.gameObject.SetActive(false);
        }
        
        // 設置所有角色為Q版顯示
        foreach (var character in characters)
        {
            if (character.interactionComponent != null)
            {
                character.interactionComponent.SetDisplayMode(CharacterDisplayMode.QVersion);
            }
        }
    }
    
    /// <summary>
    /// 進入角色對話模式
    /// </summary>
    private void EnterCharacterDialogMode()
    {
        Debug.Log("[SceneInteractionController] 進入角色對話模式");
        
        // 啟用對話Canvas
        if (dialogCanvas != null)
        {
            dialogCanvas.gameObject.SetActive(true);
        }
        
        // 設置當前角色為對話顯示
        if (currentActiveCharacter?.interactionComponent != null)
        {
            currentActiveCharacter.interactionComponent.SetDisplayMode(CharacterDisplayMode.Dialog);
        }
        
        // 隱藏其他角色或設置為背景
        foreach (var character in characters)
        {
            if (character != currentActiveCharacter && character.interactionComponent != null)
            {
                character.interactionComponent.SetDisplayMode(CharacterDisplayMode.Background);
            }
        }
    }
    
    #endregion
    
    #region 角色管理
    
    /// <summary>
    /// 添加角色
    /// </summary>
    public void AddCharacter(CharacterInteractionData character)
    {
        if (character != null && !characters.Contains(character))
        {
            characters.Add(character);
            SetupCharacterComponents(character);
            Debug.Log($"[SceneInteractionController] 添加角色: {character.characterName}");
        }
    }
    
    /// <summary>
    /// 移除角色
    /// </summary>
    public void RemoveCharacter(CharacterInteractionData character)
    {
        if (character != null && characters.Contains(character))
        {
            if (character.interactionComponent != null)
            {
                character.interactionComponent.OnCharacterClicked.RemoveListener(HandleCharacterClicked);
            }
            
            characters.Remove(character);
            Debug.Log($"[SceneInteractionController] 移除角色: {character.characterName}");
        }
    }
    
    /// <summary>
    /// 獲取角色
    /// </summary>
    public CharacterInteractionData GetCharacter(string characterName)
    {
        return characters.Find(c => c.characterName == characterName);
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 手動觸發角色互動
    /// </summary>
    public void TriggerCharacterInteraction(string characterName)
    {
        var character = GetCharacter(characterName);
        if (character != null)
        {
            TriggerCharacterInteraction(character);
        }
    }
    
    /// <summary>
    /// 設置角色動畫
    /// </summary>
    public void SetCharacterAnimation(string characterName, string animationName)
    {
        var character = GetCharacter(characterName);
        if (character?.interactionComponent != null)
        {
            character.interactionComponent.PlayAnimation(animationName);
        }
    }
    
    #endregion
}

/// <summary>
/// 角色互動數據
/// </summary>
[System.Serializable]
public class CharacterInteractionData
{
    public string characterName;
    public GameObject gameObject;
    public SkeletonGraphic skeletonGraphic;
    public InteractionType interactionType = InteractionType.CharacterTalk;
    
    [System.NonSerialized]
    public CharacterInteractionComponent interactionComponent;
}

/// <summary>
/// 角色互動組件
/// 附加到角色GameObject上處理互動邏輯
/// </summary>
public class CharacterInteractionComponent : MonoBehaviour
{
    private CharacterInteractionData characterData;
    private SkeletonGraphic skeletonGraphic;
    private CharacterDisplayMode currentDisplayMode = CharacterDisplayMode.QVersion;
    
    // 角色點擊事件
    public UnityEvent<CharacterInteractionData> OnCharacterClicked;
    
    /// <summary>
    /// 初始化角色互動組件
    /// </summary>
    public void Initialize(CharacterInteractionData data)
    {
        characterData = data;
        skeletonGraphic = data.skeletonGraphic;
        
        // 添加點擊檢測
        SetupClickDetection();
        
        Debug.Log($"[CharacterInteractionComponent] 初始化角色互動: {data.characterName}");
    }
    
    /// <summary>
    /// 設置點擊檢測
    /// </summary>
    private void SetupClickDetection()
    {
        // 確保有Collider用於點擊檢測
        var collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        
        // 添加EventTrigger組件用於UI事件
        var eventTrigger = GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }
        
        // 設置點擊事件
        var clickEvent = new UnityEngine.EventSystems.EventTrigger.Entry
        {
            eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick
        };
        clickEvent.callback.AddListener((eventData) => OnCharacterClick());
        eventTrigger.triggers.Add(clickEvent);
    }
    
    /// <summary>
    /// 角色點擊處理
    /// </summary>
    private void OnCharacterClick()
    {
        Debug.Log($"[CharacterInteractionComponent] 角色被點擊: {characterData.characterName}");
        OnCharacterClicked?.Invoke(characterData);
    }
    
    /// <summary>
    /// 設置顯示模式
    /// </summary>
    public void SetDisplayMode(CharacterDisplayMode mode)
    {
        currentDisplayMode = mode;
        
        switch (mode)
        {
            case CharacterDisplayMode.QVersion:
                SetQVersionDisplay();
                break;
                
            case CharacterDisplayMode.Dialog:
                SetDialogDisplay();
                break;
                
            case CharacterDisplayMode.Background:
                SetBackgroundDisplay();
                break;
        }
    }
    
    /// <summary>
    /// 設置Q版顯示
    /// </summary>
    private void SetQVersionDisplay()
    {
        gameObject.SetActive(true);
        
        if (skeletonGraphic != null)
        {
            // 設置Q版比例和位置
            transform.localScale = Vector3.one * 0.8f; // Q版稍小
            
            // 播放idle動畫
            PlayAnimation("idle");
        }
    }
    
    /// <summary>
    /// 設置對話顯示
    /// </summary>
    private void SetDialogDisplay()
    {
        gameObject.SetActive(true);
        
        if (skeletonGraphic != null)
        {
            // 設置對話模式比例和位置
            transform.localScale = Vector3.one; // 正常比例
            
            // 播放對話動畫
            PlayAnimation("talk");
        }
    }
    
    /// <summary>
    /// 設置背景顯示
    /// </summary>
    private void SetBackgroundDisplay()
    {
        // 在對話模式中隱藏或設置為背景
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 播放動畫
    /// </summary>
    public void PlayAnimation(string animationName)
    {
        if (skeletonGraphic != null && skeletonGraphic.AnimationState != null)
        {
            skeletonGraphic.AnimationState.SetAnimation(0, animationName, true);
        }
    }
}

/// <summary>
/// 角色顯示模式
/// </summary>
public enum CharacterDisplayMode
{
    QVersion,      // Q版總覽模式
    Dialog,        // 對話模式
    Background     // 背景模式
}