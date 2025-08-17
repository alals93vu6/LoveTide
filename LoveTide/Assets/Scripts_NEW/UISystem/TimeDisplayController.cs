using System;
using UnityEngine;
using UnityEngine.UI;

namespace LoveTide.UI
{
    /// <summary>
    /// 時間顯示控制器 - 管理遊戲中的時間UI顯示
    /// 包括日期、星期、時段的顯示和更新
    /// </summary>
    public class TimeDisplayController : MonoBehaviour
    {
        [Header("=== UI組件引用 ===")]
        [SerializeField] private Text dayText;
        [SerializeField] private Text weekText;
        [SerializeField] private Text timeSlotText;
        [SerializeField] private Text workStatusText;
        
        [Header("=== 顯示格式 ===")]
        [SerializeField] private string dayFormat = "Day {0}";
        [SerializeField] private string weekFormat = "Week {0}";
        [SerializeField] private string timeSlotFormat = "Time {0}";
        
        [Header("=== 時段顯示設定 ===")]
        [SerializeField] private string[] timeSlotNames = {
            "深夜", "早晨", "上午", "中午", "下午", "傍晚", "晚上", "夜晚", "深夜", "凌晨"
        };
        
        [Header("=== 顏色設定 ===")]
        [SerializeField] private Color workTimeColor = Color.blue;
        [SerializeField] private Color dormTimeColor = Color.green;
        [SerializeField] private Color vacationColor = new Color(1f, 0.5f, 0f, 1f); // Orange color
        
        // 時間狀態
        private int currentDay = 1;
        private int currentWeek = 1;
        private int currentTimeSlot = 1;
        private bool isWorkTime = true;
        private bool isVacation = false;
        
        // 核心管理器引用
        private LoveTide.Core.NewGameManager gameManager;
        
        #region Unity生命週期
        
        void Awake()
        {
            InitializeComponents();
        }
        
        void Start()
        {
            InitializeTimeDisplay();
            SubscribeToEvents();
        }
        
        void Update()
        {
            UpdateTimeDisplay();
        }
        
        #endregion
        
        #region 初始化
        
        void InitializeComponents()
        {
            // 自動尋找UI組件如果沒有手動指定
            if (dayText == null)
                dayText = transform.Find("DayText")?.GetComponent<Text>();
                
            if (weekText == null)
                weekText = transform.Find("WeekText")?.GetComponent<Text>();
                
            if (timeSlotText == null)
                timeSlotText = transform.Find("TimeSlotText")?.GetComponent<Text>();
                
            if (workStatusText == null)
                workStatusText = transform.Find("WorkStatusText")?.GetComponent<Text>();
        }
        
        void InitializeTimeDisplay()
        {
            // 獲取遊戲管理器引用
            gameManager = LoveTide.Core.NewGameManager.Instance;
            
            if (gameManager != null)
            {
                // 獲取當前時間信息
                var timeInfo = gameManager.GetCurrentTime();
                currentDay = timeInfo.day;
                currentWeek = timeInfo.week;
                currentTimeSlot = timeInfo.timer;
                
                isWorkTime = gameManager.IsWorkTime;
                isVacation = gameManager.IsVacation;
            }
            
            // 初始顯示
            RefreshTimeDisplay();
        }
        
        void SubscribeToEvents()
        {
            // 訂閱時間變更事件
            if (gameManager != null)
            {
                LoveTide.Core.NewGameManager.OnWorkTimeChanged += OnWorkTimeChanged;
            }
        }
        
        #endregion
        
        #region 時間顯示更新
        
        void UpdateTimeDisplay()
        {
            if (gameManager == null) return;
            
            // 檢查時間是否有變化
            var timeInfo = gameManager.GetCurrentTime();
            bool timeChanged = false;
            
            if (currentDay != timeInfo.day)
            {
                currentDay = timeInfo.day;
                timeChanged = true;
            }
            
            if (currentWeek != timeInfo.week)
            {
                currentWeek = timeInfo.week;
                timeChanged = true;
            }
            
            if (currentTimeSlot != timeInfo.timer)
            {
                currentTimeSlot = timeInfo.timer;
                timeChanged = true;
            }
            
            bool statusChanged = false;
            
            if (isWorkTime != gameManager.IsWorkTime)
            {
                isWorkTime = gameManager.IsWorkTime;
                statusChanged = true;
            }
            
            if (isVacation != gameManager.IsVacation)
            {
                isVacation = gameManager.IsVacation;
                statusChanged = true;
            }
            
            // 如果有變化才更新顯示
            if (timeChanged || statusChanged)
            {
                RefreshTimeDisplay();
            }
        }
        
        void RefreshTimeDisplay()
        {
            UpdateDayDisplay();
            UpdateWeekDisplay();
            UpdateTimeSlotDisplay();
            UpdateWorkStatusDisplay();
        }
        
        void UpdateDayDisplay()
        {
            if (dayText != null)
            {
                dayText.text = string.Format(dayFormat, currentDay);
            }
        }
        
        void UpdateWeekDisplay()
        {
            if (weekText != null)
            {
                weekText.text = string.Format(weekFormat, currentWeek);
            }
        }
        
        void UpdateTimeSlotDisplay()
        {
            if (timeSlotText != null)
            {
                // 顯示時段數字和名稱
                string timeSlotName = GetTimeSlotName(currentTimeSlot);
                string displayText = string.Format(timeSlotFormat, currentTimeSlot);
                
                if (!string.IsNullOrEmpty(timeSlotName))
                {
                    displayText += $" ({timeSlotName})";
                }
                
                timeSlotText.text = displayText;
                
                // 設置顏色
                Color textColor = GetTimeSlotColor(currentTimeSlot);
                timeSlotText.color = textColor;
            }
        }
        
        void UpdateWorkStatusDisplay()
        {
            if (workStatusText != null)
            {
                string statusText;
                Color statusColor;
                
                if (isVacation)
                {
                    statusText = "假期";
                    statusColor = vacationColor;
                }
                else if (isWorkTime)
                {
                    statusText = "工作時間";
                    statusColor = workTimeColor;
                }
                else
                {
                    statusText = "宿舍時間";
                    statusColor = dormTimeColor;
                }
                
                workStatusText.text = statusText;
                workStatusText.color = statusColor;
            }
        }
        
        #endregion
        
        #region 輔助方法
        
        string GetTimeSlotName(int timeSlot)
        {
            if (timeSlot >= 1 && timeSlot <= timeSlotNames.Length)
            {
                return timeSlotNames[timeSlot - 1];
            }
            return "";
        }
        
        Color GetTimeSlotColor(int timeSlot)
        {
            // 根據時段返回對應顏色
            if (timeSlot >= 1 && timeSlot <= 6)
            {
                return workTimeColor; // 工作時間
            }
            else if (timeSlot >= 7 && timeSlot <= 9)
            {
                return dormTimeColor; // 宿舍時間
            }
            else
            {
                return Color.gray; // 其他時間
            }
        }
        
        #endregion
        
        #region 事件處理
        
        void OnWorkTimeChanged(bool workTime)
        {
            isWorkTime = workTime;
            UpdateWorkStatusDisplay();
        }
        
        #endregion
        
        #region 公開API
        
        /// <summary>
        /// 強制刷新時間顯示
        /// </summary>
        public void ForceRefresh()
        {
            if (gameManager != null)
            {
                var timeInfo = gameManager.GetCurrentTime();
                currentDay = timeInfo.day;
                currentWeek = timeInfo.week;
                currentTimeSlot = timeInfo.timer;
                
                isWorkTime = gameManager.IsWorkTime;
                isVacation = gameManager.IsVacation;
            }
            
            RefreshTimeDisplay();
        }
        
        /// <summary>
        /// 設置時間顯示格式
        /// </summary>
        public void SetDisplayFormats(string dayFmt, string weekFmt, string timeFmt)
        {
            if (!string.IsNullOrEmpty(dayFmt))
                dayFormat = dayFmt;
            if (!string.IsNullOrEmpty(weekFmt))
                weekFormat = weekFmt;
            if (!string.IsNullOrEmpty(timeFmt))
                timeSlotFormat = timeFmt;
                
            RefreshTimeDisplay();
        }
        
        /// <summary>
        /// 設置顏色主題
        /// </summary>
        public void SetColorTheme(Color workColor, Color dormColor, Color vacationColor)
        {
            workTimeColor = workColor;
            dormTimeColor = dormColor;
            this.vacationColor = vacationColor;
            
            RefreshTimeDisplay();
        }
        
        /// <summary>
        /// 獲取當前顯示的時間信息
        /// </summary>
        public (int day, int week, int timeSlot, bool isWorkTime, bool isVacation) GetDisplayedTime()
        {
            return (currentDay, currentWeek, currentTimeSlot, isWorkTime, isVacation);
        }
        
        #endregion
        
        #region 清理
        
        void OnDestroy()
        {
            // 取消事件訂閱
            if (gameManager != null)
            {
                LoveTide.Core.NewGameManager.OnWorkTimeChanged -= OnWorkTimeChanged;
            }
        }
        
        #endregion
    }
}