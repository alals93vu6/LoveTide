# 🎮 模組名稱：GameManagerTest

> 主遊戲邏輯控制器，負責統籌整個遊戲流程和系統協調

---

## 🔖 模組功能

GameManagerTest 是遊戲的核心控制器，負責管理遊戲狀態、場景切換、系統初始化、以及各子系統間的協調工作。

---

## 📍 檔案位置

**路徑**: `Assets/Scripts/GamePlay/GameManagerTest.cs`  
**命名空間**: `LoveTide.GamePlay`  
**繼承**: `MonoBehaviour`

---

## 🧩 公開方法一覽

| 方法名稱 | 功能描述 | 參數 | 回傳值 |
|----------|----------|------|---------|
| `Initialize()` | 初始化遊戲系統 | 無 | void |
| `StartNewGame()` | 開始新遊戲 | 無 | void |
| `LoadGame(SaveData data)` | 載入遊戲存檔 | SaveData | bool |
| `SaveGame()` | 儲存遊戲進度 | 無 | bool |
| `ChangeScene(string sceneName)` | 切換遊戲場景 | string | void |
| `PauseGame()` | 暫停遊戲 | 無 | void |
| `ResumeGame()` | 恢復遊戲 | 無 | void |
| `SetGameState(GameState state)` | 設定遊戲狀態 | GameState | void |
| `GetCurrentTime()` | 取得當前遊戲時間 | 無 | GameTime |
| `TriggerEvent(string eventName)` | 觸發遊戲事件 | string | void |

---

## 🎯 主要屬性

### 📊 遊戲狀態管理
```csharp
public GameState currentGameState;          // 當前遊戲狀態
public string currentSceneName;             // 當前場景名稱
public bool isGamePaused;                   // 遊戲是否暫停
public float gameSpeed = 1.0f;              // 遊戲速度倍率
```

### 🎮 系統管理器引用
```csharp
[Header("System Managers")]
public TimeManagerTest timeManager;         // 時間管理器
public NumericalRecords numericalRecords;   // 數值記錄系統
public GameUICtrlmanager uiManager;         // UI管理器
public SaveDataManager saveManager;         // 存檔管理器
public bgmManager audioManager;             // 音效管理器
```

### 🎭 場景控制
```csharp
[Header("Scene Control")]
public BackgroundCtrl backgroundController; // 背景控制器
public GamePlayingManagerDrama dramaManager; // 劇情管理器
public string defaultSceneName = "MainRoom"; // 預設場景名稱
```

---

## 🔄 核心流程控制

### 🚀 遊戲初始化流程
```csharp
public void Initialize()
{
    // 1. 初始化子系統
    InitializeSubSystems();
    
    // 2. 載入遊戲設定
    LoadGameSettings();
    
    // 3. 設定初始狀態
    SetInitialGameState();
    
    // 4. 註冊事件監聽
    RegisterEventHandlers();
    
    // 5. 啟動遊戲循環
    StartGameLoop();
}
```

### 🎯 遊戲狀態機
```csharp
public enum GameState
{
    MainMenu,       // 主選單
    Loading,        // 載入中
    Playing,        // 遊戲中
    Dialog,         // 對話中
    MiniGame,       // 小遊戲
    Settings,       // 設定選單
    Paused,         // 暫停
    Saving          // 存檔中
}
```

---

## 🎭 場景管理

### 🏠 場景切換邏輯
```csharp
public void ChangeScene(string sceneName)
{
    // 1. 觸發場景切換事件
    EventBus.Instance.Publish("SceneChangeStart", sceneName);
    
    // 2. 保存當前場景狀態
    SaveCurrentSceneState();
    
    // 3. 切換背景和音效
    backgroundController.ChangeBackground(sceneName);
    audioManager.PlaySceneBGM(sceneName);
    
    // 4. 更新UI狀態
    uiManager.UpdateSceneUI(sceneName);
    
    // 5. 觸發場景切換完成事件
    EventBus.Instance.Publish("SceneChangeComplete", sceneName);
}
```

### 📋 場景狀態記錄
```csharp
[System.Serializable]
public class SceneState
{
    public string sceneName;
    public Vector3 playerPosition;
    public Dictionary<string, bool> objectStates;
    public float sceneTime;
    public string currentBGM;
}
```

---

## ⏰ 時間系統整合

### 📅 時間管理
```csharp
public void UpdateGameTime()
{
    if (!isGamePaused && currentGameState == GameState.Playing)
    {
        timeManager.UpdateTime();
        
        // 檢查時間觸發事件
        CheckTimeEvents();
        
        // 更新UI時間顯示
        uiManager.UpdateTimeDisplay(timeManager.currentTime);
    }
}
```

### 🎯 時間事件處理
```csharp
private void CheckTimeEvents()
{
    var currentTime = timeManager.currentTime;
    
    // 檢查日期變化
    if (timeManager.hasDateChanged)
    {
        OnDateChanged(currentTime.day);
    }
    
    // 檢查時段變化
    if (timeManager.hasTimeOfDayChanged)
    {
        OnTimeOfDayChanged(currentTime.timeOfDay);
    }
}
```

---

## 💾 存檔系統整合

### 📁 存檔管理
```csharp
public bool SaveGame()
{
    try
    {
        var saveData = new SaveData
        {
            gameState = currentGameState,
            sceneName = currentSceneName,
            gameTime = timeManager.currentTime,
            playerData = numericalRecords.GetAllData(),
            sceneStates = GetAllSceneStates()
        };
        
        return saveManager.SaveGame(saveData);
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Save game failed: {e.Message}");
        return false;
    }
}
```

### 📂 載入遊戲
```csharp
public bool LoadGame(SaveData data)
{
    try
    {
        // 恢復遊戲狀態
        SetGameState(data.gameState);
        
        // 恢復場景
        ChangeScene(data.sceneName);
        
        // 恢復時間
        timeManager.LoadTime(data.gameTime);
        
        // 恢復玩家數據
        numericalRecords.LoadData(data.playerData);
        
        return true;
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Load game failed: {e.Message}");
        return false;
    }
}
```

---

## 🎮 小遊戲系統整合

### 🎣 小遊戲啟動
```csharp
public void StartMiniGame(MiniGameType gameType)
{
    // 1. 保存當前狀態
    SaveCurrentGameState();
    
    // 2. 切換到小遊戲狀態
    SetGameState(GameState.MiniGame);
    
    // 3. 載入小遊戲場景
    LoadMiniGameScene(gameType);
    
    // 4. 通知相關系統
    EventBus.Instance.Publish("MiniGameStart", gameType);
}
```

### 🏆 小遊戲結束
```csharp
public void EndMiniGame(MiniGameResult result)
{
    // 1. 處理遊戲結果
    ProcessMiniGameResult(result);
    
    // 2. 更新玩家數據
    numericalRecords.UpdateFromMiniGame(result);
    
    // 3. 恢復遊戲狀態
    RestoreGameState();
    
    // 4. 觸發結束事件
    EventBus.Instance.Publish("MiniGameEnd", result);
}
```

---

## 📡 事件系統整合

### 🔔 事件註冊
```csharp
private void RegisterEventHandlers()
{
    EventBus.Instance.Subscribe<DialogEvent>("DialogStart", OnDialogStart);
    EventBus.Instance.Subscribe<DialogEvent>("DialogEnd", OnDialogEnd);
    EventBus.Instance.Subscribe<AffectionEvent>("AffectionChanged", OnAffectionChanged);
    EventBus.Instance.Subscribe<TimeEvent>("TimeChanged", OnTimeChanged);
}
```

### 🎭 事件處理
```csharp
private void OnDialogStart(DialogEvent eventData)
{
    SetGameState(GameState.Dialog);
    PauseGameSystems();
}

private void OnDialogEnd(DialogEvent eventData)
{
    SetGameState(GameState.Playing);
    ResumeGameSystems();
}

private void OnAffectionChanged(AffectionEvent eventData)
{
    // 檢查是否觸發特殊事件
    CheckAffectionTriggers(eventData.characterId, eventData.newValue);
}
```

---

## 🛡 錯誤處理

### 🚨 異常捕獲
```csharp
public void HandleSystemError(string systemName, System.Exception error)
{
    Debug.LogError($"System error in {systemName}: {error.Message}");
    
    // 嘗試恢復系統
    if (TryRecoverSystem(systemName))
    {
        Debug.Log($"Successfully recovered system: {systemName}");
    }
    else
    {
        // 進入安全模式
        EnterSafeMode();
    }
}
```

### 🔧 系統恢復
```csharp
private bool TryRecoverSystem(string systemName)
{
    switch (systemName)
    {
        case "TimeManager":
            return timeManager.TryRecover();
        case "SaveManager":
            return saveManager.TryRecover();
        case "UIManager":
            return uiManager.TryRecover();
        default:
            return false;
    }
}
```

---

## 🔁 呼叫關係

### 📊 被呼叫情況
- **啟動時**: Unity Start() 方法自動呼叫
- **場景切換**: 背景控制器觸發
- **UI操作**: UI管理器觸發
- **時間事件**: 時間管理器觸發
- **存檔操作**: 存檔管理器觸發

### 🎯 呼叫對象
- **TimeManagerTest**: 時間更新和查詢
- **NumericalRecords**: 數據讀寫
- **GameUICtrlmanager**: UI狀態更新
- **SaveDataManager**: 存檔載入
- **EventBus**: 事件發布訂閱

---

## 🚀 性能優化

### 📊 優化要點
```csharp
// 避免每幀更新的操作
private float lastUpdateTime = 0f;
private const float UPDATE_INTERVAL = 0.1f;

private void Update()
{
    if (Time.time - lastUpdateTime > UPDATE_INTERVAL)
    {
        UpdateGameSystems();
        lastUpdateTime = Time.time;
    }
}
```

### 🔍 記憶體管理
- **事件取消訂閱**: OnDestroy 中取消所有事件訂閱
- **對象池**: 重用頻繁創建的對象
- **資源釋放**: 及時釋放不需要的資源

---

## 💬 Claude 使用提示

修改 GameManagerTest 時請注意：
1. **先閱讀 `Architecture/遊戲流程架構.md`** 了解整體設計
2. **確認系統依賴關係** 避免循環依賴
3. **測試狀態轉換** 確保狀態機邏輯正確
4. **檢查事件處理** 確保事件正確註冊和取消訂閱
5. **驗證錯誤處理** 測試異常情況的處理
6. **性能測試** 確保修改不影響遊戲性能

常見修改場景：
- 新增遊戲狀態
- 修改場景切換邏輯
- 整合新的子系統
- 優化遊戲性能
- 添加新的事件處理