using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LoveTide.Interaction
{
    /// <summary>
    /// 靜態物件互動系統 - 管理場景中固定位置的互動物件
    /// 包括工作、貓咪、外出、喝酒等互動按鈕
    /// </summary>
    public class StaticObjectInteraction : MonoBehaviour
    {
        [Header("=== 工作場景互動按鈕 ===")]
        [SerializeField] private Button helpWorkButton;
        [SerializeField] private Button catInteractionButton;
        [SerializeField] private Button goOutAloneButton;
        
        [Header("=== 宿舍場景互動按鈕 ===")]
        [SerializeField] private Button inviteDrinkingButton;
        [SerializeField] private Button dormInteractionButton;
        
        [Header("=== 通用互動按鈕 ===")]
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button saveGameButton;
        
        [Header("=== 互動反饋 ===")]
        [SerializeField] private GameObject hoverEffectPrefab;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip buttonHoverSound;
        
        [Header("=== 場景狀態控制 ===")]
        [SerializeField] private GameObject workSceneUI;
        [SerializeField] private GameObject dormSceneUI;
        
        // 核心管理器引用
        private LoveTide.Core.InteractionManager interactionManager;
        
        // 互動狀態追蹤
        private Dictionary<string, Button> interactionButtons = new Dictionary<string, Button>();
        private Dictionary<string, GameObject> hoverEffects = new Dictionary<string, GameObject>();
        private Dictionary<string, bool> buttonStates = new Dictionary<string, bool>();
        
        // 場景狀態
        private bool isWorkTime = true;
        private bool interactionsEnabled = true;
        
        // 公開屬性
        public bool InteractionsEnabled => interactionsEnabled;
        public Dictionary<string, bool> ButtonStates => new Dictionary<string, bool>(buttonStates);
        
        // 事件系統
        public static event Action<string> OnStaticInteractionTriggered;
        public static event Action<string, bool> OnInteractionStateChanged;
        
        #region Unity生命週期
        
        void Awake()
        {
            InitializeButtonMappings();
        }
        
        void Start()
        {
            StartCoroutine(InitializeStaticInteraction());
        }
        
        #endregion
        
        #region 初始化
        
        void InitializeButtonMappings()
        {
            // 建立按鈕映射
            RegisterButton("HelpWork", helpWorkButton);
            RegisterButton("CatPlay", catInteractionButton);
            RegisterButton("GoOutAlone", goOutAloneButton);
            RegisterButton("InviteDrinking", inviteDrinkingButton);
            RegisterButton("DormInteraction", dormInteractionButton);
            RegisterButton("Settings", settingsButton);
            RegisterButton("SaveGame", saveGameButton);
            
            // 初始化按鈕狀態
            InitializeButtonStates();
        }
        
        void RegisterButton(string interactionName, Button button)
        {
            if (button != null)
            {
                interactionButtons[interactionName] = button;
                buttonStates[interactionName] = false; // 預設禁用
                
                // 添加點擊事件
                button.onClick.AddListener(() => OnInteractionButtonClicked(interactionName));
                
                // 添加Hover事件
                AddHoverEvents(button, interactionName);
            }
        }
        
        void InitializeButtonStates()
        {
            // 設置預設狀態
            buttonStates["HelpWork"] = false;
            buttonStates["CatPlay"] = true;  // 貓咪互動通常可用
            buttonStates["GoOutAlone"] = false;
            buttonStates["InviteDrinking"] = false;
            buttonStates["DormInteraction"] = false;
            buttonStates["Settings"] = true;  // 設定始終可用
            buttonStates["SaveGame"] = true;  // 保存始終可用
        }
        
        void AddHoverEvents(Button button, string interactionName)
        {
            // 添加EventTrigger組件處理Hover事件
            UnityEngine.EventSystems.EventTrigger eventTrigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            }
            
            // Hover Enter
            UnityEngine.EventSystems.EventTrigger.Entry hoverEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
            hoverEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
            hoverEnter.callback.AddListener((data) => OnButtonHoverEnter(interactionName));
            eventTrigger.triggers.Add(hoverEnter);
            
            // Hover Exit
            UnityEngine.EventSystems.EventTrigger.Entry hoverExit = new UnityEngine.EventSystems.EventTrigger.Entry();
            hoverExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
            hoverExit.callback.AddListener((data) => OnButtonHoverExit(interactionName));
            eventTrigger.triggers.Add(hoverExit);
        }
        
        public void Initialize(LoveTide.Core.InteractionManager manager)
        {
            interactionManager = manager;
            Debug.Log("[StaticObjectInteraction] 靜態物件互動系統初始化");
        }
        
        IEnumerator InitializeStaticInteraction()
        {
            // 等待必要組件初始化
            yield return new WaitForSeconds(0.1f);
            
            // 驗證按鈕配置
            ValidateButtonConfiguration();
            
            // 創建Hover效果
            CreateHoverEffects();
            
            // 設置初始場景狀態
            UpdateSceneUI();
            
            Debug.Log("[StaticObjectInteraction] 靜態物件互動系統初始化完成");
        }
        
        void ValidateButtonConfiguration()
        {
            foreach (var kvp in interactionButtons)
            {
                if (kvp.Value == null)
                {
                    Debug.LogWarning($"[StaticObjectInteraction] 互動按鈕 {kvp.Key} 引用缺失");
                }
            }
        }
        
        void CreateHoverEffects()
        {
            if (hoverEffectPrefab == null) return;
            
            foreach (var kvp in interactionButtons)
            {
                if (kvp.Value != null)
                {
                    CreateHoverEffect(kvp.Key, kvp.Value);
                }
            }
        }
        
        void CreateHoverEffect(string interactionName, Button button)
        {
            if (hoverEffectPrefab == null) return;
            
            // 創建Hover效果物件
            GameObject hoverEffect = Instantiate(hoverEffectPrefab, button.transform);
            hoverEffect.name = $"HoverEffect_{interactionName}";
            hoverEffect.SetActive(false);
            
            // 儲存引用
            hoverEffects[interactionName] = hoverEffect;
        }
        
        #endregion
        
        #region 互動控制
        
        public void EnableInteraction(string interactionName, bool enable)
        {
            if (!interactionButtons.ContainsKey(interactionName))
            {
                Debug.LogWarning($"[StaticObjectInteraction] 找不到互動: {interactionName}");
                return;
            }
            
            bool oldState = buttonStates[interactionName];
            buttonStates[interactionName] = enable;
            
            Button button = interactionButtons[interactionName];
            if (button != null)
            {
                button.interactable = enable && interactionsEnabled;
                button.gameObject.SetActive(enable);
                
                // 更新視覺狀態
                UpdateButtonVisualState(interactionName, enable);
            }
            
            if (oldState != enable)
            {
                OnInteractionStateChanged?.Invoke(interactionName, enable);
                Debug.Log($"[StaticObjectInteraction] 互動 {interactionName} {(enable ? "啟用" : "禁用")}");
            }
        }
        
        void UpdateButtonVisualState(string interactionName, bool enabled)
        {
            Button button = interactionButtons[interactionName];
            if (button == null) return;
            
            // 更新按鈕顏色
            ColorBlock colors = button.colors;
            if (enabled)
            {
                colors.normalColor = Color.white;
                colors.disabledColor = Color.gray;
            }
            else
            {
                colors.normalColor = Color.gray;
                colors.disabledColor = Color.gray * 0.5f;
            }
            button.colors = colors;
        }
        
        public void SetAllInteractionsEnabled(bool enabled)
        {
            interactionsEnabled = enabled;
            
            foreach (var kvp in interactionButtons)
            {
                if (kvp.Value != null)
                {
                    bool shouldBeEnabled = enabled && buttonStates[kvp.Key];
                    kvp.Value.interactable = shouldBeEnabled;
                    
                    UpdateButtonVisualState(kvp.Key, shouldBeEnabled);
                }
            }
            
            Debug.Log($"[StaticObjectInteraction] 所有互動{(enabled ? "啟用" : "禁用")}");
        }
        
        #endregion
        
        #region 場景狀態管理
        
        public void UpdateSceneState(bool newIsWorkTime)
        {
            if (isWorkTime == newIsWorkTime) return;
            
            isWorkTime = newIsWorkTime;
            
            // 更新場景相關的互動可用性
            UpdateSceneSpecificInteractions();
            
            // 更新場景UI
            UpdateSceneUI();
            
            Debug.Log($"[StaticObjectInteraction] 場景狀態更新: {(isWorkTime ? "工作時間" : "宿舍時間")}");
        }
        
        void UpdateSceneSpecificInteractions()
        {
            if (isWorkTime)
            {
                // 工作時間可用的互動
                EnableInteraction("HelpWork", true);
                EnableInteraction("CatPlay", true);
                EnableInteraction("GoOutAlone", true);
                EnableInteraction("InviteDrinking", false);
                EnableInteraction("DormInteraction", false);
            }
            else
            {
                // 宿舍時間可用的互動
                EnableInteraction("HelpWork", false);
                EnableInteraction("CatPlay", true); // 貓咪隨時可互動
                EnableInteraction("GoOutAlone", false); // 晚上不能獨自外出
                EnableInteraction("InviteDrinking", true);
                EnableInteraction("DormInteraction", true);
            }
        }
        
        void UpdateSceneUI()
        {
            if (workSceneUI != null)
            {
                workSceneUI.SetActive(isWorkTime);
            }
            
            if (dormSceneUI != null)
            {
                dormSceneUI.SetActive(!isWorkTime);
            }
        }
        
        #endregion
        
        #region 事件處理
        
        void OnInteractionButtonClicked(string interactionName)
        {
            if (!interactionsEnabled || !buttonStates[interactionName])
            {
                Debug.LogWarning($"[StaticObjectInteraction] 互動 {interactionName} 當前不可用");
                return;
            }
            
            Debug.Log($"[StaticObjectInteraction] 觸發互動: {interactionName}");
            
            // 播放點擊音效
            PlayClickSound();
            
            // 播放點擊動畫
            StartCoroutine(PlayClickAnimation(interactionName));
            
            // 處理互動邏輯
            ProcessInteraction(interactionName);
            
            // 通知系統
            OnStaticInteractionTriggered?.Invoke(interactionName);
        }
        
        void ProcessInteraction(string interactionName)
        {
            switch (interactionName)
            {
                case "HelpWork":
                    ProcessHelpWorkInteraction();
                    break;
                case "CatPlay":
                    ProcessCatInteraction();
                    break;
                case "GoOutAlone":
                    ProcessGoOutInteraction();
                    break;
                case "InviteDrinking":
                    ProcessDrinkingInteraction();
                    break;
                case "DormInteraction":
                    ProcessDormInteraction();
                    break;
                case "Settings":
                    ProcessSettingsInteraction();
                    break;
                case "SaveGame":
                    ProcessSaveGameInteraction();
                    break;
                default:
                    Debug.LogWarning($"[StaticObjectInteraction] 未處理的互動類型: {interactionName}");
                    break;
            }
        }
        
        void ProcessHelpWorkInteraction()
        {
            Debug.Log("[StaticObjectInteraction] 處理幫忙工作互動");
            
            // 通過InteractionManager觸發互動
            if (interactionManager != null)
            {
                interactionManager.TriggerInteraction("HelpWork");
            }
        }
        
        void ProcessCatInteraction()
        {
            Debug.Log("[StaticObjectInteraction] 處理貓咪互動");
            
            if (interactionManager != null)
            {
                interactionManager.TriggerInteraction("CatPlay");
            }
        }
        
        void ProcessGoOutInteraction()
        {
            Debug.Log("[StaticObjectInteraction] 處理外出互動");
            
            if (interactionManager != null)
            {
                interactionManager.TriggerInteraction("GoOutAlone");
            }
        }
        
        void ProcessDrinkingInteraction()
        {
            Debug.Log("[StaticObjectInteraction] 處理邀請喝酒互動");
            
            if (interactionManager != null)
            {
                interactionManager.TriggerInteraction("InviteDrinking");
            }
        }
        
        void ProcessDormInteraction()
        {
            Debug.Log("[StaticObjectInteraction] 處理宿舍互動");
            
            if (interactionManager != null)
            {
                interactionManager.TriggerInteraction("DormInteraction");
            }
        }
        
        void ProcessSettingsInteraction()
        {
            Debug.Log("[StaticObjectInteraction] 打開設定選單");
            
            // 這裡可以觸發設定選單的顯示
            // 通過UIManager或其他方式
        }
        
        void ProcessSaveGameInteraction()
        {
            Debug.Log("[StaticObjectInteraction] 保存遊戲");
            
            // 觸發遊戲保存
            if (interactionManager != null)
            {
                interactionManager.TriggerInteraction("SaveGame");
            }
        }
        
        #endregion
        
        #region Hover效果
        
        void OnButtonHoverEnter(string interactionName)
        {
            if (!interactionsEnabled || !buttonStates[interactionName]) return;
            
            // 顯示Hover效果
            if (hoverEffects.ContainsKey(interactionName))
            {
                GameObject hoverEffect = hoverEffects[interactionName];
                if (hoverEffect != null)
                {
                    hoverEffect.SetActive(true);
                    StartCoroutine(PlayHoverEnterAnimation(hoverEffect));
                }
            }
            
            // 播放Hover音效
            PlayHoverSound();
            
            Debug.Log($"[StaticObjectInteraction] Hover進入: {interactionName}");
        }
        
        void OnButtonHoverExit(string interactionName)
        {
            // 隱藏Hover效果
            if (hoverEffects.ContainsKey(interactionName))
            {
                GameObject hoverEffect = hoverEffects[interactionName];
                if (hoverEffect != null)
                {
                    StartCoroutine(PlayHoverExitAnimation(hoverEffect));
                }
            }
            
            Debug.Log($"[StaticObjectInteraction] Hover退出: {interactionName}");
        }
        
        IEnumerator PlayHoverEnterAnimation(GameObject hoverEffect)
        {
            if (hoverEffect == null) yield break;
            
            // 簡單的縮放動畫
            Transform transform = hoverEffect.transform;
            Vector3 startScale = Vector3.zero;
            Vector3 targetScale = Vector3.one;
            
            float duration = 0.2f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }
            
            transform.localScale = targetScale;
        }
        
        IEnumerator PlayHoverExitAnimation(GameObject hoverEffect)
        {
            if (hoverEffect == null) yield break;
            
            Transform transform = hoverEffect.transform;
            Vector3 startScale = transform.localScale;
            Vector3 targetScale = Vector3.zero;
            
            float duration = 0.15f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }
            
            hoverEffect.SetActive(false);
        }
        
        #endregion
        
        #region 動畫效果
        
        IEnumerator PlayClickAnimation(string interactionName)
        {
            if (!interactionButtons.ContainsKey(interactionName)) yield break;
            
            Button button = interactionButtons[interactionName];
            if (button == null) yield break;
            
            Transform buttonTransform = button.transform;
            Vector3 originalScale = buttonTransform.localScale;
            Vector3 targetScale = originalScale * 0.9f;
            
            // 縮小動畫
            float duration = 0.1f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                buttonTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }
            
            // 恢復動畫
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                buttonTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }
            
            buttonTransform.localScale = originalScale;
        }
        
        #endregion
        
        #region 音效控制
        
        void PlayClickSound()
        {
            if (buttonClickSound != null)
            {
                // 播放點擊音效
                // AudioSource.PlayClipAtPoint(buttonClickSound, transform.position);
            }
        }
        
        void PlayHoverSound()
        {
            if (buttonHoverSound != null)
            {
                // 播放Hover音效
                // AudioSource.PlayClipAtPoint(buttonHoverSound, transform.position);
            }
        }
        
        #endregion
        
        #region 公開API
        
        /// <summary>
        /// 檢查特定互動是否可用
        /// </summary>
        public bool IsInteractionEnabled(string interactionName)
        {
            return buttonStates.ContainsKey(interactionName) && buttonStates[interactionName] && interactionsEnabled;
        }
        
        /// <summary>
        /// 獲取所有可用的互動列表
        /// </summary>
        public List<string> GetAvailableInteractions()
        {
            var available = new List<string>();
            
            foreach (var kvp in buttonStates)
            {
                if (kvp.Value && interactionsEnabled)
                {
                    available.Add(kvp.Key);
                }
            }
            
            return available;
        }
        
        /// <summary>
        /// 強制刷新所有按鈕狀態
        /// </summary>
        public void RefreshAllButtonStates()
        {
            UpdateSceneSpecificInteractions();
            UpdateSceneUI();
        }
        
        /// <summary>
        /// 獲取按鈕引用
        /// </summary>
        public Button GetButton(string interactionName)
        {
            return interactionButtons.ContainsKey(interactionName) ? interactionButtons[interactionName] : null;
        }
        
        #endregion
        
        #region 清理
        
        void OnDestroy()
        {
            // 清理按鈕事件
            foreach (var kvp in interactionButtons)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.onClick.RemoveAllListeners();
                }
            }
            
            // 清理靜態事件
            OnStaticInteractionTriggered = null;
            OnInteractionStateChanged = null;
        }
        
        #endregion
    }
}