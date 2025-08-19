using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 遊戲數據管理器
/// 
/// 職責:
/// 1. 管理遊戲數據的載入、保存、同步
/// 2. 整合現有的NumericalRecords系統
/// 3. 提供統一的數據訪問接口
/// 4. 處理數據驗證和錯誤恢復
/// 
/// 基於架構文檔: 存檔系統架構.md 和 數據流架構.md
/// </summary>
public class GameDataManager : MonoBehaviour
{
    [Header("== 數據系統引用 ==")]
    [SerializeField] private NumericalRecords numericalRecords;
    [SerializeField] private NumericalRecords_PlayerSetting playerSettings;
    
    [Header("== 數據管理配置 ==")]
    [SerializeField] private bool autoSave = true;
    [SerializeField] private float autoSaveInterval = 300f; // 5分鐘自動保存
    [SerializeField] private bool isInitialized = false;
    
    // 當前遊戲數據
    private GameData currentGameData;
    
    // 數據變更事件
    public System.Action<string, object> OnDataChanged;
    public System.Action OnDataSaved;
    public System.Action OnDataLoaded;
    
    // 自動保存計時器
    private float autoSaveTimer = 0f;
    
    public bool IsInitialized => isInitialized;
    public GameData CurrentGameData => currentGameData;
    
    /// <summary>
    /// 初始化遊戲數據管理器
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[GameDataManager] 初始化遊戲數據管理器");
        
        // 查找並綁定現有系統
        FindAndBindExistingSystems();
        
        // 初始化遊戲數據
        InitializeGameData();
        
        // 載入遊戲數據
        LoadGame();
        
        isInitialized = true;
        Debug.Log("[GameDataManager] 初始化完成");
    }
    
    void Update()
    {
        // 自動保存邏輯
        if (autoSave && isInitialized)
        {
            autoSaveTimer += Time.deltaTime;
            if (autoSaveTimer >= autoSaveInterval)
            {
                autoSaveTimer = 0f;
                SaveGame();
                Debug.Log("[GameDataManager] 執行自動保存");
            }
        }
    }
    
    /// <summary>
    /// 查找並綁定現有系統
    /// </summary>
    private void FindAndBindExistingSystems()
    {
        // 查找NumericalRecords
        if (numericalRecords == null)
        {
            numericalRecords = FindObjectOfType<NumericalRecords>();
            if (numericalRecords == null)
            {
                Debug.LogWarning("[GameDataManager] 找不到NumericalRecords組件");
            }
        }
        
        // 查找PlayerSettings
        if (playerSettings == null)
        {
            playerSettings = FindObjectOfType<NumericalRecords_PlayerSetting>();
            if (playerSettings == null)
            {
                Debug.LogWarning("[GameDataManager] 找不到NumericalRecords_PlayerSetting組件");
            }
        }
    }
    
    /// <summary>
    /// 初始化遊戲數據
    /// </summary>
    private void InitializeGameData()
    {
        currentGameData = new GameData
        {
            gameVersion = Application.version,
            saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            gameProgress = new GameProgress
            {
                currentScene = "tavern",
                dramaProgress = 0,
                affectionLevel = 0
            },
            gameValues = new GameValues
            {
                playerMoney = 1000,
                fishingLevel = 1,
                workExperience = 0,
                catAffection = 0
            },
            gameSettings = new GameSettings
            {
                bgmVolume = 1.0f,
                sfxVolume = 1.0f,
                voiceVolume = 1.0f,
                isFullscreen = false
            }
        };
    }
    
    /// <summary>
    /// 保存遊戲
    /// </summary>
    public void SaveGame()
    {
        try
        {
            // 更新保存時間
            currentGameData.saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            // 從現有系統同步數據
            SyncDataFromExistingSystems();
            
            // 使用JSON保存到PlayerPrefs
            string jsonData = JsonUtility.ToJson(currentGameData, true);
            PlayerPrefs.SetString("GameSaveData", jsonData);
            PlayerPrefs.Save();
            
            Debug.Log("[GameDataManager] 遊戲保存成功");
            OnDataSaved?.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameDataManager] 保存遊戲失敗: {e.Message}");
        }
    }
    
    /// <summary>
    /// 載入遊戲
    /// </summary>
    public void LoadGame()
    {
        try
        {
            if (PlayerPrefs.HasKey("GameSaveData"))
            {
                string jsonData = PlayerPrefs.GetString("GameSaveData");
                GameData loadedData = JsonUtility.FromJson<GameData>(jsonData);
                
                // 版本兼容性檢查
                if (IsCompatibleVersion(loadedData.gameVersion))
                {
                    currentGameData = loadedData;
                    
                    // 同步數據到現有系統
                    SyncDataToExistingSystems();
                    
                    Debug.Log("[GameDataManager] 遊戲載入成功");
                    OnDataLoaded?.Invoke();
                }
                else
                {
                    Debug.LogWarning("[GameDataManager] 存檔版本不兼容，使用默認數據");
                }
            }
            else
            {
                Debug.Log("[GameDataManager] 沒有找到存檔，使用默認數據");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameDataManager] 載入遊戲失敗: {e.Message}");
            // 載入失敗時使用默認數據
        }
    }
    
    /// <summary>
    /// 從現有系統同步數據
    /// </summary>
    private void SyncDataFromExistingSystems()
    {
        // 從NumericalRecords同步數據
        if (numericalRecords != null)
        {
            // 假設NumericalRecords有這些屬性，需要根據實際結構調整
            // currentGameData.gameValues.playerMoney = numericalRecords.GetMoney();
            // currentGameData.gameProgress.affectionLevel = numericalRecords.GetAffection();
            // 暫時使用靜態值，實際需要根據NumericalRecords的API調整
        }
        
        // 從PlayerSettings同步設置
        if (playerSettings != null)
        {
            // 假設PlayerSettings有音量設置，需要根據實際結構調整
            // currentGameData.gameSettings.bgmVolume = playerSettings.GetBGMVolume();
            // currentGameData.gameSettings.sfxVolume = playerSettings.GetSFXVolume();
        }
    }
    
    /// <summary>
    /// 同步數據到現有系統
    /// </summary>
    private void SyncDataToExistingSystems()
    {
        // 同步到NumericalRecords
        if (numericalRecords != null)
        {
            // 假設NumericalRecords有設置方法，需要根據實際結構調整
            // numericalRecords.SetMoney(currentGameData.gameValues.playerMoney);
            // numericalRecords.SetAffection(currentGameData.gameProgress.affectionLevel);
        }
        
        // 同步到PlayerSettings
        if (playerSettings != null)
        {
            // 假設PlayerSettings有設置方法，需要根據實際結構調整
            // playerSettings.SetBGMVolume(currentGameData.gameSettings.bgmVolume);
            // playerSettings.SetSFXVolume(currentGameData.gameSettings.sfxVolume);
        }
    }
    
    /// <summary>
    /// 檢查版本兼容性
    /// </summary>
    private bool IsCompatibleVersion(string saveVersion)
    {
        // 實現版本兼容性檢查邏輯
        // 暫時返回true，實際需要根據版本策略實現
        return true;
    }
    
    #region 數據訪問接口
    
    /// <summary>
    /// 獲取當前遊戲數據
    /// </summary>
    public GameData GetCurrentGameData()
    {
        return currentGameData;
    }
    
    /// <summary>
    /// 更新遊戲進度
    /// </summary>
    public void UpdateGameProgress(string key, object value)
    {
        switch (key.ToLower())
        {
            case "currentscene":
                currentGameData.gameProgress.currentScene = value.ToString();
                break;
            case "dramaprogress":
                currentGameData.gameProgress.dramaProgress = Convert.ToInt32(value);
                break;
            case "affectionlevel":
                currentGameData.gameProgress.affectionLevel = Convert.ToInt32(value);
                break;
        }
        
        OnDataChanged?.Invoke(key, value);
    }
    
    /// <summary>
    /// 更新遊戲數值
    /// </summary>
    public void UpdateGameValue(string key, object value)
    {
        switch (key.ToLower())
        {
            case "playermoney":
                currentGameData.gameValues.playerMoney = Convert.ToInt32(value);
                break;
            case "fishinglevel":
                currentGameData.gameValues.fishingLevel = Convert.ToInt32(value);
                break;
            case "workexperience":
                currentGameData.gameValues.workExperience = Convert.ToInt32(value);
                break;
            case "cataffection":
                currentGameData.gameValues.catAffection = Convert.ToInt32(value);
                break;
        }
        
        OnDataChanged?.Invoke(key, value);
    }
    
    /// <summary>
    /// 更新遊戲設置
    /// </summary>
    public void UpdateGameSetting(string key, object value)
    {
        switch (key.ToLower())
        {
            case "bgmvolume":
                currentGameData.gameSettings.bgmVolume = Convert.ToSingle(value);
                break;
            case "sfxvolume":
                currentGameData.gameSettings.sfxVolume = Convert.ToSingle(value);
                break;
            case "voicevolume":
                currentGameData.gameSettings.voiceVolume = Convert.ToSingle(value);
                break;
            case "isfullscreen":
                currentGameData.gameSettings.isFullscreen = Convert.ToBoolean(value);
                break;
        }
        
        OnDataChanged?.Invoke(key, value);
    }
    
    /// <summary>
    /// 重置遊戲數據
    /// </summary>
    public void ResetGameData()
    {
        InitializeGameData();
        SyncDataToExistingSystems();
        Debug.Log("[GameDataManager] 遊戲數據已重置");
    }
    
    #endregion
}

// 遊戲數據結構
[System.Serializable]
public class GameData
{
    public string gameVersion;
    public string saveTime;
    public GameProgress gameProgress;
    public GameValues gameValues;
    public GameSettings gameSettings;
}

[System.Serializable]
public class GameProgress
{
    public string currentScene;
    public int dramaProgress;
    public int affectionLevel;
}

[System.Serializable]
public class GameValues
{
    public int playerMoney;
    public int fishingLevel;
    public int workExperience;
    public int catAffection;
}

[System.Serializable]
public class GameSettings
{
    public float bgmVolume;
    public float sfxVolume;
    public float voiceVolume;
    public bool isFullscreen;
}