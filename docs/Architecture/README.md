# 🏗️ 模組架構總覽

> LoveTide 專案的系統架構設計與模組整合說明

---

## 🎯 概述

LoveTide 採用模組化架構設計，各系統間通過事件機制和統一的管理器進行通信，確保高內聚、低耦合的程式碼結構。

---

## 🧱 核心架構層級

### 📊 系統層級結構
```
🎮 遊戲層 (Game Layer)
├── 🎭 劇情系統 (Drama System)
├── ❤️ 養成系統 (Relationship System)  
├── 🎣 小遊戲系統 (Mini-Game System)
└── 🔞 成人內容系統 (Adult Content System)

🏗️ 管理層 (Management Layer)
├── 🎯 遊戲管理器 (GameManager)
├── 🎵 音效管理器 (AudioManager)
├── 🖼️ UI 管理器 (UIManager)
└── 💾 存檔管理器 (SaveManager)

🔧 工具層 (Utility Layer)
├── 📡 事件總線 (EventBus)
├── 📊 數據管理 (DataManager)
├── ⏰ 時間系統 (TimeSystem)
└── 🎨 資源管理 (ResourceManager)
```

---

## 🔄 系統間通信

### 📡 事件驅動架構
```csharp
// 事件發布
EventBus.Instance.Publish("AffectionChanged", new AffectionData());

// 事件訂閱
EventBus.Instance.Subscribe<AffectionData>("AffectionChanged", OnAffectionChanged);
```

### 🎯 主要事件類型
- **遊戲狀態事件**: 場景切換、暫停、繼續
- **劇情事件**: 對話開始、結束、選擇分支
- **養成事件**: 好感度變化、等級提升
- **UI 事件**: 按鈕點擊、介面更新
- **音效事件**: 音樂切換、音效播放

---

## 🎮 主要系統模組

### 🎯 遊戲核心管理 (GameManager)
- **職責**: 統籌遊戲整體流程
- **管理範圍**: 場景切換、狀態控制、存檔載入
- **通信方式**: 中央調度器模式

### 🎭 劇情播放架構 (Drama System)  
- **組件**: `GamePlayingManagerDrama` + JSON 數據
- **流程**: 數據載入 → UI 顯示 → 動畫播放 → 用戶輸入
- **整合**: 與背景、音效、UI 系統協作

### ❤️ 養成互動架構 (Relationship System)
- **核心**: 時間管理 + 數值系統 + 條件判斷
- **數據流**: 選擇 → 條件檢查 → 數值更新 → UI 反饋
- **擴展性**: 支援多角色、多數值維度

---

## 📊 數據流架構

### 🔄 數據流向圖
```
用戶輸入 → 事件觸發 → 邏輯處理 → 數據更新 → UI 更新 → 持久化存儲
    ↑                                                           ↓
載入時數據恢復 ←─────────────────────────────────────────── 存檔系統
```

### 💾 數據分層
- **持久層**: PlayerPrefs、JSON 檔案
- **業務層**: 遊戲邏輯、數值計算
- **表現層**: UI 顯示、動畫播放
- **輸入層**: 用戶操作、事件觸發

---

## 🎨 UI 系統架構

### 📱 UI 層級結構
```
🖼️ UI Canvas 層級
├── HUD Layer (永久顯示)
│   └── 快捷按鈕
├── Dialog Layer (對話系統)
│   ├── 對話框
│   ├── 角色立繪
│   └── 選項按鈕
├── Menu Layer (選單系統)
│   ├── 主選單
│   ├── 設定選單
│   └── 存檔選單
└── Popup Layer (彈出視窗)
    ├── 確認對話框
    ├── 提示訊息
    └── 錯誤訊息
```

### 🎯 UI 管理策略
- **單例模式**: UIManager 統一管理所有 UI
- **棧式管理**: UI 面板採用堆疊管理
- **事件驅動**: UI 更新通過事件觸發
- **資源池**: 頻繁使用的 UI 元件使用對象池

---

## 🎵 音效系統架構

### 🔊 音效分類管理
```csharp
public enum AudioCategory
{
    BGM,        // 背景音樂
    SFX,        // 音效
    Voice,      // 語音
    Ambient     // 環境音
}
```

### 🎶 音效管理流程
1. **音效註冊**: 系統啟動時載入音效資源
2. **播放請求**: 通過事件系統請求播放
3. **音量控制**: 根據設定調整各類音效音量
4. **記憶體管理**: 動態載入/卸載音效資源

---

## ⏰ 時間系統架構

### 📅 時間管理層次
```csharp
public class TimeSystem
{
    public GameTime currentTime;      // 遊戲內時間
    public float timeScale;           // 時間倍率
    public bool isPaused;             // 是否暫停
    
    // 時間事件
    public UnityEvent<int> OnDayChanged;
    public UnityEvent<TimeOfDay> OnTimeOfDayChanged;
}
```

### 🔄 時間相關系統
- **劇情系統**: 特定時間觸發事件
- **養成系統**: 時間消耗和冷卻機制
- **小遊戲**: 時間限制和獎勵系統
- **存檔系統**: 時間戳記錄

---

## 💾 存檔系統架構

### 📋 存檔數據結構
```json
{
    "gameVersion": "1.0.0",
    "saveTime": "2025-07-15T10:30:00",
    "gameProgress": {
        "currentScene": "main_room",
        "dramaProgress": 15,
        "affectionLevel": 65
    },
    "gameData": {
        "playerMoney": 5000,
        "fishingLevel": 8,
        "unlockedContent": ["fishing", "dating"]
    }
}
```

### 🛡 存檔安全機制
- **版本檢查**: 檢查存檔兼容性
- **數據驗證**: 防止數據篡改
- **備份機制**: 自動備份重要存檔
- **錯誤恢復**: 存檔損壞時的恢復機制

---

## 🔌 擴展性設計

### 🧩 模組化原則
- **接口導向**: 使用接口定義系統間契約
- **依賴注入**: 減少系統間的硬耦合
- **配置驅動**: 通過配置文件控制系統行為
- **插件架構**: 支援功能模組的動態載入

### 🎯 未來擴展點
- **多語言支援**: 國際化架構設計
- **雲端存檔**: 雲端同步功能
- **MOD 支援**: 用戶自定義內容
- **多平台適配**: 不同平台的適配層

---

## 💬 Claude 使用提示

閱讀架構文件時建議順序：
1. **先讀本檔案** 了解整體架構概念
2. **閱讀具體系統架構**:
   - `遊戲流程架構.md` - 了解遊戲流程
   - `數據流架構.md` - 了解數據管理
   - `UI架構設計.md` - 了解介面設計
   - `音效系統架構.md` - 了解音效管理

修改系統架構前請：
- 評估對其他系統的影響
- 確保向後兼容性
- 更新相關的程式碼文件
- 進行充分的整合測試