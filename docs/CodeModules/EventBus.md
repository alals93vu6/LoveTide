# 📡 模組名稱：EventBus

> 事件總線系統，負責遊戲中各模組間的去耦合通信

---

## 🔖 模組功能

EventBus 是遊戲的核心通信系統，實現了發布-訂閱模式，讓各個模組能夠以低耦合的方式進行通信和數據交換。

---

## 📍 檔案位置

**路徑**: `Assets/Scripts/Core/EventBus.cs`  
**命名空間**: `LoveTide.Core`  
**設計模式**: Singleton Pattern
**繼承**: `MonoBehaviour`

---

## 🧩 公開方法一覽

| 方法名稱 | 功能描述 | 參數 | 回傳值 |
|----------|----------|------|---------|
| `Subscribe<T>(string eventName, Action<T> handler)` | 訂閱事件 | string, Action<T> | void |
| `Subscribe(string eventName, Action handler)` | 訂閱無參數事件 | string, Action | void |
| `Unsubscribe<T>(string eventName, Action<T> handler)` | 取消訂閱事件 | string, Action<T> | void |
| `Unsubscribe(string eventName, Action handler)` | 取消訂閱無參數事件 | string, Action | void |
| `Publish<T>(string eventName, T data)` | 發布事件 | string, T | void |
| `Publish(string eventName)` | 發布無參數事件 | string | void |
| `Clear()` | 清除所有事件訂閱 | 無 | void |
| `GetSubscriberCount(string eventName)` | 取得訂閱者數量 | string | int |
| `IsEventRegistered(string eventName)` | 檢查事件是否已註冊 | string | bool |

---

## 🎯 主要屬性

### 📊 事件管理
```csharp
[Header("Event Management")]
public bool enableDebugLog = false;                    // 啟用除錯日誌
public bool enableEventHistory = true;                // 啟用事件歷史記錄
public int maxEventHistory = 100;                     // 最大事件歷史記錄數

// 事件訂閱字典
private Dictionary<string, List<Delegate>> eventSubscribers = new Dictionary<string, List<Delegate>>();

// 事件歷史記錄
private Queue<EventLogEntry> eventHistory = new Queue<EventLogEntry>();
```

### 🔒 單例模式
```csharp
private static EventBus _instance;
public static EventBus Instance
{
    get
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<EventBus>();
            if (_instance == null)
            {
                GameObject go = new GameObject("EventBus");
                _instance = go.AddComponent<EventBus>();
                DontDestroyOnLoad(go);
            }
        }
        return _instance;
    }
}
```

---

## 📡 事件訂閱系統

### 🎯 泛型事件訂閱
```csharp
public void Subscribe<T>(string eventName, Action<T> handler)
{
    if (string.IsNullOrEmpty(eventName) || handler == null)
    {
        Debug.LogWarning("EventBus: Invalid event name or handler");
        return;
    }
    
    if (!eventSubscribers.ContainsKey(eventName))
    {
        eventSubscribers[eventName] = new List<Delegate>();
    }
    
    eventSubscribers[eventName].Add(handler);
    
    if (enableDebugLog)
    {
        Debug.Log($"EventBus: Subscribed to event '{eventName}' with handler type {typeof(T).Name}");
    }
}
```

### 🎪 無參數事件訂閱
```csharp
public void Subscribe(string eventName, Action handler)
{
    if (string.IsNullOrEmpty(eventName) || handler == null)
    {
        Debug.LogWarning("EventBus: Invalid event name or handler");
        return;
    }
    
    if (!eventSubscribers.ContainsKey(eventName))
    {
        eventSubscribers[eventName] = new List<Delegate>();
    }
    
    eventSubscribers[eventName].Add(handler);
    
    if (enableDebugLog)
    {
        Debug.Log($"EventBus: Subscribed to event '{eventName}' (no parameters)");
    }
}
```

### 🔄 取消訂閱
```csharp
public void Unsubscribe<T>(string eventName, Action<T> handler)
{
    if (string.IsNullOrEmpty(eventName) || handler == null)
        return;
    
    if (eventSubscribers.ContainsKey(eventName))
    {
        eventSubscribers[eventName].Remove(handler);
        
        // 如果沒有訂閱者，移除事件
        if (eventSubscribers[eventName].Count == 0)
        {
            eventSubscribers.Remove(eventName);
        }
        
        if (enableDebugLog)
        {
            Debug.Log($"EventBus: Unsubscribed from event '{eventName}'");
        }
    }
}
```

---

## 📢 事件發布系統

### 🚀 泛型事件發布
```csharp
public void Publish<T>(string eventName, T data)
{
    if (string.IsNullOrEmpty(eventName))
    {
        Debug.LogWarning("EventBus: Cannot publish event with empty name");
        return;
    }
    
    // 記錄事件歷史
    if (enableEventHistory)
    {
        RecordEventHistory(eventName, typeof(T), data);
    }
    
    if (eventSubscribers.ContainsKey(eventName))
    {
        var subscribers = eventSubscribers[eventName].ToList(); // 複製列表防止修改
        
        foreach (var subscriber in subscribers)
        {
            try
            {
                if (subscriber is Action<T> typedHandler)
                {
                    typedHandler.Invoke(data);
                }
                else if (subscriber is Action simpleHandler && data == null)
                {
                    simpleHandler.Invoke();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"EventBus: Error invoking handler for event '{eventName}': {e.Message}");
            }
        }
        
        if (enableDebugLog)
        {
            Debug.Log($"EventBus: Published event '{eventName}' to {subscribers.Count} subscribers");
        }
    }
    else if (enableDebugLog)
    {
        Debug.Log($"EventBus: No subscribers for event '{eventName}'");
    }
}
```

### 🎯 無參數事件發布
```csharp
public void Publish(string eventName)
{
    if (string.IsNullOrEmpty(eventName))
    {
        Debug.LogWarning("EventBus: Cannot publish event with empty name");
        return;
    }
    
    // 記錄事件歷史
    if (enableEventHistory)
    {
        RecordEventHistory(eventName, null, null);
    }
    
    if (eventSubscribers.ContainsKey(eventName))
    {
        var subscribers = eventSubscribers[eventName].ToList();
        
        foreach (var subscriber in subscribers)
        {
            try
            {
                if (subscriber is Action handler)
                {
                    handler.Invoke();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"EventBus: Error invoking handler for event '{eventName}': {e.Message}");
            }
        }
        
        if (enableDebugLog)
        {
            Debug.Log($"EventBus: Published event '{eventName}' to {subscribers.Count} subscribers");
        }
    }
}
```

---

## 📚 事件歷史記錄

### 📝 歷史記錄結構
```csharp
[System.Serializable]
public class EventLogEntry
{
    public string eventName;
    public string dataType;
    public string dataValue;
    public DateTime timestamp;
    public int subscriberCount;
    
    public EventLogEntry(string eventName, System.Type dataType, object data, int subscriberCount)
    {
        this.eventName = eventName;
        this.dataType = dataType?.Name ?? "void";
        this.dataValue = data?.ToString() ?? "null";
        this.timestamp = DateTime.Now;
        this.subscriberCount = subscriberCount;
    }
}
```

### 🔍 記錄事件歷史
```csharp
private void RecordEventHistory(string eventName, System.Type dataType, object data)
{
    int subscriberCount = GetSubscriberCount(eventName);
    var logEntry = new EventLogEntry(eventName, dataType, data, subscriberCount);
    
    eventHistory.Enqueue(logEntry);
    
    // 保持歷史記錄數量限制
    while (eventHistory.Count > maxEventHistory)
    {
        eventHistory.Dequeue();
    }
}
```

### 📊 取得事件歷史
```csharp
public List<EventLogEntry> GetEventHistory()
{
    return eventHistory.ToList();
}

public List<EventLogEntry> GetEventHistory(string eventName)
{
    return eventHistory.Where(entry => entry.eventName == eventName).ToList();
}
```

---

## 🔍 事件監控系統

### 📈 事件統計
```csharp
[System.Serializable]
public class EventStatistics
{
    public Dictionary<string, int> eventPublishCount = new Dictionary<string, int>();
    public Dictionary<string, int> eventSubscriberCount = new Dictionary<string, int>();
    public Dictionary<string, DateTime> lastEventTime = new Dictionary<string, DateTime>();
    
    public void RecordEventPublish(string eventName)
    {
        if (!eventPublishCount.ContainsKey(eventName))
            eventPublishCount[eventName] = 0;
        
        eventPublishCount[eventName]++;
        lastEventTime[eventName] = DateTime.Now;
    }
    
    public void UpdateSubscriberCount(string eventName, int count)
    {
        eventSubscriberCount[eventName] = count;
    }
}
```

### 📊 監控和除錯
```csharp
public EventStatistics GetEventStatistics()
{
    var stats = new EventStatistics();
    
    foreach (var kvp in eventSubscribers)
    {
        stats.eventSubscriberCount[kvp.Key] = kvp.Value.Count;
    }
    
    return stats;
}

public void PrintEventStatistics()
{
    var stats = GetEventStatistics();
    Debug.Log("=== EventBus Statistics ===");
    
    foreach (var kvp in stats.eventSubscriberCount)
    {
        Debug.Log($"Event: {kvp.Key}, Subscribers: {kvp.Value}");
    }
}
```

---

## 🎯 常用事件定義

### 📋 系統事件
```csharp
public static class SystemEvents
{
    public const string GAME_START = "GameStart";
    public const string GAME_PAUSE = "GamePause";
    public const string GAME_RESUME = "GameResume";
    public const string GAME_QUIT = "GameQuit";
    public const string SCENE_CHANGE = "SceneChange";
    public const string SAVE_GAME = "SaveGame";
    public const string LOAD_GAME = "LoadGame";
}
```

### 🎭 遊戲事件
```csharp
public static class GameEvents
{
    public const string DIALOG_START = "DialogStart";
    public const string DIALOG_END = "DialogEnd";
    public const string CHOICE_SELECTED = "ChoiceSelected";
    public const string AFFECTION_CHANGED = "AffectionChanged";
    public const string TIME_CHANGED = "TimeChanged";
    public const string MONEY_CHANGED = "MoneyChanged";
    public const string MINI_GAME_START = "MiniGameStart";
    public const string MINI_GAME_END = "MiniGameEnd";
}
```

### 🎨 UI事件
```csharp
public static class UIEvents
{
    public const string BUTTON_CLICK = "ButtonClick";
    public const string PANEL_OPEN = "PanelOpen";
    public const string PANEL_CLOSE = "PanelClose";
    public const string MENU_CHANGE = "MenuChange";
    public const string SETTING_CHANGE = "SettingChange";
}
```

---

## 🎪 事件數據結構

### 📊 常用事件數據類型
```csharp
[System.Serializable]
public class DialogEvent
{
    public string characterName;
    public string dialogText;
    public int dialogIndex;
}

[System.Serializable]
public class AffectionEvent
{
    public string characterId;
    public int oldValue;
    public int newValue;
    public string reason;
}

[System.Serializable]
public class SceneChangeEvent
{
    public string fromScene;
    public string toScene;
    public float transitionTime;
}

[System.Serializable]
public class TimeEvent
{
    public int day;
    public TimeOfDay timeOfDay;
    public int hour;
    public int minute;
}
```

---

## 🔧 效能優化

### 🚀 記憶體優化
```csharp
private void OnDestroy()
{
    // 清除所有事件訂閱
    Clear();
}

public void Clear()
{
    eventSubscribers.Clear();
    eventHistory.Clear();
    
    if (enableDebugLog)
    {
        Debug.Log("EventBus: Cleared all event subscriptions");
    }
}
```

### 🎯 性能監控
```csharp
private void Update()
{
    // 定期清理無效的訂閱者
    if (Time.time % 10.0f < Time.deltaTime) // 每10秒執行一次
    {
        CleanupInvalidSubscribers();
    }
}

private void CleanupInvalidSubscribers()
{
    var keysToRemove = new List<string>();
    
    foreach (var kvp in eventSubscribers)
    {
        kvp.Value.RemoveAll(subscriber => subscriber == null);
        
        if (kvp.Value.Count == 0)
        {
            keysToRemove.Add(kvp.Key);
        }
    }
    
    foreach (var key in keysToRemove)
    {
        eventSubscribers.Remove(key);
    }
}
```

---

## 🛡 錯誤處理

### 🚨 安全檢查
```csharp
private bool ValidateEventName(string eventName)
{
    if (string.IsNullOrEmpty(eventName))
    {
        Debug.LogWarning("EventBus: Event name cannot be null or empty");
        return false;
    }
    
    if (eventName.Length > 100)
    {
        Debug.LogWarning("EventBus: Event name too long (max 100 characters)");
        return false;
    }
    
    return true;
}
```

### 🔍 除錯工具
```csharp
[System.Diagnostics.Conditional("UNITY_EDITOR")]
public void DebugPrintAllEvents()
{
    Debug.Log("=== EventBus Debug Info ===");
    foreach (var kvp in eventSubscribers)
    {
        Debug.Log($"Event: {kvp.Key}, Subscribers: {kvp.Value.Count}");
    }
}
```

---

## 🔁 呼叫關係

### 📊 被呼叫情況
- **所有遊戲模組**: 發布和訂閱事件
- **GameManagerTest**: 系統事件管理
- **UI系統**: UI事件處理
- **劇情系統**: 劇情事件通知

### 🎯 呼叫對象
- **無直接呼叫對象**: 作為通信媒介，不直接呼叫其他模組

---

## 💬 Claude 使用提示

使用 EventBus 時請注意：
1. **先理解事件驅動架構** 了解發布-訂閱模式的優缺點
2. **正確取消訂閱** 在對象銷毀時取消事件訂閱防止記憶體洩漏
3. **命名規範** 使用有意義的事件名稱，建議使用常數定義
4. **適當使用** 不要過度使用事件系統，簡單的直接調用更合適
5. **錯誤處理** 在事件處理函數中添加適當的錯誤處理

常見使用模式：
```csharp
// 訂閱事件
private void Start()
{
    EventBus.Instance.Subscribe<AffectionEvent>("AffectionChanged", OnAffectionChanged);
}

// 取消訂閱
private void OnDestroy()
{
    EventBus.Instance.Unsubscribe<AffectionEvent>("AffectionChanged", OnAffectionChanged);
}

// 發布事件
private void ChangeAffection(int newValue)
{
    var eventData = new AffectionEvent { newValue = newValue };
    EventBus.Instance.Publish("AffectionChanged", eventData);
}
```