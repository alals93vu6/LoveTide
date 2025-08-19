using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// 數據驗證器
/// 
/// 職責:
/// 1. 驗證遊戲數據的完整性和有效性
/// 2. 提供數據格式檢查和範圍驗證
/// 3. 檢測數據異常和潛在問題
/// 4. 支援自定義驗證規則
/// 
/// 基於架構文檔: 確保數據的一致性和安全性
/// 防止無效數據破壞遊戲狀態
/// </summary>
public class DataValidator : MonoBehaviour
{
    [Header("== 驗證配置 ==")]
    [SerializeField] private bool enableValidation = true;
    [SerializeField] private bool enableDebugLog = false;
    [SerializeField] private ValidationLevel validationLevel = ValidationLevel.Normal;
    
    [Header("== 數值範圍 ==")]
    [SerializeField] private ValueRange moneyRange = new ValueRange(0, 999999);
    [SerializeField] private ValueRange affectionRange = new ValueRange(0, 100);
    [SerializeField] private ValueRange experienceRange = new ValueRange(0, 9999);
    
    // 驗證規則字典
    private Dictionary<string, ValidationRule> validationRules = new Dictionary<string, ValidationRule>();
    
    // 驗證結果緩存
    private Dictionary<string, ValidationResult> resultCache = new Dictionary<string, ValidationResult>();
    
    // 單例模式
    public static DataValidator Instance { get; private set; }
    
    public bool EnableValidation => enableValidation;
    public ValidationLevel CurrentLevel => validationLevel;
    
    void Awake()
    {
        // 單例設置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 初始化數據驗證器
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[DataValidator] 初始化數據驗證器");
        
        // 設置默認驗證規則
        SetupDefaultValidationRules();
        
        Debug.Log("[DataValidator] 數據驗證器初始化完成");
    }
    
    /// <summary>
    /// 設置默認驗證規則
    /// </summary>
    private void SetupDefaultValidationRules()
    {
        // 金錢驗證
        AddValidationRule("money", new ValidationRule
        {
            name = "money",
            validationType = ValidationType.NumericRange,
            minValue = moneyRange.min,
            maxValue = moneyRange.max,
            isRequired = true,
            errorMessage = "金錢數值超出有效範圍"
        });
        
        // 好感度驗證
        AddValidationRule("affection", new ValidationRule
        {
            name = "affection",
            validationType = ValidationType.NumericRange,
            minValue = affectionRange.min,
            maxValue = affectionRange.max,
            isRequired = true,
            errorMessage = "好感度數值超出有效範圍"
        });
        
        // 經驗值驗證
        AddValidationRule("experience", new ValidationRule
        {
            name = "experience",
            validationType = ValidationType.NumericRange,
            minValue = experienceRange.min,
            maxValue = experienceRange.max,
            isRequired = true,
            errorMessage = "經驗值超出有效範圍"
        });
        
        // 角色名稱驗證
        AddValidationRule("characterName", new ValidationRule
        {
            name = "characterName",
            validationType = ValidationType.StringPattern,
            pattern = @"^[a-zA-Z\u4e00-\u9fa5]{1,20}$",
            isRequired = true,
            errorMessage = "角色名稱格式無效"
        });
        
        // 存檔名稱驗證
        AddValidationRule("saveName", new ValidationRule
        {
            name = "saveName",
            validationType = ValidationType.StringLength,
            minLength = 1,
            maxLength = 50,
            isRequired = true,
            errorMessage = "存檔名稱長度無效"
        });
        
        // 場景名稱驗證
        AddValidationRule("sceneName", new ValidationRule
        {
            name = "sceneName",
            validationType = ValidationType.EnumValue,
            allowedValues = new string[] { "tavern", "dormitory" },
            isRequired = true,
            errorMessage = "場景名稱無效"
        });
        
        // 互動類型驗證
        AddValidationRule("interactionType", new ValidationRule
        {
            name = "interactionType",
            validationType = ValidationType.EnumValue,
            allowedValues = new string[] { "CharacterTalk", "Work", "CatInteraction", "GoOut", "Rest", "DrinkingInvite" },
            isRequired = true,
            errorMessage = "互動類型無效"
        });
        
        LogValidationRulesSetup();
    }
    
    /// <summary>
    /// 記錄驗證規則設置
    /// </summary>
    private void LogValidationRulesSetup()
    {
        if (enableDebugLog)
        {
            Debug.Log($"[DataValidator] 設置了 {validationRules.Count} 個驗證規則:");
            foreach (var rule in validationRules.Values)
            {
                Debug.Log($"  - {rule.name}: {rule.validationType}");
            }
        }
    }
    
    #region 驗證規則管理
    
    /// <summary>
    /// 添加驗證規則
    /// </summary>
    public void AddValidationRule(string key, ValidationRule rule)
    {
        validationRules[key] = rule;
        
        if (enableDebugLog)
        {
            Debug.Log($"[DataValidator] 添加驗證規則: {key}");
        }
    }
    
    /// <summary>
    /// 移除驗證規則
    /// </summary>
    public void RemoveValidationRule(string key)
    {
        if (validationRules.ContainsKey(key))
        {
            validationRules.Remove(key);
            
            if (enableDebugLog)
            {
                Debug.Log($"[DataValidator] 移除驗證規則: {key}");
            }
        }
    }
    
    /// <summary>
    /// 獲取驗證規則
    /// </summary>
    public ValidationRule GetValidationRule(string key)
    {
        return validationRules.ContainsKey(key) ? validationRules[key] : null;
    }
    
    #endregion
    
    #region 數據驗證方法
    
    /// <summary>
    /// 驗證單個數值
    /// </summary>
    public ValidationResult ValidateValue(string key, object value)
    {
        if (!enableValidation)
        {
            return new ValidationResult { isValid = true };
        }
        
        // 檢查緩存
        string cacheKey = $"{key}_{value}";
        if (resultCache.ContainsKey(cacheKey))
        {
            return resultCache[cacheKey];
        }
        
        ValidationResult result = new ValidationResult { isValid = true };
        
        if (!validationRules.ContainsKey(key))
        {
            if (enableDebugLog)
            {
                Debug.LogWarning($"[DataValidator] 沒有找到驗證規則: {key}");
            }
            return result;
        }
        
        ValidationRule rule = validationRules[key];
        result = ValidateByRule(value, rule);
        
        // 緩存結果
        resultCache[cacheKey] = result;
        
        if (enableDebugLog && !result.isValid)
        {
            Debug.LogWarning($"[DataValidator] 驗證失敗: {key} = {value}, 錯誤: {result.errorMessage}");
        }
        
        return result;
    }
    
    /// <summary>
    /// 根據規則驗證
    /// </summary>
    private ValidationResult ValidateByRule(object value, ValidationRule rule)
    {
        ValidationResult result = new ValidationResult { isValid = true };
        
        // 檢查必填
        if (rule.isRequired && (value == null || string.IsNullOrEmpty(value.ToString())))
        {
            result.isValid = false;
            result.errorMessage = $"{rule.name} 為必填項目";
            return result;
        }
        
        if (value == null)
        {
            return result;
        }
        
        switch (rule.validationType)
        {
            case ValidationType.NumericRange:
                result = ValidateNumericRange(value, rule);
                break;
                
            case ValidationType.StringLength:
                result = ValidateStringLength(value, rule);
                break;
                
            case ValidationType.StringPattern:
                result = ValidateStringPattern(value, rule);
                break;
                
            case ValidationType.EnumValue:
                result = ValidateEnumValue(value, rule);
                break;
                
            case ValidationType.Custom:
                result = ValidateCustom(value, rule);
                break;
        }
        
        if (!result.isValid && string.IsNullOrEmpty(result.errorMessage))
        {
            result.errorMessage = rule.errorMessage;
        }
        
        return result;
    }
    
    /// <summary>
    /// 驗證數值範圍
    /// </summary>
    private ValidationResult ValidateNumericRange(object value, ValidationRule rule)
    {
        ValidationResult result = new ValidationResult { isValid = true };
        
        if (!IsNumericType(value))
        {
            result.isValid = false;
            result.errorMessage = "數值類型不正確";
            return result;
        }
        
        double numValue = Convert.ToDouble(value);
        
        if (numValue < rule.minValue || numValue > rule.maxValue)
        {
            result.isValid = false;
            result.errorMessage = $"數值 {numValue} 超出範圍 [{rule.minValue}, {rule.maxValue}]";
        }
        
        return result;
    }
    
    /// <summary>
    /// 驗證字串長度
    /// </summary>
    private ValidationResult ValidateStringLength(object value, ValidationRule rule)
    {
        ValidationResult result = new ValidationResult { isValid = true };
        
        string strValue = value.ToString();
        
        if (strValue.Length < rule.minLength || strValue.Length > rule.maxLength)
        {
            result.isValid = false;
            result.errorMessage = $"字串長度 {strValue.Length} 超出範圍 [{rule.minLength}, {rule.maxLength}]";
        }
        
        return result;
    }
    
    /// <summary>
    /// 驗證字串模式
    /// </summary>
    private ValidationResult ValidateStringPattern(object value, ValidationRule rule)
    {
        ValidationResult result = new ValidationResult { isValid = true };
        
        string strValue = value.ToString();
        
        if (!string.IsNullOrEmpty(rule.pattern))
        {
            if (!Regex.IsMatch(strValue, rule.pattern))
            {
                result.isValid = false;
                result.errorMessage = $"字串 '{strValue}' 不符合模式 '{rule.pattern}'";
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// 驗證枚舉值
    /// </summary>
    private ValidationResult ValidateEnumValue(object value, ValidationRule rule)
    {
        ValidationResult result = new ValidationResult { isValid = true };
        
        string strValue = value.ToString();
        
        if (rule.allowedValues != null && rule.allowedValues.Length > 0)
        {
            bool isValid = false;
            foreach (string allowedValue in rule.allowedValues)
            {
                if (strValue.Equals(allowedValue, StringComparison.OrdinalIgnoreCase))
                {
                    isValid = true;
                    break;
                }
            }
            
            if (!isValid)
            {
                result.isValid = false;
                result.errorMessage = $"值 '{strValue}' 不在允許的範圍內: [{string.Join(", ", rule.allowedValues)}]";
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// 自定義驗證
    /// </summary>
    private ValidationResult ValidateCustom(object value, ValidationRule rule)
    {
        ValidationResult result = new ValidationResult { isValid = true };
        
        if (rule.customValidator != null)
        {
            result = rule.customValidator(value);
        }
        
        return result;
    }
    
    /// <summary>
    /// 批量驗證數據
    /// </summary>
    public ValidationResult ValidateData(Dictionary<string, object> data)
    {
        ValidationResult overallResult = new ValidationResult { isValid = true };
        List<string> errors = new List<string>();
        
        foreach (var kvp in data)
        {
            ValidationResult result = ValidateValue(kvp.Key, kvp.Value);
            if (!result.isValid)
            {
                overallResult.isValid = false;
                errors.Add(result.errorMessage);
            }
        }
        
        if (!overallResult.isValid)
        {
            overallResult.errorMessage = string.Join("; ", errors);
        }
        
        return overallResult;
    }
    
    /// <summary>
    /// 驗證遊戲數據
    /// </summary>
    public ValidationResult ValidateGameData(GameData gameData)
    {
        if (gameData == null)
        {
            return new ValidationResult
            {
                isValid = false,
                errorMessage = "遊戲數據為空"
            };
        }
        
        Dictionary<string, object> dataToValidate = new Dictionary<string, object>
        {
            { "money", gameData.gameValues?.playerMoney ?? 0 },
            { "affection", gameData.gameProgress?.affectionLevel ?? 0 },
            { "experience", gameData.gameValues?.workExperience ?? 0 },
            { "sceneName", gameData.gameProgress?.currentScene ?? "" }
        };
        
        return ValidateData(dataToValidate);
    }
    
    #endregion
    
    #region 輔助方法
    
    /// <summary>
    /// 檢查是否為數值類型
    /// </summary>
    private bool IsNumericType(object value)
    {
        return value is int || value is float || value is double || value is decimal || value is long || value is short;
    }
    
    /// <summary>
    /// 清除驗證緩存
    /// </summary>
    public void ClearValidationCache()
    {
        resultCache.Clear();
        
        if (enableDebugLog)
        {
            Debug.Log("[DataValidator] 驗證緩存已清除");
        }
    }
    
    /// <summary>
    /// 設置驗證模式
    /// </summary>
    public void SetValidationEnabled(bool enabled)
    {
        enableValidation = enabled;
        
        if (!enabled)
        {
            ClearValidationCache();
        }
        
        Debug.Log($"[DataValidator] 驗證模式: {(enabled ? "啟用" : "禁用")}");
    }
    
    /// <summary>
    /// 設置驗證等級
    /// </summary>
    public void SetValidationLevel(ValidationLevel level)
    {
        validationLevel = level;
        
        // 根據等級調整驗證嚴格度
        switch (level)
        {
            case ValidationLevel.Strict:
                // 嚴格模式：啟用所有驗證
                enableValidation = true;
                break;
                
            case ValidationLevel.Normal:
                // 普通模式：啟用基本驗證
                enableValidation = true;
                break;
                
            case ValidationLevel.Lenient:
                // 寬鬆模式：只驗證關鍵數據
                enableValidation = true;
                break;
                
            case ValidationLevel.Disabled:
                // 禁用驗證
                enableValidation = false;
                break;
        }
        
        Debug.Log($"[DataValidator] 驗證等級設置為: {level}");
    }
    
    #endregion
    
    #region 特殊驗證方法
    
    /// <summary>
    /// 驗證存檔數據完整性
    /// </summary>
    public ValidationResult ValidateSaveDataIntegrity(string jsonData)
    {
        ValidationResult result = new ValidationResult { isValid = true };
        
        try
        {
            // 檢查JSON格式
            GameData gameData = JsonUtility.FromJson<GameData>(jsonData);
            
            if (gameData == null)
            {
                result.isValid = false;
                result.errorMessage = "無法解析存檔數據";
                return result;
            }
            
            // 驗證數據完整性
            result = ValidateGameData(gameData);
            
        }
        catch (Exception e)
        {
            result.isValid = false;
            result.errorMessage = $"存檔數據驗證失敗: {e.Message}";
        }
        
        return result;
    }
    
    /// <summary>
    /// 驗證互動參數
    /// </summary>
    public ValidationResult ValidateInteractionParameters(InteractionType interactionType, Dictionary<string, object> parameters)
    {
        ValidationResult result = new ValidationResult { isValid = true };
        
        // 驗證互動類型
        ValidationResult typeResult = ValidateValue("interactionType", interactionType.ToString());
        if (!typeResult.isValid)
        {
            return typeResult;
        }
        
        // 根據互動類型驗證特定參數
        switch (interactionType)
        {
            case InteractionType.Work:
                result = ValidateWorkParameters(parameters);
                break;
                
            case InteractionType.CatInteraction:
                result = ValidateCatInteractionParameters(parameters);
                break;
                
            // 其他互動類型的驗證...
        }
        
        return result;
    }
    
    /// <summary>
    /// 驗證工作參數
    /// </summary>
    private ValidationResult ValidateWorkParameters(Dictionary<string, object> parameters)
    {
        ValidationResult result = new ValidationResult { isValid = true };
        
        // 檢查必要參數
        if (!parameters.ContainsKey("duration") || !parameters.ContainsKey("reward"))
        {
            result.isValid = false;
            result.errorMessage = "工作參數缺少必要項目";
            return result;
        }
        
        // 驗證工作時間
        object duration = parameters["duration"];
        if (!IsNumericType(duration) || Convert.ToDouble(duration) <= 0)
        {
            result.isValid = false;
            result.errorMessage = "工作時間無效";
            return result;
        }
        
        // 驗證獎勵
        object reward = parameters["reward"];
        if (!IsNumericType(reward) || Convert.ToDouble(reward) < 0)
        {
            result.isValid = false;
            result.errorMessage = "工作獎勵無效";
            return result;
        }
        
        return result;
    }
    
    /// <summary>
    /// 驗證貓咪互動參數
    /// </summary>
    private ValidationResult ValidateCatInteractionParameters(Dictionary<string, object> parameters)
    {
        ValidationResult result = new ValidationResult { isValid = true };
        
        // 檢查是否有魚
        if (parameters.ContainsKey("fishCount"))
        {
            object fishCount = parameters["fishCount"];
            if (!IsNumericType(fishCount) || Convert.ToInt32(fishCount) < 0)
            {
                result.isValid = false;
                result.errorMessage = "魚的數量無效";
                return result;
            }
        }
        
        return result;
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 獲取驗證統計
    /// </summary>
    public ValidationStats GetValidationStats()
    {
        return new ValidationStats
        {
            totalRules = validationRules.Count,
            cacheSize = resultCache.Count,
            validationLevel = validationLevel,
            isEnabled = enableValidation
        };
    }
    
    #endregion
}

/// <summary>
/// 驗證類型
/// </summary>
public enum ValidationType
{
    NumericRange,   // 數值範圍
    StringLength,   // 字串長度
    StringPattern,  // 字串模式
    EnumValue,      // 枚舉值
    Custom          // 自定義
}

/// <summary>
/// 驗證等級
/// </summary>
public enum ValidationLevel
{
    Disabled,   // 禁用
    Lenient,    // 寬鬆
    Normal,     // 普通
    Strict      // 嚴格
}

/// <summary>
/// 驗證規則
/// </summary>
[System.Serializable]
public class ValidationRule
{
    public string name;
    public ValidationType validationType;
    public bool isRequired;
    
    // 數值範圍
    public double minValue;
    public double maxValue;
    
    // 字串長度
    public int minLength;
    public int maxLength;
    
    // 字串模式
    public string pattern;
    
    // 枚舉值
    public string[] allowedValues;
    
    // 錯誤消息
    public string errorMessage;
    
    // 自定義驗證器
    public System.Func<object, ValidationResult> customValidator;
}

/// <summary>
/// 驗證結果
/// </summary>
[System.Serializable]
public class ValidationResult
{
    public bool isValid;
    public string errorMessage;
    public string fieldName;
    public object invalidValue;
}

/// <summary>
/// 數值範圍
/// </summary>
[System.Serializable]
public class ValueRange
{
    public double min;
    public double max;
    
    public ValueRange(double min, double max)
    {
        this.min = min;
        this.max = max;
    }
}

/// <summary>
/// 驗證統計
/// </summary>
[System.Serializable]
public class ValidationStats
{
    public int totalRules;
    public int cacheSize;
    public ValidationLevel validationLevel;
    public bool isEnabled;
}