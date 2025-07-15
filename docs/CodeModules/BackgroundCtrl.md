# 🖼️ 模組名稱：BackgroundCtrl

> 背景控制器模組，負責場景背景的切換、動畫播放和視覺效果管理

---

## 🔖 模組功能

BackgroundCtrl 負責管理遊戲中所有背景相關的顯示和控制，包括靜態背景圖片切換、Spine 動畫背景播放、背景特效和場景氛圍控制。

---

## 📍 檔案位置

**路徑**: `Assets/Scripts/GamePlay/BackgroundCtrl.cs`  
**命名空間**: `LoveTide.GamePlay`  
**繼承**: `MonoBehaviour`

---

## 🧩 公開方法一覽

| 方法名稱 | 功能描述 | 參數 | 回傳值 |
|----------|----------|------|---------|
| `ChangeBackground(string bgName)` | 切換背景圖片 | string | void |
| `PlaySpineAnimation(string animName)` | 播放Spine動畫背景 | string | void |
| `SetBackgroundAlpha(float alpha)` | 設定背景透明度 | float | void |
| `FadeInBackground(float duration)` | 背景淡入效果 | float | void |
| `FadeOutBackground(float duration)` | 背景淡出效果 | float | void |
| `CrossFadeBackground(string newBg, float duration)` | 交叉淡入新背景 | string, float | void |
| `SetTimeOfDayEffect(TimeOfDay time)` | 設定時間效果 | TimeOfDay | void |
| `EnableParticleEffect(bool enabled)` | 啟用粒子效果 | bool | void |
| `SetWeatherEffect(WeatherType weather)` | 設定天氣效果 | WeatherType | void |
| `ResetBackground()` | 重置背景設定 | 無 | void |

---

## 🎯 主要屬性

### 🖼️ 背景圖片控制
```csharp
[Header("Background Images")]
public Image staticBackground;              // 靜態背景圖片
public Image overlayBackground;             // 覆蓋背景圖片
public BackgroundDatabase backgroundDB;     // 背景資料庫
public string currentBackgroundName;        // 當前背景名稱
```

### 🎭 Spine 動畫控制
```csharp
[Header("Spine Animation")]
public SkeletonGraphic spineBackground;     // Spine背景動畫
public SkeletonDataAsset[] spineAssets;     // Spine資源
public string currentAnimationName;         // 當前動畫名稱
public bool loopAnimation = true;           // 動畫循環
```

### 🎨 視覺效果
```csharp
[Header("Visual Effects")]
public ParticleSystem[] particleEffects;   // 粒子效果系統
public Light sceneLight;                    // 場景光源
public Camera backgroundCamera;             // 背景攝影機
public Material[] effectMaterials;          // 效果材質
```

### ⏰ 時間效果
```csharp
[Header("Time Effects")]
public Color morningTint = new Color(1f, 0.9f, 0.8f);
public Color noonTint = Color.white;
public Color eveningTint = new Color(1f, 0.8f, 0.6f);
public Color nightTint = new Color(0.3f, 0.3f, 0.8f);
public float tintTransitionSpeed = 2.0f;
```

---

## 🎨 背景切換系統

### 🖼️ 靜態背景切換
```csharp
public void ChangeBackground(string bgName)
{
    var bgData = backgroundDB.GetBackground(bgName);
    if (bgData != null)
    {
        // 1. 載入新背景圖片
        var newSprite = LoadBackgroundSprite(bgData.spritePath);
        
        // 2. 開始切換動畫
        StartCoroutine(BackgroundTransition(newSprite, bgData.transitionType));
        
        // 3. 更新當前背景記錄
        currentBackgroundName = bgName;
        
        // 4. 觸發背景切換事件
        EventBus.Instance.Publish("BackgroundChanged", bgName);
    }
    else
    {
        Debug.LogWarning($"Background not found: {bgName}");
    }
}
```

### 🎪 背景轉場動畫
```csharp
private IEnumerator BackgroundTransition(Sprite newSprite, TransitionType type)
{
    switch (type)
    {
        case TransitionType.Fade:
            yield return StartCoroutine(FadeTransition(newSprite));
            break;
        case TransitionType.Slide:
            yield return StartCoroutine(SlideTransition(newSprite));
            break;
        case TransitionType.Dissolve:
            yield return StartCoroutine(DissolveTransition(newSprite));
            break;
        default:
            staticBackground.sprite = newSprite;
            break;
    }
}
```

### 🌅 淡入淡出效果
```csharp
private IEnumerator FadeTransition(Sprite newSprite)
{
    // 設定覆蓋背景為新圖片
    overlayBackground.sprite = newSprite;
    overlayBackground.color = new Color(1, 1, 1, 0);
    
    // 淡入新背景
    float elapsed = 0f;
    while (elapsed < fadeTransitionDuration)
    {
        elapsed += Time.deltaTime;
        float alpha = Mathf.Lerp(0, 1, elapsed / fadeTransitionDuration);
        overlayBackground.color = new Color(1, 1, 1, alpha);
        yield return null;
    }
    
    // 完成轉場
    staticBackground.sprite = newSprite;
    overlayBackground.color = new Color(1, 1, 1, 0);
}
```

---

## 🎭 Spine動畫系統

### 🎪 Spine動畫播放
```csharp
public void PlaySpineAnimation(string animName)
{
    if (spineBackground != null)
    {
        var skeletonData = FindSpineAsset(animName);
        if (skeletonData != null)
        {
            // 1. 設定Spine資源
            spineBackground.skeletonDataAsset = skeletonData;
            spineBackground.Initialize(true);
            
            // 2. 播放動畫
            var animationState = spineBackground.AnimationState;
            animationState.SetAnimation(0, animName, loopAnimation);
            
            // 3. 記錄當前動畫
            currentAnimationName = animName;
            
            // 4. 觸發動畫事件
            EventBus.Instance.Publish("SpineAnimationStart", animName);
        }
    }
}
```

### 🎯 Spine動畫控制
```csharp
public void SetSpineAnimationSpeed(float speed)
{
    if (spineBackground != null)
    {
        spineBackground.AnimationState.TimeScale = speed;
    }
}

public void PauseSpineAnimation()
{
    if (spineBackground != null)
    {
        spineBackground.AnimationState.TimeScale = 0f;
    }
}

public void ResumeSpineAnimation()
{
    if (spineBackground != null)
    {
        spineBackground.AnimationState.TimeScale = 1f;
    }
}
```

---

## 🌈 視覺效果系統

### 🎨 顏色色調控制
```csharp
public void SetTimeOfDayEffect(TimeOfDay time)
{
    Color targetTint = GetTimeOfDayTint(time);
    StartCoroutine(TransitionTint(targetTint));
}

private Color GetTimeOfDayTint(TimeOfDay time)
{
    switch (time)
    {
        case TimeOfDay.Morning:
            return morningTint;
        case TimeOfDay.Noon:
            return noonTint;
        case TimeOfDay.Evening:
            return eveningTint;
        case TimeOfDay.Night:
            return nightTint;
        default:
            return Color.white;
    }
}

private IEnumerator TransitionTint(Color targetTint)
{
    Color startTint = staticBackground.color;
    float elapsed = 0f;
    
    while (elapsed < tintTransitionSpeed)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / tintTransitionSpeed;
        
        Color currentTint = Color.Lerp(startTint, targetTint, t);
        staticBackground.color = currentTint;
        
        yield return null;
    }
    
    staticBackground.color = targetTint;
}
```

### 🌦️ 天氣效果系統
```csharp
public void SetWeatherEffect(WeatherType weather)
{
    // 停止所有粒子效果
    StopAllParticleEffects();
    
    switch (weather)
    {
        case WeatherType.Rain:
            EnableRainEffect();
            break;
        case WeatherType.Snow:
            EnableSnowEffect();
            break;
        case WeatherType.Fog:
            EnableFogEffect();
            break;
        case WeatherType.Clear:
            // 清除所有效果
            break;
    }
    
    // 觸發天氣變化事件
    EventBus.Instance.Publish("WeatherChanged", weather);
}

private void EnableRainEffect()
{
    var rainEffect = GetParticleEffect("Rain");
    if (rainEffect != null)
    {
        rainEffect.Play();
        
        // 播放雨聲
        AudioManager.Instance.PlayAmbient("Rain_Sound");
    }
}
```

---

## 🎵 音效整合

### 🔊 背景音效控制
```csharp
private void PlayBackgroundAudio(string bgName)
{
    var bgData = backgroundDB.GetBackground(bgName);
    if (bgData != null)
    {
        // 播放背景音樂
        if (!string.IsNullOrEmpty(bgData.bgmName))
        {
            AudioManager.Instance.PlayBGM(bgData.bgmName);
        }
        
        // 播放環境音效
        if (!string.IsNullOrEmpty(bgData.ambientName))
        {
            AudioManager.Instance.PlayAmbient(bgData.ambientName);
        }
    }
}
```

---

## 📊 背景資料庫

### 🗄️ 背景資料結構
```csharp
[System.Serializable]
public class BackgroundData
{
    public string backgroundName;           // 背景名稱
    public string spritePath;               // 圖片路徑
    public TransitionType transitionType;   // 轉場類型
    public float transitionDuration;        // 轉場時間
    public string bgmName;                  // 背景音樂
    public string ambientName;              // 環境音效
    public TimeOfDay preferredTime;         // 建議時間
    public bool hasSpineAnimation;          // 是否有Spine動畫
    public string spineAnimationName;       // Spine動畫名稱
}
```

### 🎯 背景資料庫
```csharp
[CreateAssetMenu(fileName = "BackgroundDatabase", menuName = "LoveTide/Background Database")]
public class BackgroundDatabase : ScriptableObject
{
    public BackgroundData[] backgrounds;
    
    public BackgroundData GetBackground(string name)
    {
        return System.Array.Find(backgrounds, bg => bg.backgroundName == name);
    }
    
    public BackgroundData[] GetBackgroundsForTime(TimeOfDay time)
    {
        return System.Array.FindAll(backgrounds, bg => bg.preferredTime == time);
    }
}
```

---

## 🔧 性能優化

### 🚀 資源管理
```csharp
public class BackgroundResourceManager : MonoBehaviour
{
    private Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();
    private Dictionary<string, SkeletonDataAsset> loadedSpineAssets = new Dictionary<string, SkeletonDataAsset>();
    
    public Sprite LoadBackgroundSprite(string path)
    {
        if (loadedSprites.ContainsKey(path))
        {
            return loadedSprites[path];
        }
        
        var sprite = Resources.Load<Sprite>(path);
        if (sprite != null)
        {
            loadedSprites[path] = sprite;
        }
        
        return sprite;
    }
    
    public void UnloadUnusedResources()
    {
        // 卸載未使用的資源
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
```

### 🎯 記憶體管理
```csharp
private void OnDestroy()
{
    // 清理資源
    StopAllCoroutines();
    StopAllParticleEffects();
    
    // 取消事件訂閱
    EventBus.Instance.Unsubscribe<TimeEvent>("TimeChanged", OnTimeChanged);
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
    EventBus.Instance.Subscribe<SceneChangeEvent>("SceneChanged", OnSceneChanged);
    EventBus.Instance.Subscribe<TimeEvent>("TimeChanged", OnTimeChanged);
    EventBus.Instance.Subscribe<WeatherEvent>("WeatherChanged", OnWeatherChanged);
}

private void OnSceneChanged(SceneChangeEvent eventData)
{
    // 根據場景載入對應背景
    string sceneBg = GetSceneBackground(eventData.sceneName);
    if (!string.IsNullOrEmpty(sceneBg))
    {
        ChangeBackground(sceneBg);
    }
}

private void OnTimeChanged(TimeEvent eventData)
{
    // 更新時間色調效果
    SetTimeOfDayEffect(eventData.timeOfDay);
}
```

---

## 🛡 錯誤處理

### 🚨 資源載入錯誤處理
```csharp
private Sprite LoadBackgroundSprite(string path)
{
    try
    {
        var sprite = Resources.Load<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogWarning($"Background sprite not found: {path}");
            return GetDefaultBackground();
        }
        return sprite;
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Failed to load background sprite: {e.Message}");
        return GetDefaultBackground();
    }
}

private Sprite GetDefaultBackground()
{
    return Resources.Load<Sprite>("Backgrounds/Default");
}
```

---

## 🔁 呼叫關係

### 📊 被呼叫情況
- **GameManagerTest**: 場景切換時呼叫
- **GamePlayingManagerDrama**: 劇情播放時呼叫
- **TimeManagerTest**: 時間變化時呼叫
- **Event System**: 事件觸發時呼叫

### 🎯 呼叫對象
- **AudioManager**: 播放背景音樂和環境音效
- **EventBus**: 發布背景相關事件
- **Resources**: 載入背景資源

---

## 💬 Claude 使用提示

修改 BackgroundCtrl 時請注意：
1. **先閱讀 `Architecture/遊戲流程架構.md`** 了解背景在整體流程中的角色
2. **參考 `Architecture/音效系統架構.md`** 了解音效整合方式
3. **測試轉場效果** 確保所有轉場動畫流暢
4. **檢查資源載入** 確保背景資源正確載入和釋放
5. **驗證Spine動畫** 測試Spine動畫播放和控制
6. **性能測試** 確保背景切換不影響遊戲性能

常見修改場景：
- 新增背景轉場效果
- 添加新的視覺效果
- 優化資源載入機制
- 整合新的背景類型
- 修改時間和天氣效果