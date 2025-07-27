# 💬 TextBox對話系統架構

> LoveTide 劇情對話的核心展示系統，負責文字渲染、角色名稱管理和動態內容生成

---

## 🎯 概述

TextBox對話系統是 LoveTide 遊戲中負責劇情對話展示的核心系統，提供打字機效果的文字渲染、多角色名稱管理、玩家名稱替換、以及基於遊戲狀態的動態文字生成功能。系統分為劇情模式和養成模式兩個版本，以適應不同的遊戲場景需求。

---

## 🎭 系統特色

### 🌟 核心功能
- **打字機效果**: 逐字顯示文字，營造閱讀節奏感
- **多角色支援**: 支援10+種不同角色的名稱顯示
- **動態文字生成**: 基於遊戲數值動態生成狀態描述
- **玩家名稱替換**: 自動替換文字中的玩家名稱佔位符
- **場景轉換**: 支援CG畫面切換和過場效果
- **快進功能**: 允許玩家快速跳過文字動畫

### 📊 技術特點
- **JSON數據驅動**: 對話內容完全由 DialogData 管理
- **協程系統**: 使用 Coroutine 實現流暢的文字動畫
- **狀態機整合**: 與遊戲主狀態無縫整合
- **模組化設計**: 劇情模式和養成模式分離實現

---

## 🏗️ 架構設計

### 📋 系統結構圖
```
💬 TextBox對話系統
├── TextBoxDrama (劇情模式)
│   ├── 文字打字機效果
│   ├── CG場景轉換
│   ├── 角色名稱管理
│   └── 劇情流程控制
├── TextBoxTestPlaying (養成模式)
│   ├── 文字打字機效果
│   ├── 動態數值顯示
│   ├── 狀態描述生成
│   └── 互動對話控制
└── 共通系統
    ├── DialogData 數據結構
    ├── Speaker 角色枚舉
    ├── 玩家名稱替換
    └── 文字速度控制
```

---

## 🎬 TextBoxDrama (劇情模式)

### 🔧 核心組件
```csharp
public class TextBoxDrama : MonoBehaviour
{
    [Header("數值")]
    public Text nameText;              // 角色名稱顯示
    public Text showText;              // 對話內容顯示
    public float letterSpeed = 0.02f;  // 打字機速度
    public int textNumber = 0;         // 當前文字索引
    public string[] getTextDate;       // 處理後的文字陣列
    
    [Header("物件")]
    public DialogData diaLog;          // 對話數據
    private DirtyTrickCtrl dirtyTrick; // 場景轉換控制
    
    [Header("狀態")]
    public bool isover = true;         // 文字顯示完成狀態
    public bool stopLoop;              // 停止打字機循環
    public bool isWait;                // 等待狀態
    public bool isEnd;                 // 對話結束狀態
}
```

### ⚙️ 主要功能實現

#### 🎯 文字數據載入與處理
```csharp
private void TextDataLoad()
{
    var arraySize = diaLog.plotOptionsList[targetNumber].dialogDataDetails.Count;
    var playerNameData = FindObjectOfType<NumericalRecords>().playerName;
    
    // 載入對話數據並替換玩家名稱
    for (int i = 0; i < diaLog.plotOptionsList[targetNumber].dialogDataDetails.Count; i++) 
    { 
        getTextDate[i] = diaLog.plotOptionsList[targetNumber].dialogDataDetails[i].sentence
                         .Replace("pName", playerNameData);
    }
}
```

#### ⌨️ 打字機效果核心實現
```csharp
private IEnumerator DisplayTextWithTypingEffect(bool OnWork)
{
    isover = true;
    if (getTextDate.Length > textNumber)
    {
        string targetText = getTextDate[textNumber];
        showText.text = "";

        if (!OnWork)  // 正常打字機效果
        {
            for (int i = 0; i < targetText.Length; i++)
            {
                if (stopLoop == true) break;  // 支援中斷快進
                
                showText.text += targetText[i];
                yield return new WaitForSeconds(letterSpeed);  // 控制打字速度
            }
            isover = false;
        }
        else  // 快進模式，直接顯示完整文字
        {
            showText.text = targetText;
            isover = false;
        }
    }
}
```

#### 🎞️ 進階對話控制與場景轉換
```csharp
public async void NextText()
{
    if (textNumber < diaLog.plotOptionsList[targetNumber].dialogDataDetails.Count - 1)
    {
        if (!diaLog.plotOptionsList[targetNumber].dialogDataDetails[textNumber].needTransition)
        {
            // 普通對話繼續
            stopLoop = false;
            textNumber++;
            ChickName();
            StartCoroutine(DisplayTextWithTypingEffect(false));
        }
        else
        {
            // 需要場景轉換的特殊對話
            var CGManager = FindObjectOfType<CGDisplay>();
            dirtyTrick.OnChangeScenes();           // 開始場景轉換效果
            
            stopLoop = false;
            textNumber++;
            isWait = true;
            
            await Task.Delay(500);                 // 等待轉換動畫
            CGManager.DisplayBackGroundChick(1);   // 顯示CG背景
            ChickName();
            StartCoroutine(DisplayTextWithTypingEffect(false));
            
            await Task.Delay(200);
            isWait = false;
        }
    }
    else
    {
        TalkOver();  // 對話結束
    }
}
```

---

## 🎮 TextBoxTestPlaying (養成模式)

### 🔧 核心組件
```csharp
public class TextBoxTestPlaying : MonoBehaviour
{
    [Header("數值")]
    public Text nameText;
    public Text showText;
    public float letterSpeed = 0.02f;
    public int textNumber = 0;
    public string[] getTextDate;

    [Header("物件")]
    public DialogData diaLog;
    public GameObject talkObject;          // 對話框物件
    public DialogDataDetected diaLogDetected;
    public NumericalRecords numericalData; // 數值記錄系統
    public ActorManagerTest actorCtrl;     // 演員控制系統

    [Header("狀態")]
    public bool isover = true;
    public bool stopLoop;
    public int listSerial;                 // 對話串列序號
}
```

### 🎯 動態內容生成系統

#### 📊 數值狀態描述生成
```csharp
private string NumericalLog(string numericalText)
{
    string periodText = "LogText";
    
    // 根據遊戲時間生成時間描述
    switch (numericalData.aTimer)
    {
        case 1: case 2: 
            periodText = "     是個涼爽的早晨，而今天也正要開始"; break;
        case 3: case 4: 
            periodText = "     默默地到了中午，太陽非常大"; break;
        case 5: case 6: 
            periodText = "     黃昏的陽光非常明顯，就快下班了"; break;
        case 7: case 8: 
            periodText = "     目前是寧靜的夜晚，很適合休息"; break;
        case 9: 
            periodText = "     快要進入半夜，時間也不晚了"; break;
    }
    
    // 組合完整的狀態描述
    numericalText = "目前已度過： " + numericalData.aDay + " 天  今日為星期  " + numericalData.aWeek + periodText + "\\n"
                    + "好感度為： " + numericalData.friendship + "   淫亂度為： " + numericalData.slutty;
    return numericalText;
}
```

#### 💝 關係狀態動態描述
```csharp
private string ConditionLog(string conditionText)
{
    string friendshipText = "A";
    
    // 根據好感度等級生成關係描述
    switch (PlayerPrefs.GetInt("FDS_LV"))
    {
        case 0: friendshipText = "總給人一種似乎有所保留的感覺，"; break;
        case 1: friendshipText = "雖然和以往比起來明顯感受到親近了不少，但多少還是會下意識的避嫌，"; break;
        case 2: friendshipText = "現在關係非常神奇，曖昧不清的感覺也總是不斷出現，"; break;
        case 3: friendshipText = "彼此明確成為了不可外傳的關係，"; break;
        case 4: friendshipText = "沉淪於背德感之中，屬實是無藥可救的一段關係，"; break;
    }

    string lustText = "B";
    
    // 根據慾望值生成行為描述
    switch (LustDetected(0))
    {
        case 0: lustText = "不過由香那活潑開朗的模樣依舊令人感到很有活力"; break;
        case 1: lustText = "明顯地感覺到由香似乎會嘗試製造更多的肢體接觸"; break;
        case 2: lustText = "兩人平時看似平平無奇的互動，但由香總是會偷偷的挑逗著我，傳達著非常明顯的訊號"; break;
        case 3: lustText = "動作扭扭捏捏的，和平常比起來更容易害羞，即使我現在馬上對她下手她也會乖乖配合吧"; break;
        case 4: lustText = "此時對於任何事務都很敏感，對話中不乏渴求著什麼的語氣，肢體語言表露出著肉眼可見的慾火焚身"; break;
    }

    conditionText = friendshipText + lustText;
    return conditionText;
}
```

#### 🌡️ 慾望值檢測系統
```csharp
private int LustDetected(int lustNumber)
{
    if (numericalData.lust == 0)
        lustNumber = 0;
    else if (numericalData.lust >= 1 && numericalData.lust <= 10)
        lustNumber = 1;
    else if (numericalData.lust >= 11 && numericalData.lust <= 20)
        lustNumber = 2;
    else if (numericalData.lust >= 21 && numericalData.lust <= 35)
        lustNumber = 3;
    else if (numericalData.lust >= 36)
        lustNumber = 4;

    return lustNumber;
}
```

---

## 🎭 角色名稱管理系統

### 👥 Speaker 枚舉與名稱對應

#### 📋 劇情模式角色清單
```csharp
private void ChickName()
{
    switch (diaLog.plotOptionsList[targetNumber].dialogDataDetails[textNumber].speaker)
    {
        case Speaker.Chorus:              nameText.text = "";       break;  // 旁白
        case Speaker.Player:              nameText.text = FindObjectOfType<NumericalRecords>().playerName; break;
        case Speaker.GirlFriend:          nameText.text = "由香";    break;
        case Speaker.GirlFriendDormitory: nameText.text = "由香";    break;
        case Speaker.GirlFriendFormal:    nameText.text = "由香";    break;
        case Speaker.GirlFriendNude:      nameText.text = "由香";    break;
        case Speaker.BoyFriend:           nameText.text = "苦主";    break;
        case Speaker.Steve:               nameText.text = "史帝夫";  break;
        case Speaker.PoliceA:             nameText.text = "警察A";   break;
        case Speaker.PoliceB:             nameText.text = "警察B";   break;
        case Speaker.PassersbyA:          nameText.text = "路人A";   break;
        case Speaker.PassersbyB:          nameText.text = "路人B";   break;
        case Speaker.TavernBoss:          nameText.text = "老闆";    break;
    }
}
```

#### 🎮 養成模式角色清單
```csharp
private void ChickName()
{
    switch (diaLog.plotOptionsList[listSerial].dialogDataDetails[textNumber].speaker)
    {
        case Speaker.Chorus:     nameText.text = " ";      break;  // 旁白
        case Speaker.Player:     nameText.text = PlayerPrefs.GetString("playerNameData" + PlayerPrefs.GetInt("GameDataNumber").ToString()); break;
        case Speaker.GirlFriend: nameText.text = "由香";   break;
        case Speaker.BoyFriend:  nameText.text = "苦主";   break;
        case Speaker.Steve:      nameText.text = "史帝夫"; break;
        case Speaker.PoliceA:    nameText.text = "警察A";  break;
        case Speaker.PoliceB:    nameText.text = "警察B";  break;
        case Speaker.PassersbyA: nameText.text = "路人";   break;
        case Speaker.PassersbyB: nameText.text = "客人";   break;
    }
}
```

---

## 🔄 系統整合與流程控制

### 📊 與其他系統的整合

#### 🎭 演員控制系統整合
```csharp
public void NextText()
{
    if (textNumber < diaLog.plotOptionsList[listSerial].dialogDataDetails.Count - 1)
    {
        stopLoop = false;
        textNumber++;
        ChickName();
        
        // 與演員控制系統同步
        actorCtrl.ActorCtrl();  // 更新角色立繪和表情
        
        StartCoroutine(DisplayTextWithTypingEffect(false));
    }
    else
    {
        DisplayTextBox(false);
        FindObjectOfType<GameManagerTest>().TalkDownEvent();  // 通知遊戲管理器對話結束
    }
}
```

#### 🎞️ 場景管理系統整合
```csharp
public void TalkOver()
{
    isEnd = true;
    dirtyTrick.OnExitGamePlayScenes();                           // 退出遊戲場景
    FindObjectOfType<GamePlayingManagerDrama>().OnTalkDown();    // 通知劇情管理器
}
```

### ⚙️ 特殊功能處理

#### 📈 特殊ID處理 (數值顯示)
```csharp
public void OnDisplayText()
{
    if (listSerial == 70)  // 特殊ID：數值狀態顯示
    {
        getTextDate[0] = NumericalLog("A");    // 生成數值日誌
        getTextDate[1] = ConditionLog("B");    // 生成關係狀態描述
    }
    else
    {
        TextDataLoad(listSerial);              // 正常載入對話數據
    }
    
    textNumber = 0;
    ChickName();
    StartCoroutine(DisplayTextWithTypingEffect(false));
    DisplayTextBox(true);
}
```

---

## ⚡ 使用者體驗優化

### 🚀 快進功能
```csharp
public void DownText()
{
    stopLoop = true;  // 停止打字機循環
    StartCoroutine(DisplayTextWithTypingEffect(true));  // 立即顯示完整文字
}
```

### 🎛️ 速度控制
- **letterSpeed**: 0.02秒/字符 (可調整的打字機速度)
- **場景轉換延遲**: 500ms (CG切換等待時間)
- **UI回應延遲**: 200ms (使用者介面反應時間)

---

## 📊 數據流程圖

```
📥 DialogData (JSON)
    ↓
🔄 TextDataLoad() (載入並處理文字)
    ↓
🔤 玩家名稱替換 ("pName" → 實際名稱)
    ↓
⌨️ DisplayTextWithTypingEffect() (打字機效果)
    ↓
👤 ChickName() (設定角色名稱)
    ↓
🎭 ActorCtrl() (同步演員控制)
    ↓
⏭️ NextText() (進入下一段對話)
    ↓
🏁 TalkOver() (對話結束處理)
```

---

## 🎨 實機畫面對應

基於實機截圖的功能驗證：

### 📸 劇情模式展示
- **`劇情模式對話1.png`**: 雙角色對話，名稱正確顯示
- **`劇情模式對話2.png`**: 單角色對話，打字機效果運行
- **`劇情模式對話_CG畫面.png`**: CG場景轉換功能

### 🎮 養成模式展示
- **`養成模式_點擊角色對話.png`**: 養成模式對話框展示
- **`養成模式_直接對話.png`**: 即時對話功能

---

## 🔮 擴展性設計

### 🆕 未來功能規劃
```csharp
// 語音系統整合
public class VoiceIntegratedTextBox : TextBoxDrama
{
    [Header("語音控制")]
    public AudioSource voicePlayer;
    public AudioClip[] characterVoices;
    
    // 與文字同步的語音播放
    public void PlayVoiceWithText(int characterID)
    {
        if (characterVoices[characterID] != null)
        {
            voicePlayer.clip = characterVoices[characterID];
            voicePlayer.Play();
        }
    }
}

// 多語言支援
public class MultiLanguageTextBox : TextBoxDrama
{
    [Header("多語言")]
    public Dictionary<string, DialogData> languageDialogs;
    public string currentLanguage = "zh-TW";
    
    public void SwitchLanguage(string language)
    {
        currentLanguage = language;
        if (languageDialogs.ContainsKey(language))
        {
            diaLog = languageDialogs[language];
        }
    }
}
```

### 🎯 功能增強方向
- **語音同步播放**: 與文字顯示同步的角色語音
- **文字特效**: 抖動、顏色變化等特殊文字效果
- **對話分支**: 玩家選擇導向不同劇情分支
- **情緒標記**: 基於對話內容的情緒分析和視覺反饋
- **自動存檔**: 對話進度的自動保存和恢復

---

## 🛠️ 開發建議

### 💡 使用指導
1. **對話數據設計**: 善用 DialogData 的結構化設計
2. **角色名稱管理**: 統一使用 Speaker 枚舉，避免硬編碼
3. **打字機速度**: 根據目標玩家群體調整 letterSpeed
4. **場景轉換**: 合理使用 needTransition 標記

### ⚠️ 注意事項
- **記憶體管理**: 大量文字數據的載入和釋放
- **協程管理**: 避免重複啟動 Coroutine 造成衝突
- **狀態同步**: 確保與其他系統的狀態一致性
- **錯誤處理**: 處理數據缺失或格式錯誤的情況

### 🔧 效能優化
- **文字快取**: 預處理常用的替換文字
- **分頁載入**: 大段對話的分頁顯示
- **GC優化**: 減少字符串操作的記憶體分配
- **UI優化**: 避免頻繁的UI元件更新

---

## 💬 Claude 使用提示

### 🎯 系統亮點
- **雙模式設計**: 劇情模式和養成模式的差異化實現
- **動態內容生成**: 基於遊戲狀態的智能文字生成
- **完整整合**: 與演員控制、場景管理的無縫整合
- **使用者友好**: 快進、暫停等完整的用戶體驗功能

### 📋 維護建議
- 新增角色時記得更新 Speaker 枚舉和 ChickName() 方法
- 新增特殊對話效果時考慮 needTransition 機制
- 數值系統變更時同步更新 NumericalLog() 和 ConditionLog()
- 保持兩個版本（Drama/TestPlaying）的功能同步

---

**系統特色**: TextBox對話系統是遊戲劇情展示的靈魂，通過精細的文字渲染和智能的內容生成，為玩家提供沉浸式的閱讀體驗。系統設計兼顧了性能效率和使用者體驗，是 LoveTide 遊戲中技術與藝術完美結合的典範！ ✨