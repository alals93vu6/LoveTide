using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色跟隨按鈕控制器
/// 
/// 功能:
/// 1. 透明Button跟隨WorldSpace角色移動
/// 2. WorldSpace ↔ ScreenSpace 座標轉換
/// 3. 動態調整按鈕大小和位置
/// 4. 處理角色移動時的按鈕同步
/// 
/// 設計理念:
/// - 解決WorldSpace角色無法直接點擊的問題
/// - 提供平滑的UI跟隨體驗
/// - 支援多解析度適配
/// </summary>
public class CharacterFollowButton : MonoBehaviour
{
    [Header("=== 跟隨目標設定 ===")]
    [SerializeField] private Transform targetCharacter;        // 要跟隨的角色Transform
    [SerializeField] private Vector3 offsetPosition = Vector3.zero;  // 相對角色的偏移位置
    
    [Header("=== 按鈕設定 ===")]
    [SerializeField] private Button followButton;             // 跟隨按鈕組件
    [SerializeField] private RectTransform buttonRect;        // 按鈕的RectTransform
    
    [Header("=== 跟隨參數 ===")]
    [SerializeField] private bool enableFollowing = true;     // 是否啟用跟隨
    [SerializeField] private float followSpeed = 10f;         // 跟隨速度
    [SerializeField] private bool smoothFollow = true;        // 平滑跟隨
    
    [Header("=== 邊界檢查 ===")]
    [SerializeField] private bool enableBoundaryCheck = true; // 啟用邊界檢查
    [SerializeField] private float boundaryPadding = 50f;     // 邊界留白
    
    // 組件引用
    private Camera mainCamera;
    private Canvas parentCanvas;
    private RectTransform canvasRect;
    
    // 狀態追蹤
    private Vector3 targetScreenPosition;
    private Vector3 lastCharacterPosition;
    private bool isInitialized = false;
    
    #region Unity 生命週期
    
    void Awake()
    {
        InitializeComponents();
    }
    
    void Start()
    {
        Initialize();
    }
    
    void Update()
    {
        if (enableFollowing && isInitialized && targetCharacter != null)
        {
            UpdateButtonPosition();
        }
    }
    
    #endregion
    
    #region 初始化
    
    /// <summary>
    /// 初始化組件引用
    /// </summary>
    private void InitializeComponents()
    {
        // 自動獲取組件
        if (followButton == null)
            followButton = GetComponent<Button>();
            
        if (buttonRect == null)
            buttonRect = GetComponent<RectTransform>();
            
        // 獲取父級Canvas
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            canvasRect = parentCanvas.GetComponent<RectTransform>();
        }
        
        // 獲取主攝影機
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
    }
    
    /// <summary>
    /// 初始化跟隨系統
    /// </summary>
    private void Initialize()
    {
        if (targetCharacter == null || mainCamera == null || buttonRect == null)
        {
            Debug.LogWarning("[CharacterFollowButton] 缺少必要組件，無法初始化");
            enabled = false;
            return;
        }
        
        // 記錄初始位置
        lastCharacterPosition = targetCharacter.position;
        
        // 設置初始按鈕位置
        UpdateButtonPosition();
        
        isInitialized = true;
        Debug.Log("[CharacterFollowButton] 角色跟隨按鈕初始化完成");
    }
    
    #endregion
    
    #region 位置更新
    
    /// <summary>
    /// 更新按鈕位置
    /// </summary>
    private void UpdateButtonPosition()
    {
        // 檢查角色是否移動
        Vector3 currentCharacterPosition = targetCharacter.position + offsetPosition;
        
        if (!HasCharacterMoved(currentCharacterPosition) && smoothFollow)
        {
            return; // 角色沒移動，跳過更新
        }
        
        // 世界座標轉螢幕座標
        Vector3 screenPosition = WorldToScreenPoint(currentCharacterPosition);
        
        // 檢查是否在螢幕範圍內
        if (!IsPositionOnScreen(screenPosition))
        {
            if (enableBoundaryCheck)
            {
                screenPosition = ClampToScreenBounds(screenPosition);
            }
            else
            {
                // 隱藏按鈕
                SetButtonVisible(false);
                return;
            }
        }
        else
        {
            // 顯示按鈕
            SetButtonVisible(true);
        }
        
        // 螢幕座標轉UI座標
        Vector2 uiPosition = ScreenToUIPosition(screenPosition);
        
        // 更新按鈕位置
        if (smoothFollow)
        {
            Vector2 currentPos = buttonRect.anchoredPosition;
            Vector2 newPos = Vector2.Lerp(currentPos, uiPosition, followSpeed * Time.deltaTime);
            buttonRect.anchoredPosition = newPos;
        }
        else
        {
            buttonRect.anchoredPosition = uiPosition;
        }
        
        // 更新追蹤位置
        lastCharacterPosition = currentCharacterPosition;
        targetScreenPosition = screenPosition;
    }
    
    /// <summary>
    /// 檢查角色是否移動
    /// </summary>
    private bool HasCharacterMoved(Vector3 currentPosition)
    {
        float moveThreshold = 0.01f;
        return Vector3.Distance(lastCharacterPosition, currentPosition) > moveThreshold;
    }
    
    /// <summary>
    /// 世界座標轉螢幕座標
    /// </summary>
    private Vector3 WorldToScreenPoint(Vector3 worldPosition)
    {
        return mainCamera.WorldToScreenPoint(worldPosition);
    }
    
    /// <summary>
    /// 螢幕座標轉UI座標
    /// </summary>
    private Vector2 ScreenToUIPosition(Vector3 screenPosition)
    {
        Vector2 uiPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPosition, parentCanvas.worldCamera, out uiPosition);
        return uiPosition;
    }
    
    #endregion
    
    #region 邊界檢查
    
    /// <summary>
    /// 檢查位置是否在螢幕範圍內
    /// </summary>
    private bool IsPositionOnScreen(Vector3 screenPosition)
    {
        return screenPosition.x >= -boundaryPadding && 
               screenPosition.x <= Screen.width + boundaryPadding &&
               screenPosition.y >= -boundaryPadding && 
               screenPosition.y <= Screen.height + boundaryPadding &&
               screenPosition.z > 0; // 確保在攝影機前方
    }
    
    /// <summary>
    /// 將位置限制在螢幕邊界內
    /// </summary>
    private Vector3 ClampToScreenBounds(Vector3 screenPosition)
    {
        screenPosition.x = Mathf.Clamp(screenPosition.x, boundaryPadding, Screen.width - boundaryPadding);
        screenPosition.y = Mathf.Clamp(screenPosition.y, boundaryPadding, Screen.height - boundaryPadding);
        return screenPosition;
    }
    
    #endregion
    
    #region 按鈕控制
    
    /// <summary>
    /// 設置按鈕可見性
    /// </summary>
    private void SetButtonVisible(bool visible)
    {
        if (followButton != null)
        {
            followButton.gameObject.SetActive(visible);
        }
    }
    
    /// <summary>
    /// 設置按鈕可互動性
    /// </summary>
    public void SetButtonInteractable(bool interactable)
    {
        if (followButton != null)
        {
            followButton.interactable = interactable;
        }
    }
    
    #endregion
    
    #region 公開接口
    
    /// <summary>
    /// 設置跟隨目標
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        targetCharacter = newTarget;
        if (newTarget != null && isInitialized)
        {
            lastCharacterPosition = newTarget.position;
            UpdateButtonPosition();
        }
    }
    
    /// <summary>
    /// 設置跟隨偏移
    /// </summary>
    public void SetOffset(Vector3 newOffset)
    {
        offsetPosition = newOffset;
    }
    
    /// <summary>
    /// 啟用/禁用跟隨
    /// </summary>
    public void SetFollowingEnabled(bool enabled)
    {
        enableFollowing = enabled;
        if (!enabled)
        {
            SetButtonVisible(false);
        }
    }
    
    /// <summary>
    /// 強制更新位置
    /// </summary>
    public void ForceUpdatePosition()
    {
        if (isInitialized)
        {
            UpdateButtonPosition();
        }
    }
    
    /// <summary>
    /// 獲取當前目標的螢幕位置
    /// </summary>
    public Vector3 GetTargetScreenPosition()
    {
        return targetScreenPosition;
    }
    
    #endregion
    
    #region Debug 功能
    
    void OnDrawGizmos()
    {
        if (targetCharacter != null)
        {
            // 繪製跟隨目標
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetCharacter.position + offsetPosition, 0.5f);
            
            // 繪製連接線
            if (buttonRect != null)
            {
                Gizmos.color = Color.yellow;
                Vector3 buttonWorldPos = buttonRect.transform.position;
                Gizmos.DrawLine(targetCharacter.position + offsetPosition, buttonWorldPos);
            }
        }
    }
    
    #endregion
}