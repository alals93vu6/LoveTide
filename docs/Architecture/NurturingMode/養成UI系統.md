# 🎨 UI架構設計

> LoveTide 的使用者介面架構設計與管理系統

---

## 🎯 概述

UI架構設計負責管理遊戲中所有使用者介面的顯示、交互和狀態管理，提供一致且流暢的用戶體驗。

---

## 🧱 UI層級結構

### 📊 Canvas 層級架構 (更新版)

#### 🔄 **舊系統 Canvas 結構**
```
🖼️ UI Root Canvas (舊系統)
├── 📱 Background Layer (Order: 0)
│   ├── 場景背景
│   ├── 角色立繪
│   └── 環境效果
├── 🎮 Game UI Layer (Order: 100)
│   ├── HUD 元素
│   ├── 互動按鈕
│   └── 狀態顯示
├── 💬 Dialog Layer (Order: 200)
│   ├── 對話框
│   ├── 選項列表
│   └── 角色名稱
├── 📋 Menu Layer (Order: 300)
│   ├── 主選單
│   ├── 設定選單
│   └── 存檔選單
└── 🚨 Popup Layer (Order: 400)
    ├── 確認對話框
    ├── 提示訊息
    └── 載入畫面
```

#### 🆕 **新系統 Canvas 結構** (重製場景專用)
```
🖼️ UI Root Canvas (新系統)
├── 📱 Background Canvas (Order: 0)
│   ├── 場景背景圖片
│   ├── 裝飾性元素
│   └── 環境特效
├── 🆕 Static Interaction Canvas (Order: 40)
│   ├── 🚪 門互動按鈕
│   ├── 🪑 桌子互動按鈕
│   ├── 👥 客人互動按鈕 (協助工作)
│   └── 🏠 其他場景物件按鈕
├── 🆕 Dynamic Character Canvas (Order: 50)
│   ├── 🚶‍♀️ 移動角色互動按鈕 (跟隨 Spine)
│   ├── ✨ 角色互動視覺效果
│   └── 🎯 角色互動提示
├── 🎮 Game UI Canvas (Order: 60)
│   └── ⚙️ 設定按鈕
├── 💬 Dialog Canvas (Order: 70)
│   ├── 對話框系統
│   ├── 角色名稱顯示
│   └── 選項按鈕
├── 📋 Menu Canvas (Order: 80)
│   ├── 主選單面板
│   ├── 設定選單面板
│   └── 存檔選單面板
└── 🚨 Popup Canvas (Order: 90)
    ├── 確認對話框
    ├── 載入提示
    └── 錯誤訊息
```

### 🎯 層級管理策略
- **Z-Order 管理**: 嚴格的層級順序控制
- **自動排序**: 同層級內的自動排序機制
- **動態調整**: 根據需要動態調整層級
- **遮罩處理**: 適當的遮罩和透明度控制

---

## 🏗️ UI管理器架構

### 📋 主要UI管理器
```csharp
public class UIManager : MonoBehaviour
{
    [Header("UI Containers")]
    public Transform backgroundContainer;
    public Transform gameUIContainer;
    public Transform dialogContainer;
    public Transform menuContainer;
    public Transform popupContainer;
    
    [Header("UI Panels")]
    public Dictionary<string, UIPanel> registeredPanels;
    
    public void ShowPanel(string panelName);
    public void HidePanel(string panelName);
    public void TogglePanel(string panelName);
}
```

### 🔄 面板管理系統
```csharp
public abstract class UIPanel : MonoBehaviour
{
    public string panelName;
    public bool isModal;
    public bool destroyOnHide;
    
    public virtual void OnPanelShow() { }
    public virtual void OnPanelHide() { }
    public virtual void OnPanelUpdate() { }
}
```

---

## 💬 對話系統UI

### 📱 對話框架構
```csharp
public class DialogUI : UIPanel
{
    [Header("Dialog Components")]
    public Text characterNameText;
    public Text dialogText;
    public Image characterPortrait;
    public Button continueButton;
    public Transform choiceContainer;
    
    [Header("Animation")]
    public float typewriterSpeed = 0.05f;
    public AnimationCurve fadeInCurve;
    
    public void ShowDialog(DialogData data);
    public void ShowChoices(List<ChoiceData> choices);
    public void ClearDialog();
}
```

### 🎪 對話顯示效果
- **打字機效果**: 文字逐字顯示
- **角色立繪**: 根據情緒切換表情
- **背景切換**: 配合劇情切換背景
- **選項動畫**: 選項按鈕的動畫效果

### 📋 對話數據結構
```csharp
public class DialogData
{
    public string characterName;
    public string dialogText;
    public Sprite characterPortrait;
    public Color textColor;
    public AudioClip voiceClip;
    public float displaySpeed;
}
```

---

## 🎮 遊戲HUD設計

### 📊 HUD元素配置
```csharp
public class GameHUD : UIPanel
{
    [Header("Status Display")]
    public Text moneyText;
    public Text dayText;
    public Text timeText;
    public Slider affectionSlider;
    
    [Header("Quick Actions")]
    public Button menuButton;
    public Button saveButton;
    public Button settingsButton;
    
    public void UpdateMoney(int amount);
    public void UpdateTime(GameTime time);
    public void UpdateAffection(float value);
}
```

### 🎯 HUD佈局原則
- **視覺層次**: 重要資訊優先顯示
- **一致性**: 統一的視覺風格
- **可讀性**: 清晰的字體和對比度
- **響應式**: 適應不同螢幕尺寸

---

## 📋 選單系統設計

### 🎛️ 主選單架構
```csharp
public class MainMenu : UIPanel
{
    [Header("Menu Buttons")]
    public Button startGameButton;
    public Button loadGameButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button quitButton;
    
    [Header("Background")]
    public Image backgroundImage;
    public SkeletonGraphic characterAnimation;
    
    public void OnStartGame();
    public void OnLoadGame();
    public void OnSettings();
    public void OnCredits();
    public void OnQuit();
}
```

### ⚙️ 設定選單架構
```csharp
public class SettingsMenu : UIPanel
{
    [Header("Audio Settings")]
    public Slider masterVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;
    
    [Header("Game Settings")]
    public Dropdown languageDropdown;
    public Toggle fullscreenToggle;
    public Slider textSpeedSlider;
    
    public void OnApplySettings();
    public void OnResetSettings();
    public void OnCloseSettings();
}
```

---

## 🎨 UI動畫系統

### 🎪 動畫控制器
```csharp
public class UIAnimationController : MonoBehaviour
{
    [Header("Animation Settings")]
    public float defaultDuration = 0.3f;
    public Ease defaultEase = Ease.OutQuad;
    
    public void FadeIn(CanvasGroup target, float duration = -1);
    public void FadeOut(CanvasGroup target, float duration = -1);
    public void SlideIn(RectTransform target, Vector2 direction);
    public void SlideOut(RectTransform target, Vector2 direction);
    public void ScaleIn(Transform target);
    public void ScaleOut(Transform target);
}
```

### 📱 轉場動畫
```csharp
public class TransitionManager : MonoBehaviour
{
    [Header("Transition Effects")]
    public Image transitionImage;
    public AnimationCurve transitionCurve;
    
    public void FadeTransition(float duration, System.Action onComplete = null);
    public void SlideTransition(Vector2 direction, float duration);
    public void CircleTransition(Vector2 center, float duration);
}
```

---

## 🔧 UI響應式設計

### 📱 螢幕適配策略
```csharp
public class UIScaler : MonoBehaviour
{
    [Header("Scale Settings")]
    public float referenceWidth = 1920f;
    public float referenceHeight = 1080f;
    public float matchWidthOrHeight = 0.5f;
    
    private void Start()
    {
        AdjustUIScale();
    }
    
    private void AdjustUIScale()
    {
        var scaler = GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(referenceWidth, referenceHeight);
        scaler.matchWidthOrHeight = matchWidthOrHeight;
    }
}
```

### 🎯 適配方案
- **比例縮放**: 根據螢幕比例調整UI大小
- **錨點設定**: 適當的錨點確保UI位置正確
- **安全區域**: 處理異形螢幕的安全區域
- **字體縮放**: 動態調整字體大小

---

## 🎮 互動系統設計

### 👆 觸控/點擊處理
```csharp
public class UIInteractionHandler : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float doubleClickTime = 0.3f;
    public float longPressTime = 1.0f;
    
    public UnityEvent OnClick;
    public UnityEvent OnDoubleClick;
    public UnityEvent OnLongPress;
    
    private void Update()
    {
        HandleInput();
    }
    
    private void HandleInput()
    {
        // 處理各種輸入事件
    }
}
```

### 🎯 手勢識別
- **點擊**: 基本的點擊操作
- **雙擊**: 快速連續點擊
- **長按**: 長時間按住
- **拖拽**: 拖拽操作（如果需要）

---

## 🔊 UI音效整合

### 🎵 UI音效管理
```csharp
public class UISoundManager : MonoBehaviour
{
    [Header("UI Sounds")]
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;
    public AudioClip panelOpenSound;
    public AudioClip panelCloseSound;
    public AudioClip errorSound;
    
    public void PlayButtonClick();
    public void PlayButtonHover();
    public void PlayPanelOpen();
    public void PlayPanelClose();
    public void PlayError();
}
```

### 🎶 音效觸發時機
- **按鈕操作**: 點擊、懸停音效
- **面板切換**: 開啟、關閉音效
- **確認操作**: 確認、取消音效
- **錯誤提示**: 錯誤、警告音效

---

## 📊 UI性能優化

### 🚀 優化策略
```csharp
public class UIOptimizer : MonoBehaviour
{
    [Header("Optimization Settings")]
    public bool useObjectPooling = true;
    public bool enableUIBatching = true;
    public int maxUIElements = 100;
    
    private Queue<GameObject> uiPool = new Queue<GameObject>();
    
    public GameObject GetUIElement(GameObject prefab);
    public void ReturnUIElement(GameObject element);
    public void BatchUIUpdates();
}
```

### 🎯 性能優化點
- **對象池**: 重用UI元素減少GC
- **批量更新**: 批量處理UI更新
- **LayerMask**: 適當的LayerMask設定
- **Canvas分離**: 分離靜態和動態UI

---

## 🔍 UI測試與除錯

### 🛠 除錯工具
```csharp
public class UIDebugger : MonoBehaviour
{
    [Header("Debug Options")]
    public bool showUIBounds = false;
    public bool showRaycastTargets = false;
    public bool logUIEvents = false;
    
    private void OnGUI()
    {
        if (showUIBounds)
        {
            DrawUIBounds();
        }
    }
    
    private void DrawUIBounds()
    {
        // 繪製UI邊界用於除錯
    }
}
```

### 🎯 測試重點
- **響應式測試**: 不同解析度下的顯示
- **互動測試**: 所有互動功能正常
- **性能測試**: UI更新性能表現
- **可用性測試**: 用戶體驗友好度

---

---

## 🆕 **新系統：互動 UI 架構整合**

> ⚠️ **架構擴展**: 新增移動角色互動系統的 UI 架構設計

### 🏗️ **InteractionManager UI 整合**

#### 🎯 **新增的 UI 管理組件**
```csharp
// GameUICtrlmanager 的擴展
public class GameUICtrlmanager : MonoBehaviour
{
    [Header("新增：互動系統 Canvas")]
    public Canvas staticInteractionCanvas;   // Order: 40 - 場景物件互動
    public Canvas dynamicCharacterCanvas;    // Order: 50 - 移動角色互動
    
    [Header("新增：互動管理器")]
    public InteractionManager interactionManager;
    
    [Header("新增：互動 UI 組件")]
    public DynamicCharacterInteractionUI characterInteractionUI;
    public StaticObjectInteractionUI sceneInteractionUI;
    public InteractionFeedbackUI feedbackUI;
    
    // 🆕 新增：初始化互動 UI 系統
    public void InitializeInteractionUI()
    {
        SetupInteractionCanvases();
        InitializeInteractionComponents();
        BindInteractionEvents();
    }
}
```

#### 🎨 **動態角色互動 UI**
```csharp
// 專門處理移動角色的 UI 組件
public class DynamicCharacterInteractionUI : MonoBehaviour
{
    [Header("角色跟隨 Button")]
    public Button characterInteractionButton;    // 透明跟隨按鈕
    public RectTransform buttonRectTransform;    // Button 的 RectTransform
    
    [Header("視覺反饋元件")]
    public GameObject interactionIndicator;      // 互動指示器 UI
    public Image hoverEffectImage;               // Hover 效果圖片
    public ParticleSystem clickEffectParticle;  // 點擊特效
    
    [Header("角色引用")]
    public Transform yukaSpineTransform;         // Yuka 的 Spine Transform
    
    [Header("Button 設定")]
    public Vector2 buttonSize = new Vector2(100, 150);
    public Vector2 buttonOffset = Vector2.zero;
    
    // 核心功能：Button 跟隨角色移動
    void Update()
    {
        UpdateButtonPosition();
        UpdateButtonVisibility();
    }
    
    void UpdateButtonPosition()
    {
        if (yukaSpineTransform != null)
        {
            // World Space → Screen Space 轉換
            Vector3 worldPos = yukaSpineTransform.position + (Vector3)buttonOffset;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            buttonRectTransform.position = screenPos;
        }
    }
    
    void UpdateButtonVisibility()
    {
        // 檢查角色是否在螢幕範圍內
        bool isOnScreen = IsCharacterOnScreen();
        characterInteractionButton.gameObject.SetActive(isOnScreen);
    }
}
```

#### 🏠 **靜態物件互動 UI**
```csharp
// 處理場景物件的 UI 組件
public class StaticObjectInteractionUI : MonoBehaviour
{
    [Header("場景互動按鈕")]
    public List<SceneInteractionButton> sceneButtons;
    
    [System.Serializable]
    public class SceneInteractionButton
    {
        public Button button;                    // 互動按鈕
        public InteractionType type;             // 互動類型
        public string objectName;                // 物件名稱
        public Vector2 worldPosition;            // 世界座標位置
        public bool isEnabled = true;            // 是否啟用
    }
    
    void Start()
    {
        InitializeSceneButtons();
    }
    
    void InitializeSceneButtons()
    {
        foreach (var sceneBtn in sceneButtons)
        {
            // 設定按鈕點擊事件
            sceneBtn.button.onClick.AddListener(() => {
                HandleSceneInteraction(sceneBtn.type, sceneBtn.objectName);
            });
            
            // 設定按鈕 Hover 效果
            AddHoverEffect(sceneBtn.button);
        }
    }
}
```

### 🎪 **UI 動畫系統增強**

#### ✨ **互動反饋動畫**
```csharp
// 專門處理互動反饋的動畫控制器
public class InteractionFeedbackUI : MonoBehaviour
{
    [Header("Hover 動畫")]
    public float hoverScaleMultiplier = 1.1f;
    public float hoverAnimationDuration = 0.2f;
    
    [Header("點擊動畫")]
    public float clickScaleMultiplier = 0.9f;
    public float clickAnimationDuration = 0.1f;
    
    [Header("指示器動畫")]
    public GameObject interactionIndicator;
    public float indicatorPulseSpeed = 2f;
    
    // 🎯 Hover 進入動畫
    public void OnHoverEnter(RectTransform target)
    {
        target.DOScale(hoverScaleMultiplier, hoverAnimationDuration)
              .SetEase(Ease.OutBack);
    }
    
    // 🎯 Hover 離開動畫
    public void OnHoverExit(RectTransform target)
    {
        target.DOScale(1f, hoverAnimationDuration)
              .SetEase(Ease.OutBack);
    }
    
    // 🎯 點擊動畫
    public void OnClick(RectTransform target)
    {
        target.DOScale(clickScaleMultiplier, clickAnimationDuration)
              .SetEase(Ease.OutQuad)
              .OnComplete(() => {
                  target.DOScale(1f, clickAnimationDuration)
                        .SetEase(Ease.OutBack);
              });
    }
    
    // 🎯 指示器脈衝動畫
    void Update()
    {
        if (interactionIndicator != null && interactionIndicator.activeInHierarchy)
        {
            float pulse = Mathf.Sin(Time.time * indicatorPulseSpeed) * 0.1f + 1f;
            interactionIndicator.transform.localScale = Vector3.one * pulse;
        }
    }
}
```

### 📱 **響應式設計增強**

#### 🔧 **多解析度適配**
```csharp
// 針對新互動系統的響應式設計
public class InteractionUIScaler : MonoBehaviour
{
    [Header("互動 UI 縮放設定")]
    public float minButtonSize = 80f;           // 最小按鈕尺寸
    public float maxButtonSize = 120f;          // 最大按鈕尺寸
    public float referenceWidth = 1920f;        // 參考寬度
    
    [Header("Canvas 引用")]
    public Canvas dynamicCharacterCanvas;
    public Canvas staticInteractionCanvas;
    
    void Start()
    {
        AdjustInteractionUIScale();
    }
    
    void AdjustInteractionUIScale()
    {
        float screenRatio = Screen.width / referenceWidth;
        float buttonScale = Mathf.Clamp(screenRatio, minButtonSize/100f, maxButtonSize/100f);
        
        // 調整動態角色互動按鈕大小
        AdjustDynamicCharacterUI(buttonScale);
        
        // 調整靜態物件互動按鈕大小
        AdjustStaticInteractionUI(buttonScale);
    }
}
```

### 🎮 **新的互動事件系統**

#### 📡 **UI 事件管理**
```csharp
// 統一管理所有互動 UI 事件
public class InteractionUIEventManager : MonoBehaviour
{
    [Header("事件系統")]
    public UnityEvent<InteractionType> OnInteractionStart;
    public UnityEvent<InteractionType> OnInteractionEnd;
    public UnityEvent<Vector2> OnInteractionHover;
    
    [Header("音效整合")]
    public UISoundManager soundManager;
    
    // 🎯 處理互動開始事件
    public void HandleInteractionStart(InteractionType type)
    {
        // 播放互動音效
        soundManager.PlayInteractionSound(type);
        
        // 觸發互動開始事件
        OnInteractionStart?.Invoke(type);
        
        // 記錄互動統計
        RecordInteractionStats(type);
    }
    
    // 🎯 處理互動結束事件
    public void HandleInteractionEnd(InteractionType type)
    {
        OnInteractionEnd?.Invoke(type);
    }
    
    // 🎯 處理 Hover 事件
    public void HandleInteractionHover(Vector2 position)
    {
        OnInteractionHover?.Invoke(position);
    }
}
```

### 🔄 **新舊系統 UI 對比**

| 項目 | 舊系統 UI | 新系統 UI |
|------|-----------|-----------|
| **互動檢測** | OnMouseDown() | UI Button.onClick |
| **Canvas 層級** | 5層固定 | 7層動態管理 |
| **角色互動** | 固定區域 | 透明按鈕跟隨 |
| **視覺反饋** | 基本 | 豐富動畫效果 |
| **響應式支援** | 有限 | 完全響應式 |
| **事件系統** | 分散 | 統一管理 |

---

## 💬 Claude 使用提示

### 🔍 **舊系統 UI 開發**
了解UI架構時請：
1. 先理解UI層級結構和管理方式
2. 關注動畫和轉場效果
3. 注意響應式設計和性能優化
4. 理解與其他系統的整合方式
5. 搭配閱讀 `CodeModules/GameUICtrlmanager.md` 了解實作

### 🆕 **新系統 UI 開發**
開發新互動 UI 時請：
1. **Canvas 分層** - 嚴格按照新的 7層 Canvas 架構
2. **按鈕跟隨** - 理解 World Space ↔ Screen Space 轉換
3. **事件綁定** - 使用統一的 InteractionUIEventManager
4. **動畫系統** - 利用 InteractionFeedbackUI 提供反饋
5. **響應式設計** - 確保各種解析度下的正確顯示

### 🔄 **系統選擇指南**
- **舊系統 UI**: 現有場景 UI 維護、快速修復
- **新系統 UI**: 重製場景、需要動態互動效果
- **混合開發**: 不同場景可以使用不同 UI 系統

修改UI架構時需要：
- 確保UI層級的正確性
- 測試不同解析度的顯示效果
- 考慮性能影響
- 保持一致的視覺風格
- 更新相關的動畫和音效配置
- 🆕 **新增**: 驗證新舊 UI 系統的協調工作