using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LoveTide.UI
{
    /// <summary>
    /// UI響應式系統 - 處理不同解析度和設備的UI適配
    /// 確保UI在各種螢幕尺寸下都能正確顯示
    /// </summary>
    public class UIResponseSystem : MonoBehaviour
    {
        [Header("=== 響應式設定 ===")]
        [SerializeField] private Vector2 referenceResolution = new Vector2(1920, 1080);
        [SerializeField] private float matchWidthOrHeight = 0.5f;
        [SerializeField] private bool enableAutoScaling = true;
        
        [Header("=== 適配模式 ===")]
        [SerializeField] private ScreenMatchMode screenMatchMode = ScreenMatchMode.MatchWidthOrHeight;
        [SerializeField] private bool enableSafeAreaSupport = true;
        
        [Header("=== 響應式組件 ===")]
        [SerializeField] private List<ResponsiveElement> responsiveElements = new List<ResponsiveElement>();
        
        // UI管理器引用
        private NewUIManager uiManager;
        
        // 螢幕狀態追蹤
        private Vector2 lastScreenSize;
        private float lastScreenRatio;
        private Rect lastSafeArea;
        
        // Canvas Scaler引用
        private Dictionary<Canvas, CanvasScaler> canvasScalers = new Dictionary<Canvas, CanvasScaler>();
        
        #region 響應式元素結構
        
        [System.Serializable]
        public class ResponsiveElement
        {
            [Header("目標組件")]
            public RectTransform targetTransform;
            public ResponsiveType type;
            
            [Header("尺寸設定")]
            public Vector2 phoneSize = Vector2.zero;
            public Vector2 tabletSize = Vector2.zero;
            public Vector2 desktopSize = Vector2.zero;
            
            [Header("位置設定")]
            public Vector2 phonePosition = Vector2.zero;
            public Vector2 tabletPosition = Vector2.zero;
            public Vector2 desktopPosition = Vector2.zero;
            
            [Header("縮放設定")]
            public float phoneScale = 1f;
            public float tabletScale = 1f;
            public float desktopScale = 1f;
            
            public bool IsValid => targetTransform != null;
        }
        
        public enum ResponsiveType
        {
            SizeOnly,       // 只調整尺寸
            PositionOnly,   // 只調整位置
            ScaleOnly,      // 只調整縮放
            SizeAndPosition,// 調整尺寸和位置
            All             // 調整所有屬性
        }
        
        public enum ScreenMatchMode
        {
            MatchWidthOrHeight,
            MatchWidth,
            MatchHeight,
            Expand,
            Shrink
        }
        
        public enum DeviceType
        {
            Phone,    // 手機
            Tablet,   // 平板
            Desktop   // 桌面
        }
        
        #endregion
        
        #region Unity生命週期
        
        void Awake()
        {
            InitializeScreenTracking();
        }
        
        void Start()
        {
            ApplyInitialSettings();
        }
        
        void Update()
        {
            CheckScreenChanges();
        }
        
        #endregion
        
        #region 初始化
        
        public void Initialize(NewUIManager manager)
        {
            uiManager = manager;
            
            InitializeCanvasScalers();
            ApplyResponsiveSettings();
            
            Debug.Log("[UIResponseSystem] UI響應式系統初始化");
        }
        
        void InitializeScreenTracking()
        {
            lastScreenSize = new Vector2(Screen.width, Screen.height);
            lastScreenRatio = (float)Screen.width / Screen.height;
            lastSafeArea = Screen.safeArea;
        }
        
        void InitializeCanvasScalers()
        {
            // 找到所有Canvas並設置CanvasScaler
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();
            
            foreach (Canvas canvas in allCanvases)
            {
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    SetupCanvasScaler(canvas);
                }
            }
        }
        
        void SetupCanvasScaler(Canvas canvas)
        {
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            }
            
            // 設置CanvasScaler屬性
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.matchWidthOrHeight = matchWidthOrHeight;
            
            switch (screenMatchMode)
            {
                case ScreenMatchMode.MatchWidthOrHeight:
                    scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                    break;
                case ScreenMatchMode.Expand:
                    scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                    break;
                case ScreenMatchMode.Shrink:
                    scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
                    break;
            }
            
            canvasScalers[canvas] = scaler;
            
            Debug.Log($"[UIResponseSystem] Canvas {canvas.name} 響應式設定完成");
        }
        
        void ApplyInitialSettings()
        {
            if (enableAutoScaling)
            {
                ApplyResponsiveSettings();
            }
            
            if (enableSafeAreaSupport)
            {
                ApplySafeAreaSettings();
            }
        }
        
        #endregion
        
        #region 螢幕變化檢測
        
        void CheckScreenChanges()
        {
            Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
            float currentScreenRatio = (float)Screen.width / Screen.height;
            Rect currentSafeArea = Screen.safeArea;
            
            // 檢查解析度變化
            if (Vector2.Distance(currentScreenSize, lastScreenSize) > 1f)
            {
                OnScreenSizeChanged(currentScreenSize);
                lastScreenSize = currentScreenSize;
            }
            
            // 檢查螢幕比例變化
            if (Mathf.Abs(currentScreenRatio - lastScreenRatio) > 0.01f)
            {
                OnScreenRatioChanged(currentScreenRatio);
                lastScreenRatio = currentScreenRatio;
            }
            
            // 檢查安全區域變化
            if (currentSafeArea != lastSafeArea)
            {
                OnSafeAreaChanged(currentSafeArea);
                lastSafeArea = currentSafeArea;
            }
        }
        
        void OnScreenSizeChanged(Vector2 newSize)
        {
            Debug.Log($"[UIResponseSystem] 螢幕尺寸變更: {newSize}");
            
            if (enableAutoScaling)
            {
                ApplyResponsiveSettings();
            }
        }
        
        void OnScreenRatioChanged(float newRatio)
        {
            Debug.Log($"[UIResponseSystem] 螢幕比例變更: {newRatio:F2}");
            
            // 更新Canvas Scaler設定
            UpdateCanvasScalerSettings();
        }
        
        void OnSafeAreaChanged(Rect newSafeArea)
        {
            Debug.Log($"[UIResponseSystem] 安全區域變更: {newSafeArea}");
            
            if (enableSafeAreaSupport)
            {
                ApplySafeAreaSettings();
            }
        }
        
        #endregion
        
        #region 響應式設定應用
        
        void ApplyResponsiveSettings()
        {
            DeviceType currentDeviceType = DetectDeviceType();
            
            foreach (ResponsiveElement element in responsiveElements)
            {
                if (element.IsValid)
                {
                    ApplyElementSettings(element, currentDeviceType);
                }
            }
            
            Debug.Log($"[UIResponseSystem] 應用響應式設定 - 設備類型: {currentDeviceType}");
        }
        
        DeviceType DetectDeviceType()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float screenDiagonal = Mathf.Sqrt(screenWidth * screenWidth + screenHeight * screenHeight);
            
            // 基於螢幕對角線尺寸判斷設備類型
            if (screenDiagonal < 1000f)
            {
                return DeviceType.Phone;
            }
            else if (screenDiagonal < 1500f)
            {
                return DeviceType.Tablet;
            }
            else
            {
                return DeviceType.Desktop;
            }
        }
        
        void ApplyElementSettings(ResponsiveElement element, DeviceType deviceType)
        {
            Vector2 targetSize = Vector2.zero;
            Vector2 targetPosition = Vector2.zero;
            float targetScale = 1f;
            
            // 根據設備類型選擇設定
            switch (deviceType)
            {
                case DeviceType.Phone:
                    targetSize = element.phoneSize;
                    targetPosition = element.phonePosition;
                    targetScale = element.phoneScale;
                    break;
                case DeviceType.Tablet:
                    targetSize = element.tabletSize;
                    targetPosition = element.tabletPosition;
                    targetScale = element.tabletScale;
                    break;
                case DeviceType.Desktop:
                    targetSize = element.desktopSize;
                    targetPosition = element.desktopPosition;
                    targetScale = element.desktopScale;
                    break;
            }
            
            // 應用設定
            ApplyTransformSettings(element, targetSize, targetPosition, targetScale);
        }
        
        void ApplyTransformSettings(ResponsiveElement element, Vector2 size, Vector2 position, float scale)
        {
            RectTransform rectTransform = element.targetTransform;
            
            switch (element.type)
            {
                case ResponsiveType.SizeOnly:
                    if (size != Vector2.zero)
                        rectTransform.sizeDelta = size;
                    break;
                    
                case ResponsiveType.PositionOnly:
                    rectTransform.anchoredPosition = position;
                    break;
                    
                case ResponsiveType.ScaleOnly:
                    rectTransform.localScale = Vector3.one * scale;
                    break;
                    
                case ResponsiveType.SizeAndPosition:
                    if (size != Vector2.zero)
                        rectTransform.sizeDelta = size;
                    rectTransform.anchoredPosition = position;
                    break;
                    
                case ResponsiveType.All:
                    if (size != Vector2.zero)
                        rectTransform.sizeDelta = size;
                    rectTransform.anchoredPosition = position;
                    rectTransform.localScale = Vector3.one * scale;
                    break;
            }
        }
        
        #endregion
        
        #region 安全區域支援
        
        void ApplySafeAreaSettings()
        {
            Rect safeArea = Screen.safeArea;
            
            // 找到需要應用安全區域的Canvas
            foreach (var kvp in canvasScalers)
            {
                Canvas canvas = kvp.Key;
                if (canvas != null && canvas.name.Contains("SafeArea"))
                {
                    ApplySafeAreaToCanvas(canvas, safeArea);
                }
            }
        }
        
        void ApplySafeAreaToCanvas(Canvas canvas, Rect safeArea)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            if (canvasRect == null) return;
            
            // 計算安全區域的相對位置
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            
            // 應用安全區域
            canvasRect.anchorMin = anchorMin;
            canvasRect.anchorMax = anchorMax;
            canvasRect.offsetMin = Vector2.zero;
            canvasRect.offsetMax = Vector2.zero;
            
            Debug.Log($"[UIResponseSystem] 應用安全區域到 {canvas.name}");
        }
        
        #endregion
        
        #region Canvas Scaler管理
        
        void UpdateCanvasScalerSettings()
        {
            foreach (var kvp in canvasScalers)
            {
                CanvasScaler scaler = kvp.Value;
                if (scaler != null)
                {
                    // 根據當前螢幕比例調整matchWidthOrHeight
                    float currentRatio = (float)Screen.width / Screen.height;
                    float referenceRatio = referenceResolution.x / referenceResolution.y;
                    
                    if (currentRatio > referenceRatio)
                    {
                        // 寬螢幕：更注重高度匹配
                        scaler.matchWidthOrHeight = 1f;
                    }
                    else
                    {
                        // 窄螢幕：更注重寬度匹配
                        scaler.matchWidthOrHeight = 0f;
                    }
                }
            }
        }
        
        #endregion
        
        #region 公開API
        
        /// <summary>
        /// 添加響應式元素
        /// </summary>
        public void AddResponsiveElement(ResponsiveElement element)
        {
            if (element.IsValid && !responsiveElements.Contains(element))
            {
                responsiveElements.Add(element);
                
                // 立即應用設定
                if (enableAutoScaling)
                {
                    DeviceType currentDeviceType = DetectDeviceType();
                    ApplyElementSettings(element, currentDeviceType);
                }
            }
        }
        
        /// <summary>
        /// 移除響應式元素
        /// </summary>
        public void RemoveResponsiveElement(ResponsiveElement element)
        {
            responsiveElements.Remove(element);
        }
        
        /// <summary>
        /// 強制刷新所有響應式設定
        /// </summary>
        public void ForceRefreshSettings()
        {
            ApplyResponsiveSettings();
            
            if (enableSafeAreaSupport)
            {
                ApplySafeAreaSettings();
            }
        }
        
        /// <summary>
        /// 設置參考解析度
        /// </summary>
        public void SetReferenceResolution(Vector2 resolution)
        {
            referenceResolution = resolution;
            
            // 更新所有Canvas Scaler
            foreach (var kvp in canvasScalers)
            {
                CanvasScaler scaler = kvp.Value;
                if (scaler != null)
                {
                    scaler.referenceResolution = resolution;
                }
            }
        }
        
        /// <summary>
        /// 獲取當前設備類型
        /// </summary>
        public DeviceType GetCurrentDeviceType()
        {
            return DetectDeviceType();
        }
        
        /// <summary>
        /// 獲取當前螢幕信息
        /// </summary>
        public (Vector2 size, float ratio, Rect safeArea) GetScreenInfo()
        {
            return (
                new Vector2(Screen.width, Screen.height),
                (float)Screen.width / Screen.height,
                Screen.safeArea
            );
        }
        
        #endregion
    }
}