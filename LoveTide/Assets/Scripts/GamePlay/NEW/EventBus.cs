using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 事件總線系統
/// 
/// 職責:
/// 1. 提供全域的事件發布/訂閱機制
/// 2. 解耦系統間的直接依賴關係
/// 3. 支援強型別和弱型別事件
/// 4. 管理事件的生命週期
/// 
/// 基於架構文檔: CoreSystems數據流架構.md
/// 實現事件驅動架構的核心通信機制
/// </summary>
public class EventBus : MonoBehaviour
{
    [Header("== 事件配置 ==")]
    [SerializeField] private bool enableDebugLog = false;
    [SerializeField] private bool enableEventHistory = false;
    [SerializeField] private int maxHistoryCount = 100;
    
    [Header("== 狀態監控 ==")]
    [SerializeField] private int totalEventsSent = 0;
    [SerializeField] private int totalListeners = 0;
    
    // 事件字典
    private Dictionary<Type, List<object>> eventListeners = new Dictionary<Type, List<object>>();
    private Dictionary<string, UnityEvent<object>> stringEventListeners = new Dictionary<string, UnityEvent<object>>();
    
    // 事件歷史
    private Queue<EventHistoryEntry> eventHistory = new Queue<EventHistoryEntry>();
    
    // 單例模式
    public static EventBus Instance { get; private set; }
    
    // 事件統計
    public int TotalEventsSent => totalEventsSent;
    public int TotalListeners => totalListeners;
    
    void Awake()
    {
        // 單例設置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 初始化事件總線
    /// </summary>
    private void Initialize()
    {
        Debug.Log("[EventBus] 事件總線初始化完成");
    }
    
    #region 強型別事件系統
    
    /// <summary>
    /// 註冊事件監聽器
    /// </summary>
    public void Subscribe<T>(Action<T> listener) where T : IEvent
    {
        Type eventType = typeof(T);
        
        if (!eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType] = new List<object>();
        }
        
        eventListeners[eventType].Add(listener);
        totalListeners++;
        
        if (enableDebugLog)
        {
            Debug.Log($"[EventBus] 註冊監聽器: {eventType.Name} (總計: {totalListeners})");
        }
    }
    
    /// <summary>
    /// 取消註冊事件監聽器
    /// </summary>
    public void Unsubscribe<T>(Action<T> listener) where T : IEvent
    {
        Type eventType = typeof(T);
        
        if (eventListeners.ContainsKey(eventType))
        {
            if (eventListeners[eventType].Remove(listener))
            {
                totalListeners--;
                
                if (enableDebugLog)
                {
                    Debug.Log($"[EventBus] 取消註冊監聽器: {eventType.Name} (總計: {totalListeners})");
                }
                
                // 如果沒有監聽器了，移除字典項目
                if (eventListeners[eventType].Count == 0)
                {
                    eventListeners.Remove(eventType);
                }
            }
        }
    }
    
    /// <summary>
    /// 發布事件
    /// </summary>
    public void Publish<T>(T eventData) where T : IEvent
    {
        Type eventType = typeof(T);
        
        if (eventListeners.ContainsKey(eventType))
        {
            var listeners = eventListeners[eventType];
            
            // 創建副本以避免在迭代過程中修改集合
            var listenersCopy = new List<object>(listeners);
            
            foreach (var listener in listenersCopy)
            {
                try
                {
                    if (listener is Action<T> typedListener)
                    {
                        typedListener.Invoke(eventData);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[EventBus] 執行事件監聽器時發生錯誤: {eventType.Name} - {e.Message}");
                }
            }
            
            totalEventsSent++;
            
            if (enableDebugLog)
            {
                Debug.Log($"[EventBus] 發布事件: {eventType.Name} 給 {listenersCopy.Count} 個監聽器");
            }
        }
        else
        {
            if (enableDebugLog)
            {
                Debug.Log($"[EventBus] 沒有監聽器訂閱事件: {eventType.Name}");
            }
        }
        
        // 記錄事件歷史
        if (enableEventHistory)
        {
            RecordEventHistory(eventType.Name, eventData.ToString());
        }
    }
    
    #endregion
    
    #region 字串事件系統
    
    /// <summary>
    /// 註冊字串事件監聽器
    /// </summary>
    public void Subscribe(string eventName, UnityAction<object> listener)
    {
        if (!stringEventListeners.ContainsKey(eventName))
        {
            stringEventListeners[eventName] = new UnityEvent<object>();
        }
        
        stringEventListeners[eventName].AddListener(listener);
        totalListeners++;
        
        if (enableDebugLog)
        {
            Debug.Log($"[EventBus] 註冊字串事件監聽器: {eventName} (總計: {totalListeners})");
        }
    }
    
    /// <summary>
    /// 取消註冊字串事件監聽器
    /// </summary>
    public void Unsubscribe(string eventName, UnityAction<object> listener)
    {
        if (stringEventListeners.ContainsKey(eventName))
        {
            stringEventListeners[eventName].RemoveListener(listener);
            totalListeners--;
            
            if (enableDebugLog)
            {
                Debug.Log($"[EventBus] 取消註冊字串事件監聽器: {eventName} (總計: {totalListeners})");
            }
            
            // 如果沒有監聽器了，移除字典項目
            if (stringEventListeners[eventName].GetPersistentEventCount() == 0)
            {
                stringEventListeners.Remove(eventName);
            }
        }
    }
    
    /// <summary>
    /// 發布字串事件
    /// </summary>
    public void Publish(string eventName, object eventData = null)
    {
        if (stringEventListeners.ContainsKey(eventName))
        {
            stringEventListeners[eventName].Invoke(eventData);
            totalEventsSent++;
            
            if (enableDebugLog)
            {
                Debug.Log($"[EventBus] 發布字串事件: {eventName}");
            }
        }
        else
        {
            if (enableDebugLog)
            {
                Debug.Log($"[EventBus] 沒有監聽器訂閱字串事件: {eventName}");
            }
        }
        
        // 記錄事件歷史
        if (enableEventHistory)
        {
            RecordEventHistory(eventName, eventData?.ToString() ?? "null");
        }
    }
    
    #endregion
    
    #region 事件歷史管理
    
    /// <summary>
    /// 記錄事件歷史
    /// </summary>
    private void RecordEventHistory(string eventName, string eventData)
    {
        var historyEntry = new EventHistoryEntry
        {
            eventName = eventName,
            eventData = eventData,
            timestamp = DateTime.Now,
            frameCount = Time.frameCount
        };
        
        eventHistory.Enqueue(historyEntry);
        
        // 限制歷史記錄數量
        while (eventHistory.Count > maxHistoryCount)
        {
            eventHistory.Dequeue();
        }
    }
    
    /// <summary>
    /// 獲取事件歷史
    /// </summary>
    public EventHistoryEntry[] GetEventHistory()
    {
        return eventHistory.ToArray();
    }
    
    /// <summary>
    /// 清除事件歷史
    /// </summary>
    public void ClearEventHistory()
    {
        eventHistory.Clear();
        Debug.Log("[EventBus] 事件歷史已清除");
    }
    
    #endregion
    
    #region 系統管理
    
    /// <summary>
    /// 清除所有監聽器
    /// </summary>
    public void ClearAllListeners()
    {
        eventListeners.Clear();
        stringEventListeners.Clear();
        totalListeners = 0;
        
        Debug.Log("[EventBus] 所有事件監聽器已清除");
    }
    
    /// <summary>
    /// 清除指定類型的監聽器
    /// </summary>
    public void ClearListeners<T>() where T : IEvent
    {
        Type eventType = typeof(T);
        
        if (eventListeners.ContainsKey(eventType))
        {
            int removedCount = eventListeners[eventType].Count;
            eventListeners.Remove(eventType);
            totalListeners -= removedCount;
            
            Debug.Log($"[EventBus] 清除 {eventType.Name} 的 {removedCount} 個監聽器");
        }
    }
    
    /// <summary>
    /// 清除指定字串事件的監聽器
    /// </summary>
    public void ClearListeners(string eventName)
    {
        if (stringEventListeners.ContainsKey(eventName))
        {
            int removedCount = stringEventListeners[eventName].GetPersistentEventCount();
            stringEventListeners.Remove(eventName);
            totalListeners -= removedCount;
            
            Debug.Log($"[EventBus] 清除字串事件 {eventName} 的 {removedCount} 個監聽器");
        }
    }
    
    /// <summary>
    /// 獲取監聽器數量
    /// </summary>
    public int GetListenerCount<T>() where T : IEvent
    {
        Type eventType = typeof(T);
        
        if (eventListeners.ContainsKey(eventType))
        {
            return eventListeners[eventType].Count;
        }
        
        return 0;
    }
    
    /// <summary>
    /// 獲取字串事件監聽器數量
    /// </summary>
    public int GetListenerCount(string eventName)
    {
        if (stringEventListeners.ContainsKey(eventName))
        {
            return stringEventListeners[eventName].GetPersistentEventCount();
        }
        
        return 0;
    }
    
    /// <summary>
    /// 獲取事件統計信息
    /// </summary>
    public EventBusStats GetStats()
    {
        return new EventBusStats
        {
            totalEventsSent = totalEventsSent,
            totalListeners = totalListeners,
            typedEventTypes = eventListeners.Count,
            stringEventTypes = stringEventListeners.Count,
            historyCount = eventHistory.Count
        };
    }
    
    /// <summary>
    /// 重置統計信息
    /// </summary>
    public void ResetStats()
    {
        totalEventsSent = 0;
        
        Debug.Log("[EventBus] 統計信息已重置");
    }
    
    #endregion
    
    #region Debug工具
    
    /// <summary>
    /// 設置Debug模式
    /// </summary>
    public void SetDebugMode(bool enabled)
    {
        enableDebugLog = enabled;
        Debug.Log($"[EventBus] Debug模式: {(enabled ? "啟用" : "禁用")}");
    }
    
    /// <summary>
    /// 設置事件歷史
    /// </summary>
    public void SetEventHistory(bool enabled)
    {
        enableEventHistory = enabled;
        
        if (!enabled)
        {
            ClearEventHistory();
        }
        
        Debug.Log($"[EventBus] 事件歷史: {(enabled ? "啟用" : "禁用")}");
    }
    
    /// <summary>
    /// 列印所有註冊的事件
    /// </summary>
    public void LogAllRegisteredEvents()
    {
        Debug.Log("=== 強型別事件 ===");
        foreach (var kvp in eventListeners)
        {
            Debug.Log($"  {kvp.Key.Name}: {kvp.Value.Count} 個監聽器");
        }
        
        Debug.Log("=== 字串事件 ===");
        foreach (var kvp in stringEventListeners)
        {
            Debug.Log($"  {kvp.Key}: {kvp.Value.GetPersistentEventCount()} 個監聽器");
        }
    }
    
    #endregion
    
    void OnDestroy()
    {
        // 清理所有監聽器
        ClearAllListeners();
    }
}

/// <summary>
/// 事件接口
/// </summary>
public interface IEvent
{
    DateTime Timestamp { get; }
}

/// <summary>
/// 基礎事件類
/// </summary>
public abstract class BaseEvent : IEvent
{
    public DateTime Timestamp { get; private set; }
    
    protected BaseEvent()
    {
        Timestamp = DateTime.Now;
    }
}

/// <summary>
/// 事件歷史記錄
/// </summary>
[System.Serializable]
public class EventHistoryEntry
{
    public string eventName;
    public string eventData;
    public DateTime timestamp;
    public int frameCount;
}

/// <summary>
/// 事件總線統計
/// </summary>
[System.Serializable]
public class EventBusStats
{
    public int totalEventsSent;
    public int totalListeners;
    public int typedEventTypes;
    public int stringEventTypes;
    public int historyCount;
}

// 常用的系統事件類型
namespace GameEvents
{
    /// <summary>
    /// 遊戲狀態變更事件
    /// </summary>
    public class GameStateChangedEvent : BaseEvent
    {
        public GameState oldState;
        public GameState newState;
        
        public GameStateChangedEvent(GameState oldState, GameState newState)
        {
            this.oldState = oldState;
            this.newState = newState;
        }
    }
    
    /// <summary>
    /// 場景變更事件
    /// </summary>
    public class SceneChangedEvent : BaseEvent
    {
        public SceneMode oldScene;
        public SceneMode newScene;
        
        public SceneChangedEvent(SceneMode oldScene, SceneMode newScene)
        {
            this.oldScene = oldScene;
            this.newScene = newScene;
        }
    }
    
    /// <summary>
    /// 互動觸發事件
    /// </summary>
    public class InteractionTriggeredEvent : BaseEvent
    {
        public InteractionType interactionType;
        public GameObject targetObject;
        
        public InteractionTriggeredEvent(InteractionType interactionType, GameObject targetObject)
        {
            this.interactionType = interactionType;
            this.targetObject = targetObject;
        }
    }
    
    /// <summary>
    /// 數值變更事件
    /// </summary>
    public class ValueChangedEvent : BaseEvent
    {
        public string valueName;
        public object oldValue;
        public object newValue;
        
        public ValueChangedEvent(string valueName, object oldValue, object newValue)
        {
            this.valueName = valueName;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
    
    /// <summary>
    /// 對話開始事件
    /// </summary>
    public class DialogStartedEvent : BaseEvent
    {
        public string characterName;
        public string dialogId;
        
        public DialogStartedEvent(string characterName, string dialogId)
        {
            this.characterName = characterName;
            this.dialogId = dialogId;
        }
    }
    
    /// <summary>
    /// 對話結束事件
    /// </summary>
    public class DialogEndedEvent : BaseEvent
    {
        public string characterName;
        public string dialogId;
        public bool wasCompleted;
        
        public DialogEndedEvent(string characterName, string dialogId, bool wasCompleted)
        {
            this.characterName = characterName;
            this.dialogId = dialogId;
            this.wasCompleted = wasCompleted;
        }
    }
}