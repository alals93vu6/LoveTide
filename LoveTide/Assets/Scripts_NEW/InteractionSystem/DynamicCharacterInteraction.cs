using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LoveTide.Interaction
{
    /// <summary>
    /// 動態角色互動系統 - 處理移動角色（如由香）的互動邏輯
    /// 實現透明Button跟隨角色移動的核心技術
    /// </summary>
    public class DynamicCharacterInteraction : MonoBehaviour
    {
        [Header("=== 角色引用 ===")]
        [SerializeField] private Transform yukaSpineTransform;
        [SerializeField] private ActorManagerTest actorManager;
        
        [Header("=== 互動UI組件 ===")]
        [SerializeField] private Button yukaInteractionButton;
        [SerializeField] private Image yukaHoverEffect;
        [SerializeField] private GameObject interactionIndicator;
        [SerializeField] private Canvas dynamicCanvas;
        
        [Header("=== Button跟隨設定 ===")]
        [SerializeField] private Vector2 buttonSize = new Vector2(100, 150);
        [SerializeField] private Vector2 buttonOffset = new Vector2(0, 50);
        [SerializeField] private float followSpeed = 10f;
        [SerializeField] private bool smoothFollow = true;
        
        [Header("=== 邊界檢查 ===")]
        [SerializeField] private float screenBorderMargin = 50f;
        [SerializeField] private bool hideWhenOffScreen = true;
        
        [Header("=== 互動反饋 ===")]
        [SerializeField] private float hoverEffectScale = 1.1f;
        [SerializeField] private Color hoverColor = Color.white;
        [SerializeField] private float indicatorAnimationSpeed = 2f;
        
        // 核心管理器引用
        private LoveTide.Core.InteractionManager interactionManager;
        private Camera mainCamera;
        
        // 互動狀態
        private bool interactionEnabled = false;
        private bool isHovering = false;
        private bool isButtonVisible = false;
        
        // 座標轉換緩存
        private Vector3 lastWorldPosition = Vector3.zero;
        private Vector2 targetScreenPosition = Vector2.zero;
        private RectTransform buttonRectTransform;
        
        // 對話選單系統
        private YukaDialogMenu dialogMenu;
        private bool isDialogMenuOpen = false;
        
        // 公開屬性
        public bool InteractionEnabled => interactionEnabled;
        public bool IsHovering => isHovering;
        public Vector2 CurrentButtonPosition => buttonRectTransform?.anchoredPosition ?? Vector2.zero;
        
        // 事件系統
        public static event Action<bool> OnYukaHoverStateChanged;
        public static event Action OnYukaClicked;
        public static event Action<List<YukaDialogOption>> OnDialogMenuRequested;
        
        #region Unity生命週期
        
        void Awake()
        {
            InitializeComponents();
        }
        
        void Start()
        {
            StartCoroutine(InitializeDynamicInteraction());
        }
        
        void Update()
        {
            if (interactionEnabled)
            {
                UpdateButtonPosition();
                UpdateButtonVisibility();
                UpdateInteractionIndicator();
            }
        }
        
        #endregion
        
        #region 初始化
        
        void InitializeComponents()
        {
            // 獲取主攝影機
            mainCamera = Camera.main;
            if (mainCamera == null)
                mainCamera = FindObjectOfType<Camera>();
            
            // 獲取Button的RectTransform
            if (yukaInteractionButton != null)
            {
                buttonRectTransform = yukaInteractionButton.GetComponent<RectTransform>();
                
                // 設置Button屬性
                SetupInteractionButton();
            }
            
            // 獲取對話選單系統
            dialogMenu = GetComponent<YukaDialogMenu>();
            if (dialogMenu == null)
            {
                dialogMenu = gameObject.AddComponent<YukaDialogMenu>();
            }
        }
        
        void SetupInteractionButton()
        {
            if (buttonRectTransform == null) return;
            
            // 設置Button尺寸
            buttonRectTransform.sizeDelta = buttonSize;
            
            // 設置透明背景
            Image buttonImage = yukaInteractionButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                Color transparent = buttonImage.color;
                transparent.a = 0f; // 完全透明
                buttonImage.color = transparent;
            }
            
            // 添加事件監聽
            yukaInteractionButton.onClick.AddListener(OnYukaButtonClicked);
            
            // 添加Hover事件
            AddHoverEvents();
            
            // 初始隱藏
            yukaInteractionButton.gameObject.SetActive(false);
        }
        
        void AddHoverEvents()
        {
            // 添加EventTrigger組件處理Hover事件
            EventTrigger eventTrigger = yukaInteractionButton.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = yukaInteractionButton.gameObject.AddComponent<EventTrigger>();
            }
            
            // Hover Enter
            EventTrigger.Entry hoverEnter = new EventTrigger.Entry();
            hoverEnter.eventID = EventTriggerType.PointerEnter;
            hoverEnter.callback.AddListener((data) => OnYukaHoverEnter());
            eventTrigger.triggers.Add(hoverEnter);
            
            // Hover Exit
            EventTrigger.Entry hoverExit = new EventTrigger.Entry();
            hoverExit.eventID = EventTriggerType.PointerExit;
            hoverExit.callback.AddListener((data) => OnYukaHoverExit());
            eventTrigger.triggers.Add(hoverExit);
        }
        
        public void Initialize(LoveTide.Core.InteractionManager manager)
        {
            interactionManager = manager;
            Debug.Log("[DynamicCharacterInteraction] 動態角色互動系統初始化");
        }
        
        IEnumerator InitializeDynamicInteraction()
        {
            // 等待必要組件初始化
            while (yukaSpineTransform == null)
            {
                // 嘗試從ActorManager獲取由香Transform
                if (actorManager != null)
                {
                    yukaSpineTransform = FindYukaTransform();
                }
                yield return new WaitForSeconds(0.1f);
            }
            
            // 驗證組件完整性
            if (!ValidateComponents())
            {
                Debug.LogError("[DynamicCharacterInteraction] 組件驗證失敗！");
                yield break;
            }
            
            // 初始化對話選單
            if (dialogMenu != null)
            {
                dialogMenu.Initialize(this);
            }
            
            Debug.Log("[DynamicCharacterInteraction] 動態角色互動系統初始化完成");
        }
        
        Transform FindYukaTransform()
        {
            // 嘗試從多種方式找到由香的Transform
            if (actorManager != null)
            {
                // 方法1: 從ActorManager獲取
                return actorManager.transform;
            }
            
            // 方法2: 通過名稱查找
            GameObject yukaObject = GameObject.Find("Yuka");
            if (yukaObject != null)
            {
                return yukaObject.transform;
            }
            
            // 方法3: 通過標籤查找
            GameObject taggedYuka = GameObject.FindWithTag("Yuka");
            if (taggedYuka != null)
            {
                return taggedYuka.transform;
            }
            
            return null;
        }
        
        bool ValidateComponents()
        {
            if (yukaSpineTransform == null)
            {
                Debug.LogError("[DynamicCharacterInteraction] 由香Transform引用缺失！");
                return false;
            }
            
            if (yukaInteractionButton == null)
            {
                Debug.LogError("[DynamicCharacterInteraction] 由香互動Button引用缺失！");
                return false;
            }
            
            if (mainCamera == null)
            {
                Debug.LogError("[DynamicCharacterInteraction] 主攝影機引用缺失！");
                return false;
            }
            
            return true;
        }
        
        #endregion
        
        #region 位置更新
        
        void UpdateButtonPosition()
        {
            if (yukaSpineTransform == null || buttonRectTransform == null) return;
            
            // 檢查由香位置是否變化
            Vector3 currentWorldPos = yukaSpineTransform.position;
            if (Vector3.Distance(currentWorldPos, lastWorldPosition) < 0.01f && !smoothFollow)
            {
                return; // 位置沒有顯著變化
            }
            
            lastWorldPosition = currentWorldPos;
            
            // World Space → Screen Space 座標轉換
            Vector3 worldPosWithOffset = currentWorldPos + (Vector3)buttonOffset;
            Vector2 screenPos = ConvertWorldToScreenPoint(worldPosWithOffset);
            
            // 邊界檢查
            screenPos = ClampToScreenBounds(screenPos);
            
            // 更新Button位置
            if (smoothFollow)
            {
                UpdateButtonPositionSmooth(screenPos);
            }
            else
            {
                buttonRectTransform.position = screenPos;
            }
            
            targetScreenPosition = screenPos;
        }
        
        Vector2 ConvertWorldToScreenPoint(Vector3 worldPosition)
        {
            if (mainCamera == null) return Vector2.zero;
            
            // 世界座標轉換為螢幕座標
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(worldPosition);
            
            // 處理攝影機後方的情況
            if (screenPoint.z < 0)
            {
                screenPoint.x = -screenPoint.x;
                screenPoint.y = -screenPoint.y;
            }
            
            return new Vector2(screenPoint.x, screenPoint.y);
        }
        
        Vector2 ClampToScreenBounds(Vector2 screenPosition)
        {
            float halfButtonWidth = buttonSize.x * 0.5f;
            float halfButtonHeight = buttonSize.y * 0.5f;
            
            // 計算邊界
            float minX = screenBorderMargin + halfButtonWidth;
            float maxX = Screen.width - screenBorderMargin - halfButtonWidth;
            float minY = screenBorderMargin + halfButtonHeight;
            float maxY = Screen.height - screenBorderMargin - halfButtonHeight;
            
            // 限制在螢幕邊界內
            screenPosition.x = Mathf.Clamp(screenPosition.x, minX, maxX);
            screenPosition.y = Mathf.Clamp(screenPosition.y, minY, maxY);
            
            return screenPosition;
        }
        
        void UpdateButtonPositionSmooth(Vector2 targetPosition)
        {
            if (buttonRectTransform == null) return;
            
            Vector2 currentPosition = buttonRectTransform.position;
            Vector2 newPosition = Vector2.Lerp(currentPosition, targetPosition, followSpeed * Time.deltaTime);
            
            buttonRectTransform.position = newPosition;
        }
        
        #endregion
        
        #region 可見性控制
        
        void UpdateButtonVisibility()
        {
            bool shouldBeVisible = ShouldButtonBeVisible();
            
            if (isButtonVisible != shouldBeVisible)
            {
                isButtonVisible = shouldBeVisible;
                yukaInteractionButton.gameObject.SetActive(shouldBeVisible);
                
                // 更新相關UI元素
                UpdateRelatedUIElements(shouldBeVisible);
            }
        }
        
        bool ShouldButtonBeVisible()
        {
            if (!interactionEnabled) return false;
            
            // 檢查由香是否在螢幕範圍內
            if (hideWhenOffScreen && !IsYukaOnScreen())
            {
                return false;
            }
            
            // 檢查遊戲狀態
            if (interactionManager != null)
            {
                return interactionManager.InteractionsEnabled;
            }
            
            return true;
        }
        
        bool IsYukaOnScreen()
        {
            if (yukaSpineTransform == null || mainCamera == null) return false;
            
            Vector3 screenPos = mainCamera.WorldToScreenPoint(yukaSpineTransform.position);
            
            // 檢查是否在螢幕範圍內（考慮一些邊界容差）
            float margin = 100f;
            return screenPos.x >= -margin && screenPos.x <= Screen.width + margin &&
                   screenPos.y >= -margin && screenPos.y <= Screen.height + margin &&
                   screenPos.z > 0; // 在攝影機前方
        }
        
        void UpdateRelatedUIElements(bool visible)
        {
            // 更新Hover效果可見性
            if (yukaHoverEffect != null)
            {
                yukaHoverEffect.gameObject.SetActive(visible && isHovering);
            }
            
            // 更新互動指示器
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(visible);
            }
        }
        
        #endregion
        
        #region 互動控制
        
        public void EnableInteraction(bool enable)
        {
            interactionEnabled = enable;
            
            if (!enable)
            {
                // 禁用時隱藏所有相關UI
                if (yukaInteractionButton != null)
                    yukaInteractionButton.gameObject.SetActive(false);
                
                if (isDialogMenuOpen)
                    CloseDialogMenu();
            }
            
            Debug.Log($"[DynamicCharacterInteraction] 由香互動{(enable ? "啟用" : "禁用")}");
        }
        
        void OnYukaButtonClicked()
        {
            if (!interactionEnabled || isDialogMenuOpen) return;
            
            Debug.Log("[DynamicCharacterInteraction] 由香被點擊");
            
            // 觸發點擊事件
            OnYukaClicked?.Invoke();
            
            // 播放點擊效果
            PlayClickEffect();
            
            // 顯示對話選單
            ShowYukaDialogMenu();
        }
        
        void PlayClickEffect()
        {
            // 點擊視覺效果
            if (yukaInteractionButton != null)
            {
                StartCoroutine(ButtonClickAnimation());
            }
            
            // 點擊音效（如果需要）
            // AudioManager.PlaySFX("ButtonClick");
        }
        
        IEnumerator ButtonClickAnimation()
        {
            if (buttonRectTransform == null) yield break;
            
            Vector3 originalScale = buttonRectTransform.localScale;
            Vector3 targetScale = originalScale * 0.9f;
            
            // 縮小動畫
            float duration = 0.1f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                buttonRectTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }
            
            // 恢復原始大小
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                buttonRectTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }
            
            buttonRectTransform.localScale = originalScale;
        }
        
        #endregion
        
        #region Hover效果
        
        void OnYukaHoverEnter()
        {
            if (!interactionEnabled) return;
            
            isHovering = true;
            OnYukaHoverStateChanged?.Invoke(true);
            
            // 顯示Hover效果
            ShowHoverEffect();
            
            Debug.Log("[DynamicCharacterInteraction] 由香Hover開始");
        }
        
        void OnYukaHoverExit()
        {
            isHovering = false;
            OnYukaHoverStateChanged?.Invoke(false);
            
            // 隱藏Hover效果
            HideHoverEffect();
            
            Debug.Log("[DynamicCharacterInteraction] 由香Hover結束");
        }
        
        void ShowHoverEffect()
        {
            if (yukaHoverEffect != null)
            {
                yukaHoverEffect.gameObject.SetActive(true);
                yukaHoverEffect.color = hoverColor;
                
                // Hover動畫
                StartCoroutine(HoverScaleAnimation(true));
            }
        }
        
        void HideHoverEffect()
        {
            if (yukaHoverEffect != null)
            {
                StartCoroutine(HoverScaleAnimation(false));
            }
        }
        
        IEnumerator HoverScaleAnimation(bool scaleUp)
        {
            if (yukaHoverEffect == null) yield break;
            
            RectTransform hoverRect = yukaHoverEffect.GetComponent<RectTransform>();
            if (hoverRect == null) yield break;
            
            Vector3 startScale = hoverRect.localScale;
            Vector3 targetScale = scaleUp ? Vector3.one * hoverEffectScale : Vector3.one;
            
            float duration = 0.2f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                hoverRect.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }
            
            hoverRect.localScale = targetScale;
            
            if (!scaleUp)
            {
                yukaHoverEffect.gameObject.SetActive(false);
            }
        }
        
        #endregion
        
        #region 互動指示器
        
        void UpdateInteractionIndicator()
        {
            if (interactionIndicator == null) return;
            
            // 簡單的浮動動畫
            float floatOffset = Mathf.Sin(Time.time * indicatorAnimationSpeed) * 10f;
            Vector3 pos = interactionIndicator.transform.localPosition;
            pos.y = floatOffset;
            interactionIndicator.transform.localPosition = pos;
        }
        
        #endregion
        
        #region 對話選單系統
        
        void ShowYukaDialogMenu()
        {
            if (dialogMenu == null) return;
            
            // 獲取可用的對話選項
            List<YukaDialogOption> availableOptions = GetAvailableDialogOptions();
            
            if (availableOptions.Count == 0)
            {
                Debug.LogWarning("[DynamicCharacterInteraction] 沒有可用的對話選項");
                return;
            }
            
            // 顯示對話選單
            dialogMenu.ShowMenu(availableOptions);
            isDialogMenuOpen = true;
            
            // 通知其他系統
            OnDialogMenuRequested?.Invoke(availableOptions);
        }
        
        List<YukaDialogOption> GetAvailableDialogOptions()
        {
            var options = new List<YukaDialogOption>();
            
            // 基礎對話始終可用
            options.Add(YukaDialogOption.NormalTalk);
            
            // 根據條件添加其他選項
            if (CheckAffectionLevel(20))
            {
                options.Add(YukaDialogOption.FlirtTalk);
            }
            
            if (CheckTimeAvailableForGoingOut())
            {
                options.Add(YukaDialogOption.GoOutTogether);
            }
            
            if (CheckSexualInteractionUnlocked())
            {
                options.Add(YukaDialogOption.SexualInteraction);
            }
            
            return options;
        }
        
        bool CheckAffectionLevel(int requiredLevel)
        {
            // 從InteractionManager獲取好感度檢查
            if (interactionManager != null)
            {
                return interactionManager.IsInteractionAvailable("FlirtTalk");
            }
            return false;
        }
        
        bool CheckTimeAvailableForGoingOut()
        {
            // 檢查時間是否允許外出
            if (interactionManager != null)
            {
                return interactionManager.CurrentTimeSlot <= 6; // 不能太晚
            }
            return false;
        }
        
        bool CheckSexualInteractionUnlocked()
        {
            // 檢查特殊互動解鎖條件
            if (interactionManager != null)
            {
                return interactionManager.IsInteractionAvailable("SexualInteraction");
            }
            return false;
        }
        
        public void OnDialogOptionSelected(YukaDialogOption option)
        {
            Debug.Log($"[DynamicCharacterInteraction] 選擇對話選項: {option}");
            
            // 關閉對話選單
            CloseDialogMenu();
            
            // 處理對話選項
            ProcessDialogOption(option);
        }
        
        void ProcessDialogOption(YukaDialogOption option)
        {
            string interactionType = "";
            
            switch (option)
            {
                case YukaDialogOption.NormalTalk:
                    interactionType = "NormalTalk";
                    break;
                case YukaDialogOption.FlirtTalk:
                    interactionType = "FlirtTalk";
                    break;
                case YukaDialogOption.GoOutTogether:
                    interactionType = "GoOutTogether";
                    break;
                case YukaDialogOption.SexualInteraction:
                    interactionType = "SexualInteraction";
                    break;
            }
            
            // 觸發互動
            if (!string.IsNullOrEmpty(interactionType) && interactionManager != null)
            {
                interactionManager.TriggerInteraction(interactionType);
            }
        }
        
        void CloseDialogMenu()
        {
            if (dialogMenu != null)
            {
                dialogMenu.HideMenu();
            }
            isDialogMenuOpen = false;
        }
        
        #endregion
        
        #region 公開API
        
        /// <summary>
        /// 強制更新Button位置
        /// </summary>
        public void ForceUpdatePosition()
        {
            UpdateButtonPosition();
        }
        
        /// <summary>
        /// 設置Button偏移
        /// </summary>
        public void SetButtonOffset(Vector2 offset)
        {
            buttonOffset = offset;
        }
        
        /// <summary>
        /// 獲取由香當前螢幕位置
        /// </summary>
        public Vector2 GetYukaScreenPosition()
        {
            if (yukaSpineTransform == null) return Vector2.zero;
            return ConvertWorldToScreenPoint(yukaSpineTransform.position);
        }
        
        #endregion
        
        #region 事件清理
        
        void OnDestroy()
        {
            // 清理Button事件
            if (yukaInteractionButton != null)
            {
                yukaInteractionButton.onClick.RemoveAllListeners();
            }
            
            // 清理靜態事件
            OnYukaHoverStateChanged = null;
            OnYukaClicked = null;
            OnDialogMenuRequested = null;
        }
        
        #endregion
    }
    
    /// <summary>
    /// 由香對話選項枚舉
    /// </summary>
    public enum YukaDialogOption
    {
        NormalTalk,        // 對話 (基礎互動)
        FlirtTalk,         // 調情 (需好感度)
        GoOutTogether,     // 和由香一起外出
        SexualInteraction  // 捉i (特殊解鎖條件)
    }
}