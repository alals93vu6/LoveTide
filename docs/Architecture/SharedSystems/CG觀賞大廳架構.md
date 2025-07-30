# 🏛️ CG觀賞大廳架構

> LoveTide CG觀賞系統的技術架構設計，包含解鎖管理、大廳UI控制和播放器架構

---

## 🎯 概述

CG觀賞大廳架構是一個獨立於主遊戲流程的模組化系統，負責管理CG內容的解鎖記錄、大廳界面控制和全螢幕播放功能。系統採用JSON驅動的解鎖機制，通過事件映射實現與劇情系統的解耦整合。

---

## 🏗️ 系統架構圖

```
🏛️ CG觀賞大廳系統架構
│
├── 📊 數據層 (Data Layer)
│   ├── CGUnlockManager - JSON讀寫管理
│   ├── CGMappingData - 事件映射表
│   ├── CGDatabase - CG資源數據庫
│   └── UnlockDataModel - 解鎖數據模型
│
├── 🎮 控制層 (Control Layer)  
│   ├── CGGalleryController - 大廳主控制器
│   ├── CGViewerController - 播放器控制器
│   ├── CategoryManager - 分類管理器
│   └── NavigationController - 導航控制器
│
├── 🎨 展示層 (Presentation Layer)
│   ├── CGGalleryUI - 大廳界面組件
│   ├── CGThumbnailGrid - 縮圖網格系統
│   ├── CGFullscreenViewer - 全螢幕播放器
│   └── UIEffectsManager - UI動效管理
│
└── 🔗 整合層 (Integration Layer)
    ├── MainMenuIntegration - 主選單整合
    ├── StorySystemIntegration - 劇情系統整合
    ├── AudioSystemIntegration - 音效系統整合
    └── SaveSystemIntegration - 存檔系統協作
```

---

## 📊 數據層架構

### 🗂️ CGUnlockManager 解鎖管理器
```csharp
public class CGUnlockManager : MonoBehaviour
{
    [Header("數據文件")]
    public string unlockDataPath = "/cg_unlock_data.json";
    public string mappingDataPath = "/cg_event_mapping.json";
    
    [Header("資源引用")]
    public CGMappingData mappingData;
    public CGDatabase cgDatabase;
    
    private CGUnlockData currentUnlockData;
    private Dictionary<string, List<string>> eventCGMapping;
    
    #region 單例模式
    public static CGUnlockManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    
    // 🚀 系統初始化
    void InitializeSystem()
    {
        LoadEventMapping();
        LoadUnlockData();
        
        // 訂閱劇情系統事件
        EventBus.Instance.Subscribe<StoryEventData>("StoryEventCompleted", OnStoryEventCompleted);
    }
    
    // 📖 載入解鎖數據
    public void LoadUnlockData()
    {
        string filePath = Application.persistentDataPath + unlockDataPath;
        
        if (File.Exists(filePath))
        {
            try
            {
                string jsonData = File.ReadAllText(filePath);
                currentUnlockData = JsonUtility.FromJson<CGUnlockData>(jsonData);
                ValidateUnlockData();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"載入CG解鎖數據失敗: {e.Message}");
                CreateNewUnlockData();
            }
        }
        else
        {
            CreateNewUnlockData();
        }
    }
    
    // 💾 保存解鎖數據
    public void SaveUnlockData()
    {
        try
        {
            currentUnlockData.lastUpdated = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            string jsonData = JsonUtility.ToJson(currentUnlockData, true);
            
            string filePath = Application.persistentDataPath + unlockDataPath;
            File.WriteAllText(filePath, jsonData);
            
            Debug.Log($"CG解鎖數據已保存: {currentUnlockData.totalUnlocked} 個已解鎖");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存CG解鎖數據失敗: {e.Message}");
        }
    }
    
    // 🎭 劇情事件完成處理
    void OnStoryEventCompleted(StoryEventData eventData)
    {
        if (eventCGMapping.ContainsKey(eventData.eventID))
        {
            List<string> cgIDs = eventCGMapping[eventData.eventID];
            foreach (string cgID in cgIDs)
            {
                UnlockCG(cgID, eventData.eventID);
            }
        }
    }
    
    // 🔓 解鎖CG
    public bool UnlockCG(string cgID, string triggerEvent = "")
    {
        if (currentUnlockData.IsUnlocked(cgID))
        {
            return false; // 已經解鎖
        }
        
        // 添加到解鎖列表
        string category = cgDatabase.GetCGCategory(cgID);
        currentUnlockData.AddUnlockedCG(cgID, category);
        
        // 記錄解鎖詳情
        var unlockDetail = new CGUnlockDetail
        {
            unlockTime = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
            triggerEvent = triggerEvent,
            playthrough = GetCurrentPlaythrough()
        };
        currentUnlockData.unlockDetails[cgID] = unlockDetail;
        
        // 保存數據
        SaveUnlockData();
        
        // 觸發解鎖事件
        EventBus.Instance.Publish("CGUnlocked", new CGUnlockEventData 
        { 
            cgID = cgID, 
            category = category 
        });
        
        Debug.Log($"CG已解鎖: {cgID} (觸發事件: {triggerEvent})");
        return true;
    }
    
    // 🔍 查詢接口
    public bool IsCGUnlocked(string cgID)
    {
        return currentUnlockData.IsUnlocked(cgID);
    }
    
    public List<string> GetUnlockedCGsByCategory(string category)
    {
        return currentUnlockData.GetUnlockedByCategory(category);
    }
    
    public CGUnlockData GetUnlockData()
    {
        return currentUnlockData;
    }
}
```

### 📋 數據模型定義
```csharp
[System.Serializable]
public class CGUnlockData
{
    public string version = "1.0";
    public string lastUpdated;
    public int totalUnlocked = 0;
    public int totalAvailable = 45;
    
    public CGCategoryData categories = new CGCategoryData();
    public Dictionary<string, CGUnlockDetail> unlockDetails = new Dictionary<string, CGUnlockDetail>();
    
    public bool IsUnlocked(string cgID)
    {
        return categories.story.unlocked.Contains(cgID) ||
               categories.adult.unlocked.Contains(cgID) ||
               categories.special.unlocked.Contains(cgID) ||
               categories.endings.unlocked.Contains(cgID);
    }
    
    public void AddUnlockedCG(string cgID, string category)
    {
        switch (category.ToLower())
        {
            case "story":
                if (!categories.story.unlocked.Contains(cgID))
                {
                    categories.story.unlocked.Add(cgID);
                    totalUnlocked++;
                }
                break;
            case "adult":
                if (!categories.adult.unlocked.Contains(cgID))
                {
                    categories.adult.unlocked.Add(cgID);
                    totalUnlocked++;
                }
                break;
            case "special":
                if (!categories.special.unlocked.Contains(cgID))
                {
                    categories.special.unlocked.Add(cgID);
                    totalUnlocked++;
                }
                break;
            case "endings":
                if (!categories.endings.unlocked.Contains(cgID))
                {
                    categories.endings.unlocked.Add(cgID);
                    totalUnlocked++;
                }
                break;
        }
    }
    
    public List<string> GetUnlockedByCategory(string category)
    {
        switch (category.ToLower())
        {
            case "story": return categories.story.unlocked;
            case "adult": return categories.adult.unlocked;
            case "special": return categories.special.unlocked;
            case "endings": return categories.endings.unlocked;
            default: return new List<string>();
        }
    }
}

[System.Serializable]
public class CGCategoryData
{
    public CGCategoryInfo story = new CGCategoryInfo { total = 18 };
    public CGCategoryInfo adult = new CGCategoryInfo { total = 15 };
    public CGCategoryInfo special = new CGCategoryInfo { total = 8 };
    public CGCategoryInfo endings = new CGCategoryInfo { total = 4 };
}

[System.Serializable]
public class CGCategoryInfo
{
    public List<string> unlocked = new List<string>();
    public int total;
}

[System.Serializable]
public class CGUnlockDetail
{
    public string unlockTime;
    public string triggerEvent;
    public int playthrough;
}
```

---

## 🎮 控制層架構

### 🏛️ CGGalleryController 大廳主控制器
```csharp
public class CGGalleryController : MonoBehaviour
{
    [Header("UI組件")]
    public CGGalleryUI galleryUI;
    public CGThumbnailGrid thumbnailGrid;
    public CategoryManager categoryManager;
    
    [Header("播放器")]
    public CGFullscreenViewer cgViewer;
    public NavigationController navigationController;
    
    [Header("數據引用")]
    public CGUnlockManager unlockManager;
    public CGDatabase cgDatabase;
    
    private string currentCategory = "story";
    private List<string> currentCGList;
    
    void Start()
    {
        InitializeGallery();
    }
    
    // 🚀 初始化大廳
    void InitializeGallery()
    {
        // 訂閱事件
        EventBus.Instance.Subscribe<CategoryChangeData>("CategoryChanged", OnCategoryChanged);
        EventBus.Instance.Subscribe<CGSelectData>("CGSelected", OnCGSelected);
        
        // 初始化UI
        categoryManager.Initialize(GetAvailableCategories());
        
        // 載入預設分類
        LoadCategory(currentCategory);
        
        // 顯示統計信息
        UpdateProgressDisplay();
    }
    
    // 📂 載入分類
    void LoadCategory(string category)
    {
        currentCategory = category;
        
        // 獲取該分類的所有CG
        List<string> allCGsInCategory = cgDatabase.GetCGsByCategory(category);
        
        // 獲取已解鎖的CG
        List<string> unlockedCGs = unlockManager.GetUnlockedCGsByCategory(category);
        
        // 構建顯示列表（包含鎖定狀態）
        currentCGList = allCGsInCategory;
        
        // 更新縮圖網格
        thumbnailGrid.BuildGrid(currentCGList, unlockedCGs);
        
        // 更新分類標題
        galleryUI.UpdateCategoryTitle(category, unlockedCGs.Count, allCGsInCategory.Count);
    }
    
    // 🔄 分類切換處理
    void OnCategoryChanged(CategoryChangeData data)
    {
        LoadCategory(data.newCategory);
    }
    
    // ▶️ CG選擇處理
    void OnCGSelected(CGSelectData data)
    {
        if (unlockManager.IsCGUnlocked(data.cgID))
        {
            // 開啟全螢幕播放器
            cgViewer.PlayCG(data.cgID);
            
            // 設定導航上下文
            navigationController.SetNavigationContext(currentCGList, data.cgID);
        }
        else
        {
            // 顯示鎖定提示
            galleryUI.ShowLockedCGMessage(data.cgID);
        }
    }
    
    // 📊 更新進度顯示
    void UpdateProgressDisplay()
    {
        CGUnlockData unlockData = unlockManager.GetUnlockData();
        galleryUI.UpdateProgressDisplay(unlockData.totalUnlocked, unlockData.totalAvailable);
    }
    
    // 🚪 返回主選單
    public void ReturnToMainMenu()
    {
        // 清理資源
        cgViewer.CleanupResources();
        
        // 載入主選單場景
        SceneManager.LoadScene("MainMenu");
    }
}
```

### 🎬 CGFullscreenViewer 全螢幕播放器
```csharp
public class CGFullscreenViewer : MonoBehaviour
{
    [Header("顯示組件")]
    public Canvas fullscreenCanvas;
    public Image cgDisplayImage;
    public SkeletonGraphic spineDisplay;
    
    [Header("控制UI")]
    public GameObject controlPanel;
    public Button previousButton;
    public Button nextButton;
    public Button closeButton;
    public Button pauseButton;
    
    [Header("設定")]
    public float fadeTransitionTime = 0.5f;
    public KeyCode[] closeKeys = { KeyCode.Escape, KeyCode.Backspace };
    public KeyCode previousKey = KeyCode.LeftArrow;
    public KeyCode nextKey = KeyCode.RightArrow;
    public KeyCode pauseKey = KeyCode.Space;
    
    private NavigationController navigationController;
    private CGDatabase cgDatabase;
    private string currentCGID;
    private bool isSpineContent = false;
    private bool isControlPanelVisible = true;
    
    void Awake()
    {
        navigationController = FindObjectOfType<NavigationController>();
        cgDatabase = FindObjectOfType<CGDatabase>();
        
        // 綁定按鈕事件
        BindButtonEvents();
        
        // 初始狀態設定
        fullscreenCanvas.gameObject.SetActive(false);
    }
    
    void Update()
    {
        HandleKeyboardInput();
        HandleControlPanelVisibility();
    }
    
    // 🎯 播放CG
    public void PlayCG(string cgID)
    {
        currentCGID = cgID;
        CGData cgData = cgDatabase.GetCGData(cgID);
        
        if (cgData != null)
        {
            // 顯示全螢幕畫布
            fullscreenCanvas.gameObject.SetActive(true);
            
            // 根據內容類型播放
            if (cgData.isSpineContent)
            {
                PlaySpineCG(cgData);
            }
            else
            {
                PlayStaticCG(cgData);
            }
            
            // 更新控制面板狀態
            UpdateControlPanel();
            
            // 淡入效果
            StartCoroutine(FadeInCG());
        }
    }
    
    // 🎪 播放Spine動畫CG
    void PlaySpineCG(CGData cgData)
    {
        isSpineContent = true;
        
        // 隱藏靜態圖片
        cgDisplayImage.gameObject.SetActive(false);
        
        // 顯示Spine動畫
        spineDisplay.gameObject.SetActive(true);
        spineDisplay.skeletonDataAsset = cgData.spineDataAsset;
        spineDisplay.Initialize(true);
        
        // 播放預設動畫
        if (!string.IsNullOrEmpty(cgData.defaultAnimation))
        {
            spineDisplay.AnimationState.SetAnimation(0, cgData.defaultAnimation, cgData.loopAnimation);
        }
    }
    
    // 🖼️ 播放靜態CG
    void PlayStaticCG(CGData cgData)
    {
        isSpineContent = false;
        
        // 隱藏Spine動畫
        spineDisplay.gameObject.SetActive(false);
        
        // 顯示靜態圖片
        cgDisplayImage.gameObject.SetActive(true);
        cgDisplayImage.sprite = cgData.cgSprite;
    }
    
    // ⌨️ 鍵盤輸入處理
    void HandleKeyboardInput()
    {
        // 關閉按鍵
        foreach (KeyCode key in closeKeys)
        {
            if (Input.GetKeyDown(key))
            {
                CloseCG();
                return;
            }
        }
        
        // 導航按鍵
        if (Input.GetKeyDown(previousKey))
        {
            ShowPreviousCG();
        }
        else if (Input.GetKeyDown(nextKey))
        {
            ShowNextCG();
        }
        
        // 暫停按鍵（僅Spine內容）
        if (Input.GetKeyDown(pauseKey) && isSpineContent)
        {
            ToggleSpineAnimation();
        }
    }
    
    // 🎮 控制面板可見性
    void HandleControlPanelVisibility()
    {
        // 滑鼠不動一段時間後隱藏控制面板
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            ShowControlPanel();
            CancelInvoke("HideControlPanel");
            Invoke("HideControlPanel", 3.0f);
        }
    }
    
    // ⬅️ 上一張CG
    public void ShowPreviousCG()
    {
        string previousCGID = navigationController.GetPreviousCG();
        if (!string.IsNullOrEmpty(previousCGID))
        {
            PlayCG(previousCGID);
        }
    }
    
    // ➡️ 下一張CG
    public void ShowNextCG()
    {
        string nextCGID = navigationController.GetNextCG();
        if (!string.IsNullOrEmpty(nextCGID))
        {
            PlayCG(nextCGID);
        }
    }
    
    // ⏸️ 切換Spine動畫播放
    public void ToggleSpineAnimation()
    {
        if (isSpineContent && spineDisplay.AnimationState != null)
        {
            if (spineDisplay.AnimationState.TimeScale > 0)
            {
                spineDisplay.AnimationState.TimeScale = 0;
                pauseButton.GetComponentInChildren<Text>().text = "▶";
            }
            else
            {
                spineDisplay.AnimationState.TimeScale = 1;
                pauseButton.GetComponentInChildren<Text>().text = "⏸";
            }
        }
    }
    
    // ❌ 關閉CG播放器
    public void CloseCG()
    {
        StartCoroutine(FadeOutAndClose());
    }
    
    // 🌊 淡出並關閉
    IEnumerator FadeOutAndClose()
    {
        // 淡出效果
        CanvasGroup canvasGroup = fullscreenCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = fullscreenCanvas.gameObject.AddComponent<CanvasGroup>();
        }
        
        float elapsedTime = 0;
        while (elapsedTime < fadeTransitionTime)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = 1 - (elapsedTime / fadeTransitionTime);
            yield return null;
        }
        
        // 清理並隱藏
        CleanupCurrentCG();
        fullscreenCanvas.gameObject.SetActive(false);
        canvasGroup.alpha = 1;
    }
    
    // 🧹 清理當前CG資源
    void CleanupCurrentCG()
    {
        if (isSpineContent && spineDisplay.AnimationState != null)
        {
            spineDisplay.AnimationState.ClearTracks();
        }
        
        currentCGID = null;
        isSpineContent = false;
    }
    
    // 🧹 清理所有資源
    public void CleanupResources()
    {
        CleanupCurrentCG();
        
        // 清理其他可能的資源引用
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
```

---

## 🎨 展示層架構

### 🖼️ CGThumbnailGrid 縮圖網格系統
```csharp
public class CGThumbnailGrid : MonoBehaviour
{
    [Header("網格設定")]
    public GridLayoutGroup gridLayout;
    public RectTransform gridContainer;
    public GameObject thumbnailPrefab;
    
    [Header("縮圖設定")]
    public Vector2 thumbnailSize = new Vector2(200, 150);
    public float spacing = 10f;
    public int columnsPerRow = 4;
    
    [Header("視覺效果")]
    public Sprite lockedThumbnailSprite;
    public Color lockedThumbnailColor = Color.gray;
    public GameObject newUnlockIndicator;
    
    private List<CGThumbnailItem> thumbnailItems = new List<CGThumbnailItem>();
    private CGDatabase cgDatabase;
    
    void Awake()
    {
        cgDatabase = FindObjectOfType<CGDatabase>();
        ConfigureGridLayout();
    }
    
    // ⚙️ 配置網格佈局
    void ConfigureGridLayout()
    {
        gridLayout.cellSize = thumbnailSize;
        gridLayout.spacing = Vector2.one * spacing;
        gridLayout.constraintCount = columnsPerRow;
    }
    
    // 🏗️ 構建縮圖網格
    public void BuildGrid(List<string> allCGs, List<string> unlockedCGs)
    {
        // 清理現有項目
        ClearGrid();
        
        // 創建縮圖項目
        foreach (string cgID in allCGs)
        {
            bool isUnlocked = unlockedCGs.Contains(cgID);
            CreateThumbnailItem(cgID, isUnlocked);
        }
        
        // 調整容器大小
        AdjustContainerSize(allCGs.Count);
    }
    
    // 🖼️ 創建單個縮圖項目
    void CreateThumbnailItem(string cgID, bool isUnlocked)
    {
        GameObject thumbnailObj = Instantiate(thumbnailPrefab, gridContainer);
        CGThumbnailItem thumbnailItem = thumbnailObj.GetComponent<CGThumbnailItem>();
        
        if (thumbnailItem != null)
        {
            // 設定縮圖數據
            CGData cgData = cgDatabase.GetCGData(cgID);
            thumbnailItem.Initialize(cgID, cgData, isUnlocked);
            
            // 設定視覺狀態
            if (isUnlocked)
            {
                SetUnlockedThumbnail(thumbnailItem, cgData);
            }
            else
            {
                SetLockedThumbnail(thumbnailItem);
            }
            
            thumbnailItems.Add(thumbnailItem);
        }
    }
    
    // ✅ 設定已解鎖縮圖
    void SetUnlockedThumbnail(CGThumbnailItem item, CGData cgData)
    {
        item.SetThumbnailImage(cgData.thumbnailSprite);
        item.SetInteractable(true);
        item.SetTitleText(cgData.title);
        
        // 檢查是否為新解鎖
        if (IsNewlyUnlocked(item.cgID))
        {
            item.ShowNewIndicator(true);
        }
    }
    
    // 🔒 設定鎖定縮圖
    void SetLockedThumbnail(CGThumbnailItem item)
    {
        item.SetThumbnailImage(lockedThumbnailSprite);
        item.SetImageColor(lockedThumbnailColor);
        item.SetInteractable(false);
        item.SetTitleText("???");
    }
    
    // 🆕 檢查是否為新解鎖
    bool IsNewlyUnlocked(string cgID)
    {
        // 檢查解鎖時間是否在最近24小時內
        CGUnlockManager unlockManager = CGUnlockManager.Instance;
        CGUnlockData unlockData = unlockManager.GetUnlockData();
        
        if (unlockData.unlockDetails.ContainsKey(cgID))
        {
            string unlockTimeStr = unlockData.unlockDetails[cgID].unlockTime;
            if (System.DateTime.TryParse(unlockTimeStr, out System.DateTime unlockTime))
            {
                return (System.DateTime.Now - unlockTime).TotalHours < 24;
            }
        }
        
        return false;
    }
    
    // 📏 調整容器大小
    void AdjustContainerSize(int itemCount)
    {
        int rows = Mathf.CeilToInt((float)itemCount / columnsPerRow);
        float containerHeight = rows * (thumbnailSize.y + spacing) + spacing;
        
        gridContainer.sizeDelta = new Vector2(gridContainer.sizeDelta.x, containerHeight);
    }
    
    // 🧹 清理網格
    void ClearGrid()
    {
        foreach (CGThumbnailItem item in thumbnailItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        thumbnailItems.Clear();
    }
}
```

### 🔲 CGThumbnailItem 縮圖項目組件
```csharp
public class CGThumbnailItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI組件")]
    public Image thumbnailImage;
    public Text titleText;
    public Button selectButton;
    public GameObject newIndicator;
    public GameObject lockIcon;
    
    [Header("動效設定")]
    public float hoverScale = 1.05f;
    public float animationDuration = 0.2f;
    
    public string cgID { get; private set; }
    private CGData cgData;
    private bool isUnlocked;
    private Vector3 originalScale;
    
    void Awake()
    {
        originalScale = transform.localScale;
    }
    
    // 🚀 初始化縮圖項目
    public void Initialize(string id, CGData data, bool unlocked)
    {
        cgID = id;
        cgData = data;
        isUnlocked = unlocked;
        
        UpdateVisualState();
    }
    
    // 🎨 更新視覺狀態
    void UpdateVisualState()
    {
        lockIcon.SetActive(!isUnlocked);
        selectButton.interactable = isUnlocked;
    }
    
    // 🖼️ 設定縮圖圖片
    public void SetThumbnailImage(Sprite sprite)
    {
        if (thumbnailImage != null && sprite != null)
        {
            thumbnailImage.sprite = sprite;
        }
    }
    
    // 🎨 設定圖片顏色
    public void SetImageColor(Color color)
    {
        if (thumbnailImage != null)
        {
            thumbnailImage.color = color;
        }
    }
    
    // 📝 設定標題文字
    public void SetTitleText(string title)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }
    }
    
    // 🔗 設定可交互狀態
    public void SetInteractable(bool interactable)
    {
        if (selectButton != null)
        {
            selectButton.interactable = interactable;
        }
    }
    
    // 🆕 顯示新解鎖指示器
    public void ShowNewIndicator(bool show)
    {
        if (newIndicator != null)
        {
            newIndicator.SetActive(show);
        }
    }
    
    #region UI事件處理
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isUnlocked)
        {
            // 觸發CG選擇事件
            EventBus.Instance.Publish("CGSelected", new CGSelectData { cgID = cgID });
            
            // 隱藏新解鎖指示器
            ShowNewIndicator(false);
        }
        else
        {
            // 顯示鎖定提示
            EventBus.Instance.Publish("CGLocked", new CGLockedData { cgID = cgID });
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isUnlocked)
        {
            // Hover放大效果
            transform.DOScale(originalScale * hoverScale, animationDuration)
                     .SetEase(Ease.OutBack);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        // 恢復原始大小
        transform.DOScale(originalScale, animationDuration)
                 .SetEase(Ease.OutBack);
    }
    
    #endregion
}
```

---

## 🔗 整合層架構

### 🏠 MainMenuIntegration 主選單整合
```csharp
public class MainMenuIntegration : MonoBehaviour
{
    [Header("主選單按鈕")]
    public Button cgGalleryButton;
    
    [Header("解鎖檢測")]
    public ProgressDetector progressDetector;
    
    void Start()
    {
        InitializeIntegration();
    }
    
    void InitializeIntegration()
    {
        // 檢查CG大廳解鎖狀態
        bool isCGGalleryUnlocked = progressDetector.IsGameCompleted();
        
        // 設定按鈕狀態
        cgGalleryButton.interactable = isCGGalleryUnlocked;
        cgGalleryButton.GetComponent<Image>().color = isCGGalleryUnlocked ? Color.white : Color.gray;
        
        if (isCGGalleryUnlocked)
        {
            cgGalleryButton.onClick.AddListener(OpenCGGallery);
        }
    }
    
    void OpenCGGallery()
    {
        SceneManager.LoadScene("CGGallery");
    }
}
```

### 🎭 StorySystemIntegration 劇情系統整合
```csharp
public class StorySystemIntegration : MonoBehaviour
{
    void Start()
    {
        // 訂閱劇情系統事件
        EventBus.Instance.Subscribe<DialogCompleteData>("DialogComplete", OnDialogComplete);
        EventBus.Instance.Subscribe<StorySceneCompleteData>("StorySceneComplete", OnStorySceneComplete);
    }
    
    void OnDialogComplete(DialogCompleteData data)
    {
        // 檢查是否需要解鎖CG
        string eventID = GenerateEventID(data);
        CGUnlockManager.Instance.ProcessEventUnlock(eventID);
    }
    
    void OnStorySceneComplete(StorySceneCompleteData data)
    {
        // 重要劇情節點完成，觸發CG解鎖檢查
        string eventID = $"story_complete_{data.sceneID}";
        CGUnlockManager.Instance.ProcessEventUnlock(eventID);
    }
    
    string GenerateEventID(DialogCompleteData data)
    {
        // 根據對話數據生成事件ID
        return $"dialog_{data.dialogID}_{data.choiceResult}";
    }
}
```

---

## 🎵 音效與視覺效果

### 🔊 CGGalleryAudio 音效管理
```csharp
public class CGGalleryAudio : MonoBehaviour
{
    [Header("背景音樂")]
    public AudioClip galleryBGM;
    public AudioSource bgmSource;
    
    [Header("UI音效")]
    public AudioClip thumbnailClickSound;
    public AudioClip categoryChangeSound;
    public AudioClip cgUnlockSound;
    public AudioSource uiSoundSource;
    
    void Start()
    {
        InitializeAudio();
        SubscribeToEvents();
    }
    
    void InitializeAudio()
    {
        // 播放大廳背景音樂
        if (galleryBGM != null && bgmSource != null)
        {
            bgmSource.clip = galleryBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }
    
    void SubscribeToEvents()
    {
        EventBus.Instance.Subscribe<CGSelectData>("CGSelected", OnCGSelected);
        EventBus.Instance.Subscribe<CategoryChangeData>("CategoryChanged", OnCategoryChanged);
        EventBus.Instance.Subscribe<CGUnlockEventData>("CGUnlocked", OnCGUnlocked);
    }
    
    void OnCGSelected(CGSelectData data)
    {
        PlayUISound(thumbnailClickSound);
    }
    
    void OnCategoryChanged(CategoryChangeData data)
    {
        PlayUISound(categoryChangeSound);
    }
    
    void OnCGUnlocked(CGUnlockEventData data)
    {
        PlayUISound(cgUnlockSound);
    }
    
    void PlayUISound(AudioClip clip)
    {
        if (clip != null && uiSoundSource != null)
        {
            uiSoundSource.PlayOneShot(clip);
        }
    }
}
```

---

## 💬 Claude 使用提示

### 🎯 架構重點
1. **模組化設計**: 各個組件職責清晰，便於維護和擴展
2. **事件驅動**: 使用EventBus實現組件間的解耦通信
3. **JSON驅動**: 解鎖數據完全由JSON管理，支援熱更新
4. **資源管理**: 適當的資源載入和清理機制

### 🔧 開發建議
- 優先實作CGUnlockManager和基礎數據結構
- 確保事件系統的穩定性和錯誤處理
- 注重UI響應性和用戶體驗
- 考慮大量CG資源的性能優化

### ⚠️ 注意事項
- JSON文件的版本兼容性處理
- 縮圖加載的性能影響
- 全螢幕播放器的記憶體管理
- 跨平台的檔案路徑處理

---

**最後更新**: 2025-07-30  
**版本**: 1.0  
**維護者**: 開發團隊 + Claude AI

> 🏛️ **架構亮點**: CG觀賞大廳架構採用分層設計和事件驅動模式，實現了高度模組化和可擴展的系統。通過JSON驅動的解鎖機制和精心設計的UI架構，為玩家提供了流暢且豐富的內容瀏覽體驗！ ✨