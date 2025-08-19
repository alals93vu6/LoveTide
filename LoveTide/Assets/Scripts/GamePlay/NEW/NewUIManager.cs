using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 新版UI管理器
/// 
/// 職責:
/// 1. 統一管理所有Canvas層級的UI系統
/// 2. 控制Canvas的顯示/隱藏和層級順序
/// 3. 協調不同互動模式下的UI顯示
/// 4. 提供UI動畫和轉場效果
/// 
/// 基於架構文檔: NurturingMode/互動系統完整設計_重製版.md
/// 管理7層Canvas架構 (Order: 0, 40, 50, 60, 70, 80, 90)
/// </summary>
public class NewUIManager : MonoBehaviour
{
    [Header("== Canvas層級配置 ==")]
    [SerializeField] private CanvasLayerConfig canvasLayers = new CanvasLayerConfig();
    
    [Header("== 當前狀態 ==")]
    [SerializeField] private InteractionMode currentInteractionMode = InteractionMode.SceneOverview;
    [SerializeField] private bool isInitialized = false;
    
    [Header("== UI動畫配置 ==")]
    [SerializeField] private float transitionDuration = 0.3f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    // UI狀態事件
    public UnityEvent<InteractionMode> OnInteractionModeChanged;
    public UnityEvent<string> OnCanvasVisibilityChanged;
    
    // 當前活躍的UI組件
    private Dictionary<string, UIComponent> activeUIComponents = new Dictionary<string, UIComponent>();
    
    public bool IsInitialized => isInitialized;
    public InteractionMode CurrentInteractionMode => currentInteractionMode;
    
    /// <summary>
    /// 初始化UI管理器
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[NewUIManager] 初始化UI管理器");
        
        // 查找並綁定所有Canvas
        FindAndBindCanvases();
        
        // 設置Canvas層級順序
        SetupCanvasOrders();
        
        // 初始化UI組件
        InitializeUIComponents();
        
        // 設置初始狀態
        SetInitialUIState();
        
        isInitialized = true;
        Debug.Log("[NewUIManager] UI管理器初始化完成");
    }
    
    /// <summary>
    /// 查找並綁定所有Canvas
    /// </summary>
    private void FindAndBindCanvases()
    {
        // 查找BackgroundCanvas (Order: 0)
        if (canvasLayers.backgroundCanvas == null)
        {
            canvasLayers.backgroundCanvas = GameObject.Find("BackgroundCanvas")?.GetComponent<Canvas>();
        }
        
        // 查找StaticInteractionCanvas (Order: 40)
        if (canvasLayers.staticInteractionCanvas == null)
        {
            canvasLayers.staticInteractionCanvas = GameObject.Find("StaticInteractionCanvas")?.GetComponent<Canvas>();
        }
        
        // 查找DynamicCharacterCanvas (Order: 50)
        if (canvasLayers.dynamicCharacterCanvas == null)
        {
            canvasLayers.dynamicCharacterCanvas = GameObject.Find("DynamicCharacterCanvas")?.GetComponent<Canvas>();
        }
        
        // 查找UICanvas (Order: 60)
        if (canvasLayers.uiCanvas == null)
        {
            canvasLayers.uiCanvas = GameObject.Find("UICanvas")?.GetComponent<Canvas>();
        }
        
        // 查找DialogCanvas (Order: 70)
        if (canvasLayers.dialogCanvas == null)
        {
            canvasLayers.dialogCanvas = GameObject.Find("DialogCanvas")?.GetComponent<Canvas>();
        }
        
        // 查找MenuCanvas (Order: 80)
        if (canvasLayers.menuCanvas == null)
        {
            canvasLayers.menuCanvas = GameObject.Find("MenuCanvas")?.GetComponent<Canvas>();
        }
        
        // 查找SystemCanvas (Order: 90)
        if (canvasLayers.systemCanvas == null)
        {
            canvasLayers.systemCanvas = GameObject.Find("SystemCanvas")?.GetComponent<Canvas>();
        }
        
        LogCanvasStatus();
    }
    
    /// <summary>
    /// 記錄Canvas狀態
    /// </summary>
    private void LogCanvasStatus()
    {
        Debug.Log($"[NewUIManager] Canvas綁定狀態:");
        Debug.Log($"  BackgroundCanvas: {(canvasLayers.backgroundCanvas != null ? "✓" : "✗")}");
        Debug.Log($"  StaticInteractionCanvas: {(canvasLayers.staticInteractionCanvas != null ? "✓" : "✗")}");
        Debug.Log($"  DynamicCharacterCanvas: {(canvasLayers.dynamicCharacterCanvas != null ? "✓" : "✗")}");
        Debug.Log($"  UICanvas: {(canvasLayers.uiCanvas != null ? "✓" : "✗")}");
        Debug.Log($"  DialogCanvas: {(canvasLayers.dialogCanvas != null ? "✓" : "✗")}");
        Debug.Log($"  MenuCanvas: {(canvasLayers.menuCanvas != null ? "✓" : "✗")}");
        Debug.Log($"  SystemCanvas: {(canvasLayers.systemCanvas != null ? "✓" : "✗")}");
    }
    
    /// <summary>
    /// 設置Canvas層級順序
    /// </summary>
    private void SetupCanvasOrders()
    {
        if (canvasLayers.backgroundCanvas != null)
            canvasLayers.backgroundCanvas.sortingOrder = 0;
            
        if (canvasLayers.staticInteractionCanvas != null)
            canvasLayers.staticInteractionCanvas.sortingOrder = 40;
            
        if (canvasLayers.dynamicCharacterCanvas != null)
            canvasLayers.dynamicCharacterCanvas.sortingOrder = 50;
            
        if (canvasLayers.uiCanvas != null)
            canvasLayers.uiCanvas.sortingOrder = 60;
            
        if (canvasLayers.dialogCanvas != null)
            canvasLayers.dialogCanvas.sortingOrder = 70;
            
        if (canvasLayers.menuCanvas != null)
            canvasLayers.menuCanvas.sortingOrder = 80;
            
        if (canvasLayers.systemCanvas != null)
            canvasLayers.systemCanvas.sortingOrder = 90;
            
        Debug.Log("[NewUIManager] Canvas層級順序設置完成");
    }
    
    /// <summary>
    /// 初始化UI組件
    /// </summary>
    private void InitializeUIComponents()
    {
        // 查找並註冊UI組件
        RegisterUIComponent("DialogSystem", canvasLayers.dialogCanvas?.gameObject);
        RegisterUIComponent("MenuSystem", canvasLayers.menuCanvas?.gameObject);
        RegisterUIComponent("StaticInteraction", canvasLayers.staticInteractionCanvas?.gameObject);
        RegisterUIComponent("DynamicCharacter", canvasLayers.dynamicCharacterCanvas?.gameObject);
        
        Debug.Log("[NewUIManager] UI組件初始化完成");
    }
    
    /// <summary>
    /// 註冊UI組件
    /// </summary>
    private void RegisterUIComponent(string componentName, GameObject componentObject)
    {
        if (componentObject != null)
        {
            var uiComponent = new UIComponent
            {
                name = componentName,
                gameObject = componentObject,
                canvas = componentObject.GetComponent<Canvas>(),
                isActive = componentObject.activeSelf
            };
            
            activeUIComponents[componentName] = uiComponent;
            Debug.Log($"[NewUIManager] 註冊UI組件: {componentName}");
        }
    }
    
    /// <summary>
    /// 設置初始UI狀態
    /// </summary>
    private void SetInitialUIState()
    {
        // 設置場景總覽模式的初始狀態
        SetInteractionMode(InteractionMode.SceneOverview);
    }
    
    #region 互動模式管理
    
    /// <summary>
    /// 設置互動模式
    /// </summary>
    public void SetInteractionMode(InteractionMode mode)
    {
        if (currentInteractionMode == mode) return;
        
        InteractionMode previousMode = currentInteractionMode;
        currentInteractionMode = mode;
        
        Debug.Log($"[NewUIManager] 互動模式變更: {previousMode} → {mode}");
        
        // 處理模式切換
        HandleInteractionModeTransition(previousMode, mode);
        
        // 觸發事件
        OnInteractionModeChanged?.Invoke(mode);
    }
    
    /// <summary>
    /// 處理互動模式切換
    /// </summary>
    private void HandleInteractionModeTransition(InteractionMode from, InteractionMode to)
    {
        switch (to)
        {
            case InteractionMode.SceneOverview:
                SetSceneOverviewMode();
                break;
                
            case InteractionMode.CharacterDialog:
                SetCharacterDialogMode();
                break;
        }
    }
    
    /// <summary>
    /// 設置場景總覽模式
    /// </summary>
    private void SetSceneOverviewMode()
    {
        Debug.Log("[NewUIManager] 設置場景總覽模式");
        
        // 啟用場景相關的Canvas
        SetCanvasActive("BackgroundCanvas", true);
        SetCanvasActive("StaticInteractionCanvas", true);
        SetCanvasActive("DynamicCharacterCanvas", true);
        SetCanvasActive("UICanvas", true);
        
        // 禁用對話相關的Canvas
        SetCanvasActive("DialogCanvas", false);
        SetCanvasActive("MenuCanvas", false);
        
        // 系統Canvas保持不變
        // SetCanvasActive("SystemCanvas", true);
    }
    
    /// <summary>
    /// 設置角色對話模式
    /// </summary>
    private void SetCharacterDialogMode()
    {
        Debug.Log("[NewUIManager] 設置角色對話模式");
        
        // 保持背景和角色Canvas
        SetCanvasActive("BackgroundCanvas", true);
        SetCanvasActive("DynamicCharacterCanvas", true);
        
        // 啟用對話相關的Canvas
        SetCanvasActive("DialogCanvas", true);
        SetCanvasActive("MenuCanvas", true);
        
        // 禁用場景互動
        SetCanvasActive("StaticInteractionCanvas", false);
        
        // UI Canvas根據需要調整
        SetCanvasActive("UICanvas", true);
    }
    
    #endregion
    
    #region Canvas控制方法
    
    /// <summary>
    /// 設置Canvas啟用狀態
    /// </summary>
    public void SetCanvasActive(string canvasName, bool active)
    {
        Canvas targetCanvas = GetCanvasByName(canvasName);
        
        if (targetCanvas != null)
        {
            if (transitionDuration > 0)
            {
                // 使用動畫切換
                StartCoroutine(AnimateCanvasTransition(targetCanvas, active));
            }
            else
            {
                // 直接切換
                targetCanvas.gameObject.SetActive(active);
            }
            
            Debug.Log($"[NewUIManager] 設置Canvas {canvasName} 為 {(active ? "啟用" : "禁用")}");
            OnCanvasVisibilityChanged?.Invoke($"{canvasName}:{active}");
        }
        else
        {
            Debug.LogWarning($"[NewUIManager] 找不到Canvas: {canvasName}");
        }
    }
    
    /// <summary>
    /// 根據名稱獲取Canvas
    /// </summary>
    private Canvas GetCanvasByName(string canvasName)
    {
        switch (canvasName)
        {
            case "BackgroundCanvas":
                return canvasLayers.backgroundCanvas;
            case "StaticInteractionCanvas":
                return canvasLayers.staticInteractionCanvas;
            case "DynamicCharacterCanvas":
                return canvasLayers.dynamicCharacterCanvas;
            case "UICanvas":
                return canvasLayers.uiCanvas;
            case "DialogCanvas":
                return canvasLayers.dialogCanvas;
            case "MenuCanvas":
                return canvasLayers.menuCanvas;
            case "SystemCanvas":
                return canvasLayers.systemCanvas;
            default:
                return null;
        }
    }
    
    /// <summary>
    /// Canvas切換動畫
    /// </summary>
    private System.Collections.IEnumerator AnimateCanvasTransition(Canvas canvas, bool targetActive)
    {
        if (canvas == null) yield break;
        
        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
        }
        
        float startAlpha = targetActive ? 0f : 1f;
        float endAlpha = targetActive ? 1f : 0f;
        
        if (targetActive)
        {
            canvas.gameObject.SetActive(true);
        }
        
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / transitionDuration;
            float curveValue = transitionCurve.Evaluate(progress);
            
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, curveValue);
            
            yield return null;
        }
        
        canvasGroup.alpha = endAlpha;
        
        if (!targetActive)
        {
            canvas.gameObject.SetActive(false);
        }
    }
    
    #endregion
    
    #region UI組件管理
    
    /// <summary>
    /// 顯示UI組件
    /// </summary>
    public void ShowUIComponent(string componentName)
    {
        if (activeUIComponents.ContainsKey(componentName))
        {
            var component = activeUIComponents[componentName];
            component.gameObject.SetActive(true);
            component.isActive = true;
            
            Debug.Log($"[NewUIManager] 顯示UI組件: {componentName}");
        }
    }
    
    /// <summary>
    /// 隱藏UI組件
    /// </summary>
    public void HideUIComponent(string componentName)
    {
        if (activeUIComponents.ContainsKey(componentName))
        {
            var component = activeUIComponents[componentName];
            component.gameObject.SetActive(false);
            component.isActive = false;
            
            Debug.Log($"[NewUIManager] 隱藏UI組件: {componentName}");
        }
    }
    
    /// <summary>
    /// 切換UI組件狀態
    /// </summary>
    public void ToggleUIComponent(string componentName)
    {
        if (activeUIComponents.ContainsKey(componentName))
        {
            var component = activeUIComponents[componentName];
            bool newState = !component.isActive;
            
            if (newState)
            {
                ShowUIComponent(componentName);
            }
            else
            {
                HideUIComponent(componentName);
            }
        }
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 顯示對話界面
    /// </summary>
    public void ShowDialogUI()
    {
        SetInteractionMode(InteractionMode.CharacterDialog);
    }
    
    /// <summary>
    /// 隱藏對話界面
    /// </summary>
    public void HideDialogUI()
    {
        SetInteractionMode(InteractionMode.SceneOverview);
    }
    
    /// <summary>
    /// 顯示選單
    /// </summary>
    public void ShowMenu()
    {
        SetCanvasActive("MenuCanvas", true);
    }
    
    /// <summary>
    /// 隱藏選單
    /// </summary>
    public void HideMenu()
    {
        SetCanvasActive("MenuCanvas", false);
    }
    
    /// <summary>
    /// 顯示系統UI (如暫停選單、設置等)
    /// </summary>
    public void ShowSystemUI()
    {
        SetCanvasActive("SystemCanvas", true);
    }
    
    /// <summary>
    /// 隱藏系統UI
    /// </summary>
    public void HideSystemUI()
    {
        SetCanvasActive("SystemCanvas", false);
    }
    
    /// <summary>
    /// 獲取Canvas狀態信息
    /// </summary>
    public string GetCanvasStatusInfo()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("Canvas狀態:");
        sb.AppendLine($"Background: {(canvasLayers.backgroundCanvas?.gameObject.activeSelf ?? false)}");
        sb.AppendLine($"StaticInteraction: {(canvasLayers.staticInteractionCanvas?.gameObject.activeSelf ?? false)}");
        sb.AppendLine($"DynamicCharacter: {(canvasLayers.dynamicCharacterCanvas?.gameObject.activeSelf ?? false)}");
        sb.AppendLine($"UI: {(canvasLayers.uiCanvas?.gameObject.activeSelf ?? false)}");
        sb.AppendLine($"Dialog: {(canvasLayers.dialogCanvas?.gameObject.activeSelf ?? false)}");
        sb.AppendLine($"Menu: {(canvasLayers.menuCanvas?.gameObject.activeSelf ?? false)}");
        sb.AppendLine($"System: {(canvasLayers.systemCanvas?.gameObject.activeSelf ?? false)}");
        
        return sb.ToString();
    }
    
    #endregion
}

/// <summary>
/// Canvas層級配置
/// </summary>
[System.Serializable]
public class CanvasLayerConfig
{
    [Header("Canvas層級 (Order順序)")]
    public Canvas backgroundCanvas;          // Order: 0
    public Canvas staticInteractionCanvas;   // Order: 40
    public Canvas dynamicCharacterCanvas;    // Order: 50
    public Canvas uiCanvas;                  // Order: 60
    public Canvas dialogCanvas;              // Order: 70
    public Canvas menuCanvas;                // Order: 80
    public Canvas systemCanvas;              // Order: 90
}

/// <summary>
/// UI組件數據結構
/// </summary>
[System.Serializable]
public class UIComponent
{
    public string name;
    public GameObject gameObject;
    public Canvas canvas;
    public bool isActive;
}