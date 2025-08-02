# 🖼️ CG觀賞系統

> LoveTide 的獨立CG收藏館系統，提供玩家瀏覽已解鎖CG內容的完整體驗

---

## 🎯 概述

CG觀賞系統是一個獨立於主遊戲流程的內容瀏覽系統，玩家可以通過主選單進入CG大廳，瀏覽在遊戲過程中解鎖的各種CG場景。系統採用JSON驅動的解鎖機制，支援分類展示、全螢幕播放和便捷的操作控制。

---

## 🎮 系統特色

### 🏛️ **獨立大廳系統**
- **獨立場景**: 完全脫離主遊戲流程的瀏覽環境
- **主選單進入**: 從主選單的"CG回放"按鈕進入
- **分類展示**: 劇情CG、成人內容、特殊事件等分類管理
- **解鎖狀態**: 鎖定/解鎖的視覺化狀態顯示

### 🔓 **解鎖機制**
- **事件驅動**: 劇情播放完成自動解鎖對應CG
- **JSON記錄**: 獨立的解鎖進度JSON文件管理
- **跨存檔**: 解鎖進度跨所有存檔槽位共享
- **永久記錄**: 解鎖狀態永久保存，不會丟失

### 🎪 **觀賞體驗**
- **全螢幕展示**: 點擊CG進入全螢幕觀賞模式
- **Spine動畫支援**: 完整播放包含Spine動畫的CG場景
- **操作控制**: 左右鍵切換、ESC關閉等便捷操作
- **高質量顯示**: 原始解析度的CG內容展示

---

## 🏗️ 系統架構

### 📊 整體架構圖
```
🖼️ CG觀賞系統
│
├── 🗂️ CG解鎖管理 (CGUnlockManager)
│   ├── JSON讀寫控制
│   ├── 事件觸發檢測
│   ├── 解鎖狀態記錄
│   └── 映射表管理
│
├── 🏛️ CG大廳界面 (CGGalleryManager)
│   ├── 縮圖網格顯示
│   ├── 分類切換控制
│   ├── 鎖定狀態顯示
│   └── 用戶操作響應
│
├── 🎬 CG播放器 (CGViewer)
│   ├── 全螢幕展示控制
│   ├── Spine動畫播放
│   ├── 切換操作處理
│   └── UI隱藏/顯示
│
└── 🔗 系統整合
    ├── 主選單整合
    ├── 劇情系統觸發
    ├── 存檔系統協作
    └── 音效系統配合
```

---

## 🔓 CG解鎖機制

### 📋 JSON數據結構
```json
{
  "cg_unlock_record": {
    "version": "1.0",
    "last_updated": "2025-07-30T15:30:00",
    "total_unlocked": 12,
    "total_available": 45,
    "categories": {
      "story": {
        "unlocked": ["cg_story_001", "cg_story_005", "cg_story_012"],
        "total": 18
      },
      "adult": {
        "unlocked": ["cg_adult_001", "cg_adult_003"],
        "total": 15
      },
      "special": {
        "unlocked": ["cg_fishing_rare", "cg_restaurant_success"],
        "total": 8
      },
      "endings": {
        "unlocked": ["cg_ending_route1"],
        "total": 4
      }
    },
    "unlock_details": {
      "cg_story_001": {
        "unlock_time": "2025-07-28T10:15:00",
        "trigger_event": "first_meeting_drama",
        "playthrough": 1
      },
      "cg_story_005": {
        "unlock_time": "2025-07-29T14:20:00", 
        "trigger_event": "confession_scene",
        "playthrough": 1
      }
    }
  }
}
```

### 🎯 事件映射機制
```json
{
  "event_cg_mapping": {
    "first_meeting_drama": ["cg_story_001"],
    "confession_scene": ["cg_story_005", "cg_story_006"],
    "fishing_rare_catch": ["cg_special_fishing_01"],
    "restaurant_milestone_100": ["cg_special_restaurant_01"],
    "adult_scene_001": ["cg_adult_001", "cg_adult_002"],
    "ending_route_1": ["cg_ending_route1"],
    "ending_route_2": ["cg_ending_route2"],
    "ending_route_3": ["cg_ending_route3"]
  }
}
```

### 🔄 解鎖觸發流程
```
劇情播放完成 → 事件ID識別 → 映射表查詢 → 檢查重複解鎖 → 更新JSON記錄 → 通知UI刷新
     ↓              ↓            ↓           ↓            ↓            ↓
TextBoxDrama → GetEventID() → CGMapping → IsUnlocked() → SaveJSON → UpdateUI()
```

---

## 🏛️ CG大廳界面

### 🎨 界面佈局設計
```
┌─────────────────────────────────────────────────────────────┐
│  🖼️ CG觀賞大廳                                    [X] 關閉  │
├─────────────────────────────────────────────────────────────┤
│  [劇情CG] [成人內容] [特殊事件] [結局CG]  📊 12/45 已解鎖    │
├─────────────────────────────────────────────────────────────┤
│  ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐          │
│  │ CG1 │ │ 🔒  │ │ CG3 │ │ 🔒  │ │ CG5 │ │ 🔒  │          │
│  │已解鎖│ │鎖定 │ │已解鎖│ │鎖定 │ │已解鎖│ │鎖定 │          │
│  └─────┘ └─────┘ └─────┘ └─────┘ └─────┘ └─────┘          │
│  ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐ ┌─────┐          │
│  │ 🔒  │ │ CG8 │ │ 🔒  │ │ 🔒  │ │ 🔒  │ │ 🔒  │          │
│  │鎖定 │ │NEW! │ │鎖定 │ │鎖定 │ │鎖定 │ │鎖定 │          │
│  └─────┘ └─────┘ └─────┘ └─────┘ └─────┘ └─────┘          │
└─────────────────────────────────────────────────────────────┘
```

### 🔍 顯示狀態類型
- **✅ 已解鎖**: 顯示CG縮圖，可點擊觀看
- **🔒 鎖定**: 顯示鎖定圖標或模糊預覽
- **🆕 新解鎖**: 剛解鎖的CG顯示"NEW"標記
- **🌟 特殊**: 隱藏CG需要特殊條件才顯示

### 📊 分類管理
- **劇情CG**: 主線劇情相關場景
- **成人內容**: 限制級內容（需年齡驗證）
- **特殊事件**: 釣魚、餐廳等小遊戲相關
- **結局CG**: 各路線結局專用場景

---

## 🎬 CG播放體驗

### 🖥️ 全螢幕播放模式
```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│                                                             │
│                    [CG場景全螢幕顯示]                        │
│                                                             │
│                 包含Spine動畫完整播放                        │
│                                                             │
│                                                             │
│  [◀] 上一張    [⏸] 暫停    [▶] 下一張    [✕] 關閉          │
└─────────────────────────────────────────────────────────────┘
```

### 🎮 操作控制
- **⬅➡ 左右鍵**: 切換上/下一張CG
- **⬆⬇ 上下鍵**: 切換CG分類
- **ESC鍵**: 關閉當前CG，返回大廳
- **空格鍵**: 暫停/播放Spine動畫
- **滑鼠滾輪**: 縮放CG內容（如支援）

### 🎪 動畫播放支援
```csharp
// CG播放器核心功能
public class CGViewer : MonoBehaviour
{
    [Header("播放控制")]
    public Image cgDisplayImage;
    public SkeletonGraphic spineAnimation; // Spine動畫支援
    public bool isSpineContent = false;
    
    [Header("操作設定")]
    public KeyCode previousKey = KeyCode.LeftArrow;
    public KeyCode nextKey = KeyCode.RightArrow;
    public KeyCode closeKey = KeyCode.Escape;
    public KeyCode pauseKey = KeyCode.Space;
    
    // 🎯 播放指定CG
    public void PlayCG(string cgID)
    {
        CGData cgData = GetCGData(cgID);
        
        if (cgData.isSpineContent)
        {
            // 播放包含Spine動畫的CG
            PlaySpineCG(cgData);
        }
        else
        {
            // 播放靜態CG
            PlayStaticCG(cgData);
        }
        
        // 進入全螢幕模式
        EnterFullscreenMode();
    }
    
    // 🎮 處理用戶輸入
    void Update()
    {
        if (Input.GetKeyDown(previousKey))
        {
            ShowPreviousCG();
        }
        else if (Input.GetKeyDown(nextKey))
        {
            ShowNextCG();
        }
        else if (Input.GetKeyDown(closeKey))
        {
            CloseViewer();
        }
        else if (Input.GetKeyDown(pauseKey) && isSpineContent)
        {
            ToggleSpineAnimation();
        }
    }
}
```

---

## 🔗 系統整合

### 🎭 與劇情系統整合
```csharp
// 在 TextBoxDrama.TalkOver() 中加入
public void TalkOver()
{
    // 原有的對話結束邏輯...
    
    // 🆕 檢查CG解鎖
    string eventID = GetCurrentEventID();
    CGUnlockManager.Instance.ProcessEventUnlock(eventID);
    
    // 原有的場景切換邏輯...
}
```

### 🏠 與主選單整合
```csharp 
// 主選單CG回放按鈕
public class MainMenu : MonoBehaviour
{
    [Header("CG系統")]
    public Button cgGalleryButton;
    public string cgGallerySceneName = "CGGallery";
    
    void Start()
    {
        cgGalleryButton.onClick.AddListener(OpenCGGallery);
    }
    
    void OpenCGGallery()
    {
        // 載入CG觀賞場景
        SceneManager.LoadScene(cgGallerySceneName);
    }
}
```

### 💾 與存檔系統協作
- **獨立存儲**: CG解鎖記錄獨立於遊戲存檔
- **跨存檔共享**: 所有存檔槽位共享解鎖進度
- **備份機制**: 自動備份解鎖記錄，防止數據丟失
- **版本兼容**: 支援遊戲更新時的數據遷移

---

## 🎵 音效與體驗

### 🔊 音效設計
- **進入音效**: 進入CG大廳的開場音效
- **解鎖音效**: 新CG解鎖時的特殊提示音
- **切換音效**: CG切換時的過渡音效
- **背景音樂**: CG大廳專用的輕柔背景音樂

### ✨ 視覺效果
- **解鎖動畫**: 新CG解鎖時的光效動畫
- **切換轉場**: CG間切換的平滑轉場效果
- **鎖定狀態**: 未解鎖CG的模糊或馬賽克效果
- **NEW標記**: 新解鎖內容的閃爍提示效果

---

## 🔮 進階功能規劃

### 📊 統計與成就
```json
{
  "cg_statistics": {
    "total_viewing_time": 1250,
    "most_viewed_cg": "cg_story_005",
    "viewing_count": {
      "cg_story_001": 15,
      "cg_story_005": 28,
      "cg_adult_001": 7
    },
    "achievement_progress": {
      "collector": {"current": 12, "target": 20},
      "completionist": {"current": 1, "target": 3},
      "enthusiast": {"current": 156, "target": 100}
    }
  }
}
```

### 🎨 自訂功能
- **播放清單**: 用戶自訂的CG播放順序
- **標籤系統**: 用戶可為CG添加個人標籤
- **評分系統**: 對喜愛的CG進行評分
- **筆記功能**: 為特定CG添加個人筆記

### 🔍 搜尋與篩選
- **關鍵字搜尋**: 按CG名稱或描述搜尋
- **角色篩選**: 按出現角色篩選CG
- **日期篩選**: 按解鎖日期排序和篩選
- **類型篩選**: 多重篩選條件組合

---

## ⚠️ 注意事項

### 🛡️ 內容管理
- **年齡驗證**: 成人內容需要年齡確認
- **分級制度**: 不同內容的適當分級標示
- **隱私保護**: 敏感內容的額外隱私保護
- **家長控制**: 可選的家長控制功能

### 🚀 性能優化
- **懶加載**: 縮圖和內容的按需載入
- **記憶體管理**: 及時釋放未使用的CG資源
- **快取策略**: 常用CG的本地快取機制
- **壓縮優化**: 適當的圖片壓縮和格式選擇

---

## 💬 Claude 使用提示

### 🎯 開發重點
1. **JSON設計**: 確保解鎖記錄的完整性和可擴展性
2. **觸發機制**: 準確捕捉劇情事件並觸發解鎖
3. **用戶體驗**: 流暢的瀏覽和播放體驗
4. **性能考量**: 大量圖片資源的高效管理

### 🔧 實作建議
- 優先實作JSON讀寫和基礎解鎖機制
- 確保與現有劇情系統的無縫整合
- 注重CG大廳的響應式設計和用戶友好性
- 考慮未來內容擴展的架構彈性

### 📋 測試重點
- 解鎖記錄的準確性和持久性
- 各種操作的響應性和穩定性
- 不同分辨率下的顯示效果
- 記憶體使用和性能表現

---

**最後更新**: 2025-07-30  
**版本**: 1.0  
**維護者**: 開發團隊 + Claude AI

> 🖼️ **系統亮點**: CG觀賞系統為玩家提供了完整的內容回顧體驗，通過精心設計的解鎖機制和直觀的操作界面，讓玩家能夠隨時重溫遊戲中的精美場景，增強了遊戲的重玩價值和收藏意義！ ✨