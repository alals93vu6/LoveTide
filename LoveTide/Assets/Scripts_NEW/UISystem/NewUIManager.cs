using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LoveTide.UI
{
    /// <summary>
    /// 新版UI管理器 - 管理7層Canvas系統和UI狀態
    /// 負責Canvas分層顯示、UI動畫和響應式設計
    /// </summary>
    public class NewUIManager : MonoBehaviour
    {
        [Header("=== Canvas層級引用 ===")]
        [SerializeField] private Canvas backgroundCanvas;        // Order: 0
        [SerializeField] private Canvas staticInteractionCanvas; // Order: 40
        [SerializeField] private Canvas dynamicCharacterCanvas;  // Order: 50
        [SerializeField] private Canvas gameUICanvas;            // Order: 60
        [SerializeField] private Canvas dialogCanvas;            // Order: 70
        [SerializeField] private Canvas menuCanvas;              // Order: 80
        [SerializeField] private Canvas popupCanvas;             // Order: 90
        
        [Header("=== UI組件管理 ===")]
        [SerializeField] private CanvasLayerManager layerManager;
        [SerializeField] private UIAnimationController animationController;
        
        [Header("=== 主要UI面板 ===")]
        [SerializeField] private GameObject timeDisplayPanel;
        [SerializeField] private GameObject statusDisplayPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject informationUIPanel;
        
        [Header("=== UI狀態 ===")]
        [SerializeField] private UIState currentUIState = UIState.Gameplay;
        [SerializeField] private bool uiSystemInitialized = false;
        
        // 核心管理器引用
        private LoveTide.Core.NewGameManager gameManager;
        
        // UI狀態緩存
        private Dictionary<string, bool> canvasVisibilityStates = new Dictionary<string, bool>();
        private Dictionary<string, GameObject> uiPanelReferences = new Dictionary<string, GameObject>();
        
        // 公開屬性
        public UIState CurrentUIState => currentUIState;
        public bool IsInitialized => uiSystemInitialized;
        
        // 事件系統
        public static event Action<UIState> OnUIStateChanged;
        public static event Action<string, bool> OnCanvasVisibilityChanged;
        
        #region UI狀態枚舉
        
        public enum UIState
        {
            Initialize,    // 初始化中
            Gameplay,      // 遊戲中
            Dialog,        // 對話中
            Menu,          // 選單中
            Settings,      // 設定中
            Loading,       // 載入中
            Transition     // 轉場中
        }
        
        #endregion
        
        #region Unity生命週期
        
        void Awake()
        {
            InitializeReferences();
            InitializeCanvasStates();
        }
        
        void Start()
        {
            StartCoroutine(InitializeUISystem());
        }
        
        void Update()
        {
            HandleUIUpdate();
        }
        
        #endregion
        
        #region 初始化
        
        void InitializeReferences()
        {
            // 初始化組件引用
            if (layerManager == null)
                layerManager = GetComponent<CanvasLayerManager>();
                
            if (animationController == null)
                animationController = GetComponent<UIAnimationController>();
        }
        
        void InitializeCanvasStates()
        {
            // 初始化Canvas可見性狀態緩存
            canvasVisibilityStates["Background"] = true;
            canvasVisibilityStates["StaticInteraction"] = false;
            canvasVisibilityStates["DynamicCharacter"] = false;
            canvasVisibilityStates["GameUI"] = true;
            canvasVisibilityStates["Dialog"] = false;
            canvasVisibilityStates["Menu"] = false;
            canvasVisibilityStates["Popup"] = false;
            
            // 初始化UI面板引用
            InitializeUIPanelReferences();
        }
        
        void InitializeUIPanelReferences()
        {
            uiPanelReferences["TimeDisplay"] = timeDisplayPanel;
            uiPanelReferences["StatusDisplay"] = statusDisplayPanel;
            uiPanelReferences["Settings"] = settingsPanel;
            uiPanelReferences["InformationUI"] = informationUIPanel;
        }
        
        public void Initialize(LoveTide.Core.NewGameManager manager)
        {
            gameManager = manager;
            Debug.Log("[NewUIManager] UI管理器初始化中...");
        }
        
        IEnumerator InitializeUISystem()
        {
            Debug.Log("[NewUIManager] 開始初始化UI系統...");
            
            SetUIState(UIState.Initialize);
            
            // 等待一幀確保所有組件都已載入
            yield return null;
            
            // 驗證Canvas引用
            if (!ValidateCanvasReferences())
            {
                Debug.LogError("[NewUIManager] Canvas引用驗證失敗！");
                yield break;
            }
            
            // 初始化Canvas分層管理器
            if (layerManager != null)
            {
                layerManager.Initialize(this);
            }
            
            // 初始化動畫控制器
            if (animationController != null)
            {
                animationController.Initialize(this);
            }
            
            // 設置初始Canvas狀態
            ApplyInitialCanvasState();
            
            // 訂閱遊戲事件
            SubscribeToGameEvents();
            
            // 系統初始化完成
            uiSystemInitialized = true;
            SetUIState(UIState.Gameplay);
            
            Debug.Log("[NewUIManager] UI系統初始化完成！");
        }
        
        bool ValidateCanvasReferences()
        {
            bool isValid = true;
            
            Canvas[] requiredCanvases = {
                backgroundCanvas, staticInteractionCanvas, dynamicCharacterCanvas,
                gameUICanvas, dialogCanvas, menuCanvas, popupCanvas
            };
            
            string[] canvasNames = {
                "Background", "StaticInteraction", "DynamicCharacter",
                "GameUI", "Dialog", "Menu", "Popup"
            };
            
            for (int i = 0; i < requiredCanvases.Length; i++)
            {
                if (requiredCanvases[i] == null)
                {
                    Debug.LogError($"[NewUIManager] {canvasNames[i]}Canvas引用缺失！");
                    isValid = false;
                }
            }
            
            return isValid;
        }
        
        void ApplyInitialCanvasState()
        {
            // 設置初始Canvas可見性
            SetCanvasActive("Background", true);
            SetCanvasActive("StaticInteraction", false);
            SetCanvasActive("DynamicCharacter", false);
            SetCanvasActive("GameUI", true);
            SetCanvasActive("Dialog", false);
            SetCanvasActive("Menu", false);
            SetCanvasActive("Popup", false);
            
            // 確保Canvas Sort Order正確
            ValidateCanvasSortOrders();
        }
        
        void ValidateCanvasSortOrders()
        {
            if (backgroundCanvas != null) backgroundCanvas.sortingOrder = 0;
            if (staticInteractionCanvas != null) staticInteractionCanvas.sortingOrder = 40;
            if (dynamicCharacterCanvas != null) dynamicCharacterCanvas.sortingOrder = 50;
            if (gameUICanvas != null) gameUICanvas.sortingOrder = 60;
            if (dialogCanvas != null) dialogCanvas.sortingOrder = 70;
            if (menuCanvas != null) menuCanvas.sortingOrder = 80;
            if (popupCanvas != null) popupCanvas.sortingOrder = 90;
            
            Debug.Log("[NewUIManager] Canvas Sort Order驗證完成");
        }
        
        void SubscribeToGameEvents()
        {
            if (gameManager != null)
            {
                LoveTide.Core.NewGameManager.OnGameStateChanged += OnGameStateChanged;
                LoveTide.Core.NewGameManager.OnTalkingStateChanged += OnTalkingStateChanged;
            }
        }
        
        #endregion
        
        #region UI狀態管理
        
        public void SetUIState(UIState newUIState)
        {
            if (currentUIState == newUIState) return;
            
            var oldState = currentUIState;
            currentUIState = newUIState;
            
            Debug.Log($"[NewUIManager] UI狀態變更: {oldState} → {newUIState}");
            
            // 處理狀態轉換
            HandleUIStateTransition(oldState, newUIState);
            
            // 通知事件
            OnUIStateChanged?.Invoke(newUIState);
        }
        
        void HandleUIStateTransition(UIState oldState, UIState newState)
        {
            // 退出舊狀態
            ExitUIState(oldState);
            
            // 進入新狀態
            EnterUIState(newState);
        }
        
        void ExitUIState(UIState state)
        {
            switch (state)
            {
                case UIState.Gameplay:
                    // 可能需要保存UI狀態
                    break;
                case UIState.Dialog:
                    // 隱藏對話相關UI
                    SetCanvasActive("Dialog", false);
                    break;
                case UIState.Menu:
                    // 隱藏選單UI
                    SetCanvasActive("Menu", false);
                    break;
                case UIState.Settings:
                    // 隱藏設定UI
                    SetUIPanel("Settings", false);
                    break;
            }
        }
        
        void EnterUIState(UIState state)
        {
            switch (state)
            {
                case UIState.Initialize:
                    ShowInitializationUI();
                    break;
                case UIState.Gameplay:
                    ShowGameplayUI();
                    break;
                case UIState.Dialog:
                    ShowDialogUI();
                    break;
                case UIState.Menu:
                    ShowMenuUI();
                    break;
                case UIState.Settings:
                    ShowSettingsUI();
                    break;
                case UIState.Loading:
                    ShowLoadingUI();
                    break;
                case UIState.Transition:
                    ShowTransitionUI();
                    break;
            }
        }
        
        #endregion
        
        #region UI顯示控制
        
        void ShowInitializationUI()
        {
            Debug.Log("[NewUIManager] 顯示初始化UI");
            
            // 只顯示背景和載入UI
            SetCanvasActive("Background", true);
            SetCanvasActive("StaticInteraction", false);
            SetCanvasActive("DynamicCharacter", false);
            SetCanvasActive("GameUI", false);
            SetCanvasActive("Dialog", false);
            SetCanvasActive("Menu", false);
            SetCanvasActive("Popup", false);
        }
        
        void ShowGameplayUI()
        {
            Debug.Log("[NewUIManager] 顯示遊戲玩法UI");
            
            // 顯示遊戲相關UI
            SetCanvasActive("Background", true);
            SetCanvasActive("StaticInteraction", true);
            SetCanvasActive("DynamicCharacter", true);
            SetCanvasActive("GameUI", true);
            SetCanvasActive("Dialog", false);
            SetCanvasActive("Menu", false);
            SetCanvasActive("Popup", false);
            
            // 顯示遊戲UI面板
            SetUIPanel("TimeDisplay", true);
            SetUIPanel("StatusDisplay", true);
        }
        
        void ShowDialogUI()
        {
            Debug.Log("[NewUIManager] 顯示對話UI");
            
            // 對話模式：隱藏互動UI，顯示對話UI
            SetCanvasActive("StaticInteraction", false);
            SetCanvasActive("DynamicCharacter", false);
            SetCanvasActive("Dialog", true);
            
            // 保持背景和基本遊戲UI
            SetCanvasActive("Background", true);
            SetCanvasActive("GameUI", true);
        }
        
        void ShowMenuUI()
        {
            Debug.Log("[NewUIManager] 顯示選單UI");
            
            // 選單模式：隱藏遊戲互動UI，顯示選單
            SetCanvasActive("StaticInteraction", false);
            SetCanvasActive("DynamicCharacter", false);
            SetCanvasActive("Dialog", false);
            SetCanvasActive("Menu", true);
        }
        
        void ShowSettingsUI()
        {
            Debug.Log("[NewUIManager] 顯示設定UI");
            
            // 顯示設定面板
            SetUIPanel("Settings", true);
            
            // 可以選擇顯示在Menu Canvas或Popup Canvas
            SetCanvasActive("Popup", true);
        }
        
        void ShowLoadingUI()
        {
            Debug.Log("[NewUIManager] 顯示載入UI");
            
            // 載入狀態：只顯示必要UI
            SetCanvasActive("StaticInteraction", false);
            SetCanvasActive("DynamicCharacter", false);
            SetCanvasActive("Menu", false);
            SetCanvasActive("Popup", true); // 用於顯示載入畫面
        }
        
        void ShowTransitionUI()
        {
            Debug.Log("[NewUIManager] 顯示轉場UI");
            
            // 轉場效果通常在最高層
            SetCanvasActive("Popup", true);
        }
        
        #endregion
        
        #region Canvas控制
        
        public void SetCanvasActive(string canvasName, bool active)
        {
            Canvas targetCanvas = GetCanvasByName(canvasName);
            
            if (targetCanvas != null)
            {
                bool wasActive = targetCanvas.gameObject.activeSelf;
                targetCanvas.gameObject.SetActive(active);
                
                // 更新狀態緩存
                canvasVisibilityStates[canvasName] = active;
                
                if (wasActive != active)
                {
                    Debug.Log($"[NewUIManager] Canvas {canvasName} {(active ? "顯示" : "隱藏")}");
                    OnCanvasVisibilityChanged?.Invoke(canvasName, active);
                }
            }
            else
            {
                Debug.LogWarning($"[NewUIManager] 找不到Canvas: {canvasName}");
            }
        }
        
        Canvas GetCanvasByName(string canvasName)
        {
            switch (canvasName)
            {
                case "Background": return backgroundCanvas;
                case "StaticInteraction": return staticInteractionCanvas;
                case "DynamicCharacter": return dynamicCharacterCanvas;
                case "GameUI": return gameUICanvas;
                case "Dialog": return dialogCanvas;
                case "Menu": return menuCanvas;
                case "Popup": return popupCanvas;
                default: return null;
            }
        }
        
        public bool IsCanvasActive(string canvasName)
        {
            return canvasVisibilityStates.ContainsKey(canvasName) && canvasVisibilityStates[canvasName];
        }
        
        #endregion
        
        #region UI面板控制
        
        public void SetUIPanel(string panelName, bool active)
        {
            if (uiPanelReferences.ContainsKey(panelName))
            {
                GameObject panel = uiPanelReferences[panelName];
                if (panel != null)
                {
                    panel.SetActive(active);
                    Debug.Log($"[NewUIManager] UI面板 {panelName} {(active ? "顯示" : "隱藏")}");
                }
            }
            else
            {
                Debug.LogWarning($"[NewUIManager] 找不到UI面板: {panelName}");
            }
        }
        
        public bool IsUIPanelActive(string panelName)
        {
            if (uiPanelReferences.ContainsKey(panelName))
            {
                GameObject panel = uiPanelReferences[panelName];
                return panel != null && panel.activeSelf;
            }
            return false;
        }
        
        public void ToggleUIPanel(string panelName)
        {
            bool currentState = IsUIPanelActive(panelName);
            SetUIPanel(panelName, !currentState);
        }
        
        #endregion
        
        #region 遊戲事件響應
        
        public void OnGameStateChanged(LoveTide.Core.GameState oldState, LoveTide.Core.GameState newState)
        {
            // 將遊戲狀態映射到UI狀態
            UIState newUIState = MapGameStateToUIState(newState);
            SetUIState(newUIState);
        }
        
        UIState MapGameStateToUIState(LoveTide.Core.GameState gameState)
        {
            switch (gameState)
            {
                case LoveTide.Core.GameState.Initialize:
                    return UIState.Initialize;
                case LoveTide.Core.GameState.Playing:
                    return UIState.Gameplay;
                case LoveTide.Core.GameState.Dialog:
                    return UIState.Dialog;
                case LoveTide.Core.GameState.Menu:
                    return UIState.Menu;
                case LoveTide.Core.GameState.Paused:
                    return UIState.Menu;
                case LoveTide.Core.GameState.Loading:
                    return UIState.Loading;
                case LoveTide.Core.GameState.Transition:
                    return UIState.Transition;
                default:
                    return UIState.Gameplay;
            }
        }
        
        void OnTalkingStateChanged(bool talking)
        {
            if (talking)
            {
                SetUIState(UIState.Dialog);
            }
            else if (currentUIState == UIState.Dialog)
            {
                SetUIState(UIState.Gameplay);
            }
        }
        
        #endregion
        
        #region 更新循環
        
        void HandleUIUpdate()
        {
            if (!uiSystemInitialized) return;
            
            // 處理輸入
            HandleUIInput();
            
            // 更新UI組件
            UpdateUIComponents();
        }
        
        void HandleUIInput()
        {
            // ESC鍵處理
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleEscapeKey();
            }
            
            // 設定鍵處理
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleSettingsPanel();
            }
        }
        
        void HandleEscapeKey()
        {
            switch (currentUIState)
            {
                case UIState.Settings:
                    SetUIState(UIState.Gameplay);
                    break;
                case UIState.Menu:
                    SetUIState(UIState.Gameplay);
                    break;
                case UIState.Dialog:
                    // 對話中通常不允許ESC
                    break;
                default:
                    SetUIState(UIState.Menu);
                    break;
            }
        }
        
        void ToggleSettingsPanel()
        {
            if (currentUIState == UIState.Settings)
            {
                SetUIState(UIState.Gameplay);
            }
            else
            {
                SetUIState(UIState.Settings);
            }
        }
        
        void UpdateUIComponents()
        {
            // 定期驗證UI狀態
            if (Time.frameCount % 60 == 0) // 每秒檢查一次
            {
                ValidateUIState();
            }
        }
        
        void ValidateUIState()
        {
            // 驗證Canvas可見性與UI狀態是否一致
            // 這裡可以添加檢查邏輯
        }
        
        #endregion
        
        #region 特殊功能
        
        public void ResetForNewDay()
        {
            Debug.Log("[NewUIManager] 重置UI狀態迎接新的一天");
            
            // 重置到遊戲玩法狀態
            SetUIState(UIState.Gameplay);
            
            // 重置UI面板狀態
            SetUIPanel("Settings", false);
            
            // 刷新UI顯示
            RefreshUIDisplay();
        }
        
        void RefreshUIDisplay()
        {
            // 刷新時間顯示
            if (timeDisplayPanel != null)
            {
                var timeDisplay = timeDisplayPanel.GetComponent<Text>();
                if (timeDisplay != null && gameManager != null)
                {
                    var timeInfo = gameManager.GetCurrentTime();
                    timeDisplay.text = $"Day {timeInfo.day} - Time {timeInfo.timer}";
                }
            }
            
            // 刷新狀態顯示
            RefreshStatusDisplay();
        }
        
        void RefreshStatusDisplay()
        {
            // 這裡可以更新角色狀態、金錢等顯示
            // 具體實現依賴於StatusDisplay組件的設計
        }
        
        #endregion
        
        #region 公開API
        
        /// <summary>
        /// 強制刷新所有UI狀態
        /// </summary>
        public void ForceRefreshUI()
        {
            ApplyInitialCanvasState();
            RefreshUIDisplay();
        }
        
        /// <summary>
        /// 檢查UI系統是否準備就緒
        /// </summary>
        public bool IsUISystemReady()
        {
            return uiSystemInitialized && ValidateCanvasReferences();
        }
        
        /// <summary>
        /// 獲取當前可見的Canvas列表
        /// </summary>
        public List<string> GetVisibleCanvases()
        {
            var visibleCanvases = new List<string>();
            foreach (var kvp in canvasVisibilityStates)
            {
                if (kvp.Value)
                {
                    visibleCanvases.Add(kvp.Key);
                }
            }
            return visibleCanvases;
        }
        
        #endregion
        
        #region 事件清理
        
        void OnDestroy()
        {
            // 取消事件訂閱
            if (gameManager != null)
            {
                LoveTide.Core.NewGameManager.OnGameStateChanged -= OnGameStateChanged;
                LoveTide.Core.NewGameManager.OnTalkingStateChanged -= OnTalkingStateChanged;
            }
            
            // 清理靜態事件
            OnUIStateChanged = null;
            OnCanvasVisibilityChanged = null;
        }
        
        #endregion
    }
}