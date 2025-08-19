using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 餐廳小遊戲控制器
/// 
/// 職責:
/// 1. 管理餐廳經營小遊戲的完整流程
/// 2. 處理客人訂單和服務機制
/// 3. 管理收入和經驗值獎勵
/// 4. 與工作系統協作
/// 
/// 基於架構文檔: GameMechanics/小遊戲_餐廳.md
/// 實現餐廳經營的互動式小遊戲
/// </summary>
public class RestaurantGameController : MonoBehaviour
{
    [Header("== 餐廳遊戲配置 ==")]
    [SerializeField] private RestaurantGameConfig gameConfig;
    [SerializeField] private Transform customerSpawnPoint;
    [SerializeField] private Transform[] tablePositions;
    
    [Header("== UI組件 ==")]
    [SerializeField] private Canvas restaurantCanvas;
    [SerializeField] private Text timerText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text moneyText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Transform orderPanel;
    
    [Header("== 菜單配置 ==")]
    [SerializeField] private MenuItem[] availableMenuItems;
    [SerializeField] private Transform menuItemContainer;
    
    [Header("== 客人配置 ==")]
    [SerializeField] private CustomerData[] customerTypes;
    [SerializeField] private Transform customerContainer;
    
    [Header("== 當前狀態 ==")]
    [SerializeField] private RestaurantGameState currentState = RestaurantGameState.Idle;
    [SerializeField] private bool isInitialized = false;
    [SerializeField] private bool isPlaying = false;
    
    // 餐廳遊戲事件
    public UnityEvent OnGameStarted;
    public UnityEvent<int> OnOrderCompleted;
    public UnityEvent<int, int> OnGameEnded; // 總分, 總收入
    public UnityEvent<CustomerData> OnCustomerServed;
    
    // 遊戲狀態變數
    private float gameTimer = 0f;
    private int currentScore = 0;
    private int currentMoney = 0;
    private int completedOrders = 0;
    
    // 客人管理
    private List<CustomerController> activeCustomers = new List<CustomerController>();
    private Queue<CustomerData> customerQueue = new Queue<CustomerData>();
    private float customerSpawnTimer = 0f;
    
    // 訂單管理
    private List<Order> activeOrders = new List<Order>();
    private Dictionary<MenuItem, int> menuItemStock = new Dictionary<MenuItem, int>();
    
    // 系統引用
    private NumericalRecords numericalRecords;
    private InteractionManager interactionManager;
    private TimeManagerTest timeManager;
    
    public bool IsInitialized => isInitialized;
    public bool IsPlaying => isPlaying;
    public RestaurantGameState CurrentState => currentState;
    public int CurrentScore => currentScore;
    public int CurrentMoney => currentMoney;
    
    /// <summary>
    /// 初始化餐廳遊戲控制器
    /// </summary>
    public void Initialize()
    {
        if (isInitialized) return;
        
        Debug.Log("[RestaurantGameController] 初始化餐廳遊戲控制器");
        
        // 查找系統引用
        FindSystemReferences();
        
        // 設置UI組件
        SetupUIComponents();
        
        // 初始化遊戲配置
        InitializeGameConfig();
        
        // 設置菜單
        SetupMenu();
        
        // 設置客人類型
        SetupCustomerTypes();
        
        // 設置初始狀態
        SetupInitialState();
        
        isInitialized = true;
        Debug.Log("[RestaurantGameController] 餐廳遊戲控制器初始化完成");
    }
    
    void Update()
    {
        if (isPlaying)
        {
            UpdateRestaurantGame();
        }
    }
    
    /// <summary>
    /// 查找系統引用
    /// </summary>
    private void FindSystemReferences()
    {
        numericalRecords = FindObjectOfType<NumericalRecords>();
        interactionManager = FindObjectOfType<InteractionManager>();
        timeManager = FindObjectOfType<TimeManagerTest>();
    }
    
    /// <summary>
    /// 設置UI組件
    /// </summary>
    private void SetupUIComponents()
    {
        // 設置開始按鈕
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGame);
        }
        
        // 查找Canvas
        if (restaurantCanvas == null)
        {
            restaurantCanvas = GetComponentInChildren<Canvas>();
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
            gameConfig = ScriptableObject.CreateInstance<RestaurantGameConfig>();
            SetupDefaultConfig();
        }
        
        // 初始化庫存
        menuItemStock.Clear();
        foreach (var item in availableMenuItems)
        {
            menuItemStock[item] = gameConfig.initialStock;
        }
    }
    
    /// <summary>
    /// 設置默認配置
    /// </summary>
    private void SetupDefaultConfig()
    {
        gameConfig.gameDuration = 60f;
        gameConfig.customerSpawnInterval = 5f;
        gameConfig.maxCustomers = 4;
        gameConfig.initialStock = 10;
        gameConfig.baseOrderTime = 15f;
        gameConfig.perfectServiceBonus = 2f;
    }
    
    /// <summary>
    /// 設置菜單
    /// </summary>
    private void SetupMenu()
    {
        if (availableMenuItems == null || availableMenuItems.Length == 0)
        {
            CreateDefaultMenu();
        }
        
        CreateMenuUI();
    }
    
    /// <summary>
    /// 創建默認菜單
    /// </summary>
    private void CreateDefaultMenu()
    {
        availableMenuItems = new MenuItem[]
        {
            new MenuItem
            {
                itemName = "咖啡",
                price = 50,
                preparationTime = 3f,
                difficulty = 1,
                customerSatisfaction = 20
            },
            new MenuItem
            {
                itemName = "三明治",
                price = 80,
                preparationTime = 5f,
                difficulty = 2,
                customerSatisfaction = 30
            },
            new MenuItem
            {
                itemName = "蛋糕",
                price = 120,
                preparationTime = 8f,
                difficulty = 3,
                customerSatisfaction = 50
            },
            new MenuItem
            {
                itemName = "特製料理",
                price = 200,
                preparationTime = 12f,
                difficulty = 4,
                customerSatisfaction = 80
            }
        };
    }
    
    /// <summary>
    /// 創建菜單UI
    /// </summary>
    private void CreateMenuUI()
    {
        if (menuItemContainer == null) return;
        
        // 清除現有菜單UI
        foreach (Transform child in menuItemContainer)
        {
            Destroy(child.gameObject);
        }
        
        // 創建菜單項目UI
        foreach (var item in availableMenuItems)
        {
            CreateMenuItemUI(item);
        }
    }
    
    /// <summary>
    /// 創建菜單項目UI
    /// </summary>
    private void CreateMenuItemUI(MenuItem item)
    {
        GameObject menuItemObj = new GameObject($"MenuItem_{item.itemName}");
        menuItemObj.transform.SetParent(menuItemContainer);
        
        // 添加Button組件
        Button itemButton = menuItemObj.AddComponent<Button>();
        
        // 添加背景Image
        Image bgImage = menuItemObj.AddComponent<Image>();
        bgImage.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
        
        // 添加文字
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(menuItemObj.transform);
        Text text = textObj.AddComponent<Text>();
        text.text = $"{item.itemName}\n${item.price}";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 14;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleCenter;
        
        // 設置RectTransform
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // 設置按鈕事件
        itemButton.onClick.AddListener(() => PrepareMenuItem(item));
        
        // 添加菜單項目組件
        MenuItemUI menuItemUI = menuItemObj.AddComponent<MenuItemUI>();
        menuItemUI.Initialize(item);
    }
    
    /// <summary>
    /// 設置客人類型
    /// </summary>
    private void SetupCustomerTypes()
    {
        if (customerTypes == null || customerTypes.Length == 0)
        {
            CreateDefaultCustomerTypes();
        }
    }
    
    /// <summary>
    /// 創建默認客人類型
    /// </summary>
    private void CreateDefaultCustomerTypes()
    {
        customerTypes = new CustomerData[]
        {
            new CustomerData
            {
                customerName = "普通客人",
                patience = 20f,
                tipGenerosity = 1f,
                preferredItems = new string[] { "咖啡", "三明治" },
                orderSize = 1
            },
            new CustomerData
            {
                customerName = "急性子客人",
                patience = 10f,
                tipGenerosity = 0.5f,
                preferredItems = new string[] { "咖啡" },
                orderSize = 1
            },
            new CustomerData
            {
                customerName = "VIP客人",
                patience = 30f,
                tipGenerosity = 3f,
                preferredItems = new string[] { "特製料理", "蛋糕" },
                orderSize = 2
            }
        };
    }
    
    /// <summary>
    /// 設置初始狀態
    /// </summary>
    private void SetupInitialState()
    {
        currentState = RestaurantGameState.Idle;
        gameTimer = 0f;
        currentScore = 0;
        currentMoney = 0;
        completedOrders = 0;
        customerSpawnTimer = 0f;
        isPlaying = false;
        
        // 清除活躍客人和訂單
        ClearActiveCustomers();
        activeOrders.Clear();
        customerQueue.Clear();
        
        // 隱藏遊戲界面
        if (restaurantCanvas != null)
        {
            restaurantCanvas.gameObject.SetActive(false);
        }
    }
    
    #region 遊戲流程控制
    
    /// <summary>
    /// 開始遊戲
    /// </summary>
    public void StartGame()
    {
        if (isPlaying) return;
        
        Debug.Log("[RestaurantGameController] 開始餐廳遊戲");
        
        isPlaying = true;
        currentState = RestaurantGameState.Playing;
        gameTimer = gameConfig.gameDuration;
        currentScore = 0;
        currentMoney = 0;
        completedOrders = 0;
        customerSpawnTimer = 0f;
        
        // 重置庫存
        foreach (var item in availableMenuItems)
        {
            menuItemStock[item] = gameConfig.initialStock;
        }
        
        // 顯示遊戲界面
        if (restaurantCanvas != null)
        {
            restaurantCanvas.gameObject.SetActive(true);
        }
        
        // 生成初始客人隊列
        GenerateCustomerQueue();
        
        OnGameStarted?.Invoke();
        UpdateUI();
    }
    
    /// <summary>
    /// 更新餐廳遊戲
    /// </summary>
    private void UpdateRestaurantGame()
    {
        // 更新計時器
        gameTimer -= Time.deltaTime;
        
        // 更新客人生成
        UpdateCustomerSpawning();
        
        // 更新活躍客人
        UpdateActiveCustomers();
        
        // 更新訂單處理
        UpdateOrderProcessing();
        
        // 檢查遊戲結束
        if (gameTimer <= 0f)
        {
            EndGame();
        }
        
        UpdateUI();
    }
    
    /// <summary>
    /// 更新客人生成
    /// </summary>
    private void UpdateCustomerSpawning()
    {
        customerSpawnTimer -= Time.deltaTime;
        
        if (customerSpawnTimer <= 0f && activeCustomers.Count < gameConfig.maxCustomers && customerQueue.Count > 0)
        {
            SpawnNextCustomer();
            customerSpawnTimer = gameConfig.customerSpawnInterval;
        }
    }
    
    /// <summary>
    /// 生成客人隊列
    /// </summary>
    private void GenerateCustomerQueue()
    {
        customerQueue.Clear();
        
        // 根據遊戲時間生成足夠的客人
        int customerCount = Mathf.RoundToInt(gameConfig.gameDuration / gameConfig.customerSpawnInterval) + 2;
        
        for (int i = 0; i < customerCount; i++)
        {
            CustomerData randomCustomer = customerTypes[Random.Range(0, customerTypes.Length)];
            customerQueue.Enqueue(randomCustomer);
        }
    }
    
    /// <summary>
    /// 生成下一個客人
    /// </summary>
    private void SpawnNextCustomer()
    {
        if (customerQueue.Count == 0) return;
        
        CustomerData customerData = customerQueue.Dequeue();
        
        // 創建客人物件
        GameObject customerObj = new GameObject($"Customer_{customerData.customerName}_{activeCustomers.Count}");
        customerObj.transform.SetParent(customerContainer);
        
        // 設置位置
        if (tablePositions != null && tablePositions.Length > 0)
        {
            int tableIndex = activeCustomers.Count % tablePositions.Length;
            customerObj.transform.position = tablePositions[tableIndex].position;
        }
        
        // 添加客人控制器
        CustomerController customerController = customerObj.AddComponent<CustomerController>();
        customerController.Initialize(customerData, this);
        
        activeCustomers.Add(customerController);
        
        Debug.Log($"[RestaurantGameController] 生成客人: {customerData.customerName}");
    }
    
    /// <summary>
    /// 更新活躍客人
    /// </summary>
    private void UpdateActiveCustomers()
    {
        for (int i = activeCustomers.Count - 1; i >= 0; i--)
        {
            CustomerController customer = activeCustomers[i];
            
            if (customer == null || customer.HasLeft)
            {
                // 客人已離開
                activeCustomers.RemoveAt(i);
                if (customer != null && customer.gameObject != null)
                {
                    Destroy(customer.gameObject);
                }
            }
        }
    }
    
    /// <summary>
    /// 更新訂單處理
    /// </summary>
    private void UpdateOrderProcessing()
    {
        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            Order order = activeOrders[i];
            order.preparationTimer -= Time.deltaTime;
            
            if (order.preparationTimer <= 0f)
            {
                // 訂單完成
                CompleteOrder(order);
                activeOrders.RemoveAt(i);
            }
        }
    }
    
    /// <summary>
    /// 準備菜單項目
    /// </summary>
    public void PrepareMenuItem(MenuItem item)
    {
        if (!isPlaying) return;
        
        // 檢查庫存
        if (menuItemStock[item] <= 0)
        {
            Debug.LogWarning($"[RestaurantGameController] {item.itemName} 庫存不足");
            return;
        }
        
        // 檢查是否有客人需要這個項目
        CustomerController targetCustomer = FindCustomerWantingItem(item);
        if (targetCustomer == null)
        {
            Debug.LogWarning($"[RestaurantGameController] 沒有客人需要 {item.itemName}");
            return;
        }
        
        // 消耗庫存
        menuItemStock[item]--;
        
        // 創建訂單
        Order newOrder = new Order
        {
            menuItem = item,
            customer = targetCustomer,
            preparationTimer = item.preparationTime,
            orderTime = Time.time
        };
        
        activeOrders.Add(newOrder);
        
        Debug.Log($"[RestaurantGameController] 開始準備 {item.itemName} 給 {targetCustomer.CustomerData.customerName}");
    }
    
    /// <summary>
    /// 查找需要特定項目的客人
    /// </summary>
    private CustomerController FindCustomerWantingItem(MenuItem item)
    {
        foreach (var customer in activeCustomers)
        {
            if (customer.WantsItem(item.itemName))
            {
                return customer;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 完成訂單
    /// </summary>
    private void CompleteOrder(Order order)
    {
        if (order.customer != null && !order.customer.HasLeft)
        {
            // 服務客人
            bool isOnTime = (Time.time - order.orderTime) <= gameConfig.baseOrderTime;
            int satisfaction = order.menuItem.customerSatisfaction;
            
            if (isOnTime)
            {
                satisfaction = Mathf.RoundToInt(satisfaction * gameConfig.perfectServiceBonus);
            }
            
            // 獲得金錢和分數
            int orderMoney = order.menuItem.price;
            int orderScore = satisfaction;
            
            if (isOnTime)
            {
                // 及時服務獎勵
                float tip = orderMoney * order.customer.CustomerData.tipGenerosity * 0.2f;
                orderMoney += Mathf.RoundToInt(tip);
                orderScore += 10;
            }
            
            currentMoney += orderMoney;
            currentScore += orderScore;
            completedOrders++;
            
            // 通知客人
            order.customer.ReceiveOrder(order.menuItem, isOnTime);
            
            Debug.Log($"[RestaurantGameController] 完成訂單: {order.menuItem.itemName} (+${orderMoney}, +{orderScore}分)");
            
            OnOrderCompleted?.Invoke(orderScore);
        }
    }
    
    /// <summary>
    /// 結束遊戲
    /// </summary>
    private void EndGame()
    {
        isPlaying = false;
        currentState = RestaurantGameState.GameOver;
        
        Debug.Log($"[RestaurantGameController] 遊戲結束 - 分數: {currentScore}, 收入: ${currentMoney}");
        
        // 更新數值系統
        if (numericalRecords != null)
        {
            // numericalRecords.AddMoney(currentMoney);
            // numericalRecords.AddWorkExperience(currentScore);
        }
        
        // 清除活躍客人
        ClearActiveCustomers();
        activeOrders.Clear();
        
        OnGameEnded?.Invoke(currentScore, currentMoney);
        
        // 延遲隱藏界面
        StartCoroutine(HideUIAfterDelay(3f));
    }
    
    /// <summary>
    /// 延遲隱藏UI
    /// </summary>
    private IEnumerator HideUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (restaurantCanvas != null)
        {
            restaurantCanvas.gameObject.SetActive(false);
        }
        
        // 重置狀態
        SetupInitialState();
    }
    
    #endregion
    
    #region UI更新
    
    /// <summary>
    /// 更新UI
    /// </summary>
    private void UpdateUI()
    {
        // 更新計時器
        if (timerText != null)
        {
            timerText.text = $"時間: {Mathf.Max(0, gameTimer):F0}s";
        }
        
        // 更新分數
        if (scoreText != null)
        {
            scoreText.text = $"分數: {currentScore}";
        }
        
        // 更新金錢
        if (moneyText != null)
        {
            moneyText.text = $"收入: ${currentMoney}";
        }
        
        // 更新開始按鈕
        if (startGameButton != null)
        {
            startGameButton.interactable = !isPlaying;
        }
    }
    
    #endregion
    
    #region 清理方法
    
    /// <summary>
    /// 清除活躍客人
    /// </summary>
    private void ClearActiveCustomers()
    {
        foreach (var customer in activeCustomers)
        {
            if (customer != null && customer.gameObject != null)
            {
                Destroy(customer.gameObject);
            }
        }
        activeCustomers.Clear();
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 客人離開通知
    /// </summary>
    public void OnCustomerLeave(CustomerController customer)
    {
        if (activeCustomers.Contains(customer))
        {
            activeCustomers.Remove(customer);
        }
    }
    
    /// <summary>
    /// 獲取遊戲統計
    /// </summary>
    public RestaurantGameStats GetGameStats()
    {
        return new RestaurantGameStats
        {
            totalScore = currentScore,
            totalMoney = currentMoney,
            completedOrders = completedOrders,
            remainingTime = gameTimer
        };
    }
    
    #endregion
}

/// <summary>
/// 餐廳遊戲狀態
/// </summary>
public enum RestaurantGameState
{
    Idle,       // 閒置
    Playing,    // 遊戲中
    GameOver    // 遊戲結束
}

/// <summary>
/// 菜單項目
/// </summary>
[System.Serializable]
public class MenuItem
{
    public string itemName;
    public int price;
    public float preparationTime;
    public int difficulty;
    public int customerSatisfaction;
    public Sprite itemSprite;
}

/// <summary>
/// 客人數據
/// </summary>
[System.Serializable]
public class CustomerData
{
    public string customerName;
    public float patience;
    public float tipGenerosity;
    public string[] preferredItems;
    public int orderSize;
    public Sprite customerSprite;
}

/// <summary>
/// 訂單
/// </summary>
[System.Serializable]
public class Order
{
    public MenuItem menuItem;
    public CustomerController customer;
    public float preparationTimer;
    public float orderTime;
}

/// <summary>
/// 餐廳遊戲配置
/// </summary>
[CreateAssetMenu(fileName = "RestaurantGameConfig", menuName = "LoveTide/RestaurantGameConfig")]
public class RestaurantGameConfig : ScriptableObject
{
    [Header("遊戲設定")]
    public float gameDuration = 60f;
    public float customerSpawnInterval = 5f;
    public int maxCustomers = 4;
    
    [Header("餐廳營運")]
    public int initialStock = 10;
    public float baseOrderTime = 15f;
    public float perfectServiceBonus = 2f;
}

/// <summary>
/// 餐廳遊戲統計
/// </summary>
[System.Serializable]
public class RestaurantGameStats
{
    public int totalScore;
    public int totalMoney;
    public int completedOrders;
    public float remainingTime;
}

/// <summary>
/// 菜單項目UI組件
/// </summary>
public class MenuItemUI : MonoBehaviour
{
    private MenuItem menuItem;
    
    public MenuItem MenuItem => menuItem;
    
    public void Initialize(MenuItem item)
    {
        menuItem = item;
    }
}

/// <summary>
/// 客人控制器
/// </summary>
public class CustomerController : MonoBehaviour
{
    private CustomerData customerData;
    private RestaurantGameController gameController;
    private float patienceTimer;
    private List<string> wantedItems = new List<string>();
    private bool hasLeft = false;
    
    public CustomerData CustomerData => customerData;
    public bool HasLeft => hasLeft;
    
    public void Initialize(CustomerData data, RestaurantGameController controller)
    {
        customerData = data;
        gameController = controller;
        patienceTimer = data.patience;
        
        // 生成想要的物品
        GenerateWantedItems();
    }
    
    void Update()
    {
        if (!hasLeft)
        {
            patienceTimer -= Time.deltaTime;
            
            if (patienceTimer <= 0f)
            {
                Leave();
            }
        }
    }
    
    private void GenerateWantedItems()
    {
        wantedItems.Clear();
        
        for (int i = 0; i < customerData.orderSize; i++)
        {
            if (customerData.preferredItems.Length > 0)
            {
                string item = customerData.preferredItems[Random.Range(0, customerData.preferredItems.Length)];
                wantedItems.Add(item);
            }
        }
    }
    
    public bool WantsItem(string itemName)
    {
        return wantedItems.Contains(itemName);
    }
    
    public void ReceiveOrder(MenuItem item, bool isOnTime)
    {
        if (wantedItems.Contains(item.itemName))
        {
            wantedItems.Remove(item.itemName);
            
            if (wantedItems.Count == 0)
            {
                // 所有訂單完成，客人離開
                Leave();
            }
        }
    }
    
    private void Leave()
    {
        hasLeft = true;
        gameController?.OnCustomerLeave(this);
    }
}