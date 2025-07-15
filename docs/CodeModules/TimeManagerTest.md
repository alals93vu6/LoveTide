# ⏰ TimeManagerTest 時間管理系統

> LoveTide 遊戲的核心時間控制系統，負責管理遊戲內時間流程、事件觸發和時間狀態判斷

---

## 🎯 概述

TimeManagerTest 是遊戲的時間管理核心，透過 `NumericalRecords` 管理三個關鍵時間變數，驅動整個遊戲的時間流程和事件觸發機制。

---

## 📁 檔案位置

```
E:\GitHub\LoveTide\LoveTide\Assets\Scripts\GamePlay\TimeManagerTest.cs
```

---

## 🧱 核心架構

### 📊 時間變數體系
```csharp
[SerializeField] private NumericalRecords numberCtrl;
[SerializeField] public bool vacation;
```

#### 🕐 核心時間變數（由 NumericalRecords 管理）
- **`aDay`**: 遊戲總天數（無限累加）
- **`aTimer`**: 時段（1-10，1-6白天，7-10晚上）
- **`aWeek`**: 星期（1-7，3-4為週末假期）

#### 🏖️ 狀態標誌
- **`vacation`**: 假期狀態（週末或晚上）

---

## 🔧 核心方法

### 📅 DetectedDayPassed()
```csharp
public void DetectedDayPassed()
{
    if (numberCtrl.aWeek >= 7) {
        numberCtrl.aWeek = 1;
    } else {
        numberCtrl.aWeek++;
    }
    numberCtrl.aTimer = 1;
    numberCtrl.aDay++;
    VacationDetected();
}
```

**功能**: 處理日期推進邏輯
- 週數循環（7→1）
- 重置時段為早上第一個時段
- 天數累加
- 觸發假期狀態檢查

**呼叫位置**:
- `GameManagerTest.cs` 第141行：`timer.DetectedDayPassed();`
- 在 `DayPassedEvent()` 方法中觸發

### 🏖️ VacationDetected()
```csharp
public void VacationDetected()
{
    if (numberCtrl.aWeek == 3 || numberCtrl.aWeek == 4)
    {
        vacation = true;
    }
    else
    {
        vacation = false;
    }
}
```

**功能**: 根據星期判斷假期狀態
- 週三、週四為假期（週末）
- 設定 `vacation` 狀態標誌

**呼叫位置**:
- `GameManagerTest.cs` 第72行：`timer.VacationDetected();`
- 遊戲開始時檢查當前假期狀態

### 🌙 DetectedPeriod()
```csharp
public void DetectedPeriod()
{
    if (numberCtrl.aTimer >= 7)
    {
        vacation = true;
    }
    else
    {
        vacation = false;
    }
}
```

**功能**: 根據時段判斷假期狀態
- 時段 >= 7 視為晚上（假期狀態）
- 動態設定 `vacation` 狀態

**使用狀況**: 方法已定義但目前未在專案中被呼叫

---

## 🔄 時間系統邏輯

### 📈 時間推進流程
```
時段推進 → 檢查時段 → 觸發事件 → 時段 >= 10 → 日期推進
    ↓           ↓           ↓              ↓
時段 += 1   假期判斷   劇情觸發      重置時段為1
```

### 🎯 事件觸發機制
```csharp
// GameManagerTest.cs 中的事件觸發邏輯
if (timemanager.aTimer >= 10)
{
    timemanager.DetectedDayPassed();
}
```

### 🗓️ 週期循環設計
```
星期循環: 1 → 2 → 3(假期) → 4(假期) → 5 → 6 → 7 → 1
時段循環: 1 → 2 → 3 → 4 → 5 → 6 → 7 → 8 → 9 → 10 → 1(次日)
```

---

## 🎮 系統整合

### 🎭 與劇情系統的交互
```csharp
// EventDetectedManager.cs 中的時間依賴
if (timemanager.aTimer >= 7 && !timemanager.vacation)
{
    // 觸發晚上劇情
}
```

**整合點**:
- 不同時段觸發不同劇情內容
- 假期狀態影響劇情選項
- 特定天數觸發強制劇情

### 🎯 與遊戲管理系統的交互
```csharp
// GameManagerTest.cs 中的時間控制
public void TimePassCheck()
{
    if (timemanager.aTimer >= 10)
    {
        timemanager.DetectedDayPassed();
    }
}
```

**整合點**:
- 時間推進驅動遊戲狀態變化
- 假期狀態影響角色外觀
- 時段變化觸發背景切換

### 🖼️ 與UI系統的交互
```csharp
// SexyButton.cs 中的時間依賴
if (timemanager.vacation && otherConditions)
{
    buttonText = "假期模式";
}
```

**整合點**:
- 假期狀態影響UI顯示
- 時間狀態控制互動選項
- 時段變化更新介面元素

---

## 📊 特定事件觸發表

### 🎯 天數觸發事件
| 天數 | 觸發條件 | 事件類型 |
|------|----------|----------|
| 3, 6 | 強制觸發 | 劇情事件 |
| 15, 24 | aTimer >= 6 | 條件劇情 |
| 32 | aTimer <= 3 | 早期劇情 |
| 37, 45 | 強制觸發 | 劇情事件 |
| 47, 49 | aTimer >= 6 | 條件劇情 |
| 51 | 強制觸發 | 劇情事件 |

### 🕐 時段觸發事件
| 時段 | 條件 | 觸發內容 |
|------|------|----------|
| 7 | 非假期 | 劇情事件檢查 |
| 8 | 非單獨狀態 | 特殊對話 |
| >= 10 | 任何狀態 | 日期推進 |

---

## 💾 存檔系統整合

### 📋 時間數據持久化
```csharp
// NumericalRecords.cs 中的存檔邏輯
PlayerPrefs.SetInt("aDay", aDay);
PlayerPrefs.SetInt("aTimer", aTimer);
PlayerPrefs.SetInt("aWeek", aWeek);
```

### 🔍 數據驗證機制
```csharp
// 載入時的數據驗證
if (aWeek == 0 || aWeek == 8) aWeek = 1;
if (aDay == 0) aDay = 1;
if (aTimer <= 0) aTimer = 1;
```

---

## ⚡ 性能考量

### 🚀 優化特點
- **輕量級設計**: 僅管理核心時間變數
- **事件驅動**: 避免不必要的Update循環
- **狀態緩存**: vacation狀態避免重複計算

### 📈 擴展性
- 時間變數透過NumericalRecords統一管理
- 事件觸發機制支援靈活擴展
- 狀態判斷邏輯可獨立修改

---

## 🛠 除錯工具

### 🔍 時間狀態檢查
```csharp
// 除錯用的時間狀態顯示
Debug.Log($"Day: {aDay}, Timer: {aTimer}, Week: {aWeek}, Vacation: {vacation}");
```

### 📊 事件觸發追蹤
```csharp
// 事件觸發日誌
Debug.Log($"Event triggered at Day {aDay}, Timer {aTimer}");
```

---

## 🔧 常見問題

### ❓ 時間推進異常
**問題**: 時間不正常推進
**檢查**:
- NumericalRecords 是否正確初始化
- DetectedDayPassed() 是否被正確呼叫
- 時間變數是否超出有效範圍

### ❓ 事件觸發失效
**問題**: 時間相關事件不觸發
**檢查**:
- 假期狀態判斷是否正確
- 時段條件是否滿足
- GameManagerTest 中的事件檢查邏輯

### ❓ 假期狀態錯誤
**問題**: 假期狀態顯示不正確
**檢查**:
- VacationDetected() 是否在適當時機呼叫
- DetectedPeriod() 是否需要啟用
- 週數和時段變數是否正確更新

---

## 📝 開發建議

### 🎯 最佳實踐
1. **時間變數修改**: 統一透過 TimeManagerTest 方法修改
2. **事件觸發**: 在 GameManagerTest 中集中管理
3. **狀態檢查**: 使用 vacation 標誌而非直接計算
4. **數據驗證**: 載入時進行邊界檢查

### 🔄 擴展建議
1. **事件系統**: 考慮整合 EventBus 進行時間事件廣播
2. **時間顯示**: 增加時間格式化和本地化支援
3. **除錯工具**: 建立時間管理的除錯介面
4. **性能優化**: 考慮時間事件的對象池管理

---

## 💬 Claude 使用提示

### 🔍 理解時間系統
1. **先了解時間變數**: aDay, aTimer, aWeek 的含義和範圍
2. **掌握事件觸發**: 研究 GameManagerTest 中的時間檢查邏輯
3. **理解狀態控制**: vacation 狀態如何影響其他系統

### 🛠 修改時間系統
1. **修改前備份**: 時間系統影響廣泛，修改前務必備份
2. **測試完整性**: 確保時間推進、事件觸發、狀態判斷都正常
3. **更新文件**: 修改後更新本文件和相關系統文件

### 🎯 常見任務
- **添加新事件**: 在 GameManagerTest 中添加時間條件判斷
- **修改時間流程**: 調整 DetectedDayPassed() 中的邏輯
- **擴展假期系統**: 修改 VacationDetected() 和 DetectedPeriod()

---

**最後更新**: 2025-07-15  
**版本**: 1.0  
**維護者**: 開發團隊 + Claude AI