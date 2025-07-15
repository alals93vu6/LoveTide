# 🧑‍💻 程式模組總覽

> LoveTide 專案的程式模組說明與使用指南

---

## 🔖 模組簡述

此資料夾包含所有重要程式模組的詳細說明，協助開發者快速了解每個模組的功能、使用方式和整合方法。

---

## 📊 模組分類

### 🎮 核心遊戲模組
- **`GameManagerTest.md`** - 主遊戲管理器
- **`TimeManagerTest.md`** - 時間系統管理器
- **`NumericalRecords.md`** - 數值記錄系統
- **`EventBus.md`** - 事件通信系統

### 🎭 劇情對話模組
- **`DialogSystem.md`** - 對話系統核心
- **`GamePlayingManagerDrama.md`** - 劇情播放管理器

### 🎨 視覺控制模組
- **`BackgroundCtrl.md`** - 背景控制器
- **`GameUICtrlmanager.md`** - UI控制管理器
- **`CGdisplay.md`** - CG圖片顯示系統

### 🎵 音效系統模組
- **`bgmManager.md`** - 背景音樂管理器
- **`AudioSystem.md`** - 音效系統總覽

### 💾 存檔系統模組
- **`SaveDataManager.md`** - 存檔管理器
- **`SaveSystem.md`** - 存檔系統總覽

### 🎣 小遊戲模組
- **`FishingSystem.md`** - 釣魚系統
- **`MiniGameBase.md`** - 小遊戲基礎架構

---

## 🔗 模組關係圖

```
🎮 GameManagerTest (核心控制器)
├── 🎭 GamePlayingManagerDrama (劇情管理)
│   ├── 💬 DialogSystem (對話顯示)
│   └── 🎨 BackgroundCtrl (背景控制)
├── ⏰ TimeManagerTest (時間管理)
├── 📊 NumericalRecords (數值記錄)
├── 🎵 bgmManager (音效管理)
├── 🖼️ GameUICtrlmanager (UI管理)
├── 💾 SaveDataManager (存檔管理)
└── 📡 EventBus (事件通信)
```

---

## 🎯 使用指南

### 📋 閱讀建議順序
1. **先讀核心模組**: `GameManagerTest.md` 了解整體架構
2. **理解數據流**: `NumericalRecords.md` 和 `EventBus.md`
3. **學習子系統**: 根據需要閱讀特定模組文件
4. **參考架構文件**: 搭配 `Architecture/` 資料夾內容

### 🔧 開發工作流程
1. **需求分析**: 確定要修改的模組
2. **閱讀文件**: 詳細了解模組功能和接口
3. **查看架構**: 確認模組間的依賴關係
4. **進行修改**: 按照模組規範進行開發
5. **測試整合**: 確保修改不影響其他模組

---

## 📚 模組開發規範

### 🎯 程式碼規範
- **命名規範**: 使用 PascalCase 命名類別和方法
- **註解規範**: 重要方法必須有 XML 註解
- **錯誤處理**: 適當的異常處理和錯誤記錄
- **性能考量**: 避免不必要的資源消耗

### 🔌 整合規範
- **接口設計**: 使用接口定義模組間契約
- **事件通信**: 優先使用 EventBus 進行通信
- **數據管理**: 統一通過 NumericalRecords 管理數據
- **錯誤傳播**: 適當的錯誤傳播機制

---

## 🛠 除錯指南

### 🔍 常見問題排查
1. **模組初始化失敗**
   - 檢查依賴關係是否正確
   - 確認初始化順序
   - 查看 Unity Console 錯誤訊息

2. **數據同步問題**
   - 檢查 EventBus 事件訂閱
   - 確認 NumericalRecords 更新
   - 驗證數據流向

3. **性能問題**
   - 檢查是否有記憶體洩漏
   - 確認事件取消訂閱
   - 監控 Update 方法調用頻率

### 📊 除錯工具
- **Unity Console**: 基本錯誤和日誌輸出
- **Unity Profiler**: 性能分析工具
- **Custom Debug**: 自定義除錯面板
- **Event Tracker**: 事件追蹤工具

---

## 🎨 模組擴展指南

### 🧩 新增模組步驟
1. **設計階段**
   - 定義模組職責和接口
   - 確定與其他模組的關係
   - 設計數據結構

2. **實作階段**
   - 實作核心功能
   - 整合 EventBus 通信
   - 添加錯誤處理

3. **測試階段**
   - 單元測試
   - 整合測試
   - 性能測試

4. **文件階段**
   - 撰寫模組說明文件
   - 更新架構文件
   - 添加使用範例

---

## 🔄 版本控制

### 📋 模組版本管理
- **主版本**: 不兼容的API變更
- **次版本**: 新功能添加
- **修訂版本**: 錯誤修正和小幅改進

### 🎯 升級指南
1. **向後兼容**: 盡量保持API向後兼容
2. **廢棄警告**: 提前標記將要廢棄的功能
3. **遷移指南**: 提供升級遷移文件
4. **測試覆蓋**: 確保新版本的測試覆蓋

---

## 📊 性能監控

### 🚀 性能指標
- **初始化時間**: 模組啟動時間
- **記憶體使用**: 運行時記憶體占用
- **CPU使用**: 處理器使用率
- **事件響應**: 事件處理延遲

### 🔍 監控工具
```csharp
public class ModulePerformanceMonitor
{
    public void TrackInitTime(string moduleName, float initTime);
    public void TrackMemoryUsage(string moduleName, long memoryUsage);
    public void TrackEventLatency(string eventName, float latency);
    public void GeneratePerformanceReport();
}
```

---

## 💬 Claude 使用提示

使用此資料夾時請：
1. **先閱讀 README**: 了解整體模組架構
2. **按需閱讀**: 根據開發需求選擇相關模組
3. **交叉參考**: 搭配 `Architecture/` 資料夾內容
4. **實作參考**: 直接查看程式碼實作細節
5. **持續更新**: 修改模組後記得更新文件

開發新功能時建議：
- 先了解現有模組是否能滿足需求
- 確認模組間的依賴關係
- 遵循現有的程式碼規範
- 進行充分的測試驗證
- 更新相關文件和註解