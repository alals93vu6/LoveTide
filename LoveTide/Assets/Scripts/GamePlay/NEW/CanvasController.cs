using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Canvas控制器
/// 
/// 職責:
/// 1. 管理單個Canvas的顯示和隱藏
/// 2. 控制Canvas內UI元素的狀態
/// 3. 處理Canvas層級的動畫效果
/// 4. 提供Canvas級別的事件管理
/// 
/// 基於架構文檔: NurturingMode/互動系統完整設計_重製版.md
/// 為每個Canvas層級提供獨立的控制邏輯
/// </summary>
public class CanvasController : MonoBehaviour
{
    [Header("== Canvas配置 ==")]
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private CanvasType canvasType;
    [SerializeField] private int sortingOrder = 0;
    
    [Header("== 動畫配置 ==")]
    [SerializeField] private bool useAnimation = true;
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("== 狀態管理 ==")]
    [SerializeField] private bool isVisible = false;
    [SerializeField] private bool isInitialized = false;
    
    // Canvas事件
    public UnityEvent OnCanvasShown;
    public UnityEvent OnCanvasHidden;
    public UnityEvent<float> OnAnimationProgress;
    
    // 組件引用
    private CanvasGroup canvasGroup;
    private GraphicRaycaster graphicRaycaster;
    
    // 動畫控制
    private Coroutine currentAnimation;
    
    public bool IsVisible => isVisible;
    public bool IsInitialized => isInitialized;
    public CanvasType CanvasType => canvasType;
    public Canvas TargetCanvas => targetCanvas;
    
    void Awake()
    {
        Initialize();
    }
    
    /// <summary>
    /// 初始化Canvas控制器
    /// </summary>
    public void Initialize()
    {
        if (isInitialized) return;
        
        Debug.Log($"[CanvasController] 初始化Canvas控制器: {canvasType}");
        
        // 設置目標Canvas
        SetupTargetCanvas();
        
        // 設置組件
        SetupComponents();
        
        // 設置初始狀態
        SetupInitialState();
        
        isInitialized = true;
        Debug.Log($"[CanvasController] Canvas控制器初始化完成: {canvasType}");
    }
    
    /// <summary>
    /// 設置目標Canvas
    /// </summary>
    private void SetupTargetCanvas()
    {
        if (targetCanvas == null)
        {
            targetCanvas = GetComponent<Canvas>();
        }
        
        if (targetCanvas != null)
        {
            targetCanvas.sortingOrder = sortingOrder;
        }
    }
    
    /// <summary>
    /// 設置組件
    /// </summary>
    private void SetupComponents()
    {
        // 設置CanvasGroup
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // 設置GraphicRaycaster
        graphicRaycaster = GetComponent<GraphicRaycaster>();
        if (graphicRaycaster == null)
        {
            graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
        }
    }
    
    /// <summary>
    /// 設置初始狀態
    /// </summary>
    private void SetupInitialState()
    {
        // 根據Canvas類型設置初始可見性
        bool initialVisibility = GetInitialVisibility();
        SetVisibilityImmediate(initialVisibility);
    }
    
    /// <summary>
    /// 獲取初始可見性
    /// </summary>
    private bool GetInitialVisibility()
    {
        switch (canvasType)
        {
            case CanvasType.Background:
            case CanvasType.StaticInteraction:
            case CanvasType.DynamicCharacter:
            case CanvasType.UI:
                return true; // 場景相關Canvas默認顯示
                
            case CanvasType.Dialog:
            case CanvasType.Menu:
                return false; // 對話和選單默認隱藏
                
            case CanvasType.System:
                return true; // 系統Canvas保持顯示
                
            default:
                return false;
        }
    }
    
    #region 顯示/隱藏控制
    
    /// <summary>
    /// 顯示Canvas
    /// </summary>
    public void ShowCanvas()
    {
        if (isVisible) return;
        
        Debug.Log($"[CanvasController] 顯示Canvas: {canvasType}");
        
        if (useAnimation)
        {
            ShowCanvasAnimated();
        }
        else
        {
            SetVisibilityImmediate(true);
        }
    }
    
    /// <summary>
    /// 隱藏Canvas
    /// </summary>
    public void HideCanvas()
    {
        if (!isVisible) return;
        
        Debug.Log($"[CanvasController] 隱藏Canvas: {canvasType}");
        
        if (useAnimation)
        {
            HideCanvasAnimated();
        }
        else
        {
            SetVisibilityImmediate(false);
        }
    }
    
    /// <summary>
    /// 切換Canvas可見性
    /// </summary>
    public void ToggleCanvas()
    {
        if (isVisible)
            HideCanvas();
        else
            ShowCanvas();
    }
    
    /// <summary>
    /// 立即設置可見性
    /// </summary>
    private void SetVisibilityImmediate(bool visible)
    {
        isVisible = visible;
        gameObject.SetActive(visible);
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
        
        // 觸發事件
        if (visible)
            OnCanvasShown?.Invoke();
        else
            OnCanvasHidden?.Invoke();
    }
    
    #endregion
    
    #region 動畫控制
    
    /// <summary>
    /// 動畫顯示Canvas
    /// </summary>
    private void ShowCanvasAnimated()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        currentAnimation = StartCoroutine(AnimateCanvas(true));
    }
    
    /// <summary>
    /// 動畫隱藏Canvas
    /// </summary>
    private void HideCanvasAnimated()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        currentAnimation = StartCoroutine(AnimateCanvas(false));
    }
    
    /// <summary>
    /// Canvas動畫協程
    /// </summary>
    private System.Collections.IEnumerator AnimateCanvas(bool show)
    {
        float startAlpha = show ? 0f : 1f;
        float endAlpha = show ? 1f : 0f;
        
        if (show)
        {
            gameObject.SetActive(true);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = startAlpha;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
        
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            float curveValue = animationCurve.Evaluate(progress);
            
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, curveValue);
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = currentAlpha;
            }
            
            OnAnimationProgress?.Invoke(progress);
            yield return null;
        }
        
        // 設置最終狀態
        if (canvasGroup != null)
        {
            canvasGroup.alpha = endAlpha;
            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;
        }
        
        if (!show)
        {
            gameObject.SetActive(false);
        }
        
        isVisible = show;
        currentAnimation = null;
        
        // 觸發事件
        if (show)
            OnCanvasShown?.Invoke();
        else
            OnCanvasHidden?.Invoke();
    }
    
    #endregion
    
    #region UI元素控制
    
    /// <summary>
    /// 設置Canvas交互性
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        if (canvasGroup != null)
        {
            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = interactable;
        }
        
        Debug.Log($"[CanvasController] 設置Canvas交互性: {canvasType} - {interactable}");
    }
    
    /// <summary>
    /// 設置Canvas透明度
    /// </summary>
    public void SetAlpha(float alpha)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = Mathf.Clamp01(alpha);
        }
    }
    
    /// <summary>
    /// 設置Canvas排序順序
    /// </summary>
    public void SetSortingOrder(int order)
    {
        sortingOrder = order;
        if (targetCanvas != null)
        {
            targetCanvas.sortingOrder = order;
        }
        
        Debug.Log($"[CanvasController] 設置排序順序: {canvasType} - {order}");
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 設置動畫配置
    /// </summary>
    public void SetAnimationConfig(bool useAnim, float duration, AnimationCurve curve)
    {
        useAnimation = useAnim;
        animationDuration = duration;
        if (curve != null)
            animationCurve = curve;
    }
    
    /// <summary>
    /// 強制停止當前動畫
    /// </summary>
    public void StopCurrentAnimation()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
    }
    
    /// <summary>
    /// 獲取Canvas狀態信息
    /// </summary>
    public CanvasStatusInfo GetStatusInfo()
    {
        return new CanvasStatusInfo
        {
            canvasType = canvasType,
            isVisible = isVisible,
            isInteractable = canvasGroup?.interactable ?? false,
            alpha = canvasGroup?.alpha ?? 1f,
            sortingOrder = sortingOrder,
            isAnimating = currentAnimation != null
        };
    }
    
    #endregion
}

/// <summary>
/// Canvas類型枚舉
/// </summary>
public enum CanvasType
{
    Background,        // Order: 0
    StaticInteraction, // Order: 40
    DynamicCharacter,  // Order: 50
    UI,               // Order: 60
    Dialog,           // Order: 70
    Menu,             // Order: 80
    System            // Order: 90
}

/// <summary>
/// Canvas狀態信息
/// </summary>
[System.Serializable]
public class CanvasStatusInfo
{
    public CanvasType canvasType;
    public bool isVisible;
    public bool isInteractable;
    public float alpha;
    public int sortingOrder;
    public bool isAnimating;
}