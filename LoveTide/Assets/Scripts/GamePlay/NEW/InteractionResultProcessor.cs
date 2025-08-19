using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 互動結果處理器
/// 
/// 職責:
/// 1. 處理各種互動的結果邏輯
/// 2. 更新數值系統(金錢、體力、好感度等)
/// 3. 觸發相應的視覺和音效反饋
/// 4. 與NumericalRecords和TimeManager協作
/// 
/// 基於架構文檔: CoreSystems數據流架構.md
/// 實現互動結果的統一處理和數值更新
/// </summary>
public class InteractionResultProcessor : MonoBehaviour
{
    [Header("== 系統引用 ==")]
    [SerializeField] private NumericalRecords numericalRecords;
    [SerializeField] private TimeManagerTest timeManager;
    
    [Header("== 結果配置 ==")]
    [SerializeField] private InteractionResultConfig resultConfig;
    
    private InteractionManager interactionManager;
    private bool isInitialized = false;
    
    /// <summary>
    /// 初始化結果處理器
    /// </summary>
    public void Initialize(InteractionManager manager)
    {
        interactionManager = manager;
        
        // 查找系統引用
        FindSystemReferences();
        
        // 初始化結果配置
        InitializeResultConfig();
        
        isInitialized = true;
        Debug.Log("[InteractionResultProcessor] 結果處理器初始化完成");
    }
    
    /// <summary>
    /// 查找系統引用
    /// </summary>
    private void FindSystemReferences()
    {
        if (numericalRecords == null)
        {
            numericalRecords = FindObjectOfType<NumericalRecords>();
        }
        
        if (timeManager == null)
        {
            timeManager = FindObjectOfType<TimeManagerTest>();
        }
    }
    
    /// <summary>
    /// 初始化結果配置
    /// </summary>
    private void InitializeResultConfig()
    {
        if (resultConfig == null)
        {
            resultConfig = ScriptableObject.CreateInstance<InteractionResultConfig>();
            // 設置默認結果配置
            SetupDefaultResultConfig();
        }
    }
    
    /// <summary>
    /// 設置默認結果配置
    /// </summary>
    private void SetupDefaultResultConfig()
    {
        Debug.Log("[InteractionResultProcessor] 設置默認互動結果配置");
    }
    
    #region 互動結果處理方法
    
    /// <summary>
    /// 處理工作互動結果
    /// </summary>
    public InteractionResult ProcessWorkInteraction()
    {
        var result = new InteractionResult
        {
            interactionType = InteractionType.Work,
            success = true,
            message = "工作完成",
            changedValues = new Dictionary<string, object>()
        };
        
        // 工作獎勵邏輯
        int moneyGained = resultConfig?.workMoneyReward ?? 50;
        int expGained = resultConfig?.workExperienceReward ?? 10;
        int staminaCost = resultConfig?.workStaminaCost ?? 20;
        
        // 更新數值 (假設NumericalRecords有這些方法)
        if (numericalRecords != null)
        {
            // result.changedValues.Add("money", moneyGained);
            // result.changedValues.Add("experience", expGained);
            // result.changedValues.Add("stamina", -staminaCost);
            
            // 實際更新數值
            // numericalRecords.AddMoney(moneyGained);
            // numericalRecords.AddExperience(expGained);
            // numericalRecords.ConsumeStamina(staminaCost);
        }
        
        // 更新時間 (假設工作需要時間)
        if (timeManager != null)
        {
            int timeHours = resultConfig?.workTimeHours ?? 2;
            // timeManager.AdvanceTime(timeHours);
            result.changedValues.Add("time", timeHours);
        }
        
        Debug.Log("[InteractionResultProcessor] 工作互動處理完成");
        return result;
    }
    
    /// <summary>
    /// 處理貓咪互動結果
    /// </summary>
    public InteractionResult ProcessCatInteraction()
    {
        var result = new InteractionResult
        {
            interactionType = InteractionType.CatInteraction,
            success = true,
            message = "與貓咪互動成功",
            changedValues = new Dictionary<string, object>()
        };
        
        // 貓咪互動邏輯
        int catAffectionGain = resultConfig?.catAffectionReward ?? 5;
        int fishCost = resultConfig?.catFishCost ?? 1;
        
        // 檢查是否有魚
        if (numericalRecords != null)
        {
            // int currentFish = numericalRecords.GetFishCount();
            // if (currentFish >= fishCost)
            // {
            //     numericalRecords.ConsumeFish(fishCost);
            //     numericalRecords.AddCatAffection(catAffectionGain);
                
                result.changedValues.Add("fish", -fishCost);
                result.changedValues.Add("catAffection", catAffectionGain);
                result.message = "餵食貓咪成功，好感度上升";
            // }
            // else
            // {
            //     result.success = false;
            //     result.message = "沒有魚，無法與貓咪互動";
            // }
        }
        
        Debug.Log("[InteractionResultProcessor] 貓咪互動處理完成");
        return result;
    }
    
    /// <summary>
    /// 處理外出互動結果
    /// </summary>
    public InteractionResult ProcessGoOutInteraction()
    {
        var result = new InteractionResult
        {
            interactionType = InteractionType.GoOut,
            success = true,
            message = "外出成功",
            changedValues = new Dictionary<string, object>()
        };
        
        // 外出邏輯
        int timeCost = resultConfig?.goOutTimeHours ?? 1;
        int staminaCost = resultConfig?.goOutStaminaCost ?? 10;
        
        // 更新時間和體力
        if (timeManager != null)
        {
            // timeManager.AdvanceTime(timeCost);
            result.changedValues.Add("time", timeCost);
        }
        
        if (numericalRecords != null)
        {
            // numericalRecords.ConsumeStamina(staminaCost);
            result.changedValues.Add("stamina", -staminaCost);
        }
        
        // 隨機事件邏輯 (可能獲得金錢或物品)
        if (Random.Range(0f, 1f) < 0.3f) // 30%機率獲得獎勵
        {
            int randomMoney = Random.Range(10, 50);
            // numericalRecords.AddMoney(randomMoney);
            result.changedValues.Add("money", randomMoney);
            result.message = $"外出時發現了 {randomMoney} 金幣！";
        }
        
        Debug.Log("[InteractionResultProcessor] 外出互動處理完成");
        return result;
    }
    
    /// <summary>
    /// 處理場景切換互動結果
    /// </summary>
    public InteractionResult ProcessSceneTransitionInteraction()
    {
        var result = new InteractionResult
        {
            interactionType = InteractionType.SceneTransition,
            success = true,
            message = "場景切換成功",
            changedValues = new Dictionary<string, object>()
        };
        
        // 場景切換通常不需要特殊的數值處理
        // 但可能需要記錄場景切換次數或更新某些狀態
        
        Debug.Log("[InteractionResultProcessor] 場景切換處理完成");
        return result;
    }
    
    /// <summary>
    /// 處理休息互動結果
    /// </summary>
    public InteractionResult ProcessRestInteraction()
    {
        var result = new InteractionResult
        {
            interactionType = InteractionType.Rest,
            success = true,
            message = "休息完成",
            changedValues = new Dictionary<string, object>()
        };
        
        // 休息邏輯
        int staminaRestore = resultConfig?.restStaminaRestore ?? 50;
        int timeAdvance = resultConfig?.restTimeHours ?? 8;
        
        // 恢復體力和推進時間
        if (numericalRecords != null)
        {
            // numericalRecords.RestoreStamina(staminaRestore);
            result.changedValues.Add("stamina", staminaRestore);
        }
        
        if (timeManager != null)
        {
            // timeManager.AdvanceTime(timeAdvance);
            result.changedValues.Add("time", timeAdvance);
        }
        
        Debug.Log("[InteractionResultProcessor] 休息互動處理完成");
        return result;
    }
    
    /// <summary>
    /// 處理邀請喝酒互動結果
    /// </summary>
    public InteractionResult ProcessDrinkingInviteInteraction()
    {
        var result = new InteractionResult
        {
            interactionType = InteractionType.DrinkingInvite,
            success = true,
            message = "邀請喝酒成功",
            changedValues = new Dictionary<string, object>()
        };
        
        // 喝酒邏輯
        int affectionGain = resultConfig?.drinkingAffectionReward ?? 10;
        int alcoholCost = resultConfig?.drinkingAlcoholCost ?? 1;
        int timeAdvance = resultConfig?.drinkingTimeHours ?? 2;
        
        // 更新好感度、消耗酒類、推進時間
        if (numericalRecords != null)
        {
            // numericalRecords.AddAffection(affectionGain);
            // numericalRecords.ConsumeAlcohol(alcoholCost);
            result.changedValues.Add("affection", affectionGain);
            result.changedValues.Add("alcohol", -alcoholCost);
        }
        
        if (timeManager != null)
        {
            // timeManager.AdvanceTime(timeAdvance);
            result.changedValues.Add("time", timeAdvance);
        }
        
        Debug.Log("[InteractionResultProcessor] 邀請喝酒互動處理完成");
        return result;
    }
    
    #endregion
    
    #region 輔助方法
    
    /// <summary>
    /// 應用互動結果到遊戲系統
    /// </summary>
    public void ApplyInteractionResult(InteractionResult result)
    {
        if (!result.success)
        {
            Debug.LogWarning($"[InteractionResultProcessor] 互動失敗: {result.message}");
            return;
        }
        
        // 這裡可以添加統一的結果應用邏輯
        // 例如：播放音效、顯示UI反饋、觸發特效等
        
        // 播放成功音效
        // AudioManager.Instance.PlaySFX("interaction_success");
        
        // 顯示數值變化UI
        ShowValueChangesFeedback(result.changedValues);
        
        Debug.Log($"[InteractionResultProcessor] 應用互動結果: {result.interactionType}");
    }
    
    /// <summary>
    /// 顯示數值變化反饋
    /// </summary>
    private void ShowValueChangesFeedback(Dictionary<string, object> changedValues)
    {
        foreach (var change in changedValues)
        {
            Debug.Log($"[InteractionResultProcessor] 數值變化: {change.Key} {change.Value}");
            
            // 這裡可以觸發UI顯示數值變化
            // UIManager.Instance.ShowValueChange(change.Key, change.Value);
        }
    }
    
    /// <summary>
    /// 檢查互動結果的有效性
    /// </summary>
    public bool ValidateInteractionResult(InteractionResult result)
    {
        // 檢查結果是否合理
        // 例如：數值變化是否在合理範圍內
        
        return result != null && result.changedValues != null;
    }
    
    #endregion
}

/// <summary>
/// 互動結果配置 (ScriptableObject)
/// </summary>
[CreateAssetMenu(fileName = "InteractionResultConfig", menuName = "LoveTide/InteractionResultConfig")]
public class InteractionResultConfig : ScriptableObject
{
    [Header("工作獎勵")]
    public int workMoneyReward = 50;
    public int workExperienceReward = 10;
    public int workStaminaCost = 20;
    public int workTimeHours = 2;
    
    [Header("貓咪互動")]
    public int catAffectionReward = 5;
    public int catFishCost = 1;
    
    [Header("外出設定")]
    public int goOutTimeHours = 1;
    public int goOutStaminaCost = 10;
    
    [Header("休息恢復")]
    public int restStaminaRestore = 50;
    public int restTimeHours = 8;
    
    [Header("喝酒邀請")]
    public int drinkingAffectionReward = 10;
    public int drinkingAlcoholCost = 1;
    public int drinkingTimeHours = 2;
}