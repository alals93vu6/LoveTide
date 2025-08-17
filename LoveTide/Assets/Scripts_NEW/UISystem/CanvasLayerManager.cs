using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LoveTide.UI
{
    /// <summary>
    /// Canvas分層管理器 - 專門管理7層Canvas的層級和可見性
    /// 確保UI層級顯示順序正確和互動優先級
    /// </summary>
    public class CanvasLayerManager : MonoBehaviour
    {
        [Header("=== Canvas層級配置 ===")]
        [SerializeField] private List<CanvasLayerInfo> canvasLayers = new List<CanvasLayerInfo>();
        
        [Header("=== 層級管理設定 ===")]
        [SerializeField] private bool autoValidateOrder = true;
        [SerializeField] private bool enableDebugLog = true;
        [SerializeField] private float layerTransitionDuration = 0.3f;
        
        // UI管理器引用
        private NewUIManager uiManager;
        
        // 層級狀態追蹤
        private Dictionary<string, CanvasLayerInfo> layerInfoMap = new Dictionary<string, CanvasLayerInfo>();
        private Dictionary<int, string> orderToLayerMap = new Dictionary<int, string>();
        
        // 公開屬性
        public int LayerCount => canvasLayers.Count;
        public bool AutoValidateOrder => autoValidateOrder;
        
        // 事件系統
        public static event Action<string, int> OnLayerOrderChanged;
        public static event Action<string, bool> OnLayerVisibilityChanged;
        
        #region Canvas層級信息結構
        
        [System.Serializable]
        public class CanvasLayerInfo
        {
            [Header("基本信息")]
            public string LayerName;
            public Canvas Canvas;
            public int SortOrder;
            
            [Header("層級設定")]
            public LayerType Type;
            public bool IsInteractable = true;
            public bool StartVisible = false;
            
            [Header("動畫設定")]
            public bool UseTransition = true;
            public TransitionType TransitionType = TransitionType.Fade;
            
            public bool IsValid => Canvas != null && !string.IsNullOrEmpty(LayerName);
        }
        
        public enum LayerType
        {
            Background,      // 背景層
            Interaction,     // 互動層
            UI,              // UI層
            Dialog,          // 對話層
            Menu,            // 選單層
            Popup,           // 彈窗層
            Overlay          // 覆蓋層
        }
        
        public enum TransitionType
        {
            None,            // 無動畫
            Fade,            // 淡入淡出
            Slide,           // 滑動
            Scale,           // 縮放
            Custom           // 自定義
        }
        
        #endregion
        
        #region Unity生命週期
        
        void Awake()
        {
            InitializeLayerMaps();
        }
        
        void Start()
        {
            if (autoValidateOrder)
            {
                ValidateAndFixLayerOrders();
            }
        }
        
        #endregion
        
        #region 初始化
        
        public void Initialize(NewUIManager manager)
        {
            uiManager = manager;
            
            InitializeDefaultLayers();
            InitializeLayerStates();
            
            if (enableDebugLog)
                LogLayerConfiguration();
                
            Debug.Log("[CanvasLayerManager] Canvas分層管理器初始化完成");
        }
        
        void InitializeLayerMaps()
        {
            layerInfoMap.Clear();
            orderToLayerMap.Clear();
            
            foreach (var layerInfo in canvasLayers)
            {
                if (layerInfo.IsValid)
                {
                    layerInfoMap[layerInfo.LayerName] = layerInfo;
                    orderToLayerMap[layerInfo.SortOrder] = layerInfo.LayerName;
                }
            }
        }
        
        void InitializeDefaultLayers()
        {
            // 如果沒有配置層級，創建默認配置
            if (canvasLayers.Count == 0)
            {
                CreateDefaultLayerConfiguration();
            }
        }
        
        void CreateDefaultLayerConfiguration()
        {
            var defaultLayers = new[]
            {
                new { Name = "Background", Order = 0, Type = LayerType.Background, Visible = true },
                new { Name = "StaticInteraction", Order = 40, Type = LayerType.Interaction, Visible = false },
                new { Name = "DynamicCharacter", Order = 50, Type = LayerType.Interaction, Visible = false },
                new { Name = "GameUI", Order = 60, Type = LayerType.UI, Visible = true },
                new { Name = "Dialog", Order = 70, Type = LayerType.Dialog, Visible = false },
                new { Name = "Menu", Order = 80, Type = LayerType.Menu, Visible = false },
                new { Name = "Popup", Order = 90, Type = LayerType.Popup, Visible = false }
            };
            
            foreach (var layer in defaultLayers)
            {
                var layerInfo = new CanvasLayerInfo
                {
                    LayerName = layer.Name,
                    SortOrder = layer.Order,
                    Type = layer.Type,
                    StartVisible = layer.Visible,
                    UseTransition = true,
                    TransitionType = TransitionType.Fade
                };
                
                canvasLayers.Add(layerInfo);
            }
            
            Debug.Log("[CanvasLayerManager] 創建默認層級配置");
        }
        
        void InitializeLayerStates()
        {
            foreach (var layerInfo in canvasLayers)
            {
                if (layerInfo.IsValid)
                {
                    // 設置Canvas Sort Order
                    layerInfo.Canvas.sortingOrder = layerInfo.SortOrder;
                    
                    // 設置初始可見性
                    SetLayerVisibility(layerInfo.LayerName, layerInfo.StartVisible, false);
                    
                    // 設置互動性
                    var graphicRaycaster = layerInfo.Canvas.GetComponent<GraphicRaycaster>();
                    if (graphicRaycaster != null)
                    {
                        graphicRaycaster.enabled = layerInfo.IsInteractable;
                    }
                }
            }
        }
        
        #endregion
        
        #region 層級控制
        
        /// <summary>
        /// 設置層級可見性
        /// </summary>
        public void SetLayerVisibility(string layerName, bool visible, bool useTransition = true)
        {
            if (!layerInfoMap.ContainsKey(layerName))
            {
                Debug.LogWarning($"[CanvasLayerManager] 找不到層級: {layerName}");
                return;
            }
            
            var layerInfo = layerInfoMap[layerName];
            
            if (layerInfo.Canvas == null)
            {
                Debug.LogError($"[CanvasLayerManager] 層級 {layerName} 的Canvas為空");
                return;
            }
            
            bool currentVisibility = layerInfo.Canvas.gameObject.activeSelf;
            if (currentVisibility == visible) return;
            
            if (useTransition && layerInfo.UseTransition)
            {
                StartCoroutine(TransitionLayerVisibility(layerInfo, visible));
            }
            else
            {
                layerInfo.Canvas.gameObject.SetActive(visible);
            }
            
            OnLayerVisibilityChanged?.Invoke(layerName, visible);
            
            if (enableDebugLog)
                Debug.Log($"[CanvasLayerManager] 層級 {layerName} {(visible ? "顯示" : "隱藏")}");
        }
        
        /// <summary>
        /// 設置層級順序
        /// </summary>
        public void SetLayerOrder(string layerName, int newOrder)
        {
            if (!layerInfoMap.ContainsKey(layerName))
            {
                Debug.LogWarning($"[CanvasLayerManager] 找不到層級: {layerName}");
                return;
            }
            
            var layerInfo = layerInfoMap[layerName];
            int oldOrder = layerInfo.SortOrder;
            
            if (oldOrder == newOrder) return;
            
            // 更新映射
            orderToLayerMap.Remove(oldOrder);
            orderToLayerMap[newOrder] = layerName;
            
            // 更新層級信息
            layerInfo.SortOrder = newOrder;
            layerInfo.Canvas.sortingOrder = newOrder;
            
            OnLayerOrderChanged?.Invoke(layerName, newOrder);
            
            if (enableDebugLog)
                Debug.Log($"[CanvasLayerManager] 層級 {layerName} 順序變更: {oldOrder} → {newOrder}");
        }
        
        /// <summary>
        /// 獲取層級信息
        /// </summary>
        public CanvasLayerInfo GetLayerInfo(string layerName)
        {
            return layerInfoMap.ContainsKey(layerName) ? layerInfoMap[layerName] : null;
        }
        
        /// <summary>
        /// 檢查層級是否可見
        /// </summary>
        public bool IsLayerVisible(string layerName)
        {
            var layerInfo = GetLayerInfo(layerName);
            return layerInfo?.Canvas?.gameObject.activeSelf ?? false;
        }
        
        /// <summary>
        /// 獲取層級順序
        /// </summary>
        public int GetLayerOrder(string layerName)
        {
            var layerInfo = GetLayerInfo(layerName);
            return layerInfo?.SortOrder ?? -1;
        }
        
        #endregion
        
        #region 層級動畫
        
        IEnumerator TransitionLayerVisibility(CanvasLayerInfo layerInfo, bool visible)
        {
            Canvas canvas = layerInfo.Canvas;
            CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
            
            // 如果沒有CanvasGroup，添加一個
            if (canvasGroup == null)
            {
                canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            }
            
            if (visible)
            {
                // 顯示動畫
                canvas.gameObject.SetActive(true);
                yield return StartCoroutine(PlayShowTransition(layerInfo, canvasGroup));
            }
            else
            {
                // 隱藏動畫
                yield return StartCoroutine(PlayHideTransition(layerInfo, canvasGroup));
                canvas.gameObject.SetActive(false);
            }
        }
        
        IEnumerator PlayShowTransition(CanvasLayerInfo layerInfo, CanvasGroup canvasGroup)
        {
            switch (layerInfo.TransitionType)
            {
                case TransitionType.Fade:
                    yield return StartCoroutine(FadeIn(canvasGroup));
                    break;
                case TransitionType.Scale:
                    yield return StartCoroutine(ScaleIn(layerInfo.Canvas.transform));
                    break;
                case TransitionType.Slide:
                    yield return StartCoroutine(SlideIn(layerInfo.Canvas.transform));
                    break;
                default:
                    canvasGroup.alpha = 1f;
                    break;
            }
        }
        
        IEnumerator PlayHideTransition(CanvasLayerInfo layerInfo, CanvasGroup canvasGroup)
        {
            switch (layerInfo.TransitionType)
            {
                case TransitionType.Fade:
                    yield return StartCoroutine(FadeOut(canvasGroup));
                    break;
                case TransitionType.Scale:
                    yield return StartCoroutine(ScaleOut(layerInfo.Canvas.transform));
                    break;
                case TransitionType.Slide:
                    yield return StartCoroutine(SlideOut(layerInfo.Canvas.transform));
                    break;
                default:
                    canvasGroup.alpha = 0f;
                    break;
            }
        }
        
        IEnumerator FadeIn(CanvasGroup canvasGroup)
        {
            float elapsed = 0f;
            canvasGroup.alpha = 0f;
            
            while (elapsed < layerTransitionDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / layerTransitionDuration);
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
        }
        
        IEnumerator FadeOut(CanvasGroup canvasGroup)
        {
            float elapsed = 0f;
            float startAlpha = canvasGroup.alpha;
            
            while (elapsed < layerTransitionDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / layerTransitionDuration);
                yield return null;
            }
            
            canvasGroup.alpha = 0f;
        }
        
        IEnumerator ScaleIn(Transform canvasTransform)
        {
            float elapsed = 0f;
            Vector3 startScale = Vector3.zero;
            Vector3 targetScale = Vector3.one;
            
            canvasTransform.localScale = startScale;
            
            while (elapsed < layerTransitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / layerTransitionDuration;
                t = Mathf.SmoothStep(0, 1, t); // 平滑曲線
                
                canvasTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }
            
            canvasTransform.localScale = targetScale;
        }
        
        IEnumerator ScaleOut(Transform canvasTransform)
        {
            float elapsed = 0f;
            Vector3 startScale = canvasTransform.localScale;
            Vector3 targetScale = Vector3.zero;
            
            while (elapsed < layerTransitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / layerTransitionDuration;
                t = Mathf.SmoothStep(0, 1, t);
                
                canvasTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }
            
            canvasTransform.localScale = targetScale;
        }
        
        IEnumerator SlideIn(Transform canvasTransform)
        {
            float elapsed = 0f;
            Vector3 startPos = canvasTransform.localPosition + Vector3.right * Screen.width;
            Vector3 targetPos = canvasTransform.localPosition;
            
            canvasTransform.localPosition = startPos;
            
            while (elapsed < layerTransitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / layerTransitionDuration;
                t = Mathf.SmoothStep(0, 1, t);
                
                canvasTransform.localPosition = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }
            
            canvasTransform.localPosition = targetPos;
        }
        
        IEnumerator SlideOut(Transform canvasTransform)
        {
            float elapsed = 0f;
            Vector3 startPos = canvasTransform.localPosition;
            Vector3 targetPos = startPos - Vector3.right * Screen.width;
            
            while (elapsed < layerTransitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / layerTransitionDuration;
                t = Mathf.SmoothStep(0, 1, t);
                
                canvasTransform.localPosition = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }
            
            canvasTransform.localPosition = targetPos;
        }
        
        #endregion
        
        #region 層級驗證
        
        /// <summary>
        /// 驗證並修復層級順序
        /// </summary>
        public void ValidateAndFixLayerOrders()
        {
            var sortedLayers = new List<CanvasLayerInfo>(canvasLayers);
            sortedLayers.Sort((a, b) => a.SortOrder.CompareTo(b.SortOrder));
            
            for (int i = 0; i < sortedLayers.Count; i++)
            {
                var layerInfo = sortedLayers[i];
                if (layerInfo.Canvas != null)
                {
                    if (layerInfo.Canvas.sortingOrder != layerInfo.SortOrder)
                    {
                        layerInfo.Canvas.sortingOrder = layerInfo.SortOrder;
                        
                        if (enableDebugLog)
                            Debug.Log($"[CanvasLayerManager] 修復層級 {layerInfo.LayerName} 的Sort Order: {layerInfo.SortOrder}");
                    }
                }
            }
            
            // 檢查順序衝突
            CheckForOrderConflicts();
        }
        
        void CheckForOrderConflicts()
        {
            var orderCounts = new Dictionary<int, int>();
            
            foreach (var layerInfo in canvasLayers)
            {
                if (layerInfo.IsValid)
                {
                    if (orderCounts.ContainsKey(layerInfo.SortOrder))
                    {
                        orderCounts[layerInfo.SortOrder]++;
                    }
                    else
                    {
                        orderCounts[layerInfo.SortOrder] = 1;
                    }
                }
            }
            
            foreach (var kvp in orderCounts)
            {
                if (kvp.Value > 1)
                {
                    Debug.LogWarning($"[CanvasLayerManager] 檢測到Sort Order衝突: {kvp.Key} (使用次數: {kvp.Value})");
                }
            }
        }
        
        #endregion
        
        #region 批量操作
        
        /// <summary>
        /// 隱藏所有層級
        /// </summary>
        public void HideAllLayers(bool useTransition = true)
        {
            foreach (var layerInfo in canvasLayers)
            {
                if (layerInfo.IsValid)
                {
                    SetLayerVisibility(layerInfo.LayerName, false, useTransition);
                }
            }
        }
        
        /// <summary>
        /// 顯示特定類型的層級
        /// </summary>
        public void ShowLayersByType(LayerType type, bool useTransition = true)
        {
            foreach (var layerInfo in canvasLayers)
            {
                if (layerInfo.IsValid && layerInfo.Type == type)
                {
                    SetLayerVisibility(layerInfo.LayerName, true, useTransition);
                }
            }
        }
        
        /// <summary>
        /// 隱藏特定類型的層級
        /// </summary>
        public void HideLayersByType(LayerType type, bool useTransition = true)
        {
            foreach (var layerInfo in canvasLayers)
            {
                if (layerInfo.IsValid && layerInfo.Type == type)
                {
                    SetLayerVisibility(layerInfo.LayerName, false, useTransition);
                }
            }
        }
        
        #endregion
        
        #region 調試功能
        
        void LogLayerConfiguration()
        {
            Debug.Log("[CanvasLayerManager] 層級配置:");
            
            var sortedLayers = new List<CanvasLayerInfo>(canvasLayers);
            sortedLayers.Sort((a, b) => a.SortOrder.CompareTo(b.SortOrder));
            
            foreach (var layerInfo in sortedLayers)
            {
                string status = layerInfo.IsValid ? "✓" : "✗";
                string visibility = layerInfo.StartVisible ? "顯示" : "隱藏";
                
                Debug.Log($"  {status} [{layerInfo.SortOrder:D2}] {layerInfo.LayerName} ({layerInfo.Type}) - {visibility}");
            }
        }
        
        public List<string> GetLayerDebugInfo()
        {
            var debugInfo = new List<string>();
            
            foreach (var layerInfo in canvasLayers)
            {
                if (layerInfo.IsValid)
                {
                    bool isVisible = IsLayerVisible(layerInfo.LayerName);
                    string info = $"{layerInfo.LayerName}: Order={layerInfo.SortOrder}, Visible={isVisible}, Type={layerInfo.Type}";
                    debugInfo.Add(info);
                }
            }
            
            return debugInfo;
        }
        
        #endregion
        
        #region 事件清理
        
        void OnDestroy()
        {
            OnLayerOrderChanged = null;
            OnLayerVisibilityChanged = null;
        }
        
        #endregion
    }
}