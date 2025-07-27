# 🚶‍♀️ 小Yuka移動系統架構

> 酒吧場景中的智能背景角色移動與行為系統

---

## 🎯 概述

小Yuka系統是養成模式中的智能背景角色，負責在酒吧場景中自然地移動和工作，為遊戲場景增添生動感和真實感。這是一個基於狀態機的自主行為系統。

---

## 🎭 系統特色

### 🌟 核心功能
- **自主移動**: 無需玩家控制，自動在場景中移動
- **智能路徑**: 根據櫃台工作需求選擇移動路徑
- **狀態切換**: Idle 和 Move 狀態的平滑切換
- **工作模擬**: 模擬真實的酒吧服務員行為
- **服裝系統**: 支援不同場景的服裝切換

### 📊 技術實現
- **Spine動畫**: 使用 `YukaQ_*` 系列動畫
- **狀態機設計**: 基於 `IState` 接口的模組化狀態
- **路徑點系統**: 預設路徑點的智能選擇
- **時間驅動**: 基於時間間隔的自動行為觸發

---

## 🏗️ 架構設計

### 📋 類別結構圖
```
🚶‍♀️ 小Yuka移動系統
├── YukaManager (主控制器)
│   ├── 狀態管理
│   ├── 路徑控制
│   ├── 動畫控制
│   └── 場景檢測
├── YukaIdleState (待機狀態)
│   ├── 隨機待機動畫
│   ├── 等待計時器
│   └── 移動觸發
└── YukaMoveState (移動狀態)
    ├── 隨機移動動畫
    ├── 路徑跟隨
    └── 到達檢測
```

---

## 🎮 YukaManager 主控制器

### 🔧 核心組件
```csharp
public class YukaManager : MonoBehaviour
{
    [Header("狀態管理")]
    public IState CurrenState = new YukaIdleState();
    
    [Header("動畫控制")]
    public SkeletonAnimation yukaAnimator;
    public string[] idleAnimator = { "YukaQ_Idle2", "YukaQ_Idle_smile" };
    public string[] walkAnimator = { "YukaQ_Walk", "YukaQ_Walk_smile" };
    
    [Header("路徑系統")]
    public GameObject[] wayPoint;           // 路徑點陣列
    public int wayTarget;                   // 當前目標路徑點
    public float wayDistance;               // 與目標距離
    
    [Header("場景檢測")]
    public bool isInCounter;                // 是否在櫃台區域
    public bool readyGoToCounter;           // 是否準備前往櫃台
}
```

### ⚙️ 主要方法說明

#### 🎯 初始化系統
```csharp
private void OnStart()
{
    // 隨機選擇起始位置 (1-12號路徑點)
    var startPosition = Random.Range(1, 12);
    
    // 隨機決定是否前往櫃台工作
    var counterDetected = Random.Range(1, 3);
    readyGoToCounter = (counterDetected == 1);
    
    // 設定起始位置和顏色
    transform.position = wayPoint[startPosition].transform.position;
    wayTarget = startPosition;
    SetColor(); // 設定角色顏色為 (0.85f, 0.85f, 0.75f, 1)
}
```

#### 🕐 定時切換系統
```csharp
private void SwitchStayLocation()
{
    switchTime += Time.deltaTime;
    
    // 每40秒切換一次工作狀態
    if (switchTime >= 40)
    {
        readyGoToCounter = !readyGoToCounter;  // 切換櫃台工作狀態
        switchTime = 0f;
    }
}
```

#### 🔄 角色翻轉檢測
```csharp
private void FlipDetected()
{
    float currentX = transform.position.x;
    
    // 根據移動方向自動翻轉角色
    if (currentX > previousX)
        yukaAnimator.transform.rotation = Quaternion.Euler(0, 180, 0);  // 向右移動
    else
        yukaAnimator.transform.rotation = Quaternion.Euler(0, 0, 0);    // 向左移動
        
    previousX = currentX;
}
```

#### 📍 櫃台區域檢測
```csharp
private void CounterDetected()
{
    // Y座標 >= 210 視為櫃台區域
    isInCounter = (transform.position.y >= 210);
}
```

---

## 🎯 智能路徑選擇系統

### 📊 路徑邏輯設計
```csharp
public void SwitchPosition()
{
    int nowTarget;
    
    if (readyGoToCounter)  // 準備去櫃台工作
    {
        if (isInCounter)
        {
            nowTarget = Random.Range(1, 4);     // 櫃台內工作區域 (1-3號點)
        }
        else
        {
            // 根據當前位置選擇櫃台入口
            nowTarget = (transform.position.x <= 650) ? 5 : 4;
        }
    }
    else  // 準備離開櫃台或在客區活動
    {
        if (isInCounter)
        {
            // 選擇櫃台出口
            nowTarget = (transform.position.x <= 650) ? 4 : 5;
        }
        else
        {
            nowTarget = Random.Range(6, 11);    // 客區活動範圍 (6-10號點)
        }
    }
    
    // 避免選擇相同位置，確保角色移動
    if (nowTarget == wayTarget)
        SwitchPosition();  // 遞迴重新選擇
    else
        wayTarget = nowTarget;
}
```

### 🗺️ 路徑點配置
基於程式碼分析的路徑點配置：

| 路徑點編號 | 區域分類 | 功能說明 |
|-----------|----------|----------|
| 1-3 | 櫃台工作區 | 櫃台內的工作位置 |
| 4-5 | 櫃台入口 | 櫃台進出的過渡點 |
| 6-10 | 客區活動範圍 | 客人座位區域的服務路徑 |
| 11-12 | 其他區域 | 可能的休息或特殊區域 |

---

## 🎪 狀態機系統

### 😴 YukaIdleState (待機狀態)

#### 🔖 狀態功能
- 播放隨機待機動畫 (`YukaQ_Idle2` 或 `YukaQ_Idle_smile`)
- 等待6秒後觸發移動
- 平滑過渡到移動狀態

#### 💻 實現細節
```csharp
public class YukaIdleState : IState
{
    private float passTime;
    private bool readySwitchPosition;
    
    public void OnEnterState(object action)
    {
        var yuka = (YukaManager)action;
        
        // 隨機選擇待機動畫
        var rangeNumber = Random.Range(0, 2);
        yuka.yukaAnimator.AnimationState.SetAnimation(0, yuka.idleAnimator[rangeNumber], true);
        
        passTime = 0f;
        readySwitchPosition = false;
    }
    
    public void OnStayState(object action)
    {
        var yuka = (YukaManager)action;
        passTime += Time.deltaTime;
        
        // 6秒後準備移動
        if (passTime >= 6 && !readySwitchPosition)
        {
            readySwitchPosition = true;
            yuka.SwitchPosition();           // 選擇新的目標位置
            yuka.ChangeState(new YukaMoveState());  // 切換到移動狀態
        }
    }
}
```

### 🚶‍♀️ YukaMoveState (移動狀態)

#### 🔖 狀態功能
- 播放隨機移動動畫 (`YukaQ_Walk` 或 `YukaQ_Walk_smile`)
- 平滑移動到目標位置
- 到達目標後切換回待機狀態

#### 💻 實現細節
```csharp
public class YukaMoveState : IState
{
    private bool isCheck;
    
    public void OnEnterState(object action)
    {
        var yuka = (YukaManager)action;
        
        // 隨機選擇移動動畫
        var rangeNumber = Random.Range(0, 2);
        yuka.yukaAnimator.AnimationState.SetAnimation(0, yuka.walkAnimator[rangeNumber], true);
        
        isCheck = false;
    }
    
    public void OnStayState(object action)
    {
        var yuka = (YukaManager)action;
        
        if (yuka.wayDistance >= 5f && !isCheck)
        {
            // 以55單位/秒的速度向目標移動
            yuka.transform.position = Vector3.MoveTowards(
                yuka.transform.position,
                yuka.wayPoint[yuka.wayTarget].transform.position,
                55 * Time.deltaTime
            );
        }
        else
        {
            // 到達目標，切換回待機狀態
            yuka.ChangeState(new YukaIdleState());
            isCheck = true;
        }
    }
}
```

---

## 🎨 動畫系統整合

### 🎭 動畫資源
```csharp
// 待機動畫 (隨機播放)
string[] idleAnimator = { 
    "YukaQ_Idle2",      // 普通待機
    "YukaQ_Idle_smile"  // 微笑待機
};

// 移動動畫 (隨機播放)
string[] walkAnimator = { 
    "YukaQ_Walk",       // 普通走路
    "YukaQ_Walk_smile"  // 微笑走路
};
```

### 🎨 視覺特效
```csharp
private void SetColor()
{
    // 設定角色顏色為淡黃色調，與背景融合
    yukaAnimator.skeleton.SetColor(new Color(0.85f, 0.85f, 0.75f, 1));
}
```

---

## ⏱️ 時間驅動系統

### 📅 時間配置
- **待機時間**: 6秒 (等待下次移動)
- **狀態切換**: 40秒 (櫃台工作狀態切換)
- **移動速度**: 55單位/秒
- **到達閾值**: 5單位距離

### 🔄 行為週期
```
🕐 開始 → 😴 待機6秒 → 🚶‍♀️ 移動到目標 → 😴 待機6秒 → 🔄 循環
                    ↓ (每40秒)
              🔄 切換櫃台工作狀態
```

---

## 🔮 未來擴展計劃

### 🏠 宿舍場景支援
```csharp
// 未來宿舍場景的小Yuka
public class DormitoryYukaManager : YukaManager
{
    [Header("宿舍專用設定")]
    public string[] casualIdleAnimator;    // 休閒服待機動畫
    public string[] casualWalkAnimator;    // 休閒服移動動畫
    public GameObject[] dormPathPoints;    // 宿舍路徑點
    
    // 服裝切換邏輯
    public void SwitchToCasualClothes()
    {
        idleAnimator = casualIdleAnimator;
        walkAnimator = casualWalkAnimator;
        wayPoint = dormPathPoints;
    }
}
```

### 🎯 行為模式擴展
- **不同服裝**: 工作服 vs 休閒服
- **場景互動**: 與家具、物品的互動
- **時間感知**: 根據遊戲內時間調整行為
- **情緒系統**: 根據玩家互動調整表情

---

## 🛠️ 開發建議

### 💡 使用指導
1. **路徑點設置**: 在場景中放置合適的路徑點 GameObject
2. **動畫資源**: 確保 Spine 動畫資源正確配置
3. **參數調整**: 根據場景大小調整移動速度和時間間隔
4. **視覺調整**: 調整角色顏色以配合場景氛圍

### ⚠️ 注意事項
- **效能考量**: 小Yuka是裝飾性角色，避免過度複雜的邏輯
- **碰撞檢測**: 當前使用座標檢測，未來可考慮物理碰撞
- **狀態管理**: 確保狀態切換的順暢，避免卡住
- **資源管理**: 動畫播放的記憶體使用優化

### 🔧 調試建議
- 使用 SerializeField 查看實時狀態
- 觀察 wayDistance 確認路徑計算正確
- 檢查 isInCounter 和 readyGoToCounter 的邏輯
- 確認動畫播放是否正確切換

---

## 💬 Claude 使用提示

### 🎯 系統特點
- **自主性強**: 一旦啟動即可自主運行，無需外部控制
- **真實感**: 模擬真實酒吧服務員的工作模式
- **模組化**: 狀態機設計便於添加新行為
- **輕量級**: 作為背景角色，設計簡潔高效

### 📋 修改建議
- 新增行為時優先考慮狀態機擴展
- 路徑點配置要考慮場景的真實性
- 時間參數可根據遊戲節奏調整
- 動畫選擇要保持自然隨機性

---

**架構特色**: 這是一個展現團隊設計功力的系統，通過簡潔的狀態機和智能路徑選擇，創造出生動的背景角色行為，為遊戲場景增添了真實感和活力。小Yuka不僅是技術實現，更是遊戲沉浸感的重要組成部分！ 🌟