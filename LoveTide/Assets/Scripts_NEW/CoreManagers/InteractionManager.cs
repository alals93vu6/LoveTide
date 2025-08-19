using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LoveTide.Interaction;

namespace LoveTide.Core
{
    /// <summary>
    /// 互動管理器 - 統一管理動態角色和靜態物件的互動邏輯
    /// 處理互動條件檢查、結果執行和狀態管理
    /// </summary>
    public class InteractionManager : MonoBehaviour
    {
        [Header("=== 核心系統引用 ===")]
        [SerializeField] private NewGameManager gameManager;
        [SerializeField] private NumericalRecords numericalRecords;
        [SerializeField] private ActorManagerTest actorManager;
        
        [Header("=== 互動系統組件 ===")]
        [SerializeField] private DynamicCharacterInteraction dynamicCharacterInteraction;
        [SerializeField] private StaticObjectInteraction staticObjectInteraction;
        
        [Header("=== Canvas引用 ===")]
        [SerializeField] private Canvas dynamicCharacterCanvas;
        [SerializeField] private Canvas staticInteractionCanvas;
        [SerializeField] private Canvas dialogCanvas;
        
        [Header("=== 場景狀態 ===")]
        [SerializeField] private bool isWorkTime = true;
        [SerializeField] private bool isVacation = false;
        [SerializeField] private int currentTimeSlot = 1;
        [SerializeField] private bool interactionsEnabled = true;
        [SerializeField] private bool dialogInteractionEnabled = false;
        
        [Header("=== 互動配置 ===")]
        [SerializeField] private InteractionConfig interactionConfig;
        
        // 公開屬性
        public bool IsWorkTime => isWorkTime;
        public bool IsVacation => isVacation;
        public int CurrentTimeSlot => currentTimeSlot;
        public bool InteractionsEnabled => interactionsEnabled;
        
        // 事件系統
        public static event Action<string> OnInteractionTriggered;
        public static event Action<bool> OnInteractionsEnabledChanged;
        public static event Action<InteractionResult> OnInteractionCompleted;
        
        #region Unity生命週期
        
        void Awake()
        {
            InitializeReferences();
        }
        
        void Start()
        {
            StartCoroutine(InitializeInteractionSystem());
        }
        
        void Update()
        {
            HandleInteractionUpdate();
        }
        
        #endregion
        
        #region 初始化
        
        void InitializeReferences()
        {
            if (gameManager == null)
                gameManager = NewGameManager.Instance;
        }
        
        public void Initialize(NewGameManager manager)
        {
            gameManager = manager;
            Debug.Log("[InteractionManager] 互動管理器初始化中...");
        }
        
        IEnumerator InitializeInteractionSystem()
        {
            Debug.Log("[InteractionManager] 開始初始化互動系統...");
            
            // 等待GameManager初始化完成
            while (!gameManager.IsGameInitialized())
            {
                yield return null;
            }
            
            // 驗證組件引用
            if (!ValidateComponents())
            {
                Debug.LogError("[InteractionManager] 組件驗證失敗！");
                yield break;
            }
            
            // 初始化子系統
            InitializeSubSystems();
            
            // 訂閱事件
            SubscribeToEvents();
            
            // 更新初始狀態
            UpdateInteractionState();
            
            Debug.Log("[InteractionManager] 互動系統初始化完成！");
        }
        
        bool ValidateComponents()
        {
            bool isValid = true;
            
            if (dynamicCharacterCanvas == null)
            {
                Debug.LogError("[InteractionManager] DynamicCharacterCanvas引用缺失！");
                isValid = false;
            }
            
            if (staticInteractionCanvas == null)
            {
                Debug.LogError("[InteractionManager] StaticInteractionCanvas引用缺失！");
                isValid = false;
            }
            
            if (dialogCanvas == null)
            {
                Debug.LogError("[InteractionManager] DialogCanvas引用缺失！");
                isValid = false;
            }
            
            return isValid;
        }
        
        void InitializeSubSystems()
        {
            // 初始化動態角色互動系統
            if (dynamicCharacterInteraction != null)
            {
                dynamicCharacterInteraction.Initialize(this);
            }
            
            // 初始化靜態物件互動系統
            if (staticObjectInteraction != null)
            {
                staticObjectInteraction.Initialize(this);
            }
        }
        
        void SubscribeToEvents()
        {
            // 訂閱GameManager事件
            if (gameManager != null)
            {
                NewGameManager.OnGameStateChanged += OnGameStateChanged;
                NewGameManager.OnWorkTimeChanged += OnWorkTimeChanged;
                NewGameManager.OnTalkingStateChanged += OnTalkingStateChanged;
            }
        }
        
        #endregion
        
        #region 狀態管理
        
        public void UpdateSceneState(bool newIsWorkTime, bool newIsVacation)
        {
            bool stateChanged = false;
            
            if (isWorkTime != newIsWorkTime)
            {
                isWorkTime = newIsWorkTime;
                stateChanged = true;
                Debug.Log($"[InteractionManager] 工作狀態更新: {isWorkTime}");
            }
            
            if (isVacation != newIsVacation)
            {
                isVacation = newIsVacation;
                stateChanged = true;
                Debug.Log($"[InteractionManager] 假期狀態更新: {isVacation}");
            }
            
            if (stateChanged)
            {
                UpdateInteractionState();
            }
        }
        
        public void OnGameStateChanged(GameState oldGameState, GameState newGameState)
        {
            Debug.Log($"[InteractionManager] 接收遊戲狀態變更: {oldGameState} → {newGameState}");
            
            switch (newGameState)
            {
                case GameState.Playing:
                    EnableGameplayInteractions();
                    break;
                case GameState.Dialog:
                    EnableDialogModeInteractions();
                    break;
                case GameState.Menu:
                case GameState.Paused:
                case GameState.Loading:
                    DisableAllInteractions();
                    break;
            }
        }
        
        void OnWorkTimeChanged(bool workTime)
        {
            isWorkTime = workTime;
            UpdateInteractionState();
        }
        
        void OnTalkingStateChanged(bool talking)
        {
            if (talking)
            {
                EnableDialogModeInteractions();
            }
            else
            {
                EnableGameplayInteractions();
            }
        }
        
        void UpdateInteractionState()
        {
            if (!interactionsEnabled) return;
            
            UpdateTimeSlot();
            UpdateAvailableInteractions();
            UpdateCanvasVisibility();
        }
        
        void UpdateTimeSlot()
        {
            if (numericalRecords != null)
            {
                currentTimeSlot = numericalRecords.aTimer;
            }
        }
        
        #endregion
        
        #region 互動控制
        
        void UpdateAvailableInteractions()
        {
            if (isWorkTime)
            {
                EnableWorkSceneInteractions();
            }
            else
            {
                EnableDormSceneInteractions();
            }
            
            // 特殊時段處理
            HandleSpecialTimeSlots();
        }
        
        void EnableWorkSceneInteractions()
        {
            Debug.Log("[InteractionManager] 啟用工作場景互動");
            
            // 啟用靜態物件互動
            if (staticObjectInteraction != null)
            {
                staticObjectInteraction.EnableInteractionType("HelpWork", true);
                staticObjectInteraction.EnableInteractionType("CatPlay", true);
                staticObjectInteraction.EnableInteractionType("GoOutAlone", true);
                staticObjectInteraction.EnableInteractionType("InviteDrinking", false); // 工作時不可邀請喝酒
            }
            
            // 啟用動態角色互動
            if (dynamicCharacterInteraction != null)
            {
                dynamicCharacterInteraction.EnableInteraction(true);
            }
        }
        
        void EnableDormSceneInteractions()
        {
            Debug.Log("[InteractionManager] 啟用宿舍場景互動");
            
            // 啟用靜態物件互動
            if (staticObjectInteraction != null)
            {
                staticObjectInteraction.EnableInteractionType("HelpWork", false); // 宿舍時不可工作
                staticObjectInteraction.EnableInteractionType("CatPlay", true);
                staticObjectInteraction.EnableInteractionType("GoOutAlone", false); // 暫時禁用外出
                staticObjectInteraction.EnableInteractionType("InviteDrinking", true);
            }
            
            // 啟用動態角色互動
            if (dynamicCharacterInteraction != null)
            {
                dynamicCharacterInteraction.EnableInteraction(true);
            }
        }
        
        void HandleSpecialTimeSlots()
        {
            // 夜晚場景統一處理 (時段7-9)
            if (currentTimeSlot >= 7)
            {
                SetYukaLocationToDormHall();
            }
            
            // 深夜限制
            if (currentTimeSlot >= 9)
            {
                LimitLateNightInteractions();
            }
        }
        
        void SetYukaLocationToDormHall()
        {
            // 確保由香在宿舍大廳（重製版變更：不再有由香房間）
            if (actorManager != null)
            {
                // 通知演員管理器設置由香位置
                try
                {
                    var method = actorManager.GetType().GetMethod("SetActorLocation");
                    if (method != null)
                    {
                        method.Invoke(actorManager, new object[] { "Yuka", "DormHall" });
                    }
                    else
                    {
                        Debug.LogWarning("[InteractionManager] ActorManager沒有SetActorLocation方法，使用替代方案");
                        // 替代方案：可能需要直接設置位置或使用其他方法
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[InteractionManager] 設置由香位置失敗: {e.Message}");
                }
            }
        }
        
        void LimitLateNightInteractions()
        {
            // 深夜限制某些互動
            if (staticObjectInteraction != null)
            {
                staticObjectInteraction.EnableInteractionType("GoOutAlone", false);
            }
        }
        
        #endregion
        
        #region 互動啟用/禁用
        
        public void SetInteractionsEnabled(bool enabled)
        {
            if (interactionsEnabled == enabled) return;
            
            interactionsEnabled = enabled;
            
            Debug.Log($"[InteractionManager] 互動系統{(enabled ? "啟用" : "禁用")}");
            
            if (enabled)
            {
                EnableGameplayInteractions();
            }
            else
            {
                DisableAllInteractions();
            }
            
            OnInteractionsEnabledChanged?.Invoke(enabled);
        }
        
        public void SetDialogInteractionEnabled(bool enabled)
        {
            dialogInteractionEnabled = enabled;
            
            if (enabled)
            {
                EnableDialogModeInteractions();
            }
        }
        
        void EnableGameplayInteractions()
        {
            if (!interactionsEnabled) return;
            
            // 啟用Canvas
            SetCanvasActive(dynamicCharacterCanvas, true);
            SetCanvasActive(staticInteractionCanvas, true);
            SetCanvasActive(dialogCanvas, false);
            
            // 更新可用互動
            UpdateAvailableInteractions();
        }
        
        void EnableDialogModeInteractions()
        {
            // 對話模式：只啟用對話Canvas
            SetCanvasActive(dynamicCharacterCanvas, false);
            SetCanvasActive(staticInteractionCanvas, false);
            SetCanvasActive(dialogCanvas, true);
        }
        
        void DisableAllInteractions()
        {
            // 禁用所有互動Canvas
            SetCanvasActive(dynamicCharacterCanvas, false);
            SetCanvasActive(staticInteractionCanvas, false);
            
            // 不禁用對話Canvas，由對話系統自己控制
        }
        
        void UpdateCanvasVisibility()
        {
            if (!interactionsEnabled) return;
            
            // 根據遊戲狀態更新Canvas可見性
            bool showGameplayUI = (gameManager.CurrentGameState == GameState.Playing);
            bool showDialogUI = (gameManager.CurrentGameState == GameState.Dialog);
            
            SetCanvasActive(dynamicCharacterCanvas, showGameplayUI);
            SetCanvasActive(staticInteractionCanvas, showGameplayUI);
            SetCanvasActive(dialogCanvas, showDialogUI);
        }
        
        void SetCanvasActive(Canvas canvas, bool active)
        {
            if (canvas != null)
            {
                canvas.gameObject.SetActive(active);
            }
        }
        
        #endregion
        
        #region 互動處理
        
        public void TriggerInteraction(string interactionType, object data = null)
        {
            Debug.Log($"[InteractionManager] 觸發互動: {interactionType}");
            
            if (!interactionsEnabled)
            {
                Debug.LogWarning("[InteractionManager] 互動系統已禁用，無法觸發互動");
                return;
            }
            
            // 檢查互動條件
            if (!CheckInteractionConditions(interactionType))
            {
                Debug.LogWarning($"[InteractionManager] 互動條件不滿足: {interactionType}");
                return;
            }
            
            // 通知系統互動開始
            OnInteractionTriggered?.Invoke(interactionType);
            
            // 執行互動邏輯
            StartCoroutine(ProcessInteraction(interactionType, data));
        }
        
        bool CheckInteractionConditions(string interactionType)
        {
            if (interactionConfig == null) return true;
            
            var config = interactionConfig.GetInteractionData(interactionType);
            if (config == null) return true;
            
            // 檢查時間條件
            if (config.RequiredTimeSlots != null && config.RequiredTimeSlots.Length > 0)
            {
                bool timeValid = Array.Exists(config.RequiredTimeSlots, t => t == currentTimeSlot);
                if (!timeValid)
                {
                    Debug.LogWarning($"[InteractionManager] 時間條件不滿足: {interactionType}");
                    return false;
                }
            }
            
            // 檢查場景條件
            if (config.RequireWorkTime.HasValue)
            {
                if (config.RequireWorkTime.Value != isWorkTime)
                {
                    Debug.LogWarning($"[InteractionManager] 場景條件不滿足: {interactionType}");
                    return false;
                }
            }
            
            // 檢查數值條件
            if (config.RequiredAffection > 0)
            {
                int currentAffection = GetCurrentAffection();
                if (currentAffection < config.RequiredAffection)
                {
                    Debug.LogWarning($"[InteractionManager] 好感度不足: {interactionType}");
                    return false;
                }
            }
            
            return true;
        }
        
        IEnumerator ProcessInteraction(string interactionType, object data)
        {
            var config = interactionConfig?.GetInteractionData(interactionType);
            if (config == null)
            {
                Debug.LogError($"[InteractionManager] 找不到互動配置: {interactionType}");
                yield break;
            }
            
            // 創建互動結果
            var result = new InteractionResult
            {
                InteractionType = interactionType,
                Success = false,
                Data = data
            };
            
            // 執行互動前置邏輯
            yield return StartCoroutine(ExecutePreInteraction(config));
            
            // 執行主要互動邏輯
            yield return StartCoroutine(ExecuteMainInteraction(config, result));
            
            // 執行後置邏輯
            yield return StartCoroutine(ExecutePostInteraction(config, result));
            
            result.Success = true;
            
            // 通知互動完成
            OnInteractionCompleted?.Invoke(result);
        }
        
        IEnumerator ExecutePreInteraction(InteractionData config)
        {
            // 播放開始音效
            if (!string.IsNullOrEmpty(config.StartSoundEffect))
            {
                // TODO: 播放音效
            }
            
            yield return null;
        }
        
        IEnumerator ExecuteMainInteraction(InteractionData config, InteractionResult result)
        {
            // 更新數值
            if (config.AffectionChange != 0)
            {
                ChangeAffection(config.AffectionChange);
            }
            
            if (config.MoneyChange != 0)
            {
                ChangeMoney(config.MoneyChange);
            }
            
            // 推進時間
            if (config.TimeCost > 0)
            {
                gameManager.AdvanceTime(config.TimeCost);
                yield return new WaitForSeconds(0.1f); // 等待時間更新
            }
            
            // 觸發對話
            if (config.DialogID > 0)
            {
                gameManager.StartDialog(config.DialogID);
                // 等待對話結束
                while (gameManager.IsTalking)
                {
                    yield return null;
                }
            }
            
            yield return null;
        }
        
        IEnumerator ExecutePostInteraction(InteractionData config, InteractionResult result)
        {
            // 播放結束音效
            if (!string.IsNullOrEmpty(config.EndSoundEffect))
            {
                // TODO: 播放音效
            }
            
            // 更新UI顯示
            UpdateInteractionState();
            
            yield return null;
        }
        
        #endregion
        
        #region 輔助方法
        
        void HandleInteractionUpdate()
        {
            // 定期檢查和更新互動狀態
            if (Time.frameCount % 60 == 0) // 每秒檢查一次
            {
                ValidateInteractionState();
            }
        }
        
        void ValidateInteractionState()
        {
            // 驗證當前互動狀態是否一致
            if (gameManager != null)
            {
                bool shouldBeWorkTime = gameManager.IsWorkTime;
                if (isWorkTime != shouldBeWorkTime)
                {
                    Debug.LogWarning("[InteractionManager] 檢測到工作時間狀態不一致，正在同步...");
                    UpdateSceneState(shouldBeWorkTime, isVacation);
                }
            }
        }
        
        int GetCurrentAffection()
        {
            if (numericalRecords == null) return 0;
            
            try
            {
                var field = numericalRecords.GetType().GetField("aAffection");
                if (field != null)
                    return (int)field.GetValue(numericalRecords);
                    
                var property = numericalRecords.GetType().GetProperty("aAffection");
                if (property != null)
                    return (int)property.GetValue(numericalRecords);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[InteractionManager] 無法獲取好感度: {e.Message}");
            }
            
            return 0;
        }
        
        void ChangeAffection(int amount)
        {
            if (numericalRecords == null) return;
            
            try
            {
                var field = numericalRecords.GetType().GetField("aAffection");
                if (field != null)
                {
                    int currentValue = (int)field.GetValue(numericalRecords);
                    field.SetValue(numericalRecords, currentValue + amount);
                    Debug.Log($"[InteractionManager] 好感度變化: {amount:+0;-0} (當前: {currentValue + amount})");
                    return;
                }
                
                var property = numericalRecords.GetType().GetProperty("aAffection");
                if (property != null && property.CanWrite)
                {
                    int currentValue = (int)property.GetValue(numericalRecords);
                    property.SetValue(numericalRecords, currentValue + amount);
                    Debug.Log($"[InteractionManager] 好感度變化: {amount:+0;-0} (當前: {currentValue + amount})");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[InteractionManager] 無法修改好感度: {e.Message}");
            }
        }
        
        void ChangeMoney(int amount)
        {
            if (numericalRecords == null) return;
            
            try
            {
                var field = numericalRecords.GetType().GetField("aMoney");
                if (field != null)
                {
                    int currentValue = (int)field.GetValue(numericalRecords);
                    field.SetValue(numericalRecords, currentValue + amount);
                    Debug.Log($"[InteractionManager] 金錢變化: {amount:+0;-0} (當前: {currentValue + amount})");
                    return;
                }
                
                var property = numericalRecords.GetType().GetProperty("aMoney");
                if (property != null && property.CanWrite)
                {
                    int currentValue = (int)property.GetValue(numericalRecords);
                    property.SetValue(numericalRecords, currentValue + amount);
                    Debug.Log($"[InteractionManager] 金錢變化: {amount:+0;-0} (當前: {currentValue + amount})");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[InteractionManager] 無法修改金錢: {e.Message}");
            }
        }
        
        #endregion
        
        #region 公開API
        
        /// <summary>
        /// 檢查特定互動是否可用
        /// </summary>
        public bool IsInteractionAvailable(string interactionType)
        {
            return interactionsEnabled && CheckInteractionConditions(interactionType);
        }
        
        /// <summary>
        /// 獲取可用的互動列表
        /// </summary>
        public List<string> GetAvailableInteractions()
        {
            var available = new List<string>();
            
            if (!interactionsEnabled) return available;
            
            if (interactionConfig != null)
            {
                foreach (var interaction in interactionConfig.GetAllInteractions())
                {
                    if (CheckInteractionConditions(interaction))
                    {
                        available.Add(interaction);
                    }
                }
            }
            
            return available;
        }
        
        /// <summary>
        /// 強制更新互動狀態
        /// </summary>
        public void ForceUpdateInteractionState()
        {
            UpdateInteractionState();
        }
        
        #endregion
        
        #region 事件清理
        
        void OnDestroy()
        {
            // 取消事件訂閱
            if (gameManager != null)
            {
                NewGameManager.OnGameStateChanged -= OnGameStateChanged;
                NewGameManager.OnWorkTimeChanged -= OnWorkTimeChanged;
                NewGameManager.OnTalkingStateChanged -= OnTalkingStateChanged;
            }
            
            // 清理靜態事件
            OnInteractionTriggered = null;
            OnInteractionsEnabledChanged = null;
            OnInteractionCompleted = null;
        }
        
        #endregion
    }

    // Fix for CS0117: 'InteractionResult' 未包含 'InteractionType' 的定義  
    // The error indicates that the `InteractionResult` class does not have a property or field named `InteractionType`.  
    // Based on the context, we need to add the missing property to the `InteractionResult` class.  

    public class InteractionResult
    {
        public string InteractionType { get; set; } // Add this property to resolve the error.  
        public bool Success { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> ChangedValues { get; set; }
        public object Data { get; set; }
    }
}