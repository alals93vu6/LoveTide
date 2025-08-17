using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LoveTide.UI;

namespace LoveTide.Core
{
    /// <summary>
    /// 新版遊戲主控制器 - 重製版GamePlay場景的核心管理器
    /// 負責遊戲狀態管理、系統協調和整體流程控制
    /// </summary>
    public class NewGameManager : MonoBehaviour
    {
        [Header("=== 核心系統引用 ===")]
        [SerializeField] private TimeManagerTest timeManager;
        [SerializeField] private NumericalRecords numericalRecords;
        [SerializeField] private ActorManagerTest actorManager;
        
        [Header("=== 新系統管理器 ===")]
        [SerializeField] private LoveTide.Core.InteractionManager interactionManager;
        [SerializeField] private LoveTide.UI.NewUIManager uiManager;
        [SerializeField] private LoveTide.Core.SceneStateManager sceneStateManager;
        
        [Header("=== 遊戲狀態 ===")]
        [SerializeField] private GameState currentGameState = GameState.Initialize;
        [SerializeField] private bool isInitialized = false;
        
        [Header("=== 場景數據 ===")]
        [SerializeField] private bool isWorkTime = true;
        [SerializeField] private bool isVacation = false;
        [SerializeField] private bool isTalking = false;
        
        // 單例模式
        public static NewGameManager Instance { get; private set; }
        
        // 公開屬性
        public GameState CurrentGameState => currentGameState;
        public bool IsWorkTime => isWorkTime;
        public bool IsVacation => isVacation;
        public bool IsTalking => isTalking;
        
        // 事件系統
        public static event Action<GameState, GameState> OnGameStateChanged;
        public static event Action<bool> OnWorkTimeChanged;
        public static event Action<bool> OnTalkingStateChanged;
        
        #region Unity生命週期
        
        void Awake()
        {
            InitializeSingleton();
        }
        
        void Start()
        {
            StartCoroutine(InitializeGameSystems());
        }
        
        void Update()
        {
            HandleGameUpdate();
        }
        
        #endregion
        
        #region 單例初始化
        
        void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[NewGameManager] 單例初始化完成");
            }
            else
            {
                Debug.LogWarning("[NewGameManager] 檢測到重複實例，銷毀當前物件");
                Destroy(gameObject);
            }
        }
        
        #endregion
        
        #region 系統初始化
        
        IEnumerator InitializeGameSystems()
        {
            Debug.Log("[NewGameManager] 開始初始化遊戲系統...");
            
            SetGameState(GameState.Initialize);
            
            // 等待一幀確保所有物件都已載入
            yield return null;
            
            // 驗證核心系統引用
            if (!ValidateSystemReferences())
            {
                Debug.LogError("[NewGameManager] 核心系統引用驗證失敗！");
                yield break;
            }
            
            // 初始化各個管理器
            yield return StartCoroutine(InitializeManagers());
            
            // 載入遊戲數據
            LoadGameData();
            
            // 初始化場景狀態
            InitializeSceneState();
            
            // 啟動音效系統
            InitializeAudioSystem();
            
            // 系統初始化完成
            isInitialized = true;
            SetGameState(GameState.Playing);
            
            Debug.Log("[NewGameManager] 遊戲系統初始化完成！");
        }
        
        bool ValidateSystemReferences()
        {
            bool isValid = true;
            
            if (timeManager == null)
            {
                Debug.LogError("[NewGameManager] TimeManager引用缺失！");
                isValid = false;
            }
            
            if (numericalRecords == null)
            {
                Debug.LogError("[NewGameManager] NumericalRecords引用缺失！");
                isValid = false;
            }
            
            if (actorManager == null)
            {
                Debug.LogError("[NewGameManager] ActorManager引用缺失！");
                isValid = false;
            }
            
            if (interactionManager == null)
            {
                Debug.LogError("[NewGameManager] InteractionManager引用缺失！");
                isValid = false;
            }
            
            if (uiManager == null)
            {
                Debug.LogError("[NewGameManager] NewUIManager引用缺失！");
                isValid = false;
            }
            
            return isValid;
        }
        
        IEnumerator InitializeManagers()
        {
            Debug.Log("[NewGameManager] 初始化管理器...");
            
            // 初始化Scene狀態管理器
            if (sceneStateManager != null)
            {
                sceneStateManager.Initialize(this);
                yield return null;
            }
            
            // 初始化UI管理器
            if (uiManager != null)
            {
                uiManager.Initialize(this);
                yield return null;
            }
            
            // 初始化互動管理器
            if (interactionManager != null)
            {
                interactionManager.Initialize(this);
                yield return null;
            }
            
            Debug.Log("[NewGameManager] 管理器初始化完成");
        }
        
        void LoadGameData()
        {
            Debug.Log("[NewGameManager] 載入遊戲數據...");
            
            // 啟動原有的數值系統
            if (numericalRecords != null)
            {
                numericalRecords.OnStart();
            }
            
            // 載入時間狀態
            try
            {
                UpdateTimeState();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[NewGameManager] 時間狀態更新失敗: {e.Message}");
            }
        }
        
        void InitializeSceneState()
        {
            Debug.Log("[NewGameManager] 初始化場景狀態...");
            
            // 根據時間管理器確定當前場景狀態
            if (timeManager != null)
            {
                try
                {
                    // 檢查是否有VacationDetected方法
                    var method = timeManager.GetType().GetMethod("VacationDetected");
                    if (method != null)
                    {
                        method.Invoke(timeManager, null);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[NewGameManager] VacationDetected調用失敗: {e.Message}");
                }
                
                UpdateSceneState();
            }
            
            // 初始化演員系統
            if (actorManager != null)
            {
                // 根據當前好感度等級初始化演員
                int friendshipLevel = PlayerPrefs.GetInt("FDS_LV", 0);
                // actorManager 會在自己的OnStart中處理初始化
            }
        }
        
        void InitializeAudioSystem()
        {
            Debug.Log("[NewGameManager] 初始化音效系統...");
            
            // 啟動BGM
            try
            {
                var bgmManager = GameObject.FindObjectOfType<MonoBehaviour>();
                if (bgmManager != null && bgmManager.GetType().Name.Contains("bgm"))
                {
                    var method = bgmManager.GetType().GetMethod("SwitchAudio");
                    if (method != null)
                    {
                        method.Invoke(bgmManager, new object[] { 2 }); // 遊戲場景BGM
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[NewGameManager] BGM系統啟動失敗: {e.Message}");
            }
        }
        
        #endregion
        
        #region 遊戲狀態管理
        
        public void SetGameState(GameState newState)
        {
            if (currentGameState == newState) return;
            
            var oldState = currentGameState;
            currentGameState = newState;
            
            Debug.Log($"[NewGameManager] 遊戲狀態變更: {oldState} → {newState}");
            
            // 處理狀態轉換邏輯
            HandleStateTransition(oldState, newState);
            
            // 通知其他系統
            OnGameStateChanged?.Invoke(oldState, newState);
            
            // 通知UI管理器
            if (uiManager != null)
            {
                uiManager.OnGameStateChanged(oldState, newState);
            }
            
            // 通知互動管理器
            if (interactionManager != null)
            {
                interactionManager.OnGameStateChanged(oldState, newState);
            }
        }
        
        void HandleStateTransition(GameState oldState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Initialize:
                    // 初始化狀態：禁用所有互動
                    SetInteractionsEnabled(false);
                    break;
                    
                case GameState.Playing:
                    // 遊戲中：啟用互動
                    SetInteractionsEnabled(true);
                    break;
                    
                case GameState.Dialog:
                    // 對話中：禁用場景互動，保留對話互動
                    SetInteractionsEnabled(false);
                    SetDialogInteractionEnabled(true);
                    break;
                    
                case GameState.Menu:
                    // 選單中：禁用遊戲互動
                    SetInteractionsEnabled(false);
                    break;
                    
                case GameState.Paused:
                    // 暫停：禁用所有互動
                    SetInteractionsEnabled(false);
                    break;
                    
                case GameState.Loading:
                    // 載入中：禁用所有互動
                    SetInteractionsEnabled(false);
                    break;
            }
        }
        
        void SetInteractionsEnabled(bool enabled)
        {
            if (interactionManager != null)
            {
                interactionManager.SetInteractionsEnabled(enabled);
            }
        }
        
        void SetDialogInteractionEnabled(bool enabled)
        {
            if (interactionManager != null)
            {
                interactionManager.SetDialogInteractionEnabled(enabled);
            }
        }
        
        #endregion
        
        #region 場景狀態管理
        
        public void UpdateSceneState()
        {
            if (timeManager == null) return;
            
            bool newIsWorkTime = DetermineWorkTime();
            bool newIsVacation = timeManager.vacation;
            
            // 更新工作時間狀態
            if (isWorkTime != newIsWorkTime)
            {
                isWorkTime = newIsWorkTime;
                OnWorkTimeChanged?.Invoke(isWorkTime);
                
                Debug.Log($"[NewGameManager] 工作時間狀態變更: {isWorkTime}");
            }
            
            // 更新假期狀態
            if (isVacation != newIsVacation)
            {
                isVacation = newIsVacation;
                Debug.Log($"[NewGameManager] 假期狀態變更: {isVacation}");
            }
            
            // 通知場景狀態管理器
            if (sceneStateManager != null)
            {
                sceneStateManager.UpdateSceneState(isWorkTime, isVacation);
            }
            
            // 通知互動管理器
            if (interactionManager != null)
            {
                interactionManager.UpdateSceneState(isWorkTime, isVacation);
            }
        }
        
        bool DetermineWorkTime()
        {
            if (timeManager == null) return true;
            
            // 根據時間判斷是否為工作時間
            // 時間1-6為工作時間，7-9為宿舍時間
            int currentTime = GetCurrentTimer();
            return currentTime >= 1 && currentTime <= 6;
        }
        
        void UpdateTimeState()
        {
            if (timeManager == null || numericalRecords == null) return;
            
            // 同步時間狀態
            UpdateSceneState();
        }
        
        #endregion
        
        #region 對話系統整合
        
        public void StartDialog(int dialogID)
        {
            Debug.Log($"[NewGameManager] 開始對話: {dialogID}");
            
            SetGameState(GameState.Dialog);
            SetTalkingState(true);
            
            // 通知演員管理器
            if (actorManager != null)
            {
                actorManager.ActorCtrl();
            }
            
            // TODO: 這裡會整合新版對話系統
            // 目前使用現有的TextBox系統作為過渡
        }
        
        public void EndDialog()
        {
            Debug.Log("[NewGameManager] 結束對話");
            
            SetTalkingState(false);
            SetGameState(GameState.Playing);
            
            // 檢查時間推進
            CheckTimeAdvancement();
        }
        
        void SetTalkingState(bool talking)
        {
            if (isTalking == talking) return;
            
            isTalking = talking;
            OnTalkingStateChanged?.Invoke(isTalking);
            
            Debug.Log($"[NewGameManager] 對話狀態變更: {isTalking}");
        }
        
        #endregion
        
        #region 時間管理整合
        
        public void AdvanceTime(int timeUnits = 1)
        {
            Debug.Log($"[NewGameManager] 推進時間: {timeUnits} 單位");
            
            if (numericalRecords != null)
            {
                try
                {
                    int currentTimer = GetFieldValue<int>(numericalRecords, "aTimer", 1);
                    SetFieldValue<int>(numericalRecords, "aTimer", currentTimer + timeUnits);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[NewGameManager] 時間推進失敗: {e.Message}");
                }
            }
            
            CheckTimeAdvancement();
        }
        
        void CheckTimeAdvancement()
        {
            if (numericalRecords == null) return;
            
            // 檢查是否需要進入新的時間段或新的一天
            int currentTimer = GetFieldValue<int>(numericalRecords, "aTimer", 1);
            if (currentTimer >= 10)
            {
                HandleDayTransition();
            }
            else
            {
                // 更新場景狀態
                UpdateSceneState();
            }
        }
        
        void HandleDayTransition()
        {
            Debug.Log("[NewGameManager] 處理日期轉換");
            
            // 觸發日期變更邏輯
            SetGameState(GameState.Loading);
            
            // 這裡會調用原有的日期推進邏輯
            // 或者新的日期管理系統
            
            StartCoroutine(ProcessDayTransition());
        }
        
        IEnumerator ProcessDayTransition()
        {
            // 保存遊戲數據
            if (numericalRecords != null)
            {
                try
                {
                    var method = numericalRecords.GetType().GetMethod("GameDataSave");
                    if (method != null)
                    {
                        method.Invoke(numericalRecords, null);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[NewGameManager] 遊戲保存失敗: {e.Message}");
                }
            }
            
            yield return new WaitForSeconds(0.5f);
            
            // 檢查是否有劇情事件
            bool hasEvent = CheckStoryEvents();
            
            if (hasEvent)
            {
                // 切換到劇情場景
                SceneManager.LoadScene("DramaScene");
            }
            else
            {
                // 繼續遊戲
                ResetForNewDay();
                SetGameState(GameState.Playing);
            }
        }
        
        bool CheckStoryEvents()
        {
            // TODO: 這裡會整合新版劇情觸發系統
            // 目前使用原有的劇情檢測邏輯作為過渡
            return false;
        }
        
        void ResetForNewDay()
        {
            Debug.Log("[NewGameManager] 重置新的一天");
            
            // 重置時間
            if (numericalRecords != null)
            {
                SetFieldValue<int>(numericalRecords, "aTimer", 1);
            }
            
            // 重置場景狀態
            UpdateSceneState();
            
            // 重置UI狀態
            if (uiManager != null)
            {
                uiManager.ResetForNewDay();
            }
        }
        
        #endregion
        
        #region 遊戲更新循環
        
        void HandleGameUpdate()
        {
            if (!isInitialized) return;
            
            // 處理輸入
            HandleInput();
            
            // 更新系統狀態
            UpdateSystemStates();
        }
        
        void HandleInput()
        {
            // 開發模式快捷鍵
            if (Application.isEditor)
            {
                HandleDebugInput();
            }
        }
        
        void HandleDebugInput()
        {
            // 快速保存 (Shift + S)
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
            {
                if (numericalRecords != null)
                {
                    numericalRecords.GameDataSave();
                    Debug.Log("[NewGameManager] 快速保存完成");
                }
            }
            
            // 重置遊戲 (Shift + R)
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
            {
                if (numericalRecords != null)
                {
                    numericalRecords.GameDataReset();
                    Debug.Log("[NewGameManager] 遊戲數據重置");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }
        
        void UpdateSystemStates()
        {
            // 定期檢查系統狀態
            if (Time.frameCount % 60 == 0) // 每秒檢查一次
            {
                ValidateSystemStates();
            }
        }
        
        void ValidateSystemStates()
        {
            // 驗證系統狀態的一致性
            if (timeManager != null && numericalRecords != null)
            {
                // 確保時間同步
                int timeManagerTimer = GetFieldValue<int>(timeManager, "aTimer", 1);
                int numericalRecordsTimer = GetFieldValue<int>(numericalRecords, "aTimer", 1);
                bool shouldUpdateTime = Math.Abs(timeManagerTimer - numericalRecordsTimer) > 0;
                if (shouldUpdateTime)
                {
                    UpdateTimeState();
                }
            }
        }
        
        #endregion
        
        #region 公開API
        
        /// <summary>
        /// 獲取當前時間信息
        /// </summary>
        public (int day, int week, int timer) GetCurrentTime()
        {
            if (numericalRecords != null)
            {
                try
                {
                    int day = GetFieldValue<int>(numericalRecords, "aDay", 1);
                    int week = GetFieldValue<int>(numericalRecords, "aWeek", 1);
                    int timer = GetFieldValue<int>(numericalRecords, "aTimer", 1);
                    return (day, week, timer);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[NewGameManager] 獲取時間信息失敗: {e.Message}");
                }
            }
            return (1, 1, 1);
        }
        
        /// <summary>
        /// 安全獲取當前時段
        /// </summary>
        int GetCurrentTimer()
        {
            if (numericalRecords != null)
            {
                return GetFieldValue<int>(numericalRecords, "aTimer", 1);
            }
            else if (timeManager != null)
            {
                return GetFieldValue<int>(timeManager, "aTimer", 1);
            }
            return 1;
        }
        
        /// <summary>
        /// 通用的字段值獲取方法
        /// </summary>
        T GetFieldValue<T>(object target, string fieldName, T defaultValue)
        {
            if (target == null) return defaultValue;
            
            try
            {
                var field = target.GetType().GetField(fieldName);
                if (field != null)
                    return (T)field.GetValue(target);
                    
                var property = target.GetType().GetProperty(fieldName);
                if (property != null)
                    return (T)property.GetValue(target);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[NewGameManager] 無法獲取 {fieldName}: {e.Message}");
            }
            
            return defaultValue;
        }
        
        /// <summary>
        /// 通用的字段值設置方法
        /// </summary>
        void SetFieldValue<T>(object target, string fieldName, T value)
        {
            if (target == null) return;
            
            try
            {
                var field = target.GetType().GetField(fieldName);
                if (field != null)
                {
                    field.SetValue(target, value);
                    return;
                }
                    
                var property = target.GetType().GetProperty(fieldName);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(target, value);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[NewGameManager] 無法設置 {fieldName}: {e.Message}");
            }
        }
        
        /// <summary>
        /// 檢查遊戲是否已初始化
        /// </summary>
        public bool IsGameInitialized()
        {
            return isInitialized;
        }
        
        /// <summary>
        /// 強制更新場景狀態
        /// </summary>
        public void ForceUpdateSceneState()
        {
            UpdateSceneState();
        }
        
        /// <summary>
        /// 暫停/恢復遊戲
        /// </summary>
        public void SetGamePaused(bool paused)
        {
            if (paused)
            {
                SetGameState(GameState.Paused);
            }
            else
            {
                SetGameState(GameState.Playing);
            }
        }
        
        #endregion
        
        #region 事件處理
        
        void OnDestroy()
        {
            // 清理事件訂閱
            OnGameStateChanged = null;
            OnWorkTimeChanged = null;
            OnTalkingStateChanged = null;
        }
        
        #endregion
    }
    /// <summary>
    /// 遊戲狀態枚舉
    /// </summary>
    public enum GameState
    {
        Initialize,    // 初始化
        Loading,       // 載入中
        Playing,       // 遊戲中
        Dialog,        // 對話中
        Menu,          // 選單中
        Paused,        // 暫停
        Transition     // 轉場中
    }
}