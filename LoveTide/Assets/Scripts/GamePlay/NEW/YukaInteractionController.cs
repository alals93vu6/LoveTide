using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 由香互動控制器
/// 
/// 功能:
/// 1. 控制由香角色的互動狀態
/// 2. 管理角色對話模式的進入和退出
/// 3. 處理角色點擊檢測和回應
/// 4. 整合表情系統和語音播放
/// 
/// 設計理念:
/// - 專門處理由香角色的所有互動邏輯
/// - 作為Scene模式和Dialog模式的橋樑
/// - 整合現有的ActorManager系統
/// </summary>
public class YukaInteractionController : MonoBehaviour
{
    [Header("=== 角色引用 ===")]
    [SerializeField] private Transform yukaCharacter;          // 由香角色Transform
    [SerializeField] private GameObject yukaGameObject;        // 由香角色GameObject
    
    [Header("=== 互動檢測 ===")]
    [SerializeField] private Button characterButton;           // 角色互動按鈕
    [SerializeField] private float interactionCooldown = 0.5f; // 互動冷卻時間
    
    [Header("=== 互動回應設定 ===")]
    [SerializeField] private bool enableHoverEffects = true;   // 啟用Hover效果
    [SerializeField] private bool enableClickEffects = true;   // 啟用點擊效果
    [SerializeField] private bool enableVoiceResponse = true;  // 啟用語音回應
    
    [Header("=== 表情控制 ===")]
    [SerializeField] private bool changeExpressionOnClick = true; // 點擊時切換表情
    [SerializeField] private float expressionChangeDelay = 0.2f;  // 表情變化延遲
    
    // 組件引用
    private CharacterFollowButton followButton;
    private CharacterInteractionFeedback feedbackController;
    //private HoverEffectController hoverController;
    
    // 狀態管理
    private bool isInteractionAvailable = true;
    private bool isInDialogMode = false;
    private float lastInteractionTime = 0f;
    private int currentExpressionIndex = 0;
    
    // 互動計數
    private int totalInteractionCount = 0;
    private int sessionInteractionCount = 0;
    
    // 事件系統
    public System.Action OnCharacterClicked;
    public System.Action OnDialogModeEntered;
    public System.Action OnDialogModeExited;
    public System.Action<int> OnExpressionChanged;
    
    #region Unity 生命週期
    
    void Awake()
    {
        InitializeComponents();
    }
    
    void Start()
    {
        InitializeController();
    }
    
    void Update()
    {
        UpdateCooldownTimer();
    }
    
    #endregion
    
    #region 初始化
    
    /// <summary>
    /// 初始化組件引用
    /// </summary>
    private void InitializeComponents()
    {
        // 自動獲取組件
        if (characterButton == null)
            characterButton = GetComponent<Button>();
            
        if (yukaGameObject == null && yukaCharacter != null)
            yukaGameObject = yukaCharacter.gameObject;
            
        // 獲取相關控制器
        followButton = GetComponent<CharacterFollowButton>();
        feedbackController = GetComponent<CharacterInteractionFeedback>();
        //hoverController = GetComponent<HoverEffectController>();
    }
    
    /// <summary>
    /// 初始化控制器
    /// </summary>
    private void InitializeController()
    {
        // 設置按鈕事件
        if (characterButton != null)
        {
            characterButton.onClick.AddListener(OnCharacterButtonClick);
        }
        
        // 初始化表情狀態
        ResetExpressionState();
        
        // 設置初始互動狀態
        SetInteractionAvailable(true);
        
        Debug.Log("[YukaInteractionController] 由香互動控制器初始化完成");
    }
    
    #endregion
    
    #region 互動處理
    
    /// <summary>
    /// 角色按鈕點擊處理
    /// </summary>
    private void OnCharacterButtonClick()
    {
        if (!CanInteract())
        {
            Debug.Log("[YukaInteractionController] 互動冷卻中，忽略點擊");
            return;
        }
        
        Debug.Log("[YukaInteractionController] 由香被點擊");
        
        // 記錄互動時間
        lastInteractionTime = Time.time;
        totalInteractionCount++;
        sessionInteractionCount++;
        
        // 處理互動邏輯
        StartCoroutine(HandleCharacterInteraction());
        
        // 觸發事件
        OnCharacterClicked?.Invoke();
    }
    
    /// <summary>
    /// 處理角色互動流程
    /// </summary>
    private IEnumerator HandleCharacterInteraction()
    {
        // 禁用互動防止重複點擊
        SetInteractionAvailable(false);
        
        // 播放點擊效果
        if (enableClickEffects && feedbackController != null)
        {
            feedbackController.PlayClickEffect();
        }
        
        // 切換表情
        if (changeExpressionOnClick)
        {
            yield return new WaitForSeconds(expressionChangeDelay);
            ChangeRandomExpression();
        }
        
        // 播放語音回應
        if (enableVoiceResponse)
        {
            PlayVoiceResponse();
        }
        
        // 檢查是否進入對話模式
        if (ShouldEnterDialogMode())
        {
            yield return new WaitForSeconds(0.5f);
            EnterDialogMode();
        }
        
        // 等待冷卻時間
        yield return new WaitForSeconds(interactionCooldown);
        
        // 恢復互動
        SetInteractionAvailable(true);
        
        Debug.Log("[YukaInteractionController] 互動處理完成");
    }
    
    /// <summary>
    /// 檢查是否可以互動
    /// </summary>
    private bool CanInteract()
    {
        return isInteractionAvailable && 
               !isInDialogMode && 
               (Time.time - lastInteractionTime) >= interactionCooldown;
    }
    
    #endregion
    
    #region 表情控制
    
    /// <summary>
    /// 重置表情狀態
    /// </summary>
    private void ResetExpressionState()
    {
        currentExpressionIndex = 0;
        // 這裡可以設置預設表情
        // 需要整合ActorManager或表情控制系統
    }
    
    /// <summary>
    /// 隨機切換表情
    /// </summary>
    private void ChangeRandomExpression()
    {
        // 生成隨機表情索引 (假設有6種基本表情)
        int newExpressionIndex = Random.Range(0, 6);
        
        // 避免連續相同表情
        if (newExpressionIndex == currentExpressionIndex)
        {
            newExpressionIndex = (newExpressionIndex + 1) % 6;
        }
        
        SetExpression(newExpressionIndex);
    }
    
    /// <summary>
    /// 設置特定表情
    /// </summary>
    public void SetExpression(int expressionIndex)
    {
        currentExpressionIndex = expressionIndex;
        
        Debug.Log($"[YukaInteractionController] 切換表情: {expressionIndex}");
        
        // 觸發表情變化事件
        OnExpressionChanged?.Invoke(expressionIndex);
        
        // 這裡需要整合實際的表情控制系統
        // 例如: ActorManager或YukaExpressionController
        /*
        var expressionController = yukaGameObject.GetComponent<YukaExpressionController>();
        if (expressionController != null)
        {
            switch (expressionIndex)
            {
                case 0: expressionController.SetNormalExpression(); break;
                case 1: expressionController.SetHappyExpression(); break;
                case 2: expressionController.SetShyExpression(); break;
                case 3: expressionController.SetExcitedExpression(); break;
                case 4: expressionController.SetSurpriseExpression(); break;
                case 5: expressionController.SetCryExpression(); break;
            }
        }
        */
    }
    
    #endregion
    
    #region 語音系統
    
    /// <summary>
    /// 播放語音回應
    /// </summary>
    private void PlayVoiceResponse()
    {
        // 根據互動次數和上下文選擇語音
        string voiceClip = GetContextualVoiceClip();
        
        Debug.Log($"[YukaInteractionController] 播放語音: {voiceClip}");
        
        // 這裡需要整合實際的語音播放系統
        // 例如: AudioManager或VoiceController
        /*
        var audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlayVoice(voiceClip);
        }
        */
    }
    
    /// <summary>
    /// 獲取上下文語音片段
    /// </summary>
    private string GetContextualVoiceClip()
    {
        // 根據互動次數返回不同語音
        if (sessionInteractionCount <= 3)
        {
            return $"greeting_{Random.Range(1, 4)}"; // greeting_1 ~ greeting_3
        }
        else if (sessionInteractionCount <= 10)
        {
            return $"casual_{Random.Range(1, 6)}";   // casual_1 ~ casual_5
        }
        else
        {
            return $"frequent_{Random.Range(1, 4)}"; // frequent_1 ~ frequent_3
        }
    }
    
    #endregion
    
    #region 對話模式
    
    /// <summary>
    /// 檢查是否應該進入對話模式
    /// </summary>
    private bool ShouldEnterDialogMode()
    {
        // 可以根據不同條件決定
        // 例如: 連續點擊次數、時間、好感度等
        return sessionInteractionCount >= 3 && Random.Range(0f, 1f) > 0.7f;
    }
    
    /// <summary>
    /// 進入對話模式
    /// </summary>
    public void EnterDialogMode()
    {
        if (isInDialogMode) return;
        
        isInDialogMode = true;
        Debug.Log("[YukaInteractionController] 進入對話模式");
        
        // 禁用場景互動
        SetInteractionAvailable(false);
        
        // 觸發對話模式事件
        OnDialogModeEntered?.Invoke();
        
        // 這裡需要整合對話系統
        // 例如: DialogManager或UI切換
    }
    
    /// <summary>
    /// 退出對話模式
    /// </summary>
    public void ExitDialogMode()
    {
        if (!isInDialogMode) return;
        
        isInDialogMode = false;
        Debug.Log("[YukaInteractionController] 退出對話模式");
        
        // 恢復場景互動
        SetInteractionAvailable(true);
        
        // 重置會話計數
        sessionInteractionCount = 0;
        
        // 觸發退出對話模式事件
        OnDialogModeExited?.Invoke();
    }
    
    #endregion
    
    #region 狀態控制
    
    /// <summary>
    /// 設置互動可用性
    /// </summary>
    public void SetInteractionAvailable(bool available)
    {
        isInteractionAvailable = available;
        
        if (characterButton != null)
        {
            characterButton.interactable = available;
        }
        
        if (followButton != null)
        {
            followButton.SetButtonInteractable(available);
        }
    }
    
    /// <summary>
    /// 更新冷卻計時器
    /// </summary>
    private void UpdateCooldownTimer()
    {
        // 這裡可以添加視覺冷卻指示器的更新邏輯
    }
    
    #endregion
    
    #region 公開接口
    
    /// <summary>
    /// 獲取互動統計
    /// </summary>
    public (int total, int session) GetInteractionStats()
    {
        return (totalInteractionCount, sessionInteractionCount);
    }
    
    /// <summary>
    /// 重置會話數據
    /// </summary>
    public void ResetSessionData()
    {
        sessionInteractionCount = 0;
        ResetExpressionState();
    }
    
    /// <summary>
    /// 強制觸發互動
    /// </summary>
    public void ForceInteraction()
    {
        if (!isInDialogMode)
        {
            OnCharacterButtonClick();
        }
    }
    
    /// <summary>
    /// 設置由香角色引用
    /// </summary>
    public void SetYukaCharacter(Transform character)
    {
        yukaCharacter = character;
        yukaGameObject = character?.gameObject;
        
        if (followButton != null)
        {
            followButton.SetTarget(character);
        }
    }
    
    #endregion
    
    #region 事件清理
    
    void OnDestroy()
    {
        // 清理按鈕事件
        if (characterButton != null)
        {
            characterButton.onClick.RemoveListener(OnCharacterButtonClick);
        }
        
        // 清理事件訂閱
        OnCharacterClicked = null;
        OnDialogModeEntered = null;
        OnDialogModeExited = null;
        OnExpressionChanged = null;
        
        Debug.Log("[YukaInteractionController] 由香互動控制器已清理");
    }
    
    #endregion
}