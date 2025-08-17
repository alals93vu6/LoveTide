using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveTide.Interaction
{
    /// <summary>
    /// 互動條件檢查器 - 檢查各種互動的解鎖條件
    /// 包括好感度、進度、時間、場景等條件的驗證
    /// </summary>
    public class InteractionConditionChecker : MonoBehaviour
    {
        [Header("=== 條件配置 ===")]
        [SerializeField] private List<InteractionCondition> conditionList = new List<InteractionCondition>();
        
        [Header("=== 調試設定 ===")]
        [SerializeField] private bool enableDebugLog = true;
        [SerializeField] private bool bypassAllConditions = false; // 開發用：跳過所有條件檢查
        
        // 數值記錄引用
        private NumericalRecords numericalRecords;
        private LoveTide.Core.NewGameManager gameManager;
        
        // 條件映射
        private Dictionary<string, InteractionCondition> conditionMap = new Dictionary<string, InteractionCondition>();
        
        #region 互動條件結構
        
        [System.Serializable]
        public class InteractionCondition
        {
            [Header("基本信息")]
            public string InteractionName;
            public string Description;
            public bool IsEnabled = true;
            
            [Header("數值條件")]
            public int RequiredAffection = 0;
            public int RequiredProgress = 0;
            public int RequiredMoney = 0;
            public int RequiredDay = 0;
            
            [Header("時間條件")]
            public List<int> AllowedTimeSlots = new List<int>();
            public bool RequireWorkTime = false;
            public bool RequireDormTime = false;
            public bool AllowDuringVacation = true;
            
            [Header("劇情條件")]
            public List<string> RequiredFlags = new List<string>();
            public List<string> ForbiddenFlags = new List<string>();
            
            [Header("冷卻時間")]
            public bool HasCooldown = false;
            public float CooldownHours = 0f;
            public bool CooldownPerDay = false;
            
            public bool IsValid => !string.IsNullOrEmpty(InteractionName);
        }
        
        #endregion
        
        #region Unity生命週期
        
        void Awake()
        {
            InitializeConditions();
        }
        
        void Start()
        {
            InitializeReferences();
        }
        
        #endregion
        
        #region 初始化
        
        void InitializeConditions()
        {
            // 建立條件映射
            conditionMap.Clear();
            
            foreach (var condition in conditionList)
            {
                if (condition.IsValid)
                {
                    conditionMap[condition.InteractionName] = condition;
                }
            }
            
            // 如果沒有預設條件，創建一些基本條件
            if (conditionList.Count == 0)
            {
                CreateDefaultConditions();
            }
        }
        
        void InitializeReferences()
        {
            // 獲取數值記錄引用
            numericalRecords = FindObjectOfType<NumericalRecords>();
            if (numericalRecords == null)
            {
                Debug.LogWarning("[InteractionConditionChecker] 找不到NumericalRecords組件");
            }
            
            // 獲取遊戲管理器引用
            gameManager = LoveTide.Core.NewGameManager.Instance;
            if (gameManager == null)
            {
                Debug.LogWarning("[InteractionConditionChecker] 找不到NewGameManager實例");
            }
        }
        
        void CreateDefaultConditions()
        {
            var defaultConditions = new[]
            {
                new InteractionCondition
                {
                    InteractionName = "NormalTalk",
                    Description = "普通對話",
                    IsEnabled = true
                },
                new InteractionCondition
                {
                    InteractionName = "FlirtTalk",
                    Description = "調情對話",
                    RequiredAffection = 20,
                    IsEnabled = true
                },
                new InteractionCondition
                {
                    InteractionName = "GoOutTogether",
                    Description = "一起外出",
                    RequiredAffection = 30,
                    AllowedTimeSlots = new List<int> { 1, 2, 3, 4, 5, 6 },
                    IsEnabled = true
                },
                new InteractionCondition
                {
                    InteractionName = "SexualInteraction",
                    Description = "親密互動",
                    RequiredAffection = 60,
                    RequiredProgress = 50,
                    IsEnabled = true
                },
                new InteractionCondition
                {
                    InteractionName = "HelpWork",
                    Description = "幫忙工作",
                    RequireWorkTime = true,
                    AllowedTimeSlots = new List<int> { 1, 2, 3, 4, 5, 6 },
                    IsEnabled = true
                },
                new InteractionCondition
                {
                    InteractionName = "InviteDrinking",
                    Description = "邀請喝酒",
                    RequiredAffection = 40,
                    RequireDormTime = true,
                    AllowedTimeSlots = new List<int> { 7, 8, 9 },
                    IsEnabled = true
                }
            };
            
            conditionList.AddRange(defaultConditions);
            InitializeConditions(); // 重新初始化映射
        }
        
        #endregion
        
        #region 條件檢查主要方法
        
        /// <summary>
        /// 檢查互動是否可用
        /// </summary>
        public bool CheckInteraction(string interactionName)
        {
            if (bypassAllConditions)
            {
                if (enableDebugLog)
                    Debug.Log($"[條件檢查] {interactionName} - 跳過所有條件檢查");
                return true;
            }
            
            if (!conditionMap.ContainsKey(interactionName))
            {
                if (enableDebugLog)
                    Debug.LogWarning($"[條件檢查] 找不到互動條件: {interactionName}");
                return true; // 沒有條件限制時預設允許
            }
            
            InteractionCondition condition = conditionMap[interactionName];
            
            if (!condition.IsEnabled)
            {
                if (enableDebugLog)
                    Debug.Log($"[條件檢查] {interactionName} - 互動已禁用");
                return false;
            }
            
            return CheckAllConditions(condition);
        }
        
        /// <summary>
        /// 檢查互動條件並返回詳細結果
        /// </summary>
        public ConditionCheckResult CheckInteractionDetailed(string interactionName)
        {
            var result = new ConditionCheckResult
            {
                InteractionName = interactionName,
                IsAllowed = false,
                FailedConditions = new List<string>()
            };
            
            if (bypassAllConditions)
            {
                result.IsAllowed = true;
                result.BypassedConditions = true;
                return result;
            }
            
            if (!conditionMap.ContainsKey(interactionName))
            {
                result.IsAllowed = true;
                result.NoConditionsFound = true;
                return result;
            }
            
            InteractionCondition condition = conditionMap[interactionName];
            
            if (!condition.IsEnabled)
            {
                result.FailedConditions.Add("互動已禁用");
                return result;
            }
            
            result.IsAllowed = CheckAllConditionsDetailed(condition, result.FailedConditions);
            return result;
        }
        
        #endregion
        
        #region 詳細條件檢查
        
        bool CheckAllConditions(InteractionCondition condition)
        {
            return CheckNumericConditions(condition) &&
                   CheckTimeConditions(condition) &&
                   CheckSceneConditions(condition) &&
                   CheckFlagConditions(condition) &&
                   CheckCooldownConditions(condition);
        }
        
        bool CheckAllConditionsDetailed(InteractionCondition condition, List<string> failedConditions)
        {
            bool result = true;
            
            if (!CheckNumericConditions(condition))
            {
                AddNumericFailureReasons(condition, failedConditions);
                result = false;
            }
            
            if (!CheckTimeConditions(condition))
            {
                AddTimeFailureReasons(condition, failedConditions);
                result = false;
            }
            
            if (!CheckSceneConditions(condition))
            {
                AddSceneFailureReasons(condition, failedConditions);
                result = false;
            }
            
            if (!CheckFlagConditions(condition))
            {
                AddFlagFailureReasons(condition, failedConditions);
                result = false;
            }
            
            if (!CheckCooldownConditions(condition))
            {
                failedConditions.Add("冷卻時間未結束");
                result = false;
            }
            
            return result;
        }
        
        bool CheckNumericConditions(InteractionCondition condition)
        {
            if (numericalRecords == null) return true;
            
            // 檢查好感度
            if (condition.RequiredAffection > 0 && GetAffection() < condition.RequiredAffection)
                return false;
            
            // 檢查進度
            if (condition.RequiredProgress > 0 && GetProgress() < condition.RequiredProgress)
                return false;
            
            // 檢查金錢
            if (condition.RequiredMoney > 0 && GetMoney() < condition.RequiredMoney)
                return false;
            
            // 檢查天數
            if (condition.RequiredDay > 0 && GetDay() < condition.RequiredDay)
                return false;
            
            return true;
        }
        
        bool CheckTimeConditions(InteractionCondition condition)
        {
            if (gameManager == null) return true;
            
            var timeInfo = gameManager.GetCurrentTime();
            
            // 檢查允許的時段
            if (condition.AllowedTimeSlots.Count > 0)
            {
                if (!condition.AllowedTimeSlots.Contains(timeInfo.timer))
                    return false;
            }
            
            // 檢查工作時間要求
            if (condition.RequireWorkTime && !gameManager.IsWorkTime)
                return false;
            
            // 檢查宿舍時間要求
            if (condition.RequireDormTime && gameManager.IsWorkTime)
                return false;
            
            // 檢查假期限制
            if (!condition.AllowDuringVacation && gameManager.IsVacation)
                return false;
            
            return true;
        }
        
        bool CheckSceneConditions(InteractionCondition condition)
        {
            // 這裡可以添加場景相關的條件檢查
            // 例如角色位置、場景狀態等
            return true;
        }
        
        bool CheckFlagConditions(InteractionCondition condition)
        {
            // 檢查必需的旗標
            foreach (string requiredFlag in condition.RequiredFlags)
            {
                if (!CheckFlag(requiredFlag))
                    return false;
            }
            
            // 檢查禁止的旗標
            foreach (string forbiddenFlag in condition.ForbiddenFlags)
            {
                if (CheckFlag(forbiddenFlag))
                    return false;
            }
            
            return true;
        }
        
        bool CheckCooldownConditions(InteractionCondition condition)
        {
            if (!condition.HasCooldown) return true;
            
            // 這裡需要實現冷卻時間檢查邏輯
            // 可以使用PlayerPrefs或其他持久化方式存儲上次使用時間
            string cooldownKey = $"Cooldown_{condition.InteractionName}";
            
            if (PlayerPrefs.HasKey(cooldownKey))
            {
                float lastUsedTime = PlayerPrefs.GetFloat(cooldownKey);
                float currentTime = Time.time;
                float cooldownDuration = condition.CooldownHours * 3600f; // 轉換為秒
                
                if (currentTime - lastUsedTime < cooldownDuration)
                    return false;
            }
            
            return true;
        }
        
        bool CheckFlag(string flagName)
        {
            // 實現旗標檢查邏輯
            // 可以從PlayerPrefs或其他地方獲取旗標狀態
            return PlayerPrefs.GetInt($"Flag_{flagName}", 0) == 1;
        }
        
        #endregion
        
        #region 失敗原因說明
        
        void AddNumericFailureReasons(InteractionCondition condition, List<string> reasons)
        {
            if (numericalRecords == null) return;
            
            int currentAffection = GetAffection();
            int currentProgress = GetProgress();
            int currentMoney = GetMoney();
            int currentDay = GetDay();
            
            if (condition.RequiredAffection > 0 && currentAffection < condition.RequiredAffection)
                reasons.Add($"好感度不足 (需要: {condition.RequiredAffection}, 當前: {currentAffection})");
            
            if (condition.RequiredProgress > 0 && currentProgress < condition.RequiredProgress)
                reasons.Add($"進度不足 (需要: {condition.RequiredProgress}, 當前: {currentProgress})");
            
            if (condition.RequiredMoney > 0 && currentMoney < condition.RequiredMoney)
                reasons.Add($"金錢不足 (需要: {condition.RequiredMoney}, 當前: {currentMoney})");
            
            if (condition.RequiredDay > 0 && currentDay < condition.RequiredDay)
                reasons.Add($"天數不足 (需要: {condition.RequiredDay}, 當前: {currentDay})");
        }
        
        // 輔助方法 - 安全獲取數值
        int GetAffection()
        {
            if (numericalRecords == null) return 0;
            
            // 嘗試通過反射或公開屬性獲取
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
                Debug.LogWarning($"[InteractionConditionChecker] 無法獲取好感度: {e.Message}");
            }
            
            return 0;
        }
        
        int GetProgress()
        {
            if (numericalRecords == null) return 0;
            
            try
            {
                var field = numericalRecords.GetType().GetField("aProgress");
                if (field != null)
                    return (int)field.GetValue(numericalRecords);
                    
                var property = numericalRecords.GetType().GetProperty("aProgress");
                if (property != null)
                    return (int)property.GetValue(numericalRecords);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[InteractionConditionChecker] 無法獲取進度: {e.Message}");
            }
            
            return 0;
        }
        
        int GetMoney()
        {
            if (numericalRecords == null) return 0;
            
            try
            {
                var field = numericalRecords.GetType().GetField("aMoney");
                if (field != null)
                    return (int)field.GetValue(numericalRecords);
                    
                var property = numericalRecords.GetType().GetProperty("aMoney");
                if (property != null)
                    return (int)property.GetValue(numericalRecords);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[InteractionConditionChecker] 無法獲取金錢: {e.Message}");
            }
            
            return 0;
        }
        
        int GetDay()
        {
            if (numericalRecords == null) return 1;
            
            try
            {
                var field = numericalRecords.GetType().GetField("aDay");
                if (field != null)
                    return (int)field.GetValue(numericalRecords);
                    
                var property = numericalRecords.GetType().GetProperty("aDay");
                if (property != null)
                    return (int)property.GetValue(numericalRecords);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[InteractionConditionChecker] 無法獲取天數: {e.Message}");
            }
            
            return 1;
        }
        
        void AddTimeFailureReasons(InteractionCondition condition, List<string> reasons)
        {
            if (gameManager == null) return;
            
            var timeInfo = gameManager.GetCurrentTime();
            
            if (condition.AllowedTimeSlots.Count > 0 && !condition.AllowedTimeSlots.Contains(timeInfo.timer))
                reasons.Add($"時段不符 (當前: {timeInfo.timer}, 允許: {string.Join(",", condition.AllowedTimeSlots)})");
            
            if (condition.RequireWorkTime && !gameManager.IsWorkTime)
                reasons.Add("需要在工作時間");
            
            if (condition.RequireDormTime && gameManager.IsWorkTime)
                reasons.Add("需要在宿舍時間");
            
            if (!condition.AllowDuringVacation && gameManager.IsVacation)
                reasons.Add("假期期間不可用");
        }
        
        void AddSceneFailureReasons(InteractionCondition condition, List<string> reasons)
        {
            // 場景相關的失敗原因
        }
        
        void AddFlagFailureReasons(InteractionCondition condition, List<string> reasons)
        {
            foreach (string requiredFlag in condition.RequiredFlags)
            {
                if (!CheckFlag(requiredFlag))
                    reasons.Add($"缺少必需旗標: {requiredFlag}");
            }
            
            foreach (string forbiddenFlag in condition.ForbiddenFlags)
            {
                if (CheckFlag(forbiddenFlag))
                    reasons.Add($"存在禁止旗標: {forbiddenFlag}");
            }
        }
        
        #endregion
        
        #region 公開API
        
        /// <summary>
        /// 添加或更新互動條件
        /// </summary>
        public void SetInteractionCondition(InteractionCondition condition)
        {
            if (condition.IsValid)
            {
                conditionMap[condition.InteractionName] = condition;
                
                // 更新列表
                var existing = conditionList.Find(c => c.InteractionName == condition.InteractionName);
                if (existing != null)
                {
                    conditionList.Remove(existing);
                }
                conditionList.Add(condition);
            }
        }
        
        /// <summary>
        /// 移除互動條件
        /// </summary>
        public void RemoveInteractionCondition(string interactionName)
        {
            conditionMap.Remove(interactionName);
            conditionList.RemoveAll(c => c.InteractionName == interactionName);
        }
        
        /// <summary>
        /// 設置旗標
        /// </summary>
        public void SetFlag(string flagName, bool value)
        {
            PlayerPrefs.SetInt($"Flag_{flagName}", value ? 1 : 0);
        }
        
        /// <summary>
        /// 記錄互動使用時間（用於冷卻）
        /// </summary>
        public void RecordInteractionUsage(string interactionName)
        {
            PlayerPrefs.SetFloat($"Cooldown_{interactionName}", Time.time);
        }
        
        /// <summary>
        /// 獲取所有可用的互動
        /// </summary>
        public List<string> GetAvailableInteractions()
        {
            var available = new List<string>();
            
            foreach (var kvp in conditionMap)
            {
                if (CheckInteraction(kvp.Key))
                {
                    available.Add(kvp.Key);
                }
            }
            
            return available;
        }
        
        #endregion
        
        #region 結果結構
        
        public class ConditionCheckResult
        {
            public string InteractionName;
            public bool IsAllowed;
            public List<string> FailedConditions;
            public bool BypassedConditions;
            public bool NoConditionsFound;
        }
        
        #endregion
    }
}