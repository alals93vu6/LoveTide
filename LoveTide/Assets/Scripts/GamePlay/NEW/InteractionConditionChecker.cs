using UnityEngine;

/// <summary>
/// 互動條件檢查器
/// 
/// 職責:
/// 1. 檢查各種互動的觸發條件
/// 2. 整合時間系統、數值系統的條件判斷
/// 3. 提供靈活的條件檢查機制
/// 
/// 基於架構文檔: CoreSystems數據流架構.md
/// 與NumericalRecords和TimeManager協作
/// </summary>
public class InteractionConditionChecker : MonoBehaviour
{
    [Header("== 系統引用 ==")]
    [SerializeField] private TimeManagerTest timeManager;
    [SerializeField] private NumericalRecords numericalRecords;
    
    [Header("== 條件配置 ==")]
    [SerializeField] private InteractionConditionConfig conditionConfig;
    
    private InteractionManager interactionManager;
    private bool isInitialized = false;
    
    /// <summary>
    /// 初始化條件檢查器
    /// </summary>
    public void Initialize(InteractionManager manager)
    {
        interactionManager = manager;
        
        // 查找系統引用
        FindSystemReferences();
        
        // 初始化條件配置
        InitializeConditionConfig();
        
        isInitialized = true;
        Debug.Log("[InteractionConditionChecker] 條件檢查器初始化完成");
    }
    
    /// <summary>
    /// 查找系統引用
    /// </summary>
    private void FindSystemReferences()
    {
        if (timeManager == null)
        {
            timeManager = FindObjectOfType<TimeManagerTest>();
        }
        
        if (numericalRecords == null)
        {
            numericalRecords = FindObjectOfType<NumericalRecords>();
        }
    }
    
    /// <summary>
    /// 初始化條件配置
    /// </summary>
    private void InitializeConditionConfig()
    {
        if (conditionConfig == null)
        {
            conditionConfig = ScriptableObject.CreateInstance<InteractionConditionConfig>();
            // 設置默認條件
            SetupDefaultConditions();
        }
    }
    
    /// <summary>
    /// 設置默認條件
    /// </summary>
    private void SetupDefaultConditions()
    {
        // 在實際項目中，這些條件應該從配置文件或ScriptableObject載入
        Debug.Log("[InteractionConditionChecker] 設置默認互動條件");
    }
    
    #region 條件檢查方法
    
    /// <summary>
    /// 檢查互動條件
    /// </summary>
    public bool CheckInteractionCondition(InteractionType interactionType)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[InteractionConditionChecker] 檢查器未初始化");
            return false;
        }
        
        switch (interactionType)
        {
            case InteractionType.CharacterTalk:
                return CheckCharacterTalkCondition();
                
            case InteractionType.Work:
                return CheckWorkCondition();
                
            case InteractionType.CatInteraction:
                return CheckCatInteractionCondition();
                
            case InteractionType.GoOut:
                return CheckGoOutCondition();
                
            case InteractionType.Rest:
                return CheckRestCondition();
                
            case InteractionType.DrinkingInvite:
                return CheckDrinkingInviteCondition();
                
            case InteractionType.SceneTransition:
                return CheckSceneTransitionCondition();
                
            default:
                Debug.LogWarning($"[InteractionConditionChecker] 未定義的互動類型條件檢查: {interactionType}");
                return true; // 默認允許
        }
    }
    
    /// <summary>
    /// 檢查角色對話條件
    /// </summary>
    private bool CheckCharacterTalkCondition()
    {
        // 角色對話通常沒有特殊限制
        return true;
    }
    
    /// <summary>
    /// 檢查工作條件
    /// </summary>
    private bool CheckWorkCondition()
    {
        // 檢查時間條件 - 工作時間通常是白天
        if (timeManager != null)
        {
            // 假設TimeManager有獲取當前時間的方法
            // int currentHour = timeManager.GetCurrentHour();
            // return currentHour >= 8 && currentHour <= 18; // 8點到18點可以工作
        }
        
        // 檢查體力條件
        if (numericalRecords != null)
        {
            // 假設NumericalRecords有體力值
            // int stamina = numericalRecords.GetStamina();
            // return stamina >= 10; // 至少需要10點體力工作
        }
        
        // 暫時返回true
        return true;
    }
    
    /// <summary>
    /// 檢查貓咪互動條件
    /// </summary>
    private bool CheckCatInteractionCondition()
    {
        // 檢查是否有魚 (釣魚→餵食→嚕貓連動機制)
        if (numericalRecords != null)
        {
            // 假設NumericalRecords有魚的數量
            // int fishCount = numericalRecords.GetFishCount();
            // return fishCount > 0; // 需要有魚才能和貓互動
        }
        
        // 檢查與貓的好感度條件
        // int catAffection = numericalRecords.GetCatAffection();
        // return catAffection >= 0; // 貓不討厭玩家
        
        // 暫時返回true
        return true;
    }
    
    /// <summary>
    /// 檢查外出條件
    /// </summary>
    private bool CheckGoOutCondition()
    {
        // 檢查時間條件 - 某些時間段不能外出
        if (timeManager != null)
        {
            // 假設深夜不能外出
            // int currentHour = timeManager.GetCurrentHour();
            // return currentHour >= 6 && currentHour <= 22; // 6點到22點可以外出
        }
        
        // 檢查是否有正在進行的任務
        // bool hasActiveTasks = CheckActiveTasks();
        // return !hasActiveTasks; // 沒有進行中的任務才能外出
        
        // 暫時返回true
        return true;
    }
    
    /// <summary>
    /// 檢查休息條件
    /// </summary>
    private bool CheckRestCondition()
    {
        // 檢查時間條件 - 通常晚上才能休息
        if (timeManager != null)
        {
            // 假設只有晚上才能休息
            // int currentHour = timeManager.GetCurrentHour();
            // return currentHour >= 20 || currentHour <= 6; // 晚上8點到早上6點可以休息
        }
        
        // 檢查疲勞度
        if (numericalRecords != null)
        {
            // 假設疲勞度達到一定程度才能休息
            // int fatigue = numericalRecords.GetFatigue();
            // return fatigue >= 30; // 疲勞度30以上才能休息
        }
        
        // 暫時返回true
        return true;
    }
    
    /// <summary>
    /// 檢查邀請喝酒條件
    /// </summary>
    private bool CheckDrinkingInviteCondition()
    {
        // 檢查時間條件 - 通常晚上才能喝酒
        if (timeManager != null)
        {
            // 假設只有晚上才能喝酒
            // int currentHour = timeManager.GetCurrentHour();
            // return currentHour >= 18; // 晚上6點後才能喝酒
        }
        
        // 檢查好感度條件
        if (numericalRecords != null)
        {
            // 假設需要一定好感度才能邀請喝酒
            // int affection = numericalRecords.GetAffection();
            // return affection >= 50; // 好感度50以上才能邀請喝酒
        }
        
        // 檢查是否有酒
        // int alcoholCount = numericalRecords.GetAlcoholCount();
        // return alcoholCount > 0; // 需要有酒才能邀請
        
        // 暫時返回true
        return true;
    }
    
    /// <summary>
    /// 檢查場景切換條件
    /// </summary>
    private bool CheckSceneTransitionCondition()
    {
        // 檢查是否有正在進行的互動
        if (interactionManager != null && interactionManager.CurrentInteraction != InteractionType.None)
        {
            Debug.Log("[InteractionConditionChecker] 有正在進行的互動，不能切換場景");
            return false;
        }
        
        // 檢查場景切換的時間限制
        // 一般情況下場景切換沒有特殊限制
        return true;
    }
    
    #endregion
    
    #region 輔助方法
    
    /// <summary>
    /// 檢查是否有進行中的任務
    /// </summary>
    private bool CheckActiveTasks()
    {
        // 實現檢查進行中任務的邏輯
        // 這裡需要與任務系統集成
        return false;
    }
    
    /// <summary>
    /// 獲取條件檢查結果的詳細信息
    /// </summary>
    public InteractionConditionResult GetConditionResult(InteractionType interactionType)
    {
        bool canInteract = CheckInteractionCondition(interactionType);
        string reason = "";
        
        if (!canInteract)
        {
            reason = GetConditionFailureReason(interactionType);
        }
        
        return new InteractionConditionResult
        {
            canInteract = canInteract,
            interactionType = interactionType,
            failureReason = reason
        };
    }
    
    /// <summary>
    /// 獲取條件失敗的原因
    /// </summary>
    private string GetConditionFailureReason(InteractionType interactionType)
    {
        // 根據具體的條件檢查邏輯返回失敗原因
        switch (interactionType)
        {
            case InteractionType.Work:
                return "體力不足或不在工作時間";
            case InteractionType.CatInteraction:
                return "沒有魚或貓咪好感度不足";
            case InteractionType.GoOut:
                return "時間太晚或有未完成的任務";
            case InteractionType.Rest:
                return "疲勞度不足或不在休息時間";
            case InteractionType.DrinkingInvite:
                return "時間太早、好感度不足或沒有酒";
            default:
                return "未知原因";
        }
    }
    
    #endregion
}

/// <summary>
/// 互動條件配置 (ScriptableObject)
/// </summary>
[CreateAssetMenu(fileName = "InteractionConditionConfig", menuName = "LoveTide/InteractionConditionConfig")]
public class InteractionConditionConfig : ScriptableObject
{
    [Header("工作條件")]
    public int minWorkHour = 8;
    public int maxWorkHour = 18;
    public int minWorkStamina = 10;
    
    [Header("貓咪互動條件")]
    public int minCatAffection = 0;
    public int minFishCount = 1;
    
    [Header("外出條件")]
    public int minGoOutHour = 6;
    public int maxGoOutHour = 22;
    
    [Header("休息條件")]
    public int minRestHour = 20;
    public int maxRestHour = 6;
    public int minRestFatigue = 30;
    
    [Header("喝酒條件")]
    public int minDrinkingHour = 18;
    public int minDrinkingAffection = 50;
    public int minAlcoholCount = 1;
}

/// <summary>
/// 互動條件結果
/// </summary>
[System.Serializable]
public class InteractionConditionResult
{
    public bool canInteract;
    public InteractionType interactionType;
    public string failureReason;
}