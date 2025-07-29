# 🖼️ CG展示系統架構

> LoveTide 劇情模式的CG場景展示、背景管理和視覺效果架構設計

---

## 🎯 概述

CG展示系統負責管理 LoveTide 劇情模式中的所有視覺場景展示，包括背景圖片切換、CG場景管理、視覺效果處理和場景轉換動畫。系統與對話系統緊密整合，提供沉浸式的視覺體驗。

---

## ⚠️ 重要架構說明

### 🚨 **演員系統改動通知**
> ⚠️ **注意**: 本文檔描述的CG與演員整合部分將隨著**演員系統大改動**而需要調整。實際實作時請參考最新的演員系統架構。

### 📋 CG系統特色
- **靜態立繪展示**: 不同角色的立繪圖片管理
- **背景場景切換**: 多種場景背景的動態載入
- **CG特殊場景**: 重要劇情節點的專用CG
- **視覺效果**: 淡入淡出、轉場效果等

---

## 🏗️ CG展示系統架構

### 📊 核心系統結構
```
🖼️ CG展示系統
│
├── 🎨 CGDisplay (CG顯示管理器)
│   ├── 背景圖片管理
│   ├── CG場景切換
│   ├── 立繪顯示控制
│   └── 視覺效果處理
│
├── 🖼️ BackgroundManager (背景管理器)
│   ├── 場景背景載入
│   ├── 背景圖片快取
│   ├── 場景預載入
│   └── 背景切換效果
│
├── 🎭 CGActorIntegration (CG演員整合)
│   ├── 立繪與CG配合
│   ├── 角色定位管理
│   ├── 表情與場景同步
│   └── 演員顯示優先級
│
└── ✨ VisualEffects (視覺效果)
    ├── 淡入淡出效果
    ├── 場景轉換動畫
    ├── 特殊視覺效果
    └── UI元素動畫
```

---

## 🎨 CGDisplay 核心實現

### 🔧 CG顯示管理器
```csharp
public class CGDisplay : MonoBehaviour
{
    [Header("CG資源")]
    public Image backgroundImage;           // 背景圖片顯示
    public Image[] cgImages;               // CG圖片陣列
    public Sprite[] backgroundSprites;     // 背景圖片資源
    
    [Header("顯示控制")]
    public bool isShowingCG = false;
    public int currentCGIndex = -1;
    public int currentBackgroundIndex = 0;
    
    [Header("效果設定")]
    public float fadeSpeed = 1.0f;
    public AnimationCurve fadeCurve;
    
    // 🖼️ 顯示背景圖片
    public void DisplayBackGroundChick(int backgroundIndex)
    {
        if (backgroundIndex >= 0 && backgroundIndex < backgroundSprites.Length)
        {
            StartCoroutine(FadeToBackground(backgroundSprites[backgroundIndex]));
            currentBackgroundIndex = backgroundIndex;
            Debug.Log($"切換背景: {backgroundIndex}");
        }
        else
        {
            Debug.LogWarning($"背景索引超出範圍: {backgroundIndex}");
        }
    }
    
    // 🎭 顯示CG場景
    public void DisplayCGScene(int cgIndex)
    {
        if (cgIndex >= 0 && cgIndex < cgImages.Length)
        {
            StartCoroutine(ShowCGWithEffect(cgIndex));
            currentCGIndex = cgIndex;
            isShowingCG = true;
            Debug.Log($"顯示CG: {cgIndex}");
        }
        else
        {
            Debug.LogWarning($"CG索引超出範圍: {cgIndex}");
        }
    }
    
    // 🚫 隱藏CG，返回背景
    public void HideCG()
    {
        if (isShowingCG)
        {
            StartCoroutine(HideCGWithEffect());
            isShowingCG = false;
            currentCGIndex = -1;
            Debug.Log("隱藏CG，返回背景");
        }
    }
}
```

### 🌊 視覺效果實現
```csharp
public class CGDisplay : MonoBehaviour
{
    // 🌅 背景淡入效果
    private IEnumerator FadeToBackground(Sprite newBackground)
    {
        // 準備新背景
        Image tempBackground = CreateTempBackgroundImage();
        tempBackground.sprite = newBackground;
        tempBackground.color = new Color(1, 1, 1, 0);
        
        // 淡入動畫
        float elapsedTime = 0;
        while (elapsedTime < fadeSpeed)
        {
            elapsedTime += Time.deltaTime;
            float alpha = fadeCurve.Evaluate(elapsedTime / fadeSpeed);
            
            // 新背景淡入
            tempBackground.color = new Color(1, 1, 1, alpha);
            // 舊背景淡出
            backgroundImage.color = new Color(1, 1, 1, 1 - alpha);
            
            yield return null;
        }
        
        // 完成切換
        backgroundImage.sprite = newBackground;
        backgroundImage.color = Color.white;
        Destroy(tempBackground.gameObject);
    }
    
    // 🎬 CG顯示效果
    private IEnumerator ShowCGWithEffect(int cgIndex)
    {
        Image cgImage = cgImages[cgIndex];
        cgImage.gameObject.SetActive(true);
        cgImage.color = new Color(1, 1, 1, 0);
        
        // CG淡入效果
        float elapsedTime = 0;
        while (elapsedTime < fadeSpeed * 0.8f) // CG顯示稍快一些
        {
            elapsedTime += Time.deltaTime;
            float alpha = fadeCurve.Evaluate(elapsedTime / (fadeSpeed * 0.8f));
            cgImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        
        cgImage.color = Color.white;
        
        // 通知其他系統CG已顯示
        NotifyCGDisplayed(cgIndex);
    }
    
    // 🚫 CG隱藏效果
    private IEnumerator HideCGWithEffect()
    {
        if (currentCGIndex >= 0)
        {
            Image cgImage = cgImages[currentCGIndex];
            
            // CG淡出效果
            float elapsedTime = 0;
            while (elapsedTime < fadeSpeed * 0.6f) // CG隱藏更快
            {
                elapsedTime += Time.deltaTime;
                float alpha = 1 - fadeCurve.Evaluate(elapsedTime / (fadeSpeed * 0.6f));
                cgImage.color = new Color(1, 1, 1, alpha);
                yield return null;
            }
            
            cgImage.color = new Color(1, 1, 1, 0);
            cgImage.gameObject.SetActive(false);
        }
        
        // 通知其他系統CG已隱藏
        NotifyCGHidden();
    }
}
```

---

## 🖼️ 背景管理系統

### 🎯 BackgroundManager 背景管理器
```csharp
public class BackgroundManager : MonoBehaviour
{
    [Header("背景資源")]
    public BackgroundData[] backgroundDatabase;
    public Dictionary<string, Sprite> backgroundCache;
    
    [Header("預載入設定")]
    public bool enablePreloading = true;
    public string[] preloadScenes; // 需要預載入的場景
    
    void Start()
    {
        InitializeBackgroundSystem();
        
        if (enablePreloading)
        {
            StartCoroutine(PreloadBackgrounds());
        }
    }
    
    private void InitializeBackgroundSystem()
    {
        backgroundCache = new Dictionary<string, Sprite>();
        
        // 載入背景數據庫
        LoadBackgroundDatabase();
    }
    
    // 🔄 預載入背景圖片
    private IEnumerator PreloadBackgrounds()
    {
        foreach (string sceneName in preloadScenes)
        {
            BackgroundData bgData = GetBackgroundData(sceneName);
            if (bgData != null && !backgroundCache.ContainsKey(sceneName))
            {
                // 異步載入背景圖片
                var loadRequest = Resources.LoadAsync<Sprite>(bgData.resourcePath);
                yield return loadRequest;
                
                if (loadRequest.asset != null)
                {
                    backgroundCache[sceneName] = loadRequest.asset as Sprite;
                    Debug.Log($"預載入背景: {sceneName}");
                }
            }
            
            yield return null; // 分幀載入，避免卡頓
        }
        
        Debug.Log($"背景預載入完成，共載入 {backgroundCache.Count} 個背景");
    }
    
    // 🎨 獲取背景圖片
    public Sprite GetBackground(string sceneName)
    {
        // 優先從快取中獲取
        if (backgroundCache.ContainsKey(sceneName))
        {
            return backgroundCache[sceneName];
        }
        
        // 即時載入
        BackgroundData bgData = GetBackgroundData(sceneName);
        if (bgData != null)
        {
            Sprite bgSprite = Resources.Load<Sprite>(bgData.resourcePath);
            if (bgSprite != null)
            {
                backgroundCache[sceneName] = bgSprite;
                return bgSprite;
            }
        }
        
        Debug.LogWarning($"找不到背景: {sceneName}");
        return null;
    }
}

[System.Serializable]
public class BackgroundData
{
    public string sceneName;        // 場景名稱
    public string resourcePath;     // 資源路徑
    public string description;      // 場景描述
    public bool isSpecialCG;       // 是否為特殊CG
}
```

---

## 🎭 CG與演員系統整合

### 🔗 CGActorIntegration 整合控制器
```csharp
public class CGActorIntegration : MonoBehaviour
{
    [Header("系統引用")]
    public CGDisplay cgDisplay;
    public ActorManagerDrama actorManager; // ⚠️ 將隨演員系統改動而調整
    
    [Header("整合設定")]
    public LayerPriority displayPriority;
    public Vector2[] actorPositions; // 演員在不同CG中的位置
    
    // 🎯 CG與演員協調顯示
    public void CoordinateActorWithCG(int cgIndex, string actorId)
    {
        // 1. 顯示對應的CG背景
        cgDisplay.DisplayCGScene(cgIndex);
        
        // 2. 調整演員位置和顯示
        AdjustActorForCG(actorId, cgIndex);
        
        // 3. 設定顯示優先級
        SetDisplayPriority(cgIndex);
    }
    
    private void AdjustActorForCG(string actorId, int cgIndex)
    {
        // ⚠️ 注意：此部分將隨演員系統大改動而需要重新設計
        
        if (actorManager != null)
        {
            // 根據CG調整演員位置
            if (cgIndex < actorPositions.Length)
            {
                Vector2 targetPosition = actorPositions[cgIndex];
                actorManager.SetActorPosition(actorId, targetPosition);
            }
            
            // 調整演員顯示設定
            actorManager.SetActorLayerOrder(actorId, GetActorLayerOrder(cgIndex));
        }
    }
    
    private void SetDisplayPriority(int cgIndex)
    {
        // 設定CG和演員的顯示優先級
        switch (displayPriority)
        {
            case LayerPriority.CGFirst:
                // CG在前，演員在後
                cgDisplay.SetSortingOrder(100);
                actorManager.SetGlobalSortingOrder(50);
                break;
                
            case LayerPriority.ActorFirst:
                // 演員在前，CG在後
                cgDisplay.SetSortingOrder(50);
                actorManager.SetGlobalSortingOrder(100);
                break;
                
            case LayerPriority.Balanced:
                // 根據場景動態調整
                DynamicPriorityAdjustment(cgIndex);
                break;
        }
    }
}

public enum LayerPriority
{
    CGFirst,     // CG優先顯示
    ActorFirst,  // 演員優先顯示
    Balanced     // 動態平衡
}
```

---

## 🎮 對話系統整合

### 💬 CG觸發機制
```csharp
public class CGDialogIntegration : MonoBehaviour
{
    [Header("對話觸發")]
    public TextBoxDrama textBoxDrama;
    public CGDisplay cgDisplay;
    
    // 🎯 根據對話內容觸發CG
    public void ProcessDialogCGCommand(DialogDataDetail dialogData)
    {
        // 檢查是否需要切換CG
        if (dialogData.switchCGDisplay)
        {
            ProcessCGSwitch(dialogData);
        }
        
        // 檢查是否需要切換背景
        if (HasBackgroundCommand(dialogData))
        {
            ProcessBackgroundSwitch(dialogData);
        }
        
        // 檢查場景轉換需求
        if (dialogData.needTransition)
        {
            ProcessSceneTransition(dialogData);
        }
    }
    
    private void ProcessCGSwitch(DialogDataDetail dialogData)
    {
        // 根據對話數據中的CG指令切換場景
        if (dialogData.switchCGImage)
        {
            // 切換到新的CG場景
            int cgIndex = GetCGIndexFromDialog(dialogData);
            cgDisplay.DisplayCGScene(cgIndex);
        }
        else
        {
            // 隱藏當前CG
            cgDisplay.HideCG();
        }
    }
    
    private void ProcessBackgroundSwitch(DialogDataDetail dialogData)
    {
        // 背景切換邏輯
        string backgroundName = GetBackgroundNameFromDialog(dialogData);
        Sprite newBackground = backgroundManager.GetBackground(backgroundName);
        
        if (newBackground != null)
        {
            cgDisplay.DisplayBackGroundChick(GetBackgroundIndex(backgroundName));
        }
    }
}
```

---

## 📊 未來JSON驅動CG系統

### 🎨 CG場景JSON結構設計
```json
{
  "cgScenes": {
    "bedroom_morning": {
      "backgroundImage": "bg_bedroom_morning",
      "cgLayers": [
        {
          "layerName": "base_scene",
          "imagePath": "CG/bedroom_morning_base",
          "zOrder": 1
        },
        {
          "layerName": "character_overlay", 
          "imagePath": "CG/yuka_bedroom_overlay",
          "zOrder": 2,
          "conditions": {
            "character": "yuka",
            "expression": "happy"
          }
        }
      ],
      "transitions": {
        "fadeIn": {"duration": 1.0, "curve": "smooth"},
        "fadeOut": {"duration": 0.8, "curve": "smooth"}
      },
      "actorIntegration": {
        "hideActors": false,
        "actorPositions": {
          "yuka": {"x": 200, "y": 100},
          "player": {"x": -200, "y": 100}
        },
        "displayPriority": "balanced"
      }
    }
  },
  
  "backgroundScenes": {
    "living_room": {
      "resourcePath": "Backgrounds/living_room",
      "preload": true,
      "variants": ["morning", "evening", "night"]
    },
    "beach": {
      "resourcePath": "Backgrounds/beach",
      "preload": false,
      "specialEffects": ["waves", "sunset"]
    }
  }
}
```

---

## 🔧 開發建議

### 💡 CG系統實作指導
1. **資源管理**: 使用快取機制提高載入效率
2. **記憶體優化**: 及時卸載不需要的CG資源
3. **視覺效果**: 提供流暢的轉場動畫
4. **整合性**: 與對話系統和演員系統緊密配合

### ⚠️ 注意事項
- **演員系統兼容**: 隨著演員系統大改動，需要調整整合介面
- **資源路徑**: 確保CG資源路徑的一致性
- **效能考量**: 大圖片的載入和顯示可能影響效能
- **UI層級**: 合理安排CG、演員、UI的顯示層級

### 🎯 效能優化建議
- **分辨率管理**: 根據設備性能載入不同分辨率的CG
- **異步載入**: 使用協程避免載入時的卡頓
- **記憶體池**: 重用常用的CG顯示組件
- **壓縮格式**: 選擇合適的圖片壓縮格式

---

## 🔗 相關架構文件導覽

### 🎭 劇情模式整合
- **🎬 劇情場景管理**: [`劇情場景管理.md`](./劇情場景管理.md) - CGDisplay的上層管理
- **🎯 劇情流程控制**: [`劇情流程控制.md`](./劇情流程控制.md) - CG觸發時機控制
- **💫 場景轉換系統**: [`場景轉換系統.md`](./場景轉換系統.md) - CG切換效果

### 🔄 系統整合
- **💬 對話系統**: [`../SharedSystems/對話系統架構.md`](../SharedSystems/對話系統架構.md) - CG觸發指令
- **🎭 演員控制**: [`../SharedSystems/演員控制架構.md`](../SharedSystems/演員控制架構.md) - CG與演員配合

---

## 💬 Claude 使用提示

### 🎯 CG系統開發重點
1. **視覺體驗**: 確保CG切換的流暢性和視覺效果
2. **資源管理**: 高效的圖片載入和記憶體管理
3. **系統整合**: 與對話和演員系統的無縫配合
4. **擴展性**: 支援未來新增的CG場景和效果

### 🚨 **演員系統改動影響**
- CG與演員位置調整功能需要重新設計
- 顯示優先級管理可能需要調整
- 整合介面將隨新演員系統而改變

---

**最後更新**: 2025-07-29  
**版本**: 1.0 (CG展示系統架構)  
**維護者**: 開發團隊 + Claude AI

> 🖼️ **核心提醒**: CG展示系統是劇情模式視覺體驗的關鍵，需要與對話系統和演員系統緊密配合。隨著演員系統的大改動，相關整合功能需要同步調整。