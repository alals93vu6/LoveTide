# 💬 模組名稱：DialogSystem

> 對話系統核心模組，負責對話顯示、文字效果和用戶互動

---

## 🔖 模組功能

DialogSystem 負責處理遊戲中所有對話相關的顯示和互動，包括文字顯示、角色立繪、選項處理和對話歷史記錄。

---

## 📍 檔案位置

**路徑**: `Assets/Scripts/Drama/DialogSystem.cs`  
**命名空間**: `LoveTide.Drama`  
**繼承**: `MonoBehaviour`

---

## 🧩 公開方法一覽

| 方法名稱 | 功能描述 | 參數 | 回傳值 |
|----------|----------|------|---------|
| `ShowDialog(DialogData data)` | 顯示對話內容 | DialogData | void |
| `ShowChoices(List<ChoiceData> choices)` | 顯示選項列表 | List<ChoiceData> | void |
| `HideDialog()` | 隱藏對話框 | 無 | void |
| `SetTypewriterSpeed(float speed)` | 設定打字機速度 | float | void |
| `SkipTypewriter()` | 跳過打字機效果 | 無 | void |
| `ClearDialog()` | 清空對話內容 | 無 | void |
| `SetDialogHistory(bool enabled)` | 設定對話歷史記錄 | bool | void |
| `GetDialogHistory()` | 取得對話歷史 | 無 | List<DialogData> |
| `ReplayDialog(int index)` | 重播歷史對話 | int | void |

---

## 🎯 主要屬性

### 📱 UI 元件引用
```csharp
[Header("UI Components")]
public Text characterNameText;              // 角色名稱文字
public Text dialogText;                     // 對話內容文字
public Image characterPortrait;             // 角色立繪
public GameObject dialogPanel;              // 對話面板
public GameObject choicePanel;              // 選項面板
public Button continueButton;               // 繼續按鈕
public Transform choiceContainer;           // 選項容器
```

### 🎪 動畫和效果
```csharp
[Header("Animation Settings")]
public float typewriterSpeed = 0.05f;       // 打字機速度
public float dialogFadeSpeed = 0.3f;        // 對話淡入速度
public AnimationCurve fadeInCurve;          // 淡入曲線
public AnimationCurve fadeOutCurve;         // 淡出曲線
```

### 💾 對話數據
```csharp
[Header("Dialog Data")]
public DialogData currentDialog;            // 當前對話數據
public List<DialogData> dialogHistory;     // 對話歷史
public int maxHistorySize = 100;            // 最大歷史記錄數
public bool enableHistory = true;           // 是否啟用歷史記錄
```

---

## 📊 數據結構

### 💬 對話資料結構
```csharp
[System.Serializable]
public class DialogData
{
    public string characterName;            // 角色名稱
    public string dialogText;               // 對話文字
    public Sprite characterPortrait;        // 角色立繪
    public Color textColor = Color.white;   // 文字顏色
    public Color nameColor = Color.white;   // 名稱顏色
    public AudioClip voiceClip;             // 語音音效
    public float displaySpeed = 1.0f;       // 顯示速度
    public bool autoAdvance = false;        // 自動前進
    public float autoAdvanceDelay = 2.0f;   // 自動前進延遲
}
```

### 🎯 選項資料結構
```csharp
[System.Serializable]
public class ChoiceData
{
    public string choiceText;               // 選項文字
    public int choiceValue;                 // 選項值
    public Color textColor = Color.white;   // 文字顏色
    public bool isEnabled = true;           // 是否啟用
    public List<Condition> conditions;      // 顯示條件
    public UnityEvent onChoiceSelected;     // 選擇事件
}
```

---

## 🎭 對話顯示系統

### 📝 對話顯示流程
```csharp
public void ShowDialog(DialogData data)
{
    // 1. 儲存對話歷史
    if (enableHistory)
    {
        AddToHistory(data);
    }
    
    // 2. 更新UI元件
    UpdateDialogUI(data);
    
    // 3. 播放語音
    PlayVoiceClip(data.voiceClip);
    
    // 4. 啟動打字機效果
    StartTypewriter(data.dialogText, data.displaySpeed);
    
    // 5. 觸發對話事件
    EventBus.Instance.Publish("DialogShow", data);
}
```

### ⌨️ 打字機效果
```csharp
private IEnumerator TypewriterEffect(string text, float speed)
{
    dialogText.text = "";
    isTyping = true;
    
    for (int i = 0; i < text.Length; i++)
    {
        dialogText.text += text[i];
        
        // 播放打字音效
        if (i % 3 == 0) // 每3個字符播放一次音效
        {
            PlayTypingSound();
        }
        
        yield return new WaitForSeconds(speed * typewriterSpeed);
        
        // 檢查是否被跳過
        if (skipTypewriter)
        {
            dialogText.text = text;
            break;
        }
    }
    
    isTyping = false;
    OnTypewriterComplete();
}
```

---

## 🎯 選項系統

### 📋 選項顯示
```csharp
public void ShowChoices(List<ChoiceData> choices)
{
    // 1. 清空現有選項
    ClearChoices();
    
    // 2. 隱藏對話面板
    dialogPanel.SetActive(false);
    
    // 3. 顯示選項面板
    choicePanel.SetActive(true);
    
    // 4. 創建選項按鈕
    foreach (var choice in choices)
    {
        if (ShouldShowChoice(choice))
        {
            CreateChoiceButton(choice);
        }
    }
    
    // 5. 觸發選項事件
    EventBus.Instance.Publish("ChoicesShow", choices);
}
```

### 🎲 選項條件檢查
```csharp
private bool ShouldShowChoice(ChoiceData choice)
{
    if (choice.conditions == null || choice.conditions.Count == 0)
        return true;
    
    foreach (var condition in choice.conditions)
    {
        if (!EvaluateCondition(condition))
            return false;
    }
    
    return true;
}
```

### 🎯 選項選擇處理
```csharp
private void OnChoiceSelected(ChoiceData choice)
{
    // 1. 記錄選擇結果
    RecordChoice(choice);
    
    // 2. 隱藏選項面板
    choicePanel.SetActive(false);
    
    // 3. 恢復對話面板
    dialogPanel.SetActive(true);
    
    // 4. 播放選擇音效
    PlayChoiceSound();
    
    // 5. 觸發選擇事件
    EventBus.Instance.Publish("ChoiceSelected", choice);
    
    // 6. 執行選擇回調
    choice.onChoiceSelected?.Invoke();
}
```

---

## 🎨 視覺效果系統

### 🖼️ 角色立繪控制
```csharp
public void UpdateCharacterPortrait(Sprite portrait)
{
    if (portrait != null)
    {
        characterPortrait.sprite = portrait;
        characterPortrait.gameObject.SetActive(true);
        
        // 播放立繪淡入動畫
        StartCoroutine(FadeInPortrait());
    }
    else
    {
        // 隱藏立繪
        StartCoroutine(FadeOutPortrait());
    }
}
```

### 🎪 對話框動畫
```csharp
private IEnumerator FadeInDialog()
{
    var canvasGroup = dialogPanel.GetComponent<CanvasGroup>();
    canvasGroup.alpha = 0f;
    
    float elapsed = 0f;
    while (elapsed < dialogFadeSpeed)
    {
        elapsed += Time.deltaTime;
        canvasGroup.alpha = fadeInCurve.Evaluate(elapsed / dialogFadeSpeed);
        yield return null;
    }
    
    canvasGroup.alpha = 1f;
}
```

---

## 🎵 音效整合

### 🔊 語音播放
```csharp
private void PlayVoiceClip(AudioClip clip)
{
    if (clip != null && voiceSource != null)
    {
        voiceSource.clip = clip;
        voiceSource.Play();
        
        // 設定語音結束回調
        StartCoroutine(WaitForVoiceEnd(clip.length));
    }
}
```

### 🎶 打字音效
```csharp
private void PlayTypingSound()
{
    if (typingSound != null && sfxSource != null)
    {
        sfxSource.PlayOneShot(typingSound);
    }
}
```

---

## 💾 對話歷史系統

### 📚 歷史記錄管理
```csharp
private void AddToHistory(DialogData data)
{
    if (dialogHistory.Count >= maxHistorySize)
    {
        dialogHistory.RemoveAt(0);
    }
    
    dialogHistory.Add(new DialogData(data));
    
    // 觸發歷史更新事件
    EventBus.Instance.Publish("DialogHistoryUpdated", dialogHistory);
}
```

### 🔍 歷史對話重播
```csharp
public void ReplayDialog(int index)
{
    if (index >= 0 && index < dialogHistory.Count)
    {
        var historyData = dialogHistory[index];
        ShowDialog(historyData);
        
        // 標記為重播狀態
        isReplaying = true;
    }
}
```

---

## 🎮 輸入處理

### 👆 點擊和按鍵處理
```csharp
private void Update()
{
    HandleInput();
}

private void HandleInput()
{
    // 點擊繼續
    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
    {
        OnContinueClick();
    }
    
    // 跳過打字機效果
    if (Input.GetKeyDown(KeyCode.LeftControl) && isTyping)
    {
        SkipTypewriter();
    }
    
    // 開啟歷史記錄
    if (Input.GetKeyDown(KeyCode.H))
    {
        ToggleHistoryPanel();
    }
}
```

### 🎯 繼續按鈕處理
```csharp
public void OnContinueClick()
{
    if (isTyping)
    {
        SkipTypewriter();
    }
    else if (currentDialog != null)
    {
        // 觸發對話繼續事件
        EventBus.Instance.Publish("DialogContinue", currentDialog);
    }
}
```

---

## 🔌 系統整合

### 📡 事件系統整合
```csharp
private void Start()
{
    RegisterEventHandlers();
}

private void RegisterEventHandlers()
{
    EventBus.Instance.Subscribe<DialogRequest>("ShowDialog", OnShowDialogRequest);
    EventBus.Instance.Subscribe<ChoiceRequest>("ShowChoices", OnShowChoicesRequest);
    EventBus.Instance.Subscribe<SettingsChange>("SettingsChanged", OnSettingsChanged);
}
```

### 🎭 劇情系統整合
```csharp
private void OnShowDialogRequest(DialogRequest request)
{
    var dialogData = LoadDialogData(request.dialogId);
    ShowDialog(dialogData);
}
```

---

## 🛡 錯誤處理

### 🚨 異常處理
```csharp
public void ShowDialog(DialogData data)
{
    try
    {
        ValidateDialogData(data);
        DisplayDialog(data);
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Dialog display error: {e.Message}");
        ShowErrorDialog("對話顯示錯誤");
    }
}

private void ValidateDialogData(DialogData data)
{
    if (data == null)
        throw new System.ArgumentNullException("DialogData cannot be null");
    
    if (string.IsNullOrEmpty(data.dialogText))
        throw new System.ArgumentException("Dialog text cannot be empty");
}
```

---

## 🚀 性能優化

### 🎯 對象池優化
```csharp
public class ChoiceButtonPool : MonoBehaviour
{
    private Queue<Button> buttonPool = new Queue<Button>();
    
    public Button GetButton()
    {
        if (buttonPool.Count > 0)
        {
            return buttonPool.Dequeue();
        }
        else
        {
            return CreateNewButton();
        }
    }
    
    public void ReturnButton(Button button)
    {
        button.gameObject.SetActive(false);
        buttonPool.Enqueue(button);
    }
}
```

---

## 🔁 呼叫關係

### 📊 被呼叫情況
- **GamePlayingManagerDrama**: 劇情播放時呼叫
- **GameUICtrlmanager**: UI事件觸發時呼叫
- **Event System**: 事件系統觸發時呼叫

### 🎯 呼叫對象
- **EventBus**: 發布對話相關事件
- **AudioManager**: 播放音效和語音
- **NumericalRecords**: 記錄選擇結果

---

## 💬 Claude 使用提示

修改 DialogSystem 時請注意：
1. **先閱讀 `GameMechanics/劇情播放系統.md`** 了解遊戲機制
2. **參考 `Architecture/UI架構設計.md`** 了解UI設計原則
3. **測試打字機效果** 確保文字顯示流暢
4. **檢查音效同步** 確保語音和音效正確播放
5. **驗證選項邏輯** 測試條件判斷和事件觸發
6. **測試多語言支援** 確保不同語言文字顯示正確

常見修改場景：
- 新增對話特效
- 修改選項顯示邏輯
- 優化打字機效果
- 添加新的對話類型
- 整合語音系統