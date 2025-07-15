# 💾 模組名稱：SaveDataManager

> 存檔管理器，負責遊戲數據的儲存、載入和管理

---

## 🔖 模組功能

SaveDataManager 負責處理遊戲的所有存檔相關功能，包括數據序列化、檔案管理、版本兼容性檢查和備份機制。

---

## 📍 檔案位置

**路徑**: `Assets/Scripts/Core/SaveDataManager.cs`  
**命名空間**: `LoveTide.Core`  
**繼承**: `MonoBehaviour`

---

## 🧩 公開方法一覽

| 方法名稱 | 功能描述 | 參數 | 回傳值 |
|----------|----------|------|---------|
| `SaveGame(SaveData data)` | 儲存遊戲數據 | SaveData | bool |
| `LoadGame(int slotIndex)` | 載入指定插槽的遊戲 | int | SaveData |
| `LoadGame(string fileName)` | 載入指定檔案的遊戲 | string | SaveData |
| `DeleteSave(int slotIndex)` | 刪除指定插槽的存檔 | int | bool |
| `GetSaveSlots()` | 取得所有存檔插槽資訊 | 無 | SaveSlotInfo[] |
| `HasSaveData(int slotIndex)` | 檢查插槽是否有存檔 | int | bool |
| `CreateBackup(int slotIndex)` | 創建存檔備份 | int | bool |
| `RestoreBackup(int slotIndex)` | 恢復存檔備份 | int | bool |
| `AutoSave()` | 自動存檔 | 無 | bool |
| `GetSaveFileSize(int slotIndex)` | 取得存檔檔案大小 | int | long |

---

## 🎯 主要屬性

### 📁 存檔設定
```csharp
[Header("Save Settings")]
public int maxSaveSlots = 10;                   // 最大存檔插槽數
public string saveFileName = "save_slot_{0}.json";  // 存檔檔案名格式
public string autoSaveFileName = "autosave.json";   // 自動存檔檔案名
public bool enableAutoSave = true;                  // 啟用自動存檔
public float autoSaveInterval = 300f;               // 自動存檔間隔(秒)
public bool enableBackup = true;                   // 啟用備份機制
public int maxBackupCount = 3;                     // 最大備份數量
```

### 🔐 安全設定
```csharp
[Header("Security Settings")]
public bool enableEncryption = true;               // 啟用加密
public string encryptionKey = "LoveTide2025";      // 加密金鑰
public bool enableChecksum = true;                 // 啟用校驗和
public bool compressData = true;                   // 壓縮數據
```

### 📊 存檔統計
```csharp
[Header("Save Statistics")]
public int totalSaveCount = 0;                     // 總存檔次數
public int totalLoadCount = 0;                     // 總載入次數
public DateTime lastSaveTime;                      // 最後存檔時間
public DateTime lastLoadTime;                      // 最後載入時間
```

---

## 📊 存檔數據結構

### 💾 主要存檔結構
```csharp
[System.Serializable]
public class SaveData
{
    [Header("Save Info")]
    public string saveName = "Save Game";          // 存檔名稱
    public DateTime saveTime;                      // 存檔時間
    public string gameVersion = "1.0.0";           // 遊戲版本
    public int saveSlot = 0;                       // 存檔插槽
    public float playtime = 0f;                    // 遊戲時間
    
    [Header("Game Progress")]
    public int currentDay = 1;                     // 當前日期
    public string currentScene = "MainRoom";        // 當前場景
    public GameState gameState = GameState.Playing; // 遊戲狀態
    public int storyProgress = 0;                  // 劇情進度
    
    [Header("Player Data")]
    public int playerMoney = 1000;                 // 玩家金錢
    public int playerLevel = 1;                    // 玩家等級
    public Dictionary<string, int> playerStats;    // 玩家統計
    public Dictionary<string, bool> unlockedContent; // 解鎖內容
    
    [Header("Character Data")]
    public Dictionary<string, CharacterSaveData> characters; // 角色數據
    
    [Header("Settings")]
    public GameSettings gameSettings;              // 遊戲設定
}
```

### 👥 角色存檔數據
```csharp
[System.Serializable]
public class CharacterSaveData
{
    public string characterId;                     // 角色ID
    public int affectionLevel = 0;                 // 好感度等級
    public float affectionValue = 0f;              // 好感度數值
    public List<string> completedEvents;           // 完成的事件
    public List<string> unlockedDialogs;           // 解鎖的對話
    public DateTime lastInteraction;               // 最後互動時間
    public Dictionary<string, object> customData;  // 自定義數據
}
```

### 📋 存檔插槽資訊
```csharp
[System.Serializable]
public class SaveSlotInfo
{
    public int slotIndex;                          // 插槽索引
    public bool hasData;                           // 是否有數據
    public string saveName;                        // 存檔名稱
    public DateTime saveTime;                      // 存檔時間
    public string gameVersion;                     // 遊戲版本
    public float playtime;                         // 遊戲時間
    public int currentDay;                         // 當前日期
    public string currentScene;                    // 當前場景
    public long fileSize;                          // 檔案大小
    public Sprite screenshot;                      // 截圖
}
```

---

## 💾 存檔系統

### 🎯 存檔流程
```csharp
public bool SaveGame(SaveData data)
{
    try
    {
        // 1. 驗證存檔數據
        if (!ValidateSaveData(data))
        {
            Debug.LogError("Save data validation failed");
            return false;
        }
        
        // 2. 更新存檔資訊
        data.saveTime = DateTime.Now;
        data.gameVersion = Application.version;
        
        // 3. 序列化數據
        string jsonData = JsonUtility.ToJson(data, true);
        
        // 4. 壓縮數據
        if (compressData)
        {
            jsonData = CompressString(jsonData);
        }
        
        // 5. 加密數據
        if (enableEncryption)
        {
            jsonData = EncryptString(jsonData);
        }
        
        // 6. 生成校驗和
        string checksum = "";
        if (enableChecksum)
        {
            checksum = GenerateChecksum(jsonData);
        }
        
        // 7. 寫入檔案
        string fileName = string.Format(saveFileName, data.saveSlot);
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        
        var saveFile = new SaveFile
        {
            data = jsonData,
            checksum = checksum,
            version = Application.version,
            timestamp = DateTime.Now.ToBinary()
        };
        
        string saveFileJson = JsonUtility.ToJson(saveFile);
        File.WriteAllText(filePath, saveFileJson);
        
        // 8. 創建備份
        if (enableBackup)
        {
            CreateBackup(data.saveSlot);
        }
        
        // 9. 更新統計
        totalSaveCount++;
        lastSaveTime = DateTime.Now;
        
        // 10. 觸發存檔事件
        EventBus.Instance.Publish("GameSaved", data);
        
        Debug.Log($"Game saved successfully to slot {data.saveSlot}");
        return true;
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Save game failed: {e.Message}");
        return false;
    }
}
```

### 📂 載入流程
```csharp
public SaveData LoadGame(int slotIndex)
{
    try
    {
        // 1. 檢查存檔是否存在
        if (!HasSaveData(slotIndex))
        {
            Debug.LogWarning($"No save data found in slot {slotIndex}");
            return null;
        }
        
        // 2. 讀取檔案
        string fileName = string.Format(saveFileName, slotIndex);
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string fileContent = File.ReadAllText(filePath);
        
        // 3. 解析存檔檔案
        var saveFile = JsonUtility.FromJson<SaveFile>(fileContent);
        
        // 4. 驗證校驗和
        if (enableChecksum && !VerifyChecksum(saveFile.data, saveFile.checksum))
        {
            Debug.LogError("Save file checksum verification failed");
            return null;
        }
        
        // 5. 解密數據
        string jsonData = saveFile.data;
        if (enableEncryption)
        {
            jsonData = DecryptString(jsonData);
        }
        
        // 6. 解壓縮數據
        if (compressData)
        {
            jsonData = DecompressString(jsonData);
        }
        
        // 7. 反序列化數據
        var saveData = JsonUtility.FromJson<SaveData>(jsonData);
        
        // 8. 版本兼容性檢查
        if (!IsVersionCompatible(saveData.gameVersion))
        {
            saveData = MigrateSaveData(saveData);
        }
        
        // 9. 更新統計
        totalLoadCount++;
        lastLoadTime = DateTime.Now;
        
        // 10. 觸發載入事件
        EventBus.Instance.Publish("GameLoaded", saveData);
        
        Debug.Log($"Game loaded successfully from slot {slotIndex}");
        return saveData;
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Load game failed: {e.Message}");
        return null;
    }
}
```

---

## 🔐 數據安全

### 🔒 加密系統
```csharp
private string EncryptString(string plainText)
{
    try
    {
        byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(encryptionKey);
        
        // 簡單的XOR加密
        byte[] encryptedBytes = new byte[plainBytes.Length];
        for (int i = 0; i < plainBytes.Length; i++)
        {
            encryptedBytes[i] = (byte)(plainBytes[i] ^ keyBytes[i % keyBytes.Length]);
        }
        
        return System.Convert.ToBase64String(encryptedBytes);
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Encryption failed: {e.Message}");
        return plainText;
    }
}

private string DecryptString(string encryptedText)
{
    try
    {
        byte[] encryptedBytes = System.Convert.FromBase64String(encryptedText);
        byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(encryptionKey);
        
        // 簡單的XOR解密
        byte[] decryptedBytes = new byte[encryptedBytes.Length];
        for (int i = 0; i < encryptedBytes.Length; i++)
        {
            decryptedBytes[i] = (byte)(encryptedBytes[i] ^ keyBytes[i % keyBytes.Length]);
        }
        
        return System.Text.Encoding.UTF8.GetString(decryptedBytes);
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Decryption failed: {e.Message}");
        return encryptedText;
    }
}
```

### 🔍 校驗和系統
```csharp
private string GenerateChecksum(string data)
{
    using (var md5 = System.Security.Cryptography.MD5.Create())
    {
        byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
        byte[] hashBytes = md5.ComputeHash(dataBytes);
        return System.Convert.ToBase64String(hashBytes);
    }
}

private bool VerifyChecksum(string data, string checksum)
{
    string calculatedChecksum = GenerateChecksum(data);
    return calculatedChecksum == checksum;
}
```

---

## 🗂️ 備份系統

### 💾 創建備份
```csharp
public bool CreateBackup(int slotIndex)
{
    try
    {
        string fileName = string.Format(saveFileName, slotIndex);
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"No save file to backup in slot {slotIndex}");
            return false;
        }
        
        // 創建備份目錄
        string backupDir = Path.Combine(Application.persistentDataPath, "backups");
        if (!Directory.Exists(backupDir))
        {
            Directory.CreateDirectory(backupDir);
        }
        
        // 生成備份檔案名
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string backupFileName = $"backup_slot_{slotIndex}_{timestamp}.json";
        string backupFilePath = Path.Combine(backupDir, backupFileName);
        
        // 複製檔案
        File.Copy(filePath, backupFilePath);
        
        // 清理舊備份
        CleanupOldBackups(slotIndex);
        
        Debug.Log($"Backup created: {backupFileName}");
        return true;
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Backup creation failed: {e.Message}");
        return false;
    }
}
```

### 🔄 清理舊備份
```csharp
private void CleanupOldBackups(int slotIndex)
{
    try
    {
        string backupDir = Path.Combine(Application.persistentDataPath, "backups");
        if (!Directory.Exists(backupDir))
            return;
        
        string pattern = $"backup_slot_{slotIndex}_*.json";
        var backupFiles = Directory.GetFiles(backupDir, pattern)
                                 .OrderByDescending(f => File.GetCreationTime(f))
                                 .Skip(maxBackupCount)
                                 .ToArray();
        
        foreach (string file in backupFiles)
        {
            File.Delete(file);
            Debug.Log($"Deleted old backup: {Path.GetFileName(file)}");
        }
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Backup cleanup failed: {e.Message}");
    }
}
```

---

## 🔄 版本遷移

### 📈 版本兼容性
```csharp
private bool IsVersionCompatible(string saveVersion)
{
    var currentVersion = new System.Version(Application.version);
    var saveVer = new System.Version(saveVersion);
    
    // 主版本不同則不兼容
    if (currentVersion.Major != saveVer.Major)
    {
        return false;
    }
    
    // 次版本向後兼容
    return currentVersion.Minor >= saveVer.Minor;
}

private SaveData MigrateSaveData(SaveData oldData)
{
    Debug.Log($"Migrating save data from version {oldData.gameVersion} to {Application.version}");
    
    // 根據版本進行數據遷移
    var currentVersion = new System.Version(Application.version);
    var saveVersion = new System.Version(oldData.gameVersion);
    
    if (saveVersion < new System.Version("1.1.0"))
    {
        // 從1.0.x遷移到1.1.0
        MigrateFrom1_0_To1_1(oldData);
    }
    
    if (saveVersion < new System.Version("1.2.0"))
    {
        // 從1.1.x遷移到1.2.0
        MigrateFrom1_1_To1_2(oldData);
    }
    
    // 更新版本號
    oldData.gameVersion = Application.version;
    
    return oldData;
}
```

---

## 🔧 自動存檔系統

### ⏰ 自動存檔
```csharp
public bool AutoSave()
{
    if (!enableAutoSave)
        return false;
    
    try
    {
        // 收集當前遊戲數據
        var saveData = CollectCurrentGameData();
        saveData.saveSlot = -1; // 自動存檔使用特殊插槽
        saveData.saveName = "Auto Save";
        
        // 存檔到自動存檔檔案
        string filePath = Path.Combine(Application.persistentDataPath, autoSaveFileName);
        string jsonData = JsonUtility.ToJson(saveData, true);
        
        if (enableEncryption)
        {
            jsonData = EncryptString(jsonData);
        }
        
        File.WriteAllText(filePath, jsonData);
        
        Debug.Log("Auto save completed");
        EventBus.Instance.Publish("AutoSaved", saveData);
        
        return true;
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Auto save failed: {e.Message}");
        return false;
    }
}
```

### 🔄 自動存檔計時器
```csharp
private float autoSaveTimer = 0f;

private void Update()
{
    if (enableAutoSave)
    {
        autoSaveTimer += Time.deltaTime;
        
        if (autoSaveTimer >= autoSaveInterval)
        {
            AutoSave();
            autoSaveTimer = 0f;
        }
    }
}
```

---

## 🔁 呼叫關係

### 📊 被呼叫情況
- **GameManagerTest**: 遊戲存檔和載入
- **UI系統**: 存檔選單操作
- **自動存檔**: 定時自動存檔
- **事件系統**: 特定事件觸發存檔

### 🎯 呼叫對象
- **EventBus**: 發布存檔相關事件
- **File System**: 檔案讀寫操作
- **JsonUtility**: JSON序列化

---

## 💬 Claude 使用提示

使用 SaveDataManager 時請注意：
1. **先閱讀 `Architecture/數據流架構.md`** 了解數據管理架構
2. **版本兼容性** 新增數據欄位時考慮向後兼容
3. **錯誤處理** 存檔載入操作都要有適當的錯誤處理
4. **安全考量** 重要數據應該加密和備份
5. **性能考量** 大量數據存檔時考慮異步操作
6. **測試覆蓋** 充分測試各種存檔載入情況

常見修改場景：
- 新增存檔數據欄位
- 修改存檔格式
- 優化存檔性能
- 添加新的備份策略
- 實現雲端存檔功能