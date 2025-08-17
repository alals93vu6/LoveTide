using System;
using UnityEngine;
using UnityEngine.UI;

namespace LoveTide.UI
{
    /// <summary>
    /// 狀態顯示控制器 - 管理角色狀態、金錢、好感度等數值的UI顯示
    /// </summary>
    public class StatusDisplayController : MonoBehaviour
    {
        [Header("=== 數值顯示UI ===")]
        [SerializeField] private Text moneyText;
        [SerializeField] private Text affectionText;
        [SerializeField] private Text progressText;
        [SerializeField] private Text healthText;
        
        [Header("=== 進度條UI ===")]
        [SerializeField] private Slider affectionSlider;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Slider healthSlider;
        
        [Header("=== 顯示格式 ===")]
        [SerializeField] private string moneyFormat = "💰 {0:N0}";
        [SerializeField] private string affectionFormat = "💖 {0}";
        [SerializeField] private string progressFormat = "📈 {0}%";
        [SerializeField] private string healthFormat = "❤️ {0}";
        
        [Header("=== 數值範圍 ===")]
        [SerializeField] private int maxAffection = 100;
        [SerializeField] private int maxProgress = 100;
        [SerializeField] private int maxHealth = 100;
        
        // 當前數值
        private int currentMoney = 0;
        private int currentAffection = 0;
        private int currentProgress = 0;
        private int currentHealth = 100;
        
        // 核心管理器引用
        private NumericalRecords numericalRecords;
        
        #region Unity生命週期
        
        void Awake()
        {
            InitializeComponents();
        }
        
        void Start()
        {
            InitializeStatusDisplay();
        }
        
        void Update()
        {
            UpdateStatusDisplay();
        }
        
        #endregion
        
        #region 初始化
        
        void InitializeComponents()
        {
            // 自動尋找UI組件
            if (moneyText == null)
                moneyText = transform.Find("MoneyText")?.GetComponent<Text>();
                
            if (affectionText == null)
                affectionText = transform.Find("AffectionText")?.GetComponent<Text>();
                
            if (progressText == null)
                progressText = transform.Find("ProgressText")?.GetComponent<Text>();
                
            if (healthText == null)
                healthText = transform.Find("HealthText")?.GetComponent<Text>();
                
            // 進度條
            if (affectionSlider == null)
                affectionSlider = transform.Find("AffectionSlider")?.GetComponent<Slider>();
                
            if (progressSlider == null)
                progressSlider = transform.Find("ProgressSlider")?.GetComponent<Slider>();
                
            if (healthSlider == null)
                healthSlider = transform.Find("HealthSlider")?.GetComponent<Slider>();
        }
        
        void InitializeStatusDisplay()
        {
            // 尋找數值記錄組件
            numericalRecords = FindObjectOfType<NumericalRecords>();
            
            if (numericalRecords != null)
            {
                // 獲取當前數值
                LoadCurrentValues();
            }
            
            // 設置進度條範圍
            SetupSliders();
            
            // 初始顯示
            RefreshStatusDisplay();
        }
        
        void LoadCurrentValues()
        {
            if (numericalRecords == null) return;
            
            currentMoney = GetFieldValue<int>(numericalRecords, "aMoney", 0);
            currentAffection = GetFieldValue<int>(numericalRecords, "aAffection", 0);
            currentProgress = GetFieldValue<int>(numericalRecords, "aProgress", 0);
            // health可能需要從其他地方獲取，暫時設為預設值
            currentHealth = 100;
        }
        
        void SetupSliders()
        {
            if (affectionSlider != null)
            {
                affectionSlider.minValue = 0;
                affectionSlider.maxValue = maxAffection;
                affectionSlider.wholeNumbers = true;
            }
            
            if (progressSlider != null)
            {
                progressSlider.minValue = 0;
                progressSlider.maxValue = maxProgress;
                progressSlider.wholeNumbers = true;
            }
            
            if (healthSlider != null)
            {
                healthSlider.minValue = 0;
                healthSlider.maxValue = maxHealth;
                healthSlider.wholeNumbers = true;
            }
        }
        
        #endregion
        
        #region 狀態更新
        
        void UpdateStatusDisplay()
        {
            if (numericalRecords == null) return;
            
            // 檢查數值是否有變化
            bool valuesChanged = false;
            
            int newMoney = GetFieldValue<int>(numericalRecords, "aMoney", 0);
            int newAffection = GetFieldValue<int>(numericalRecords, "aAffection", 0);
            int newProgress = GetFieldValue<int>(numericalRecords, "aProgress", 0);
            
            if (currentMoney != newMoney)
            {
                currentMoney = newMoney;
                valuesChanged = true;
            }
            
            if (currentAffection != newAffection)
            {
                currentAffection = newAffection;
                valuesChanged = true;
            }
            
            if (currentProgress != newProgress)
            {
                currentProgress = newProgress;
                valuesChanged = true;
            }
            
            // 如果有變化才更新顯示
            if (valuesChanged)
            {
                RefreshStatusDisplay();
            }
        }
        
        void RefreshStatusDisplay()
        {
            UpdateMoneyDisplay();
            UpdateAffectionDisplay();
            UpdateProgressDisplay();
            UpdateHealthDisplay();
        }
        
        void UpdateMoneyDisplay()
        {
            if (moneyText != null)
            {
                moneyText.text = string.Format(moneyFormat, currentMoney);
                
                // 金錢不足時變紅色
                if (currentMoney < 0)
                {
                    moneyText.color = Color.red;
                }
                else if (currentMoney < 100)
                {
                    moneyText.color = Color.yellow;
                }
                else
                {
                    moneyText.color = Color.white;
                }
            }
        }
        
        void UpdateAffectionDisplay()
        {
            if (affectionText != null)
            {
                affectionText.text = string.Format(affectionFormat, currentAffection);
                
                // 根據好感度設置顏色
                Color affectionColor = GetAffectionColor(currentAffection);
                affectionText.color = affectionColor;
            }
            
            if (affectionSlider != null)
            {
                affectionSlider.value = currentAffection;
                
                // 設置進度條顏色
                var fillImage = affectionSlider.fillRect?.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.color = GetAffectionColor(currentAffection);
                }
            }
        }
        
        void UpdateProgressDisplay()
        {
            if (progressText != null)
            {
                progressText.text = string.Format(progressFormat, currentProgress);
            }
            
            if (progressSlider != null)
            {
                progressSlider.value = currentProgress;
                
                // 設置進度條顏色
                var fillImage = progressSlider.fillRect?.GetComponent<Image>();
                if (fillImage != null)
                {
                    if (currentProgress >= 80)
                        fillImage.color = Color.green;
                    else if (currentProgress >= 50)
                        fillImage.color = Color.yellow;
                    else
                        fillImage.color = Color.red;
                }
            }
        }
        
        void UpdateHealthDisplay()
        {
            if (healthText != null)
            {
                healthText.text = string.Format(healthFormat, currentHealth);
                
                // 根據健康度設置顏色
                if (currentHealth >= 80)
                    healthText.color = Color.green;
                else if (currentHealth >= 50)
                    healthText.color = Color.yellow;
                else
                    healthText.color = Color.red;
            }
            
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
                
                // 設置進度條顏色
                var fillImage = healthSlider.fillRect?.GetComponent<Image>();
                if (fillImage != null)
                {
                    if (currentHealth >= 80)
                        fillImage.color = Color.green;
                    else if (currentHealth >= 50)
                        fillImage.color = Color.yellow;
                    else
                        fillImage.color = Color.red;
                }
            }
        }
        
        #endregion
        
        #region 輔助方法
        
        Color GetAffectionColor(int affection)
        {
            if (affection >= 80)
                return Color.magenta; // 深情
            else if (affection >= 60)
                return Color.red;     // 喜歡
            else if (affection >= 40)
                return Color.yellow;  // 普通
            else if (affection >= 20)
                return Color.cyan;    // 陌生
            else
                return Color.gray;    // 冷淡
        }
        
        string GetAffectionLevel(int affection)
        {
            if (affection >= 80)
                return "深情";
            else if (affection >= 60)
                return "喜歡";
            else if (affection >= 40)
                return "普通";
            else if (affection >= 20)
                return "陌生";
            else
                return "冷淡";
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
                Debug.LogWarning($"[StatusDisplayController] 無法獲取 {fieldName}: {e.Message}");
            }
            
            return defaultValue;
        }
        
        #endregion
        
        #region 公開API
        
        /// <summary>
        /// 強制刷新狀態顯示
        /// </summary>
        public void ForceRefresh()
        {
            LoadCurrentValues();
            RefreshStatusDisplay();
        }
        
        /// <summary>
        /// 設置顯示格式
        /// </summary>
        public void SetDisplayFormats(string money, string affection, string progress, string health)
        {
            if (!string.IsNullOrEmpty(money))
                moneyFormat = money;
            if (!string.IsNullOrEmpty(affection))
                affectionFormat = affection;
            if (!string.IsNullOrEmpty(progress))
                progressFormat = progress;
            if (!string.IsNullOrEmpty(health))
                healthFormat = health;
                
            RefreshStatusDisplay();
        }
        
        /// <summary>
        /// 設置最大值
        /// </summary>
        public void SetMaxValues(int maxAff, int maxProg, int maxHp)
        {
            maxAffection = maxAff;
            maxProgress = maxProg;
            maxHealth = maxHp;
            
            SetupSliders();
            RefreshStatusDisplay();
        }
        
        /// <summary>
        /// 獲取當前狀態
        /// </summary>
        public (int money, int affection, int progress, int health) GetCurrentStatus()
        {
            return (currentMoney, currentAffection, currentProgress, currentHealth);
        }
        
        /// <summary>
        /// 獲取好感度等級描述
        /// </summary>
        public string GetCurrentAffectionLevel()
        {
            return GetAffectionLevel(currentAffection);
        }
        
        #endregion
    }
}