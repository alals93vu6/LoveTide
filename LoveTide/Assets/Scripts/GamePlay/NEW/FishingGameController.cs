using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 釣魚小遊戲控制器
/// 
/// 職責:
/// 1. 管理釣魚小遊戲的完整流程
/// 2. 處理釣魚機制和玩家互動
/// 3. 與貓咪餵食系統連動
/// 4. 提供釣魚成果和獎勵
/// 
/// 基於架構文檔: GameMechanics/小遊戲_釣魚.md
/// 實現釣魚→餵食→嚕貓的完整循環
/// </summary>
public class FishingGameController : MonoBehaviour
{
    [Header("== 釣魚遊戲配置 ==")]
    [SerializeField] private FishingGameConfig gameConfig;
    [SerializeField] private Transform fishingArea;
    [SerializeField] private Transform hookTransform;
    
    [Header("== UI組件 ==")]
    [SerializeField] private Canvas fishingCanvas;
    [SerializeField] private Slider tensionSlider;
    [SerializeField] private Button castButton;
    [SerializeField] private Button reelButton;
    [SerializeField] private Text instructionText;
    [SerializeField] private Text resultText;
    
    [Header("== 魚類配置 ==")]
    [SerializeField] private FishData[] availableFish;
    [SerializeField] private Transform fishContainer;
    
    [Header("== 當前狀態 ==")]
    [SerializeField] private FishingState currentState = FishingState.Idle;
    [SerializeField] private bool isInitialized = false;
    [SerializeField] private bool isPlaying = false;
    
    // 釣魚事件
    public UnityEvent OnFishingStarted;
    public UnityEvent<FishData> OnFishCaught;
    public UnityEvent OnFishingFailed;
    public UnityEvent OnFishingEnded;
    
    // 釣魚機制變數
    private float currentTension = 0f;
    private float fishingTimer = 0f;
    private float biteTimer = 0f;
    private bool hasBite = false;
    private FishData currentTargetFish;
    
    // 魚群系統
    private List<GameObject> spawnedFish = new List<GameObject>();
    private Dictionary<FishData, int> caughtFishCount = new Dictionary<FishData, int>();
    
    // 系統引用
    private NumericalRecords numericalRecords;
    private InteractionManager interactionManager;
    
    public bool IsInitialized => isInitialized;
    public bool IsPlaying => isPlaying;
    public FishingState CurrentState => currentState;
    
    /// <summary>
    /// 初始化釣魚遊戲控制器
    /// </summary>
    public void Initialize()
    {
        if (isInitialized) return;
        
        Debug.Log("[FishingGameController] 初始化釣魚遊戲控制器");
        
        // 查找系統引用
        FindSystemReferences();
        
        // 設置UI組件
        SetupUIComponents();
        
        // 初始化遊戲配置
        InitializeGameConfig();
        
        // 設置魚群
        SetupFishSpawning();
        
        // 設置初始狀態
        SetupInitialState();
        
        isInitialized = true;
        Debug.Log("[FishingGameController] 釣魚遊戲控制器初始化完成");
    }
    
    void Update()
    {
        if (isPlaying)
        {
            UpdateFishingGame();
        }
    }
    
    /// <summary>
    /// 查找系統引用
    /// </summary>
    private void FindSystemReferences()
    {
        numericalRecords = FindObjectOfType<NumericalRecords>();
        interactionManager = FindObjectOfType<InteractionManager>();
    }
    
    /// <summary>
    /// 設置UI組件
    /// </summary>
    private void SetupUIComponents()
    {
        // 查找Canvas
        if (fishingCanvas == null)
        {
            fishingCanvas = GetComponentInChildren<Canvas>();
        }
        
        // 設置按鈕事件
        if (castButton != null)
        {
            castButton.onClick.AddListener(StartFishing);
        }
        
        if (reelButton != null)
        {
            reelButton.onClick.AddListener(ReelIn);
        }
        
        // 設置初始UI狀態
        UpdateUI();
    }
    
    /// <summary>
    /// 初始化遊戲配置
    /// </summary>
    private void InitializeGameConfig()
    {
        if (gameConfig == null)
        {
            gameConfig = ScriptableObject.CreateInstance<FishingGameConfig>();
            SetupDefaultConfig();
        }
        
        // 初始化釣獲統計
        caughtFishCount.Clear();
        foreach (var fish in availableFish)
        {
            caughtFishCount[fish] = 0;
        }
    }
    
    /// <summary>
    /// 設置默認配置
    /// </summary>
    private void SetupDefaultConfig()
    {
        gameConfig.maxTension = 100f;
        gameConfig.tensionIncreaseRate = 20f;
        gameConfig.tensionDecreaseRate = 10f;
        gameConfig.biteChance = 0.3f;
        gameConfig.maxFishingTime = 30f;
        gameConfig.minBiteTime = 2f;
        gameConfig.maxBiteTime = 8f;
    }
    
    /// <summary>
    /// 設置魚群生成
    /// </summary>
    private void SetupFishSpawning()
    {
        if (availableFish == null || availableFish.Length == 0)
        {
            CreateDefaultFishData();
        }
        
        SpawnFishInArea();
    }
    
    /// <summary>
    /// 創建默認魚類數據
    /// </summary>
    private void CreateDefaultFishData()
    {
        availableFish = new FishData[]
        {
            new FishData
            {
                fishName = "小魚",
                rarity = FishRarity.Common,
                catchDifficulty = 1,
                tensionResistance = 5f,
                points = 10
            },
            new FishData
            {
                fishName = "中魚",
                rarity = FishRarity.Uncommon,
                catchDifficulty = 2,
                tensionResistance = 15f,
                points = 25
            },
            new FishData
            {
                fishName = "大魚",
                rarity = FishRarity.Rare,
                catchDifficulty = 3,
                tensionResistance = 30f,
                points = 50
            }
        };
    }
    
    /// <summary>
    /// 在區域內生成魚
    /// </summary>
    private void SpawnFishInArea()
    {
        // 清除現有魚群
        ClearSpawnedFish();
        
        // 生成新魚群
        int fishCount = Random.Range(3, 8);
        for (int i = 0; i < fishCount; i++)
        {
            SpawnRandomFish();
        }
    }
    
    /// <summary>
    /// 生成隨機魚
    /// </summary>
    private void SpawnRandomFish()
    {
        if (fishContainer == null || availableFish.Length == 0)
            return;
            
        // 根據稀有度選擇魚類
        FishData fishToSpawn = SelectFishByRarity();
        
        // 創建魚物件
        GameObject fishObj = new GameObject($"Fish_{fishToSpawn.fishName}_{spawnedFish.Count}");
        fishObj.transform.SetParent(fishContainer);
        
        // 設置隨機位置
        Vector3 randomPos = GetRandomPositionInArea();
        fishObj.transform.position = randomPos;
        
        // 添加魚組件
        FishBehaviour fishBehaviour = fishObj.AddComponent<FishBehaviour>();
        fishBehaviour.Initialize(fishToSpawn);
        
        spawnedFish.Add(fishObj);
    }
    
    /// <summary>
    /// 根據稀有度選擇魚類
    /// </summary>
    private FishData SelectFishByRarity()
    {
        float randomValue = Random.Range(0f, 1f);
        
        // 稀有度機率: 常見60%, 不常見30%, 稀有10%
        if (randomValue < 0.6f)
        {
            return GetFishByRarity(FishRarity.Common);
        }
        else if (randomValue < 0.9f)
        {
            return GetFishByRarity(FishRarity.Uncommon);
        }
        else
        {
            return GetFishByRarity(FishRarity.Rare);
        }
    }
    
    /// <summary>
    /// 根據稀有度獲取魚類
    /// </summary>
    private FishData GetFishByRarity(FishRarity rarity)
    {
        List<FishData> fishOfRarity = new List<FishData>();
        
        foreach (var fish in availableFish)
        {
            if (fish.rarity == rarity)
            {
                fishOfRarity.Add(fish);
            }
        }
        
        if (fishOfRarity.Count > 0)
        {
            return fishOfRarity[Random.Range(0, fishOfRarity.Count)];
        }
        
        // 如果沒有該稀有度的魚，返回第一個可用魚類
        return availableFish[0];
    }
    
    /// <summary>
    /// 獲取區域內隨機位置
    /// </summary>
    private Vector3 GetRandomPositionInArea()
    {
        if (fishingArea != null)
        {
            Bounds areaBounds = fishingArea.GetComponent<Collider>()?.bounds ?? new Bounds(fishingArea.position, Vector3.one * 5);
            
            return new Vector3(
                Random.Range(areaBounds.min.x, areaBounds.max.x),
                areaBounds.center.y,
                Random.Range(areaBounds.min.z, areaBounds.max.z)
            );
        }
        
        return Vector3.zero;
    }
    
    /// <summary>
    /// 設置初始狀態
    /// </summary>
    private void SetupInitialState()
    {
        currentState = FishingState.Idle;
        currentTension = 0f;
        fishingTimer = 0f;
        biteTimer = 0f;
        hasBite = false;
        isPlaying = false;
        
        // 隱藏釣魚界面
        if (fishingCanvas != null)
        {
            fishingCanvas.gameObject.SetActive(false);
        }
    }
    
    #region 釣魚遊戲邏輯
    
    /// <summary>
    /// 開始釣魚
    /// </summary>
    public void StartFishing()
    {
        if (isPlaying) return;
        
        Debug.Log("[FishingGameController] 開始釣魚");
        
        isPlaying = true;
        currentState = FishingState.Casting;
        fishingTimer = 0f;
        biteTimer = Random.Range(gameConfig.minBiteTime, gameConfig.maxBiteTime);
        hasBite = false;
        currentTension = 0f;
        
        // 顯示釣魚界面
        if (fishingCanvas != null)
        {
            fishingCanvas.gameObject.SetActive(true);
        }
        
        // 選擇目標魚
        SelectTargetFish();
        
        // 更新UI
        UpdateUI();
        
        OnFishingStarted?.Invoke();
    }
    
    /// <summary>
    /// 選擇目標魚
    /// </summary>
    private void SelectTargetFish()
    {
        if (spawnedFish.Count > 0)
        {
            GameObject targetFishObj = spawnedFish[Random.Range(0, spawnedFish.Count)];
            FishBehaviour fishBehaviour = targetFishObj.GetComponent<FishBehaviour>();
            
            if (fishBehaviour != null)
            {
                currentTargetFish = fishBehaviour.FishData;
            }
        }
        
        if (currentTargetFish == null && availableFish.Length > 0)
        {
            currentTargetFish = availableFish[Random.Range(0, availableFish.Length)];
        }
    }
    
    /// <summary>
    /// 更新釣魚遊戲
    /// </summary>
    private void UpdateFishingGame()
    {
        fishingTimer += Time.deltaTime;
        
        switch (currentState)
        {
            case FishingState.Casting:
                UpdateCasting();
                break;
                
            case FishingState.Waiting:
                UpdateWaiting();
                break;
                
            case FishingState.Hooked:
                UpdateHooked();
                break;
                
            case FishingState.Reeling:
                UpdateReeling();
                break;
        }
        
        // 檢查時間限制
        if (fishingTimer >= gameConfig.maxFishingTime)
        {
            EndFishing(false);
        }
        
        UpdateUI();
    }
    
    /// <summary>
    /// 更新拋竿狀態
    /// </summary>
    private void UpdateCasting()
    {
        // 拋竿動畫時間
        if (fishingTimer >= 1f)
        {
            currentState = FishingState.Waiting;
            biteTimer = Random.Range(gameConfig.minBiteTime, gameConfig.maxBiteTime);
        }
    }
    
    /// <summary>
    /// 更新等待狀態
    /// </summary>
    private void UpdateWaiting()
    {
        biteTimer -= Time.deltaTime;
        
        if (biteTimer <= 0f)
        {
            // 檢查是否有魚上鉤
            if (Random.Range(0f, 1f) < gameConfig.biteChance)
            {
                currentState = FishingState.Hooked;
                hasBite = true;
                Debug.Log("[FishingGameController] 魚上鉤了！");
            }
            else
            {
                // 重新等待
                biteTimer = Random.Range(gameConfig.minBiteTime, gameConfig.maxBiteTime);
            }
        }
    }
    
    /// <summary>
    /// 更新上鉤狀態
    /// </summary>
    private void UpdateHooked()
    {
        // 魚會增加張力
        if (currentTargetFish != null)
        {
            currentTension += currentTargetFish.tensionResistance * Time.deltaTime;
        }
        
        // 檢查張力是否過高
        if (currentTension >= gameConfig.maxTension)
        {
            EndFishing(false);
        }
    }
    
    /// <summary>
    /// 更新收線狀態
    /// </summary>
    private void UpdateReeling()
    {
        // 玩家收線減少張力
        currentTension -= gameConfig.tensionDecreaseRate * Time.deltaTime;
        currentTension = Mathf.Max(0f, currentTension);
        
        // 檢查是否成功釣到魚
        if (currentTension <= 0f)
        {
            EndFishing(true);
        }
    }
    
    /// <summary>
    /// 收線
    /// </summary>
    public void ReelIn()
    {
        if (currentState == FishingState.Hooked)
        {
            currentState = FishingState.Reeling;
            Debug.Log("[FishingGameController] 開始收線");
        }
    }
    
    /// <summary>
    /// 結束釣魚
    /// </summary>
    private void EndFishing(bool success)
    {
        isPlaying = false;
        
        if (success && currentTargetFish != null)
        {
            // 成功釣到魚
            Debug.Log($"[FishingGameController] 成功釣到: {currentTargetFish.fishName}");
            
            // 更新統計
            caughtFishCount[currentTargetFish]++;
            
            // 更新數值系統
            if (numericalRecords != null)
            {
                // numericalRecords.AddFish(currentTargetFish.fishName);
                // numericalRecords.AddExperience(currentTargetFish.points);
            }
            
            OnFishCaught?.Invoke(currentTargetFish);
        }
        else
        {
            // 釣魚失敗
            Debug.Log("[FishingGameController] 釣魚失敗");
            OnFishingFailed?.Invoke();
        }
        
        // 重置狀態
        currentState = FishingState.Idle;
        currentTargetFish = null;
        hasBite = false;
        
        // 隱藏UI
        if (fishingCanvas != null)
        {
            fishingCanvas.gameObject.SetActive(false);
        }
        
        OnFishingEnded?.Invoke();
        
        // 重新生成魚群
        StartCoroutine(RespawnFishAfterDelay(3f));
    }
    
    /// <summary>
    /// 延遲重新生成魚群
    /// </summary>
    private IEnumerator RespawnFishAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnFishInArea();
    }
    
    #endregion
    
    #region UI更新
    
    /// <summary>
    /// 更新UI
    /// </summary>
    private void UpdateUI()
    {
        // 更新張力條
        if (tensionSlider != null)
        {
            tensionSlider.value = currentTension / gameConfig.maxTension;
        }
        
        // 更新指示文字
        if (instructionText != null)
        {
            instructionText.text = GetStateInstruction();
        }
        
        // 更新按鈕狀態
        UpdateButtonStates();
    }
    
    /// <summary>
    /// 獲取狀態指示
    /// </summary>
    private string GetStateInstruction()
    {
        switch (currentState)
        {
            case FishingState.Idle:
                return "點擊拋竿開始釣魚";
                
            case FishingState.Casting:
                return "拋竿中...";
                
            case FishingState.Waiting:
                return "等待魚上鉤...";
                
            case FishingState.Hooked:
                return "魚上鉤了！點擊收線！";
                
            case FishingState.Reeling:
                return "收線中，控制張力！";
                
            default:
                return "";
        }
    }
    
    /// <summary>
    /// 更新按鈕狀態
    /// </summary>
    private void UpdateButtonStates()
    {
        if (castButton != null)
        {
            castButton.interactable = (currentState == FishingState.Idle);
        }
        
        if (reelButton != null)
        {
            reelButton.interactable = (currentState == FishingState.Hooked || currentState == FishingState.Reeling);
        }
    }
    
    #endregion
    
    #region 清理和工具方法
    
    /// <summary>
    /// 清除生成的魚
    /// </summary>
    private void ClearSpawnedFish()
    {
        foreach (GameObject fish in spawnedFish)
        {
            if (fish != null)
            {
                Destroy(fish);
            }
        }
        spawnedFish.Clear();
    }
    
    /// <summary>
    /// 獲取釣魚統計
    /// </summary>
    public Dictionary<FishData, int> GetFishingStats()
    {
        return new Dictionary<FishData, int>(caughtFishCount);
    }
    
    /// <summary>
    /// 重置釣魚統計
    /// </summary>
    public void ResetFishingStats()
    {
        caughtFishCount.Clear();
        foreach (var fish in availableFish)
        {
            caughtFishCount[fish] = 0;
        }
    }
    
    #endregion
    
    void OnDestroy()
    {
        ClearSpawnedFish();
    }
}

/// <summary>
/// 釣魚狀態
/// </summary>
public enum FishingState
{
    Idle,       // 閒置
    Casting,    // 拋竿
    Waiting,    // 等待
    Hooked,     // 上鉤
    Reeling     // 收線
}

/// <summary>
/// 魚類稀有度
/// </summary>
public enum FishRarity
{
    Common,     // 常見
    Uncommon,   // 不常見
    Rare,       // 稀有
    Legendary   // 傳說
}

/// <summary>
/// 魚類數據
/// </summary>
[System.Serializable]
public class FishData
{
    public string fishName;
    public FishRarity rarity;
    public int catchDifficulty;
    public float tensionResistance;
    public int points;
    public Sprite fishSprite;
}

/// <summary>
/// 釣魚遊戲配置
/// </summary>
[CreateAssetMenu(fileName = "FishingGameConfig", menuName = "LoveTide/FishingGameConfig")]
public class FishingGameConfig : ScriptableObject
{
    [Header("張力系統")]
    public float maxTension = 100f;
    public float tensionIncreaseRate = 20f;
    public float tensionDecreaseRate = 10f;
    
    [Header("釣魚機制")]
    public float biteChance = 0.3f;
    public float maxFishingTime = 30f;
    public float minBiteTime = 2f;
    public float maxBiteTime = 8f;
}

/// <summary>
/// 魚類行為組件
/// </summary>
public class FishBehaviour : MonoBehaviour
{
    private FishData fishData;
    
    public FishData FishData => fishData;
    
    public void Initialize(FishData data)
    {
        fishData = data;
        name = $"Fish_{fishData.fishName}";
    }
}