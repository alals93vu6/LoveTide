using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 場景按鈕統一管理器
/// 
/// 職責:
/// 1. 統一管理所有場景的互動按鈕
/// 2. 根據場景狀態啟用/禁用對應按鈕組
/// 3. 處理所有按鈕的點擊事件和互動邏輯
/// 設計優勢:
/// - 集中管理，維護簡單
/// - 統一的條件檢查和狀態控制
/// - 便於擴展新的場景互動功能
/// </summary>
public class SceneBtnManager : MonoBehaviour
{
    [Header("=== 餐廳場景按鈕組 ===")]
    [SerializeField] private Button tavernWorkBtn;         // 協助由香工作
    [SerializeField] private Button catInteractionBtn;     // 和貓咪互動
    [SerializeField] private Button exitBtn;               // 外出系統
    
    [Header("=== 宿舍場景按鈕組 ===")]
    [SerializeField] private Button drinkingInviteBtn;     // 邀請喝酒
    [SerializeField] private Button yukaRoomBtn;           // 點擊由香房間
    [SerializeField] private Button playerRoomBtn;         // 回自己房間提早結束
    
    [Header("=== 按鈕狀態控制 ===")]
    [SerializeField] private bool enableButtonAnimations = true;
    [SerializeField] private float buttonFadeTime = 0.3f;
    
    // 按鈕組管理
    private Button[] tavernButtons;
    private Button[] dormButtons;
    
    // 初始化狀態
    private bool isInitialized = false;
    
    #region Unity 生命週期
    
    void Awake()
    {
        InitializeButtonArrays();
    }
    
    void Start()
    {
        StartCoroutine(InitializeManager());
    }
    
    #endregion
    
    #region 初始化
    
    /// <summary>
    /// 初始化按鈕陣列
    /// </summary>
    private void InitializeButtonArrays()
    {
        tavernButtons = new Button[] { tavernWorkBtn, catInteractionBtn, exitBtn };
        dormButtons = new Button[] { drinkingInviteBtn, yukaRoomBtn, playerRoomBtn };
    }
    
    /// <summary>
    /// 初始化管理器
    /// </summary>
    private IEnumerator InitializeManager()
    {
        // 等待一幀確保所有物件初始化完成
        yield return null;
        
        // 驗證按鈕引用
        ValidateButtonReferences();
        
        // 綁定按鈕事件
        BindButtonEvents();
        
        // 設置初始按鈕狀態
        SetInitialButtonState();
        
        isInitialized = true;
        Debug.Log("[SceneBtnManager] 場景按鈕管理器初始化完成");
    }
    
    /// <summary>
    /// 驗證按鈕引用
    /// </summary>
    private void ValidateButtonReferences()
    {
        List<string> missingButtons = new List<string>();
        
        if (tavernWorkBtn == null) missingButtons.Add("TavernWorkBtn");
        if (catInteractionBtn == null) missingButtons.Add("CatInteractionBtn");
        if (exitBtn == null) missingButtons.Add("ExitBtn");
        if (drinkingInviteBtn == null) missingButtons.Add("DrinkingInviteBtn");
        if (yukaRoomBtn == null) missingButtons.Add("YukaRoomBtn");
        if (playerRoomBtn == null) missingButtons.Add("PlayerRoomBtn");
        
        if (missingButtons.Count > 0)
        {
            Debug.LogWarning($"[SceneBtnManager] 缺少按鈕引用: {string.Join(", ", missingButtons)}");
        }
    }
    
    /// <summary>
    /// 綁定按鈕點擊事件
    /// </summary>
    private void BindButtonEvents()
    {
        if (tavernWorkBtn != null)
            tavernWorkBtn.onClick.AddListener(OnTavernWorkClick);
        if (catInteractionBtn != null)
            catInteractionBtn.onClick.AddListener(OnCatInteractionClick);
        if (exitBtn != null)
            exitBtn.onClick.AddListener(OnExitClick);
        if (drinkingInviteBtn != null)
            drinkingInviteBtn.onClick.AddListener(OnDrinkingInviteClick);
        if (yukaRoomBtn != null)
            yukaRoomBtn.onClick.AddListener(OnYukaRoomClick);
        if (playerRoomBtn != null)
            playerRoomBtn.onClick.AddListener(OnPlayerRoomClick);
        
        Debug.Log("[SceneBtnManager] 按鈕事件綁定完成");
    }
    
    /// <summary>
    /// 設置初始按鈕狀態
    /// </summary>
    private void SetInitialButtonState()
    {
        // 預設顯示餐廳按鈕
        ShowTavernButtons();
        HideDormButtons();
    }
    
    #endregion
    
    #region 場景切換控制
    
    /// <summary>
    /// 顯示餐廳按鈕組
    /// </summary>
    public void ShowTavernButtons()
    {
        SetButtonGroupActive(tavernButtons, true);
        Debug.Log("[SceneBtnManager] 顯示餐廳按鈕組");
    }
    
    /// <summary>
    /// 隱藏餐廳按鈕組
    /// </summary>
    public void HideTavernButtons()
    {
        SetButtonGroupActive(tavernButtons, false);
        Debug.Log("[SceneBtnManager] 隱藏餐廳按鈕組");
    }
    
    /// <summary>
    /// 顯示宿舍按鈕組
    /// </summary>
    public void ShowDormButtons()
    {
        SetButtonGroupActive(dormButtons, true);
        Debug.Log("[SceneBtnManager] 顯示宿舍按鈕組");
    }
    
    /// <summary>
    /// 隱藏宿舍按鈕組
    /// </summary>
    public void HideDormButtons()
    {
        SetButtonGroupActive(dormButtons, false);
        Debug.Log("[SceneBtnManager] 隱藏宿舍按鈕組");
    }
    
    /// <summary>
    /// 設置按鈕組的啟用狀態
    /// </summary>
    private void SetButtonGroupActive(Button[] buttons, bool active)
    {
        foreach (var button in buttons)
        {
            if (button != null)
            {
                if (enableButtonAnimations)
                {
                    StartCoroutine(AnimateButtonVisibility(button, active));
                }
                else
                {
                    button.gameObject.SetActive(active);
                }
            }
        }
    }
    
    /// <summary>
    /// 按鈕顯示/隱藏動畫
    /// </summary>
    private IEnumerator AnimateButtonVisibility(Button button, bool show)
    {
        if (show && !button.gameObject.activeSelf)
        {
            button.gameObject.SetActive(true);
            // 淡入動畫
            yield return StartCoroutine(FadeButton(button, 0f, 1f));
        }
        else if (!show && button.gameObject.activeSelf)
        {
            // 淡出動畫
            yield return StartCoroutine(FadeButton(button, 1f, 0f));
            button.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 按鈕淡入淡出動畫
    /// </summary>
    private IEnumerator FadeButton(Button button, float fromAlpha, float toAlpha)
    {
        CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = button.gameObject.AddComponent<CanvasGroup>();
        }
        
        float elapsed = 0f;
        while (elapsed < buttonFadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsed / buttonFadeTime);
            canvasGroup.alpha = alpha;
            yield return null;
        }
        
        canvasGroup.alpha = toAlpha;
    }
    
    #endregion
    
    #region 餐廳按鈕事件處理
    
    /// <summary>
    /// 協助由香工作
    /// </summary>
    public void OnTavernWorkClick()
    {
        Debug.Log("[SceneBtnManager] 點擊協助工作");
        StartCoroutine(HandleWorkInteraction());
    }
    
    /// <summary>
    /// 與貓咪互動
    /// </summary>
    public void OnCatInteractionClick()
    {
        Debug.Log("[SceneBtnManager] 點擊貓咪互動");
        StartCoroutine(HandleCatInteraction());
    }
    
    /// <summary>
    /// 外出系統
    /// </summary>
    public void OnExitClick()
    {
        Debug.Log("[SceneBtnManager] 點擊外出");
        StartCoroutine(HandleExitInteraction());
    }
    
    #endregion
    
    #region 宿舍按鈕事件處理
    
    /// <summary>
    /// 邀請喝酒
    /// </summary>
    public void OnDrinkingInviteClick()
    {
        Debug.Log("[SceneBtnManager] 點擊邀請喝酒");
        StartCoroutine(HandleDrinkingInteraction());
    }
    
    /// <summary>
    /// 點擊由香房間
    /// </summary>
    public void OnYukaRoomClick()
    {
        Debug.Log("[SceneBtnManager] 點擊由香房間");
        StartCoroutine(HandleYukaRoomInteraction());
    }
    
    /// <summary>
    /// 回玩家房間提早結束
    /// </summary>
    public void OnPlayerRoomClick()
    {
        Debug.Log("[SceneBtnManager] 點擊玩家房間");
        StartCoroutine(HandlePlayerRoomInteraction());
    }
    
    #endregion
    
    #region 互動處理協程
    
    /// <summary>
    /// 處理工作互動
    /// </summary>
    private IEnumerator HandleWorkInteraction()
    {
        SetAllButtonsInteractable(false);
        
        Debug.Log("[SceneBtnManager] 開始工作互動...");
        yield return new WaitForSeconds(1f);
        
        SetAllButtonsInteractable(true);
        Debug.Log("[SceneBtnManager] 工作互動完成");
    }
    
    /// <summary>
    /// 處理貓咪互動
    /// </summary>
    private IEnumerator HandleCatInteraction()
    {
        SetAllButtonsInteractable(false);
        
        Debug.Log("[SceneBtnManager] 開始貓咪互動...");
        yield return new WaitForSeconds(0.5f);
        
        SetAllButtonsInteractable(true);
        Debug.Log("[SceneBtnManager] 貓咪互動完成");
    }
    
    /// <summary>
    /// 處理外出互動
    /// </summary>
    private IEnumerator HandleExitInteraction()
    {
        SetAllButtonsInteractable(false);
        
        Debug.Log("[SceneBtnManager] 開始外出...");
        yield return new WaitForSeconds(0.5f);
        
        SetAllButtonsInteractable(true);
        Debug.Log("[SceneBtnManager] 外出完成");
    }
    
    /// <summary>
    /// 處理喝酒互動
    /// </summary>
    private IEnumerator HandleDrinkingInteraction()
    {
        SetAllButtonsInteractable(false);
        
        Debug.Log("[SceneBtnManager] 開始喝酒互動...");
        yield return new WaitForSeconds(1f);
        
        SetAllButtonsInteractable(true);
        Debug.Log("[SceneBtnManager] 喝酒互動完成");
    }
    
    /// <summary>
    /// 處理由香房間互動
    /// </summary>
    private IEnumerator HandleYukaRoomInteraction()
    {
        SetAllButtonsInteractable(false);
        
        Debug.Log("[SceneBtnManager] 進入由香房間...");
        yield return new WaitForSeconds(0.5f);
        
        SetAllButtonsInteractable(true);
        Debug.Log("[SceneBtnManager] 由香房間互動完成");
    }
    
    /// <summary>
    /// 處理玩家房間互動
    /// </summary>
    private IEnumerator HandlePlayerRoomInteraction()
    {
        SetAllButtonsInteractable(false);
        
        Debug.Log("[SceneBtnManager] 回到房間，結束當前時段...");
        yield return new WaitForSeconds(0.5f);
        
        SetAllButtonsInteractable(true);
        Debug.Log("[SceneBtnManager] 時段提早結束");
    }
    
    #endregion
    
    #region 工具方法
    
    /// <summary>
    /// 設置所有按鈕的可互動性
    /// </summary>
    private void SetAllButtonsInteractable(bool interactable)
    {
        foreach (var button in tavernButtons)
        {
            if (button != null) button.interactable = interactable;
        }
        
        foreach (var button in dormButtons)
        {
            if (button != null) button.interactable = interactable;
        }
    }
    
    /// <summary>
    /// 公開接口：更新所有按鈕狀態
    /// </summary>
    public void RefreshAllButtons()
    {
        if (!isInitialized) return;
        
        // 重新設置按鈕狀態
        SetInitialButtonState();
        
        Debug.Log("[SceneBtnManager] 所有按鈕狀態已刷新");
    }
    
    #endregion
    
    #region 事件清理
    
    void OnDestroy()
    {
        // 取消按鈕事件綁定
        if (tavernWorkBtn != null)
            tavernWorkBtn.onClick.RemoveAllListeners();
        if (catInteractionBtn != null)
            catInteractionBtn.onClick.RemoveAllListeners();
        if (exitBtn != null)
            exitBtn.onClick.RemoveAllListeners();
        if (drinkingInviteBtn != null)
            drinkingInviteBtn.onClick.RemoveAllListeners();
        if (yukaRoomBtn != null)
            yukaRoomBtn.onClick.RemoveAllListeners();
        if (playerRoomBtn != null)
            playerRoomBtn.onClick.RemoveAllListeners();
        
        Debug.Log("[SceneBtnManager] 場景按鈕管理器已清理");
    }
    
    #endregion
}