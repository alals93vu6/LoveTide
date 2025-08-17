using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LoveTide.Interaction
{
    /// <summary>
    /// 由香對話選單系統 - 管理由香互動的對話選項UI
    /// 動態生成和管理對話選項按鈕
    /// </summary>
    public class YukaDialogMenu : MonoBehaviour
    {
        [Header("=== 對話選單UI ===")]
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private GameObject dialogButtonPrefab;
        
        [Header("=== 選單配置 ===")]
        [SerializeField] private Vector2 menuOffset = new Vector2(50, 0);
        [SerializeField] private float showAnimationDuration = 0.3f;
        [SerializeField] private float hideAnimationDuration = 0.2f;
        
        [Header("=== 按鈕樣式 ===")]
        [SerializeField] private Color normalButtonColor = Color.white;
        [SerializeField] private Color hoveredButtonColor = Color.cyan;
        [SerializeField] private Color disabledButtonColor = Color.gray;
        
        // 動態角色互動引用
        private DynamicCharacterInteraction parentInteraction;
        
        // 選單狀態
        private bool isMenuVisible = false;
        private List<GameObject> activeButtons = new List<GameObject>();
        private CanvasGroup menuCanvasGroup;
        
        // 對話選項配置
        private Dictionary<YukaDialogOption, DialogOptionConfig> optionConfigs;
        
        #region 對話選項配置結構
        
        [System.Serializable]
        public class DialogOptionConfig
        {
            public YukaDialogOption Option;
            public string DisplayText;
            public string Description;
            public Sprite IconSprite;
            public bool RequiresCondition;
            public string ConditionDescription;
        }
        
        #endregion
        
        #region Unity生命週期
        
        void Awake()
        {
            InitializeComponents();
            SetupOptionConfigs();
        }
        
        void Start()
        {
            InitializeMenuUI();
        }
        
        #endregion
        
        #region 初始化
        
        void InitializeComponents()
        {
            // 確保選單面板存在
            if (menuPanel == null)
            {
                CreateMenuPanel();
            }
            
            // 獲取或添加CanvasGroup
            menuCanvasGroup = menuPanel.GetComponent<CanvasGroup>();
            if (menuCanvasGroup == null)
            {
                menuCanvasGroup = menuPanel.AddComponent<CanvasGroup>();
            }
            
            // 確保按鈕容器存在
            if (buttonContainer == null)
            {
                CreateButtonContainer();
            }
        }
        
        void CreateMenuPanel()
        {
            // 創建選單面板
            GameObject menuObj = new GameObject("YukaDialogMenu");
            menuObj.transform.SetParent(transform, false);
            
            menuPanel = menuObj;
            
            // 添加Image組件作為背景
            Image backgroundImage = menuObj.AddComponent<Image>();
            backgroundImage.color = new Color(0, 0, 0, 0.8f); // 半透明黑色背景
            
            // 設置RectTransform
            RectTransform rectTransform = menuObj.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 300);
        }
        
        void CreateButtonContainer()
        {
            // 創建按鈕容器
            GameObject containerObj = new GameObject("ButtonContainer");
            containerObj.transform.SetParent(menuPanel.transform, false);
            
            buttonContainer = containerObj.transform;
            
            // 添加VerticalLayoutGroup
            VerticalLayoutGroup layoutGroup = containerObj.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 10f;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;
            
            // 添加ContentSizeFitter
            ContentSizeFitter sizeFitter = containerObj.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        
        void SetupOptionConfigs()
        {
            optionConfigs = new Dictionary<YukaDialogOption, DialogOptionConfig>
            {
                {
                    YukaDialogOption.NormalTalk,
                    new DialogOptionConfig
                    {
                        Option = YukaDialogOption.NormalTalk,
                        DisplayText = "💬 對話",
                        Description = "和由香進行日常對話",
                        RequiresCondition = false
                    }
                },
                {
                    YukaDialogOption.FlirtTalk,
                    new DialogOptionConfig
                    {
                        Option = YukaDialogOption.FlirtTalk,
                        DisplayText = "😊 調情",
                        Description = "和由香進行親密對話",
                        RequiresCondition = true,
                        ConditionDescription = "需要好感度 ≥ 20"
                    }
                },
                {
                    YukaDialogOption.GoOutTogether,
                    new DialogOptionConfig
                    {
                        Option = YukaDialogOption.GoOutTogether,
                        DisplayText = "🚶‍♀️ 一起外出",
                        Description = "邀請由香一起出門",
                        RequiresCondition = true,
                        ConditionDescription = "需要適當的時間"
                    }
                },
                {
                    YukaDialogOption.SexualInteraction,
                    new DialogOptionConfig
                    {
                        Option = YukaDialogOption.SexualInteraction,
                        DisplayText = "💕 捉i",
                        Description = "和由香進行親密互動",
                        RequiresCondition = true,
                        ConditionDescription = "需要特殊解鎖條件"
                    }
                }
            };
        }
        
        void InitializeMenuUI()
        {
            // 初始隱藏選單
            menuPanel.SetActive(false);
            menuCanvasGroup.alpha = 0f;
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
        }
        
        public void Initialize(DynamicCharacterInteraction interaction)
        {
            parentInteraction = interaction;
            Debug.Log("[YukaDialogMenu] 由香對話選單初始化");
        }
        
        #endregion
        
        #region 選單顯示控制
        
        public void ShowMenu(List<YukaDialogOption> availableOptions)
        {
            if (isMenuVisible) return;
            
            Debug.Log($"[YukaDialogMenu] 顯示對話選單，選項數量: {availableOptions.Count}");
            
            // 清理舊按鈕
            ClearExistingButtons();
            
            // 創建新按鈕
            CreateDialogButtons(availableOptions);
            
            // 更新選單位置
            UpdateMenuPosition();
            
            // 顯示選單
            StartCoroutine(ShowMenuAnimation());
        }
        
        public void HideMenu()
        {
            if (!isMenuVisible) return;
            
            Debug.Log("[YukaDialogMenu] 隱藏對話選單");
            
            StartCoroutine(HideMenuAnimation());
        }
        
        void ClearExistingButtons()
        {
            foreach (GameObject button in activeButtons)
            {
                if (button != null)
                {
                    DestroyImmediate(button);
                }
            }
            activeButtons.Clear();
        }
        
        void CreateDialogButtons(List<YukaDialogOption> options)
        {
            foreach (YukaDialogOption option in options)
            {
                if (optionConfigs.ContainsKey(option))
                {
                    GameObject buttonObj = CreateDialogButton(optionConfigs[option]);
                    activeButtons.Add(buttonObj);
                }
            }
        }
        
        GameObject CreateDialogButton(DialogOptionConfig config)
        {
            GameObject buttonObj;
            
            // 使用預製件或創建新按鈕
            if (dialogButtonPrefab != null)
            {
                buttonObj = Instantiate(dialogButtonPrefab, buttonContainer);
            }
            else
            {
                buttonObj = CreateDefaultButton();
            }
            
            // 設置按鈕內容
            SetupButtonContent(buttonObj, config);
            
            // 添加點擊事件
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnDialogOptionClicked(config.Option));
            }
            
            return buttonObj;
        }
        
        GameObject CreateDefaultButton()
        {
            // 創建預設按鈕
            GameObject buttonObj = new GameObject("DialogOptionButton");
            buttonObj.transform.SetParent(buttonContainer, false);
            
            // 添加Button組件
            Button button = buttonObj.AddComponent<Button>();
            
            // 添加Image組件
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = normalButtonColor;
            
            // 設置RectTransform
            RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(180, 40);
            
            // 創建文字子物件
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            Text buttonText = textObj.AddComponent<Text>();
            buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            buttonText.fontSize = 16;
            buttonText.color = Color.black;
            buttonText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            return buttonObj;
        }
        
        void SetupButtonContent(GameObject buttonObj, DialogOptionConfig config)
        {
            // 設置按鈕文字
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = config.DisplayText;
            }
            
            // 設置按鈕顏色
            Image buttonImage = buttonObj.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = normalButtonColor;
            }
            
            // 如果有圖標，設置圖標
            if (config.IconSprite != null)
            {
                // 這裡可以添加圖標設置邏輯
            }
        }
        
        #endregion
        
        #region 選單動畫
        
        IEnumerator ShowMenuAnimation()
        {
            isMenuVisible = true;
            menuPanel.SetActive(true);
            
            // 設置初始狀態
            menuCanvasGroup.alpha = 0f;
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
            
            // 淡入動畫
            float elapsed = 0f;
            while (elapsed < showAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / showAnimationDuration;
                
                // 使用平滑曲線
                t = Mathf.SmoothStep(0f, 1f, t);
                
                menuCanvasGroup.alpha = t;
                
                yield return null;
            }
            
            // 完成動畫
            menuCanvasGroup.alpha = 1f;
            menuCanvasGroup.interactable = true;
            menuCanvasGroup.blocksRaycasts = true;
        }
        
        IEnumerator HideMenuAnimation()
        {
            // 設置開始狀態
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
            
            // 淡出動畫
            float elapsed = 0f;
            float startAlpha = menuCanvasGroup.alpha;
            
            while (elapsed < hideAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / hideAnimationDuration;
                
                menuCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
                
                yield return null;
            }
            
            // 完成動畫
            menuCanvasGroup.alpha = 0f;
            menuPanel.SetActive(false);
            isMenuVisible = false;
            
            // 清理按鈕
            ClearExistingButtons();
        }
        
        #endregion
        
        #region 位置管理
        
        void UpdateMenuPosition()
        {
            if (parentInteraction == null) return;
            
            // 獲取由香當前螢幕位置
            Vector2 yukaScreenPos = parentInteraction.GetYukaScreenPosition();
            
            // 計算選單位置（在由香旁邊）
            Vector2 menuPosition = yukaScreenPos + menuOffset;
            
            // 邊界檢查
            menuPosition = ClampMenuToScreen(menuPosition);
            
            // 設置選單位置
            RectTransform menuRect = menuPanel.GetComponent<RectTransform>();
            if (menuRect != null)
            {
                menuRect.position = menuPosition;
            }
        }
        
        Vector2 ClampMenuToScreen(Vector2 position)
        {
            RectTransform menuRect = menuPanel.GetComponent<RectTransform>();
            if (menuRect == null) return position;
            
            Vector2 menuSize = menuRect.sizeDelta;
            
            // 確保選單在螢幕範圍內
            float minX = menuSize.x * 0.5f;
            float maxX = Screen.width - menuSize.x * 0.5f;
            float minY = menuSize.y * 0.5f;
            float maxY = Screen.height - menuSize.y * 0.5f;
            
            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.y = Mathf.Clamp(position.y, minY, maxY);
            
            return position;
        }
        
        #endregion
        
        #region 事件處理
        
        void OnDialogOptionClicked(YukaDialogOption option)
        {
            Debug.Log($"[YukaDialogMenu] 點擊對話選項: {option}");
            
            // 播放點擊音效
            // AudioManager.PlaySFX("MenuClick");
            
            // 通知父級組件
            if (parentInteraction != null)
            {
                parentInteraction.OnDialogOptionSelected(option);
            }
        }
        
        #endregion
        
        #region 輔助方法
        
        /// <summary>
        /// 檢查選單是否顯示中
        /// </summary>
        public bool IsMenuVisible()
        {
            return isMenuVisible;
        }
        
        /// <summary>
        /// 設置選單偏移
        /// </summary>
        public void SetMenuOffset(Vector2 offset)
        {
            menuOffset = offset;
        }
        
        /// <summary>
        /// 獲取當前活動按鈕數量
        /// </summary>
        public int GetActiveButtonCount()
        {
            return activeButtons.Count;
        }
        
        #endregion
        
        #region 清理
        
        void OnDestroy()
        {
            // 清理所有按鈕
            ClearExistingButtons();
        }
        
        #endregion
    }
}