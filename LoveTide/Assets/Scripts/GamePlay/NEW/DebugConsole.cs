using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 調試控制台
/// 
/// 職責:
/// 1. 提供運行時調試命令介面
/// 2. 顯示遊戲日誌和錯誤信息
/// 3. 執行調試命令和腳本
/// 4. 監控系統狀態
/// 
/// 基於架構文檔: 提供開發和調試工具
/// 支援運行時調試和系統監控
/// </summary>
public class DebugConsole : MonoBehaviour
{
    [Header("== 控制台配置 ==")]
    [SerializeField] private bool enableConsole = true;
    [SerializeField] private KeyCode toggleKey = KeyCode.BackQuote; // `
    [SerializeField] private int maxLogEntries = 1000;
    [SerializeField] private int maxHistoryEntries = 50;
    
    [Header("== UI配置 ==")]
    [SerializeField] private int fontSize = 12;
    [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.8f);
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color errorColor = Color.red;
    [SerializeField] private Color warningColor = Color.yellow;
    
    // 控制台狀態
    private bool isVisible = false;
    private string inputText = "";
    private Vector2 scrollPosition = Vector2.zero;
    
    // 日誌系統
    private Queue<LogEntry> logEntries = new Queue<LogEntry>();
    private List<string> commandHistory = new List<string>();
    private int historyIndex = -1;
    
    // 命令系統
    private Dictionary<string, ConsoleCommand> commands = new Dictionary<string, ConsoleCommand>();
    
    // GUI樣式
    private GUIStyle backgroundStyle;
    private GUIStyle textStyle;
    private GUIStyle inputStyle;
    private bool stylesInitialized = false;
    
    // 事件
    public UnityEvent<string> OnCommandExecuted;
    public UnityEvent<LogEntry> OnLogAdded;
    
    // 單例模式
    public static DebugConsole Instance { get; private set; }
    
    public bool IsVisible => isVisible;
    public bool IsEnabled => enableConsole;
    
    void Awake()
    {
        // 單例設置
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        if (enableConsole && Input.GetKeyDown(toggleKey))
        {
            ToggleConsole();
        }
        
        if (isVisible)
        {
            HandleInputKeys();
        }
    }
    
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }
    
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
    
    void OnGUI()
    {
        if (isVisible && enableConsole)
        {
            DrawConsole();
        }
    }
    
    /// <summary>
    /// 初始化調試控制台
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[DebugConsole] 初始化調試控制台");
        
        // 註冊默認命令
        RegisterDefaultCommands();
        
        // 添加歡迎消息
        Log("調試控制台已啟動。輸入 'help' 查看可用命令。", LogType.Log);
        
        Debug.Log("[DebugConsole] 調試控制台初始化完成");
    }
    
    /// <summary>
    /// 初始化GUI樣式
    /// </summary>
    private void InitializeStyles()
    {
        if (stylesInitialized) return;
        
        backgroundStyle = new GUIStyle();
        backgroundStyle.normal.background = CreateTexture(backgroundColor);
        
        textStyle = new GUIStyle();
        textStyle.fontSize = fontSize;
        textStyle.normal.textColor = textColor;
        textStyle.wordWrap = true;
        textStyle.richText = true;
        
        inputStyle = new GUIStyle(GUI.skin.textField);
        inputStyle.fontSize = fontSize;
        inputStyle.normal.textColor = textColor;
        
        stylesInitialized = true;
    }
    
    /// <summary>
    /// 創建純色紋理
    /// </summary>
    private Texture2D CreateTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
    
    #region 控制台顯示
    
    /// <summary>
    /// 切換控制台顯示
    /// </summary>
    public void ToggleConsole()
    {
        isVisible = !isVisible;
        
        if (isVisible)
        {
            inputText = "";
            historyIndex = -1;
        }
    }
    
    /// <summary>
    /// 顯示控制台
    /// </summary>
    public void ShowConsole()
    {
        isVisible = true;
        inputText = "";
        historyIndex = -1;
    }
    
    /// <summary>
    /// 隱藏控制台
    /// </summary>
    public void HideConsole()
    {
        isVisible = false;
    }
    
    /// <summary>
    /// 繪製控制台
    /// </summary>
    private void DrawConsole()
    {
        InitializeStyles();
        
        float consoleHeight = Screen.height * 0.5f;
        Rect consoleRect = new Rect(0, 0, Screen.width, consoleHeight);
        
        // 繪製背景
        GUI.Box(consoleRect, "", backgroundStyle);
        
        GUILayout.BeginArea(new Rect(5, 5, Screen.width - 10, consoleHeight - 10));
        
        // 繪製日誌
        DrawLogArea();
        
        // 繪製輸入區域
        DrawInputArea();
        
        GUILayout.EndArea();
    }
    
    /// <summary>
    /// 繪製日誌區域
    /// </summary>
    private void DrawLogArea()
    {
        float inputHeight = 25f;
        float logHeight = (Screen.height * 0.5f) - inputHeight - 20f;
        
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(logHeight));
        
        foreach (var entry in logEntries)
        {
            Color entryColor = GetLogColor(entry.type);
            textStyle.normal.textColor = entryColor;
            
            string timeStamp = entry.timestamp.ToString("HH:mm:ss");
            string logText = $"[{timeStamp}] {entry.message}";
            
            GUILayout.Label(logText, textStyle);
        }
        
        GUILayout.EndScrollView();
        
        // 自動滾動到底部
        if (Event.current.type == EventType.Repaint)
        {
            scrollPosition.y = Mathf.Max(0, GUILayoutUtility.GetLastRect().height - logHeight);
        }
    }
    
    /// <summary>
    /// 繪製輸入區域
    /// </summary>
    private void DrawInputArea()
    {
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("> ", textStyle, GUILayout.Width(20));
        
        GUI.SetNextControlName("ConsoleInput");
        inputText = GUILayout.TextField(inputText, inputStyle);
        
        // 自動聚焦到輸入框
        if (Event.current.type == EventType.Repaint)
        {
            GUI.FocusControl("ConsoleInput");
        }
        
        GUILayout.EndHorizontal();
    }
    
    /// <summary>
    /// 獲取日誌顏色
    /// </summary>
    private Color GetLogColor(LogType type)
    {
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                return errorColor;
            case LogType.Warning:
                return warningColor;
            default:
                return textColor;
        }
    }
    
    #endregion
    
    #region 輸入處理
    
    /// <summary>
    /// 處理輸入按鍵
    /// </summary>
    private void HandleInputKeys()
    {
        Event e = Event.current;
        
        if (e.type == EventType.KeyDown)
        {
            switch (e.keyCode)
            {
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    ExecuteCommand(inputText);
                    break;
                    
                case KeyCode.UpArrow:
                    NavigateHistory(-1);
                    break;
                    
                case KeyCode.DownArrow:
                    NavigateHistory(1);
                    break;
                    
                case KeyCode.Tab:
                    AutoCompleteCommand();
                    break;
            }
        }
    }
    
    /// <summary>
    /// 導航命令歷史
    /// </summary>
    private void NavigateHistory(int direction)
    {
        if (commandHistory.Count == 0) return;
        
        historyIndex = Mathf.Clamp(historyIndex + direction, -1, commandHistory.Count - 1);
        
        if (historyIndex >= 0)
        {
            inputText = commandHistory[commandHistory.Count - 1 - historyIndex];
        }
        else
        {
            inputText = "";
        }
    }
    
    /// <summary>
    /// 自動完成命令
    /// </summary>
    private void AutoCompleteCommand()
    {
        if (string.IsNullOrEmpty(inputText)) return;
        
        var matches = commands.Keys.Where(cmd => cmd.StartsWith(inputText.ToLower())).ToList();
        
        if (matches.Count == 1)
        {
            inputText = matches[0];
        }
        else if (matches.Count > 1)
        {
            Log($"可能的命令: {string.Join(", ", matches)}", LogType.Log);
        }
    }
    
    #endregion
    
    #region 日誌系統
    
    /// <summary>
    /// 處理Unity日誌
    /// </summary>
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        Log(logString, type);
        
        if (type == LogType.Error || type == LogType.Exception)
        {
            Log(stackTrace, LogType.Log);
        }
    }
    
    /// <summary>
    /// 添加日誌條目
    /// </summary>
    public void Log(string message, LogType type = LogType.Log)
    {
        LogEntry entry = new LogEntry
        {
            message = message,
            type = type,
            timestamp = DateTime.Now
        };
        
        logEntries.Enqueue(entry);
        
        // 限制日誌條目數量
        while (logEntries.Count > maxLogEntries)
        {
            logEntries.Dequeue();
        }
        
        OnLogAdded?.Invoke(entry);
    }
    
    /// <summary>
    /// 清除日誌
    /// </summary>
    public void ClearLog()
    {
        logEntries.Clear();
        Log("日誌已清除", LogType.Log);
    }
    
    #endregion
    
    #region 命令系統
    
    /// <summary>
    /// 註冊默認命令
    /// </summary>
    private void RegisterDefaultCommands()
    {
        // 基本命令
        RegisterCommand("help", "顯示所有可用命令", CommandHelp);
        RegisterCommand("clear", "清除控制台", CommandClear);
        RegisterCommand("exit", "關閉控制台", CommandExit);
        RegisterCommand("quit", "退出應用程序", CommandQuit);
        
        // 系統信息命令
        RegisterCommand("info", "顯示系統信息", CommandInfo);
        RegisterCommand("fps", "顯示FPS信息", CommandFPS);
        RegisterCommand("memory", "顯示記憶體信息", CommandMemory);
        
        // 遊戲命令
        RegisterCommand("scene", "場景相關命令 [list|load <name>]", CommandScene);
        RegisterCommand("time", "時間相關命令 [get|set <scale>]", CommandTime);
        RegisterCommand("player", "玩家相關命令 [money|affection|experience] <value>", CommandPlayer);
        
        // 調試命令
        RegisterCommand("debug", "調試模式開關", CommandDebug);
        RegisterCommand("god", "無敵模式開關", CommandGod);
        RegisterCommand("noclip", "穿牆模式開關", CommandNoClip);
        
        // 對象命令
        RegisterCommand("find", "查找對象 <name>", CommandFind);
        RegisterCommand("destroy", "銷毀對象 <name>", CommandDestroy);
        RegisterCommand("list", "列出場景中的對象", CommandList);
    }
    
    /// <summary>
    /// 註冊命令
    /// </summary>
    public void RegisterCommand(string name, string description, System.Action<string[]> callback)
    {
        commands[name.ToLower()] = new ConsoleCommand
        {
            name = name,
            description = description,
            callback = callback
        };
    }
    
    /// <summary>
    /// 取消註冊命令
    /// </summary>
    public void UnregisterCommand(string name)
    {
        commands.Remove(name.ToLower());
    }
    
    /// <summary>
    /// 執行命令
    /// </summary>
    private void ExecuteCommand(string input)
    {
        if (string.IsNullOrEmpty(input.Trim()))
        {
            inputText = "";
            return;
        }
        
        // 添加到歷史記錄
        commandHistory.Add(input);
        if (commandHistory.Count > maxHistoryEntries)
        {
            commandHistory.RemoveAt(0);
        }
        
        // 顯示輸入的命令
        Log($"> {input}", LogType.Log);
        
        // 解析命令
        string[] parts = input.Trim().Split(' ');
        string commandName = parts[0].ToLower();
        string[] args = parts.Skip(1).ToArray();
        
        // 執行命令
        if (commands.ContainsKey(commandName))
        {
            try
            {
                commands[commandName].callback(args);
                OnCommandExecuted?.Invoke(input);
            }
            catch (Exception e)
            {
                Log($"執行命令時發生錯誤: {e.Message}", LogType.Error);
            }
        }
        else
        {
            Log($"未知命令: {commandName}。輸入 'help' 查看可用命令。", LogType.Warning);
        }
        
        // 清空輸入
        inputText = "";
        historyIndex = -1;
    }
    
    #endregion
    
    #region 默認命令實現
    
    private void CommandHelp(string[] args)
    {
        Log("可用命令:", LogType.Log);
        foreach (var cmd in commands.Values.OrderBy(c => c.name))
        {
            Log($"  {cmd.name} - {cmd.description}", LogType.Log);
        }
    }
    
    private void CommandClear(string[] args)
    {
        ClearLog();
    }
    
    private void CommandExit(string[] args)
    {
        HideConsole();
    }
    
    private void CommandQuit(string[] args)
    {
        Application.Quit();
    }
    
    private void CommandInfo(string[] args)
    {
        Log($"Unity版本: {Application.unityVersion}", LogType.Log);
        Log($"平台: {Application.platform}", LogType.Log);
        Log($"設備型號: {SystemInfo.deviceModel}", LogType.Log);
        Log($"操作系統: {SystemInfo.operatingSystem}", LogType.Log);
        Log($"記憶體: {SystemInfo.systemMemorySize}MB", LogType.Log);
        Log($"顯卡: {SystemInfo.graphicsDeviceName}", LogType.Log);
    }
    
    private void CommandFPS(string[] args)
    {
        if (PerformanceMonitor.Instance != null)
        {
            var data = PerformanceMonitor.Instance.CurrentData;
            Log($"FPS: {data.fps:F1}", LogType.Log);
            Log($"幀時間: {data.frameTime:F1}ms", LogType.Log);
        }
        else
        {
            Log("性能監控器未啟用", LogType.Warning);
        }
    }
    
    private void CommandMemory(string[] args)
    {
        long totalMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory(false);
        long reservedMemory = UnityEngine.Profiling.Profiler.GetTotalReservedMemory(false);
        
        Log($"已分配記憶體: {totalMemory / (1024 * 1024)}MB", LogType.Log);
        Log($"保留記憶體: {reservedMemory / (1024 * 1024)}MB", LogType.Log);
    }
    
    private void CommandScene(string[] args)
    {
        if (args.Length == 0)
        {
            Log($"當前場景: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}", LogType.Log);
            return;
        }
        
        switch (args[0].ToLower())
        {
            case "list":
                for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
                {
                    string scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                    Log($"  {i}: {sceneName}", LogType.Log);
                }
                break;
                
            case "load":
                if (args.Length > 1)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(args[1]);
                    Log($"載入場景: {args[1]}", LogType.Log);
                }
                else
                {
                    Log("請指定場景名稱", LogType.Warning);
                }
                break;
        }
    }
    
    private void CommandTime(string[] args)
    {
        if (args.Length == 0)
        {
            Log($"時間縮放: {Time.timeScale}", LogType.Log);
            return;
        }
        
        switch (args[0].ToLower())
        {
            case "get":
                Log($"時間縮放: {Time.timeScale}", LogType.Log);
                break;
                
            case "set":
                if (args.Length > 1 && float.TryParse(args[1], out float scale))
                {
                    Time.timeScale = scale;
                    Log($"時間縮放設置為: {scale}", LogType.Log);
                }
                else
                {
                    Log("請提供有效的數值", LogType.Warning);
                }
                break;
        }
    }
    
    private void CommandPlayer(string[] args)
    {
        if (args.Length < 2)
        {
            Log("用法: player [money|affection|experience] <value>", LogType.Warning);
            return;
        }
        
        string property = args[0].ToLower();
        if (int.TryParse(args[1], out int value))
        {
            // 這裡需要根據實際的數值系統實現
            Log($"設置 {property} 為 {value}", LogType.Log);
        }
        else
        {
            Log("請提供有效的數值", LogType.Warning);
        }
    }
    
    private void CommandDebug(string[] args)
    {
        Debug.unityLogger.logEnabled = !Debug.unityLogger.logEnabled;
        Log($"調試模式: {(Debug.unityLogger.logEnabled ? "啟用" : "禁用")}", LogType.Log);
    }
    
    private void CommandGod(string[] args)
    {
        Log("無敵模式功能需要實現", LogType.Warning);
    }
    
    private void CommandNoClip(string[] args)
    {
        Log("穿牆模式功能需要實現", LogType.Warning);
    }
    
    private void CommandFind(string[] args)
    {
        if (args.Length == 0)
        {
            Log("請指定要查找的對象名稱", LogType.Warning);
            return;
        }
        
        GameObject[] objects = FindObjectsOfType<GameObject>();
        var matches = objects.Where(obj => obj.name.ToLower().Contains(args[0].ToLower())).ToList();
        
        Log($"找到 {matches.Count} 個匹配的對象:", LogType.Log);
        foreach (var obj in matches.Take(10)) // 限制顯示數量
        {
            Log($"  {obj.name} (位置: {obj.transform.position})", LogType.Log);
        }
    }
    
    private void CommandDestroy(string[] args)
    {
        if (args.Length == 0)
        {
            Log("請指定要銷毀的對象名稱", LogType.Warning);
            return;
        }
        
        GameObject obj = GameObject.Find(args[0]);
        if (obj != null)
        {
            Destroy(obj);
            Log($"已銷毀對象: {args[0]}", LogType.Log);
        }
        else
        {
            Log($"找不到對象: {args[0]}", LogType.Warning);
        }
    }
    
    private void CommandList(string[] args)
    {
        GameObject[] objects = FindObjectsOfType<GameObject>();
        Log($"場景中共有 {objects.Length} 個對象:", LogType.Log);
        
        foreach (var obj in objects.Take(20)) // 限制顯示數量
        {
            Log($"  {obj.name}", LogType.Log);
        }
        
        if (objects.Length > 20)
        {
            Log($"  ... 還有 {objects.Length - 20} 個對象", LogType.Log);
        }
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 設置控制台啟用狀態
    /// </summary>
    public void SetEnabled(bool enabled)
    {
        enableConsole = enabled;
        
        if (!enabled)
        {
            HideConsole();
        }
    }
    
    /// <summary>
    /// 獲取所有日誌條目
    /// </summary>
    public LogEntry[] GetAllLogs()
    {
        return logEntries.ToArray();
    }
    
    /// <summary>
    /// 獲取命令歷史
    /// </summary>
    public string[] GetCommandHistory()
    {
        return commandHistory.ToArray();
    }
    
    #endregion
}

/// <summary>
/// 日誌條目
/// </summary>
[System.Serializable]
public class LogEntry
{
    public string message;
    public LogType type;
    public DateTime timestamp;
}

/// <summary>
/// 控制台命令
/// </summary>
[System.Serializable]
public class ConsoleCommand
{
    public string name;
    public string description;
    public System.Action<string[]> callback;
}