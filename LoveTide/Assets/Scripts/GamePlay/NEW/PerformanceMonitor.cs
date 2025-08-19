using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Events;

/// <summary>
/// 性能監控器
/// 
/// 職責:
/// 1. 監控遊戲性能指標(FPS、記憶體、CPU等)
/// 2. 檢測性能瓶頸和異常
/// 3. 提供性能數據報告
/// 4. 自動優化建議
/// 
/// 基於架構文檔: 提供性能監控和優化支援
/// 確保遊戲流暢運行和用戶體驗
/// </summary>
public class PerformanceMonitor : MonoBehaviour
{
    [Header("== 監控配置 ==")]
    [SerializeField] private bool enableMonitoring = true;
    [SerializeField] private bool enableDebugDisplay = false;
    [SerializeField] private float updateInterval = 1f;
    [SerializeField] private int maxHistoryEntries = 300; // 5分鐘歷史(每秒1次)
    
    [Header("== 性能閾值 ==")]
    [SerializeField] private PerformanceThresholds thresholds = new PerformanceThresholds();
    
    [Header("== UI顯示 ==")]
    [SerializeField] private bool showFPS = true;
    [SerializeField] private bool showMemory = true;
    [SerializeField] private bool showCPU = false;
    [SerializeField] private Vector2 displayPosition = new Vector2(10, 10);
    
    // 性能數據
    private PerformanceData currentData = new PerformanceData();
    private Queue<PerformanceData> performanceHistory = new Queue<PerformanceData>();
    
    // 計時器和計數器
    private float updateTimer = 0f;
    private int frameCount = 0;
    private float frameTimeSum = 0f;
    
    // 統計數據
    private PerformanceStats stats = new PerformanceStats();
    
    // 性能事件
    public UnityEvent<PerformanceData> OnPerformanceUpdated;
    public UnityEvent<PerformanceWarning> OnPerformanceWarning;
    public UnityEvent<PerformanceReport> OnPerformanceReport;
    
    // 警告系統
    private Dictionary<PerformanceMetric, float> warningCooldowns = new Dictionary<PerformanceMetric, float>();
    private const float WARNING_COOLDOWN = 5f; // 警告冷卻時間
    
    // 單例模式
    public static PerformanceMonitor Instance { get; private set; }
    
    public bool IsMonitoring => enableMonitoring;
    public PerformanceData CurrentData => currentData;
    public PerformanceStats Stats => stats;
    
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
        if (enableMonitoring)
        {
            UpdatePerformanceData();
            UpdateWarningCooldowns();
        }
    }
    
    void OnGUI()
    {
        if (enableDebugDisplay)
        {
            DrawPerformanceDisplay();
        }
    }
    
    /// <summary>
    /// 初始化性能監控器
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[PerformanceMonitor] 初始化性能監控器");
        
        // 初始化警告冷卻
        foreach (PerformanceMetric metric in Enum.GetValues(typeof(PerformanceMetric)))
        {
            warningCooldowns[metric] = 0f;
        }
        
        // 設置默認閾值
        SetupDefaultThresholds();
        
        Debug.Log("[PerformanceMonitor] 性能監控器初始化完成");
    }
    
    /// <summary>
    /// 設置默認閾值
    /// </summary>
    private void SetupDefaultThresholds()
    {
        if (thresholds == null)
        {
            thresholds = new PerformanceThresholds();
        }
        
        // 根據平台調整默認閾值
        if (Application.isMobilePlatform)
        {
            thresholds.minFPS = 30f;
            thresholds.maxMemoryMB = 512f;
            thresholds.maxCPUUsage = 80f;
        }
        else
        {
            thresholds.minFPS = 60f;
            thresholds.maxMemoryMB = 1024f;
            thresholds.maxCPUUsage = 70f;
        }
    }
    
    #region 性能數據更新
    
    /// <summary>
    /// 更新性能數據
    /// </summary>
    private void UpdatePerformanceData()
    {
        // 累積幀數據
        frameCount++;
        frameTimeSum += Time.deltaTime;
        updateTimer += Time.deltaTime;
        
        if (updateTimer >= updateInterval)
        {
            // 計算性能指標
            CalculatePerformanceMetrics();
            
            // 檢查性能警告
            CheckPerformanceWarnings();
            
            // 更新歷史記錄
            UpdatePerformanceHistory();
            
            // 更新統計數據
            UpdateStatistics();
            
            // 觸發更新事件
            OnPerformanceUpdated?.Invoke(currentData);
            
            // 重置計數器
            ResetCounters();
        }
    }
    
    /// <summary>
    /// 計算性能指標
    /// </summary>
    private void CalculatePerformanceMetrics()
    {
        // FPS計算
        currentData.fps = frameCount / updateInterval;
        currentData.frameTime = frameTimeSum / frameCount * 1000f; // 毫秒
        
        // 記憶體使用
        currentData.memoryUsedMB = Profiler.GetTotalAllocatedMemory(false) / (1024f * 1024f);
        currentData.memoryReservedMB = Profiler.GetTotalReservedMemory(false) / (1024f * 1024f);
        
        // Unity特定記憶體
        currentData.unityMemoryMB = Profiler.GetAllocatedMemoryForGraphicsDriver() / (1024f * 1024f);
        
        // 渲染統計
        currentData.drawCalls = GetDrawCalls();
        currentData.triangles = GetTriangleCount();
        
        // CPU使用率(簡化估算)
        currentData.cpuUsage = EstimateCPUUsage();
        
        // 垃圾回收
        currentData.gcAllocations = Profiler.GetMonoUsedSize() / (1024f * 1024f);
        
        // 時間戳
        currentData.timestamp = Time.time;
    }
    
    /// <summary>
    /// 獲取Draw Call數量
    /// </summary>
    private int GetDrawCalls()
    {
        // 這需要使用Unity的內部API或Frame Debugger
        // 這裡提供一個簡化的估算
        return UnityEngine.Rendering.FrameDebugger.enabled ? 
               UnityEngine.Rendering.FrameDebugger.count : 0;
    }
    
    /// <summary>
    /// 獲取三角形數量
    /// </summary>
    private int GetTriangleCount()
    {
        // 簡化估算，實際需要遍歷場景中的渲染器
        return 0; // 需要實際實現
    }
    
    /// <summary>
    /// 估算CPU使用率
    /// </summary>
    private float EstimateCPUUsage()
    {
        // 基於幀時間估算CPU使用率
        float targetFrameTime = 1f / 60f; // 60FPS的目標幀時間
        float actualFrameTime = frameTimeSum / frameCount;
        
        return Mathf.Clamp01(actualFrameTime / targetFrameTime) * 100f;
    }
    
    /// <summary>
    /// 重置計數器
    /// </summary>
    private void ResetCounters()
    {
        updateTimer = 0f;
        frameCount = 0;
        frameTimeSum = 0f;
    }
    
    #endregion
    
    #region 性能警告系統
    
    /// <summary>
    /// 檢查性能警告
    /// </summary>
    private void CheckPerformanceWarnings()
    {
        // FPS警告
        if (currentData.fps < thresholds.minFPS && CanTriggerWarning(PerformanceMetric.FPS))
        {
            TriggerWarning(PerformanceMetric.FPS, $"FPS過低: {currentData.fps:F1} < {thresholds.minFPS}");
        }
        
        // 記憶體警告
        if (currentData.memoryUsedMB > thresholds.maxMemoryMB && CanTriggerWarning(PerformanceMetric.Memory))
        {
            TriggerWarning(PerformanceMetric.Memory, $"記憶體使用過高: {currentData.memoryUsedMB:F1}MB > {thresholds.maxMemoryMB}MB");
        }
        
        // CPU警告
        if (currentData.cpuUsage > thresholds.maxCPUUsage && CanTriggerWarning(PerformanceMetric.CPU))
        {
            TriggerWarning(PerformanceMetric.CPU, $"CPU使用率過高: {currentData.cpuUsage:F1}% > {thresholds.maxCPUUsage}%");
        }
        
        // 幀時間警告
        if (currentData.frameTime > thresholds.maxFrameTimeMS && CanTriggerWarning(PerformanceMetric.FrameTime))
        {
            TriggerWarning(PerformanceMetric.FrameTime, $"幀時間過長: {currentData.frameTime:F1}ms > {thresholds.maxFrameTimeMS}ms");
        }
    }
    
    /// <summary>
    /// 檢查是否可以觸發警告
    /// </summary>
    private bool CanTriggerWarning(PerformanceMetric metric)
    {
        return warningCooldowns[metric] <= 0f;
    }
    
    /// <summary>
    /// 觸發性能警告
    /// </summary>
    private void TriggerWarning(PerformanceMetric metric, string message)
    {
        PerformanceWarning warning = new PerformanceWarning
        {
            metric = metric,
            message = message,
            timestamp = Time.time,
            currentValue = GetMetricValue(metric),
            thresholdValue = GetThresholdValue(metric)
        };
        
        warningCooldowns[metric] = WARNING_COOLDOWN;
        
        Debug.LogWarning($"[PerformanceMonitor] {message}");
        OnPerformanceWarning?.Invoke(warning);
    }
    
    /// <summary>
    /// 獲取指標值
    /// </summary>
    private float GetMetricValue(PerformanceMetric metric)
    {
        switch (metric)
        {
            case PerformanceMetric.FPS:
                return currentData.fps;
            case PerformanceMetric.Memory:
                return currentData.memoryUsedMB;
            case PerformanceMetric.CPU:
                return currentData.cpuUsage;
            case PerformanceMetric.FrameTime:
                return currentData.frameTime;
            default:
                return 0f;
        }
    }
    
    /// <summary>
    /// 獲取閾值
    /// </summary>
    private float GetThresholdValue(PerformanceMetric metric)
    {
        switch (metric)
        {
            case PerformanceMetric.FPS:
                return thresholds.minFPS;
            case PerformanceMetric.Memory:
                return thresholds.maxMemoryMB;
            case PerformanceMetric.CPU:
                return thresholds.maxCPUUsage;
            case PerformanceMetric.FrameTime:
                return thresholds.maxFrameTimeMS;
            default:
                return 0f;
        }
    }
    
    /// <summary>
    /// 更新警告冷卻
    /// </summary>
    private void UpdateWarningCooldowns()
    {
        var keys = new List<PerformanceMetric>(warningCooldowns.Keys);
        foreach (var key in keys)
        {
            if (warningCooldowns[key] > 0f)
            {
                warningCooldowns[key] -= Time.deltaTime;
            }
        }
    }
    
    #endregion
    
    #region 歷史記錄和統計
    
    /// <summary>
    /// 更新性能歷史
    /// </summary>
    private void UpdatePerformanceHistory()
    {
        // 複製當前數據到歷史
        PerformanceData historyData = new PerformanceData();
        CopyPerformanceData(currentData, historyData);
        
        performanceHistory.Enqueue(historyData);
        
        // 限制歷史記錄數量
        while (performanceHistory.Count > maxHistoryEntries)
        {
            performanceHistory.Dequeue();
        }
    }
    
    /// <summary>
    /// 複製性能數據
    /// </summary>
    private void CopyPerformanceData(PerformanceData source, PerformanceData target)
    {
        target.fps = source.fps;
        target.frameTime = source.frameTime;
        target.memoryUsedMB = source.memoryUsedMB;
        target.memoryReservedMB = source.memoryReservedMB;
        target.unityMemoryMB = source.unityMemoryMB;
        target.cpuUsage = source.cpuUsage;
        target.drawCalls = source.drawCalls;
        target.triangles = source.triangles;
        target.gcAllocations = source.gcAllocations;
        target.timestamp = source.timestamp;
    }
    
    /// <summary>
    /// 更新統計數據
    /// </summary>
    private void UpdateStatistics()
    {
        stats.totalSamples++;
        
        // FPS統計
        if (currentData.fps < stats.minFPS || stats.minFPS == 0)
            stats.minFPS = currentData.fps;
        if (currentData.fps > stats.maxFPS)
            stats.maxFPS = currentData.fps;
        stats.averageFPS = (stats.averageFPS * (stats.totalSamples - 1) + currentData.fps) / stats.totalSamples;
        
        // 記憶體統計
        if (currentData.memoryUsedMB > stats.peakMemoryMB)
            stats.peakMemoryMB = currentData.memoryUsedMB;
        stats.averageMemoryMB = (stats.averageMemoryMB * (stats.totalSamples - 1) + currentData.memoryUsedMB) / stats.totalSamples;
        
        // CPU統計
        if (currentData.cpuUsage > stats.peakCPUUsage)
            stats.peakCPUUsage = currentData.cpuUsage;
        stats.averageCPUUsage = (stats.averageCPUUsage * (stats.totalSamples - 1) + currentData.cpuUsage) / stats.totalSamples;
    }
    
    #endregion
    
    #region UI顯示
    
    /// <summary>
    /// 繪製性能顯示
    /// </summary>
    private void DrawPerformanceDisplay()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 12;
        style.normal.textColor = Color.white;
        
        float yOffset = displayPosition.y;
        
        if (showFPS)
        {
            string fpsText = $"FPS: {currentData.fps:F1} ({currentData.frameTime:F1}ms)";
            Color fpsColor = currentData.fps >= thresholds.minFPS ? Color.green : Color.red;
            style.normal.textColor = fpsColor;
            
            GUI.Label(new Rect(displayPosition.x, yOffset, 200, 20), fpsText, style);
            yOffset += 20;
        }
        
        if (showMemory)
        {
            string memoryText = $"Memory: {currentData.memoryUsedMB:F1}MB / {currentData.memoryReservedMB:F1}MB";
            Color memoryColor = currentData.memoryUsedMB <= thresholds.maxMemoryMB ? Color.green : Color.red;
            style.normal.textColor = memoryColor;
            
            GUI.Label(new Rect(displayPosition.x, yOffset, 300, 20), memoryText, style);
            yOffset += 20;
        }
        
        if (showCPU)
        {
            string cpuText = $"CPU: {currentData.cpuUsage:F1}%";
            Color cpuColor = currentData.cpuUsage <= thresholds.maxCPUUsage ? Color.green : Color.red;
            style.normal.textColor = cpuColor;
            
            GUI.Label(new Rect(displayPosition.x, yOffset, 200, 20), cpuText, style);
            yOffset += 20;
        }
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 啟用/禁用監控
    /// </summary>
    public void SetMonitoringEnabled(bool enabled)
    {
        enableMonitoring = enabled;
        
        if (!enabled)
        {
            ResetCounters();
        }
        
        Debug.Log($"[PerformanceMonitor] 性能監控: {(enabled ? "啟用" : "禁用")}");
    }
    
    /// <summary>
    /// 設置顯示選項
    /// </summary>
    public void SetDebugDisplay(bool enabled)
    {
        enableDebugDisplay = enabled;
    }
    
    /// <summary>
    /// 獲取性能歷史
    /// </summary>
    public PerformanceData[] GetPerformanceHistory()
    {
        return performanceHistory.ToArray();
    }
    
    /// <summary>
    /// 生成性能報告
    /// </summary>
    public PerformanceReport GenerateReport()
    {
        PerformanceReport report = new PerformanceReport
        {
            generationTime = DateTime.Now,
            sessionDuration = Time.time,
            currentData = currentData,
            statistics = stats,
            thresholds = thresholds,
            historyCount = performanceHistory.Count
        };
        
        OnPerformanceReport?.Invoke(report);
        return report;
    }
    
    /// <summary>
    /// 重置統計數據
    /// </summary>
    public void ResetStatistics()
    {
        stats = new PerformanceStats();
        performanceHistory.Clear();
        
        Debug.Log("[PerformanceMonitor] 統計數據已重置");
    }
    
    /// <summary>
    /// 設置性能閾值
    /// </summary>
    public void SetThresholds(PerformanceThresholds newThresholds)
    {
        thresholds = newThresholds;
        Debug.Log("[PerformanceMonitor] 性能閾值已更新");
    }
    
    /// <summary>
    /// 手動觸發垃圾回收
    /// </summary>
    public void ForceGarbageCollection()
    {
        System.GC.Collect();
        Debug.Log("[PerformanceMonitor] 手動觸發垃圾回收");
    }
    
    #endregion
}

/// <summary>
/// 性能指標
/// </summary>
public enum PerformanceMetric
{
    FPS,
    Memory,
    CPU,
    FrameTime,
    DrawCalls,
    Triangles
}

/// <summary>
/// 性能數據
/// </summary>
[System.Serializable]
public class PerformanceData
{
    public float fps;
    public float frameTime; // 毫秒
    public float memoryUsedMB;
    public float memoryReservedMB;
    public float unityMemoryMB;
    public float cpuUsage; // 百分比
    public int drawCalls;
    public int triangles;
    public float gcAllocations;
    public float timestamp;
}

/// <summary>
/// 性能閾值
/// </summary>
[System.Serializable]
public class PerformanceThresholds
{
    public float minFPS = 30f;
    public float maxMemoryMB = 512f;
    public float maxCPUUsage = 80f;
    public float maxFrameTimeMS = 33.33f; // 30FPS
    public int maxDrawCalls = 1000;
    public int maxTriangles = 100000;
}

/// <summary>
/// 性能統計
/// </summary>
[System.Serializable]
public class PerformanceStats
{
    public int totalSamples;
    
    public float minFPS;
    public float maxFPS;
    public float averageFPS;
    
    public float peakMemoryMB;
    public float averageMemoryMB;
    
    public float peakCPUUsage;
    public float averageCPUUsage;
}

/// <summary>
/// 性能警告
/// </summary>
[System.Serializable]
public class PerformanceWarning
{
    public PerformanceMetric metric;
    public string message;
    public float timestamp;
    public float currentValue;
    public float thresholdValue;
}

/// <summary>
/// 性能報告
/// </summary>
[System.Serializable]
public class PerformanceReport
{
    public DateTime generationTime;
    public float sessionDuration;
    public PerformanceData currentData;
    public PerformanceStats statistics;
    public PerformanceThresholds thresholds;
    public int historyCount;
}