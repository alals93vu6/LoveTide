using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

/// <summary>
/// 靜態物件互動系統
/// 
/// 職責:
/// 1. 管理場景中的靜態互動物件(工作台、貓咪、出入口等)
/// 2. 提供UI Button + Canvas的互動機制
/// 3. 替代舊有的Physics-based互動系統
/// 4. 與InteractionManager協作觸發互動事件
/// 
/// 基於架構文檔: NurturingMode/互動系統完整設計_重製版.md
/// 實現Canvas-based的靜態物件互動
/// </summary>
public class StaticObjectInteraction : MonoBehaviour
{
    [Header("== 靜態互動物件配置 ==")]
    [SerializeField] private List<StaticInteractionObject> interactionObjects = new List<StaticInteractionObject>();
    
    [Header("== Canvas引用 ==")]
    [SerializeField] private Canvas staticInteractionCanvas; // Order: 40
    
    [Header("== 互動狀態 ==")]
    [SerializeField] private bool isInitialized = false;
    [SerializeField] private bool interactionsEnabled = true;
    
    // 互動事件
    public UnityEvent<InteractionType, GameObject> OnInteractionTriggered;
    
    // 當前活躍的互動物件
    private StaticInteractionObject currentActiveObject;
    
    public bool IsInitialized => isInitialized;
    public bool InteractionsEnabled => interactionsEnabled;
    
    /// <summary>
    /// 初始化靜態物件互動系統（帶InteractionManager參數）
    /// </summary>
    public void Initialize(LoveTide.Core.InteractionManager interactionManager)
{
    BasicInitialize(); // 呼叫基本初始化
    
    if (interactionManager != null)
    {
        Debug.Log("[StaticObjectInteraction] 與 InteractionManager 建立關聯");
        // 可以在這裡添加特定的綁定邏輯
    }
}

private void BasicInitialize()
{
    Debug.Log("[StaticObjectInteraction] 基本初始化");
    // 原本無參數 Initialize 的邏輯寫這裡
}
    
    ///<summary>
    /// 初始化靜態物件互動系統（帶InteractionManager參數）
    /// </summary>
    public void InitializeWithManager(LoveTide.Core.InteractionManager interactionManager)
    {
        BasicInitialize(); // 呼叫基本初始化

        if (interactionManager != null)
        {
            Debug.Log("[StaticObjectInteraction] 與 InteractionManager 建立關聯");
            // 可以在這裡添加特定的綁定邏輯
        }
    }

    /// <summary>
    /// 基本初始化方法
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[StaticObjectInteraction] 初始化靜態物件互動系統");

        // 查找Canvas
        FindStaticInteractionCanvas();

        // 設置互動物件
        SetupInteractionObjects();

        // 綁定事件
        BindInteractionEvents();

        isInitialized = true;
        Debug.Log("[StaticObjectInteraction] 靜態物件互動系統初始化完成");
    }
    

    /// <summary>
    /// 查找靜態互動Canvas
    /// </summary>
    private void FindStaticInteractionCanvas()
    {
        if (staticInteractionCanvas == null)
        {
            // 查找名為StaticInteractionCanvas的Canvas
            staticInteractionCanvas = GameObject.Find("StaticInteractionCanvas")?.GetComponent<Canvas>();
            
            if (staticInteractionCanvas == null)
            {
                Debug.LogWarning("[StaticObjectInteraction] 找不到StaticInteractionCanvas");
            }
            else
            {
                Debug.Log("[StaticObjectInteraction] 找到StaticInteractionCanvas");
            }
        }
    }
    
    /// <summary>
    /// 設置互動物件
    /// </summary>
    private void SetupInteractionObjects()
    {
        // 如果沒有手動配置，自動查找場景中的互動物件
        if (interactionObjects.Count == 0)
        {
            AutoDiscoverInteractionObjects();
        }
        
        // 初始化每個互動物件
        foreach (var obj in interactionObjects)
        {
            if (obj != null)
            {
                obj.Initialize();
            }
        }
    }
    
    /// <summary>
    /// 自動發現場景中的互動物件
    /// </summary>
    private void AutoDiscoverInteractionObjects()
    {
        // 查找所有帶有StaticInteractionObject組件的物件
        StaticInteractionObject[] foundObjects = FindObjectsOfType<StaticInteractionObject>();
        
        foreach (var obj in foundObjects)
        {
            if (!interactionObjects.Contains(obj))
            {
                interactionObjects.Add(obj);
                Debug.Log($"[StaticObjectInteraction] 發現互動物件: {obj.name}");
            }
        }
    }
    
    /// <summary>
    /// 綁定互動事件
    /// </summary>
    private void BindInteractionEvents()
    {
        foreach (var obj in interactionObjects)
        {
            if (obj != null)
            {
                // 綁定每個物件的互動事件到統一處理方法
                obj.OnObjectInteracted.AddListener(HandleObjectInteraction);
            }
        }
    }
    
    /// <summary>
    /// 處理物件互動
    /// </summary>
    private void HandleObjectInteraction(StaticInteractionObject interactionObject)
    {
        if (!interactionsEnabled)
        {
            Debug.LogWarning("[StaticObjectInteraction] 互動系統已禁用");
            return;
        }
        
        Debug.Log($"[StaticObjectInteraction] 處理物件互動: {interactionObject.InteractionType} - {interactionObject.name}");
        
        // 設置當前活躍物件
        currentActiveObject = interactionObject;
        
        // 觸發互動事件，傳遞給InteractionManager
        OnInteractionTriggered?.Invoke(interactionObject.InteractionType, interactionObject.gameObject);
    }
    
    #region 互動控制方法
    
    /// <summary>
    /// 啟用所有互動
    /// </summary>
    public void EnableAllInteractions()
    {
        interactionsEnabled = true;
        
        foreach (var obj in interactionObjects)
        {
            if (obj != null)
            {
                obj.SetInteractionEnabled(true);
            }
        }
        
        Debug.Log("[StaticObjectInteraction] 所有靜態互動已啟用");
    }
    
    /// <summary>
    /// 禁用所有互動
    /// </summary>
    public void DisableAllInteractions()
    {
        interactionsEnabled = false;
        
        foreach (var obj in interactionObjects)
        {
            if (obj != null)
            {
                obj.SetInteractionEnabled(false);
            }
        }
        
        Debug.Log("[StaticObjectInteraction] 所有靜態互動已禁用");
    }
    
    /// <summary>
    /// 啟用特定類型的互動
    /// </summary>
    public void EnableInteractionByType(InteractionType interactionType)
    {
        foreach (var obj in interactionObjects)
        {
            if (obj != null && obj.InteractionType == interactionType)
            {
                obj.SetInteractionEnabled(true);
            }
        }
        
        Debug.Log($"[StaticObjectInteraction] 啟用互動類型: {interactionType}");
    }
    
    /// <summary>
    /// 禁用特定類型的互動
    /// </summary>
    public void DisableInteractionType(InteractionType interactionType)
    {
        foreach (var obj in interactionObjects)
        {
            if (obj != null && obj.InteractionType == interactionType)
            {
                obj.SetInteractionEnabled(false);
            }
        }
        
        Debug.Log($"[StaticObjectInteraction] 禁用互動類型: {interactionType}");
    }
    
    /// <summary>
    /// 獲取指定類型的所有互動物件
    /// </summary>
    public List<StaticInteractionObject> GetInteractionObjectsByType(InteractionType interactionType)
    {
        List<StaticInteractionObject> result = new List<StaticInteractionObject>();
        
        foreach (var obj in interactionObjects)
        {
            if (obj != null && obj.InteractionType == interactionType)
            {
                result.Add(obj);
            }
        }
        
        return result;
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 添加新的互動物件
    /// </summary>
    public void AddInteractionObject(StaticInteractionObject newObject)
    {
        if (newObject != null && !interactionObjects.Contains(newObject))
        {
            interactionObjects.Add(newObject);
            newObject.Initialize();
            newObject.OnObjectInteracted.AddListener(HandleObjectInteraction);
            
            Debug.Log($"[StaticObjectInteraction] 添加互動物件: {newObject.name}");
        }
    }
    
    /// <summary>
    /// 移除互動物件
    /// </summary>
    public void RemoveInteractionObject(StaticInteractionObject objectToRemove)
    {
        if (objectToRemove != null && interactionObjects.Contains(objectToRemove))
        {
            objectToRemove.OnObjectInteracted.RemoveListener(HandleObjectInteraction);
            interactionObjects.Remove(objectToRemove);
            
            Debug.Log($"[StaticObjectInteraction] 移除互動物件: {objectToRemove.name}");
        }
    }
    
    /// <summary>
    /// 獲取當前活躍的互動物件
    /// </summary>
    public StaticInteractionObject GetCurrentActiveObject()
    {
        return currentActiveObject;
    }

    /// <summary>
    /// 啟用/禁用特定名稱的互動
    /// </summary>
    public void EnableInteractionType(string interactionName, bool enabled)
    {
        foreach (var obj in interactionObjects)
        {
            if (obj != null && obj.name.Contains(interactionName))
            {
                obj.SetInteractionEnabled(enabled);
            }
        }
    }

    #endregion
}

/// <summary>
/// 靜態互動物件組件
/// 附加到場景中的靜態互動物件上
/// </summary>
public class StaticInteractionObject : MonoBehaviour
{
    [Header("== 互動配置 ==")]
    [SerializeField] private InteractionType interactionType = InteractionType.Work;
    [SerializeField] private string interactionName = "互動";
    [SerializeField] private string interactionDescription = "點擊進行互動";
    
    [Header("== UI組件 ==")]
    [SerializeField] private Button interactionButton;
    [SerializeField] private GameObject interactionUI;
    
    [Header("== 互動狀態 ==")]
    [SerializeField] private bool isInteractionEnabled = true;
    [SerializeField] private bool isInitialized = false;
    
    // 互動事件
    public UnityEvent<StaticInteractionObject> OnObjectInteracted;
    
    public InteractionType InteractionType => interactionType;
    public string InteractionName => interactionName;
    public string InteractionDescription => interactionDescription;
    public bool IsInteractionEnabled => isInteractionEnabled;
    public bool IsInitialized => isInitialized;
    
    /// <summary>
    /// 初始化互動物件
    /// </summary>
    public void Initialize()
    {
        // 查找Button組件
        if (interactionButton == null)
        {
            interactionButton = GetComponentInChildren<Button>();
        }
        
        // 查找互動UI
        if (interactionUI == null)
        {
            interactionUI = transform.Find("InteractionUI")?.gameObject;
        }
        
        // 綁定Button事件
        if (interactionButton != null)
        {
            interactionButton.onClick.AddListener(TriggerInteraction);
        }
        
        // 設置初始狀態
        UpdateInteractionState();
        
        isInitialized = true;
        Debug.Log($"[StaticInteractionObject] 初始化互動物件: {name} - {interactionType}");
    }
    
    /// <summary>
    /// 觸發互動
    /// </summary>
    public void TriggerInteraction()
    {
        if (!isInteractionEnabled)
        {
            Debug.LogWarning($"[StaticInteractionObject] 互動已禁用: {name}");
            return;
        }
        
        Debug.Log($"[StaticInteractionObject] 觸發互動: {name} - {interactionType}");
        
        // 播放互動音效
        // AudioManager.Instance?.PlaySFX("button_click");
        
        // 觸發互動事件
        OnObjectInteracted?.Invoke(this);
    }
    
    /// <summary>
    /// 設置互動啟用狀態
    /// </summary>
    public void SetInteractionEnabled(bool enabled)
    {
        isInteractionEnabled = enabled;
        UpdateInteractionState();
    }
    
    /// <summary>
    /// 更新互動狀態
    /// </summary>
    private void UpdateInteractionState()
    {
        // 更新Button狀態
        if (interactionButton != null)
        {
            interactionButton.interactable = isInteractionEnabled;
        }
        
        // 更新UI顯示
        if (interactionUI != null)
        {
            interactionUI.SetActive(isInteractionEnabled);
        }
        
        // 更新視覺反饋
        UpdateVisualFeedback();
    }
    
    /// <summary>
    /// 更新視覺反饋
    /// </summary>
    private void UpdateVisualFeedback()
    {
        // 根據互動狀態調整物件的視覺效果
        // 例如：改變透明度、顏色、發光效果等
        
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color color = renderer.material.color;
            color.a = isInteractionEnabled ? 1.0f : 0.5f;
            renderer.material.color = color;
        }
    }
    
    /// <summary>
    /// 設置互動信息
    /// </summary>
    public void SetInteractionInfo(InteractionType type, string name, string description)
    {
        interactionType = type;
        interactionName = name;
        interactionDescription = description;
    }
}