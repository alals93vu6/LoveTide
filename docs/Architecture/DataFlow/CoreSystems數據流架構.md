# 🌊 CoreSystems 數據流架構

> LoveTide 劇情播放系統與養成互動系統之間的完整數據流設計與整合架構

---

## 🎯 概述

CoreSystems 數據流架構專注於描述劇情播放系統和養成互動系統之間的數據傳遞、狀態同步和事件驅動機制。系統採用事件總線模式實現解耦通信，通過JSON驅動的數據管理和時間系統協調，實現了兩大核心系統的無縫整合。

---

## 🏗️ 系統架構圖

```
🌊 CoreSystems 數據流架構
│
├── 📊 數據源層 (Data Source Layer)
│   ├── DialogDataManager - JSON對話數據源
│   ├── NumericalRecords - 數值數據源
│   ├── StoryProgressTracker - 劇情進度數據源
│   └── TimeManagerTest - 時間數據源
│
├── 🚌 事件總線層 (Event Bus Layer)
│   ├── EventBus - 核心事件總線
│   ├── StoryEvents - 劇情系統事件
│   ├── NurturingEvents - 養成系統事件
│   └── TimeEvents - 時間系統事件
│
├── 🎮 控制層 (Control Layer)
│   ├── GamePlayingManagerDrama - 劇情主控制器
│   ├── PlayerActorTest - 養成互動控制器
│   ├── StatCalculationEngine - 數值計算引擎
│   └── InteractionResultProcessor - 互動結果處理器
│
├── 🔄 同步層 (Synchronization Layer)
│   ├── StoryModeIntegration - 劇情模式整合服務
│   ├── NurturingModeIntegration - 養成模式整合服務
│   ├── SaveSystemIntegration - 存檔系統整合服務
│   └── CGUnlockIntegration - CG解鎖整合服務
│
└── 🎨 展示層 (Presentation Layer)
    ├── TextBoxDrama - 劇情文字展示
    ├── InteractionFeedbackUI - 互動反饋展示
    ├── StatsDisplayController - 數值展示控制
    └── ProgressVisualization - 進度視覺化
```

---

## 🌊 完整數據流程圖

```
🎮 遊戲流程數據流
│
🌅 遊戲啟動
    ↓
📊 數據初始化階段
    ├── NumericalRecords.Initialize() - 載入玩家數值
    ├── TimeManagerTest.Initialize() - 初始化時間系統
    ├── DialogDataManager.Initialize() - 載入對話數據緩存
    └── StoryProgressTracker.Initialize() - 載入劇情進度
    ↓
🎯 模式選擇判斷
    ├── 檢查劇情觸發條件
    ├── 分析當前遊戲狀態
    └── 決定進入哪個模式
    ↓
🔄 主遊戲循環 (雙模式數據流)
│
├── 💖 養成模式數據流 ─────────┐
│   │                          │
│   👆 玩家互動觸發             │
│       ↓                      │
│   🔍 InteractionConditionChecker │
│       ├── 檢查好感度等級      │
│       ├── 檢查時間條件        │
│       ├── 檢查精力數值        │
│       └── 檢查解鎖狀態        │
│       ↓                      │
│   🧮 StatCalculationEngine    │
│       ├── 計算基礎成功率      │
│       ├── 應用時間修正器      │
│       ├── 應用精力修正器      │
│       └── 計算數值變化        │
│       ↓                      │
│   📊 NumericalRecords.ModifyStat() │
│       ├── 更新好感度          │
│       ├── 更新淫亂度          │
│       ├── 更新慾望值          │
│       ├── 更新精力值          │
│       └── 觸發數值變更事件    │
│       ↓                      │
│   ⏰ TimeManagerTest.AdvanceTime() │
│       ├── 推進時間單位        │
│       ├── 檢查定時事件        │
│       ├── 應用時間效果        │
│       └── 觸發時間推進事件    │
│       ↓                      │
│   📈 進度檢查階段             │
│       ├── ProgressionManager.CheckLevelUp() │
│       ├── StoryProgressTracker.CheckTriggers() │
│       └── 評估劇情觸發條件    │
│       ↓                      │
│   🎯 條件判斷                │
│       ├── ❌ 不滿足劇情條件 ──┘
│       └── ✅ 滿足劇情條件
│           ↓
│   📡 EventBus.Publish("TriggerStory")
│       ↓
│   🎭 切換到劇情模式
│
└── 🎭 劇情模式數據流
    │
    🎬 劇情播放觸發
        ↓
    📖 DialogDataManager.LoadDialogData()
        ├── 從緩存獲取對話數據
        ├── 處理玩家名稱替換
        ├── 處理動態內容標記
        └── 返回處理後的對話數據
        ↓
    🎮 GamePlayingManagerDrama.StartStoryPlayback()
        ├── 設定劇情狀態
        ├── 初始化播放環境
        └── 開始播放第一段對話
        ↓
    🎨 展示層並行執行
        ├── TextBoxDrama.StartTextDisplay() - 文字顯示
        ├── ActorManagerDrama.SetupActors() - 角色演出
        ├── CGDisplay.DisplayScene() - CG展示 (如需要)
        └── StoryAudioManager.PlayAudio() - 音效播放
        ↓
    👆 等待玩家輸入
        ├── 點擊或按鍵繼續
        ├── 選擇分支 (如有)
        └── 跳過動畫效果
        ↓
    🔀 選擇處理 (如有選擇分支)
        ├── ChoiceSystemManager.ShowChoices()
        ├── 等待玩家選擇
        ├── ProcessChoiceResult()
        └── ApplyChoiceConsequences()
        ↓
    📈 劇情進度更新
        ├── StoryProgressTracker.UpdateProgress()
        ├── 設定事件標記
        ├── 檢查解鎖內容
        └── 觸發進度更新事件
        ↓
    🖼️ CG解鎖檢查
        ├── CGUnlockManager.ProcessEventUnlock()
        ├── 檢查劇情完成事件
        ├── 解鎖相應CG內容
        └── 觸發解鎖通知事件
        ↓
    💾 自動存檔
        ├── SaveSystemIntegration.AutoSave()
        ├── 保存劇情進度
        ├── 保存數值狀態
        └── 保存時間狀態
        ↓
    🔄 劇情結束處理
        ├── 清理劇情環境
        ├── 檢查後續劇情
        └── 返回養成模式
        ↓
    🔄 返回主遊戲循環
```

---

## 📡 事件總線通信架構

### 🚌 EventBus 核心事件系統

```csharp
// 事件總線核心實現
public class EventBus : MonoBehaviour
{
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
                    GameObject eventBusObject = new GameObject("EventBus");
                    _instance = eventBusObject.AddComponent<EventBus>();
                    DontDestroyOnLoad(eventBusObject);
                }
            }
            return _instance;
        }
    }
    
    // 事件訂閱字典
    private Dictionary<string, List<System.Action<object>>> eventDictionary;
    
    // 🚀 初始化事件總線
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            eventDictionary = new Dictionary<string, List<System.Action<object>>>();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    // 📡 訂閱事件
    public void Subscribe<T>(string eventType, System.Action<T> listener)
    {
        if (!eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = new List<System.Action<object>>();
        }
        
        eventDictionary[eventType].Add((data) => listener((T)data));
    }
    
    // 📤 發布事件
    public void Publish<T>(string eventType, T eventData)
    {
        if (eventDictionary.ContainsKey(eventType))
        {
            foreach (var listener in eventDictionary[eventType])
            {
                try
                {
                    listener(eventData);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"事件處理錯誤 [{eventType}]: {e.Message}");
                }
            }
        }
        
        // 記錄事件日誌
        LogEvent(eventType, eventData);
    }
    
    // 📝 事件日誌記錄
    void LogEvent<T>(string eventType, T eventData)
    {
        if (Application.isEditor)
        {
            Debug.Log($"[EventBus] {eventType}: {JsonUtility.ToJson(eventData, true)}");
        }
    }
}
```

### 📋 核心事件定義

```csharp
// 數值變更事件
[System.Serializable]
public class StatChangedEvent
{
    public string statName;
    public int oldValue;
    public int newValue;
    public int changeAmount;
    public System.DateTime timestamp;
}

// 好感度等級提升事件
[System.Serializable]
public class FriendshipLevelUpEvent
{
    public int oldLevel;
    public int newLevel;
    public int currentFriendship;
    public string[] unlockedContent;
}

// 互動完成事件
[System.Serializable]
public class InteractionCompleteEvent
{
    public InteractionType interactionType;
    public InteractionResult result;
    public System.DateTime timestamp;
}

// 時間推進事件
[System.Serializable]
public class TimeAdvancedEvent
{
    public int oldTime;
    public int newTime;
    public int oldDay;
    public int newDay;
    public bool dayChanged;
}

// 新一天開始事件
[System.Serializable]
public class NewDayStartedEvent
{
    public int newDay;
    public int newWeek;
    public int totalDays;
    public List<string> todayEvents;
}

// 劇情觸發事件
[System.Serializable]
public class StoryTriggerEvent
{
    public string storyID;
    public string triggerReason;
    public Dictionary<string, object> triggerData;
}

// 劇情完成事件
[System.Serializable]
public class StoryCompleteEvent
{
    public string dialogID;
    public System.DateTime completedAt;
    public Dictionary<string, int> finalStats;
}

// 選擇完成事件
[System.Serializable]
public class ChoiceMadeEvent
{
    public string dialogID;
    public int choiceIndex;
    public string choiceText;
    public Dictionary<string, int> consequences;
}

// CG解鎖事件
[System.Serializable]
public class CGUnlockedEvent
{
    public string cgID;
    public string eventID;
    public string unlockReason;
}
```

---

## 🔄 系統間數據同步機制

### 🎭 劇情到養成模式的數據傳遞

```csharp
public class StoryToNurturingSync : MonoBehaviour
{
    [Header("系統引用")]
    public NumericalRecords numericalRecords;
    public TimeManagerTest timeManager;
    public StoryProgressTracker progressTracker;
    
    void Start()
    {
        // 訂閱劇情系統事件
        EventBus.Instance.Subscribe<StoryCompleteEvent>("StoryComplete", OnStoryComplete);
        EventBus.Instance.Subscribe<ChoiceMadeEvent>("ChoiceMade", OnChoiceMade);
    }
    
    // 📚 劇情完成處理
    void OnStoryComplete(StoryCompleteEvent eventData)
    {
        // 1. 同步數值變化
        SyncStatsFromStory(eventData);
        
        // 2. 更新時間狀態
        SyncTimeFromStory(eventData);
        
        // 3. 更新進度記錄
        SyncProgressFromStory(eventData);
        
        // 4. 觸發養成模式準備事件
        TriggerNurturingModeReady();
    }
    
    // 📊 同步數值從劇情
    void SyncStatsFromStory(StoryCompleteEvent eventData)
    {
        foreach (var stat in eventData.finalStats)
        {
            numericalRecords.ModifyStat(stat.Key, stat.Value);
        }
    }
    
    // ⏰ 同步時間從劇情
    void SyncTimeFromStory(StoryCompleteEvent eventData)
    {
        // 劇情可能消耗時间
        timeManager.AdvanceTime(1); // 大型劇情消耗1個時間單位
    }
    
    // 🎯 觸發養成模式準備
    void TriggerNurturingModeReady()
    {
        EventBus.Instance.Publish("NurturingModeReady", new NurturingModeReadyEvent
        {
            previousMode = "Story",
            timestamp = System.DateTime.Now
        });
    }
}
```

### 💖 養成到劇情模式的數據傳遞

```csharp
public class NurturingToStorySync : MonoBehaviour
{
    [Header("觸發條件")]
    public StoryTriggerConfig triggerConfig;
    
    void Start()
    {
        // 訂閱養成系統事件
        EventBus.Instance.Subscribe<FriendshipLevelUpEvent>("FriendshipLevelUp", OnFriendshipLevelUp);
        EventBus.Instance.Subscribe<StatChangedEvent>("StatChanged", OnStatChanged);
        EventBus.Instance.Subscribe<NewDayStartedEvent>("NewDayStarted", OnNewDayStarted);
    }
    
    // 💝 好感度等級提升處理
    void OnFriendshipLevelUp(FriendshipLevelUpEvent eventData)
    {
        // 檢查是否觸發新劇情
        CheckStoryTriggers("friendship_level_up", eventData);
    }
    
    // 📊 數值變更處理
    void OnStatChanged(StatChangedEvent eventData)
    {
        // 檢查特殊數值觸發
        CheckStoryTriggers("stat_changed", eventData);
    }
    
    // 🌅 新一天開始處理
    void OnNewDayStarted(NewDayStartedEvent eventData)
    {
        // 檢查時間觸發的劇情
        CheckStoryTriggers("new_day", eventData);
    }
    
    // 🎭 檢查劇情觸發
    void CheckStoryTriggers(string triggerType, object eventData)
    {
        var triggeredStories = triggerConfig.GetTriggeredStories(triggerType, eventData);
        
        foreach (var storyTrigger in triggeredStories)
        {
            if (CanTriggerStory(storyTrigger))
            {
                TriggerStory(storyTrigger);
            }
        }
    }
    
    // ✅ 檢查是否可以觸發劇情
    bool CanTriggerStory(StoryTriggerData storyTrigger)
    {
        // 檢查前置條件
        foreach (var condition in storyTrigger.prerequisites)
        {
            if (!EvaluateCondition(condition))
            {
                return false;
            }
        }
        
        return true;
    }
    
    // 🎬 觸發劇情
    void TriggerStory(StoryTriggerData storyTrigger)
    {
        EventBus.Instance.Publish("TriggerStory", new StoryTriggerEvent
        {
            storyID = storyTrigger.storyID,
            triggerReason = storyTrigger.triggerReason,
            triggerData = CreateTriggerData(storyTrigger)
        });
    }
}
```

---

## 💾 存檔系統數據同步

### 📁 統一存檔管理

```csharp
public class UnifiedSaveManager : MonoBehaviour
{
    [Header("存檔組件")]
    public SaveDataManager coreManager;
    
    [Header("數據源")]
    public NumericalRecords numericalRecords;
    public TimeManagerTest timeManager;
    public StoryProgressTracker storyTracker;
    
    void Start()
    {
        // 訂閱需要存檔的事件
        EventBus.Instance.Subscribe<StoryCompleteEvent>("StoryComplete", OnAutoSave);
        EventBus.Instance.Subscribe<FriendshipLevelUpEvent>("FriendshipLevelUp", OnAutoSave);
        EventBus.Instance.Subscribe<NewDayStartedEvent>("NewDayStarted", OnAutoSave);
    }
    
    // 💾 自動存檔觸發
    void OnAutoSave<T>(T eventData)
    {
        StartCoroutine(PerformAutoSave());
    }
    
    // 🔄 執行自動存檔
    IEnumerator PerformAutoSave()
    {
        // 收集所有系統數據
        var saveData = new UnifiedSaveData
        {
            // 數值數據
            numericalData = CreateNumericalSnapshot(),
            
            // 時間數據
            timeData = CreateTimeSnapshot(),
            
            // 劇情進度數據
            storyData = CreateStorySnapshot(),
            
            // 元數據
            metadata = CreateMetadataSnapshot()
        };
        
        // 異步保存
        yield return StartCoroutine(coreManager.SaveDataAsync(saveData));
        
        // 觸發存檔完成事件
        EventBus.Instance.Publish("SaveComplete", new SaveCompleteEvent
        {
            saveData = saveData,
            timestamp = System.DateTime.Now
        });
    }
    
    // 📊 創建數值快照
    NumericalSnapshot CreateNumericalSnapshot()
    {
        return new NumericalSnapshot
        {
            friendship = numericalRecords.friendship,
            slutty = numericalRecords.slutty,
            lust = numericalRecords.lust,
            money = numericalRecords.money,
            energy = numericalRecords.energy,
            playerName = numericalRecords.playerName,
            playerLevel = numericalRecords.playerLevel,
            experience = numericalRecords.experience,
            totalInteractions = numericalRecords.totalInteractions,
            interactionStats = numericalRecords.GetInteractionStats()
        };
    }
    
    // ⏰ 創建時間快照
    TimeSnapshot CreateTimeSnapshot()
    {
        return new TimeSnapshot
        {
            currentDay = timeManager.aDay,
            currentWeek = timeManager.aWeek,
            currentTime = timeManager.aTimer,
            totalDays = timeManager.totalDays,
            dailyEvents = timeManager.dailyEvents,
            scheduledEvents = timeManager.scheduledEvents
        };
    }
    
    // 📚 創建劇情快照
    StorySnapshot CreateStorySnapshot()
    {
        return new StorySnapshot
        {
            storyProgress = storyTracker.progressData.storyProgress,
            eventFlags = storyTracker.progressData.eventFlags,
            completedStories = storyTracker.GetCompletedStories(),
            unlockedContent = storyTracker.GetUnlockedContent()
        };
    }
}
```

---

## 🎯 性能優化與數據管理

### ⚡ 數據流優化策略

```csharp
public class DataFlowOptimizer : MonoBehaviour
{
    [Header("緩存配置")]
    public int maxCacheSize = 100;
    public float cacheExpiryTime = 300f; // 5分鐘過期
    
    [Header("批處理配置")]
    public int batchSize = 10;
    public float batchInterval = 0.1f;
    
    // 事件批處理隊列
    private Queue<EventData> eventQueue;
    private Dictionary<string, CachedData> dataCache;
    private Coroutine batchProcessor;
    
    void Start()
    {
        InitializeOptimizer();
    }
    
    // 🚀 初始化優化器
    void InitializeOptimizer()
    {
        eventQueue = new Queue<EventData>();
        dataCache = new Dictionary<string, CachedData>();
        
        // 開始批處理協程
        batchProcessor = StartCoroutine(ProcessEventBatch());
        
        // 開始緩存清理協程
        StartCoroutine(CleanupCache());
    }
    
    // 📦 批處理事件
    IEnumerator ProcessEventBatch()
    {
        while (true)
        {
            yield return new WaitForSeconds(batchInterval);
            
            if (eventQueue.Count > 0)
            {
                var batchEvents = new List<EventData>();
                
                // 收集批次事件
                for (int i = 0; i < batchSize && eventQueue.Count > 0; i++)
                {
                    batchEvents.Add(eventQueue.Dequeue());
                }
                
                // 批量處理事件
                ProcessEventsBatch(batchEvents);
            }
        }
    }
    
    // 🔄 處理批次事件
    void ProcessEventsBatch(List<EventData> events)
    {
        // 按類型分組事件
        var groupedEvents = events.GroupBy(e => e.eventType).ToList();
        
        foreach (var group in groupedEvents)
        {
            // 批量處理同類型事件
            ProcessEventGroup(group.Key, group.ToList());
        }
    }
    
    // 🧹 清理過期緩存
    IEnumerator CleanupCache()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f); // 每分鐘清理一次
            
            var expiredKeys = new List<string>();
            
            foreach (var kvp in dataCache)
            {
                if (Time.time - kvp.Value.cacheTime > cacheExpiryTime)
                {
                    expiredKeys.Add(kvp.Key);
                }
            }
            
            foreach (var key in expiredKeys)
            {
                dataCache.Remove(key);
            }
            
            // 清理事件日誌
            CleanupEventLogs();
        }
    }
}
```

---

## 📊 數據流監控與調試

### 🔍 數據流監控器

```csharp
public class DataFlowMonitor : MonoBehaviour
{
    [Header("監控配置")]
    public bool enableMonitoring = true;
    public bool enableDetailedLogging = false;
    public int maxLogEntries = 1000;
    
    [Header("性能監控")]
    public float performanceThreshold = 0.1f; // 100ms閾值
    
    // 監控數據
    private List<DataFlowLogEntry> flowLogs;
    private Dictionary<string, PerformanceMetrics> systemMetrics;
    
    void Start()
    {
        if (enableMonitoring)
        {
            InitializeMonitoring();
        }
    }
    
    // 🚀 初始化監控
    void InitializeMonitoring()
    {
        flowLogs = new List<DataFlowLogEntry>();
        systemMetrics = new Dictionary<string, PerformanceMetrics>();
        
        // 訂閱所有系統事件進行監控
        SubscribeToAllEvents();
        
        // 開始性能監控
        StartCoroutine(MonitorPerformance());
    }
    
    // 📡 訂閱所有事件
    void SubscribeToAllEvents()
    {
        EventBus.Instance.Subscribe<StatChangedEvent>("StatChanged", (data) => LogDataFlow("StatChanged", data));
        EventBus.Instance.Subscribe<InteractionCompleteEvent>("InteractionComplete", (data) => LogDataFlow("InteractionComplete", data));
        EventBus.Instance.Subscribe<StoryTriggerEvent>("TriggerStory", (data) => LogDataFlow("TriggerStory", data));
        EventBus.Instance.Subscribe<StoryCompleteEvent>("StoryComplete", (data) => LogDataFlow("StoryComplete", data));
        EventBus.Instance.Subscribe<TimeAdvancedEvent>("TimeAdvanced", (data) => LogDataFlow("TimeAdvanced", data));
    }
    
    // 📝 記錄數據流
    void LogDataFlow(string eventType, object eventData)
    {
        var logEntry = new DataFlowLogEntry
        {
            timestamp = System.DateTime.Now,
            eventType = eventType,
            eventData = JsonUtility.ToJson(eventData),
            systemState = CaptureSystemState()
        };
        
        flowLogs.Add(logEntry);
        
        // 維護日誌大小
        if (flowLogs.Count > maxLogEntries)
        {
            flowLogs.RemoveAt(0);
        }
        
        // 詳細日誌輸出
        if (enableDetailedLogging)
        {
            Debug.Log($"[DataFlow] {eventType}: {logEntry.eventData}");
        }
    }
    
    // 📊 捕獲系統狀態
    SystemStateSnapshot CaptureSystemState()
    {
        return new SystemStateSnapshot
        {
            memoryUsage = System.GC.GetTotalMemory(false),
            frameRate = 1.0f / Time.unscaledDeltaTime,
            activeEventCount = EventBus.Instance.GetActiveEventCount(),
            cacheSize = GetCurrentCacheSize()
        };
    }
    
    // ⚡ 性能監控
    IEnumerator MonitorPerformance()
    {
        while (enableMonitoring)
        {
            yield return new WaitForSeconds(1f);
            
            // 檢查各系統性能
            CheckSystemPerformance();
            
            // 檢查內存使用
            CheckMemoryUsage();
            
            // 檢查事件處理延遲
            CheckEventProcessingDelay();
        }
    }
    
    // 🎯 獲取數據流統計
    public DataFlowStatistics GetDataFlowStatistics()
    {
        return new DataFlowStatistics
        {
            totalEvents = flowLogs.Count,
            eventTypeBreakdown = GetEventTypeBreakdown(),
            averageProcessingTime = GetAverageProcessingTime(),
            peakMemoryUsage = GetPeakMemoryUsage(),
            systemHealth = EvaluateSystemHealth()
        };
    }
}
```

---

## 💬 Claude 使用提示

### 🎯 數據流架構重點
1. **事件驅動**: 使用EventBus實現系統間解耦通信
2. **異步處理**: 批處理和異步操作提高性能
3. **數據同步**: 確保各系統數據的一致性
4. **監控調試**: 完整的數據流監控和調試機制

### 🔧 開發建議
- 優先實作EventBus核心系統
- 確保事件處理的異常安全性
- 注重數據同步的時序問題
- 考慮大量事件的性能影響

### ⚠️ 注意事項
- 事件循環依賴的預防
- 數據一致性的保證機制
- 內存泄漏的監控和預防
- 異步操作的錯誤處理

---

**最後更新**: 2025-07-30  
**版本**: 1.0  
**維護者**: 開發團隊 + Claude AI

> 🌊 **架構亮點**: CoreSystems數據流架構採用事件驅動和批處理優化設計，通過統一的事件總線和智能的數據同步機制，實現了劇情播放系統與養成互動系統的無縫整合。系統不僅保證了數據的一致性，還提供了完整的監控和調試功能！ ✨