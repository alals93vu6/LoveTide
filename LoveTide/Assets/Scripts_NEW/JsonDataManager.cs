using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JsonDataManager : MonoBehaviour
{
    [Header("資料狀態")]
    [SerializeField] private List<DialogLine> allDialogs = new List<DialogLine>();
    [SerializeField] private bool isDataLoaded = false;
    
    [Header("設定")]
    [SerializeField] private string currentJsonFileName = "GameTest";
    
    // 單例模式
    public static JsonDataManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadDialogData();
    }
    
    /// <summary>
    /// 載入對話資料 - 目前無條件載入指定的JSON檔案
    /// </summary>
    public void LoadDialogData()
    {
        Debug.Log("開始載入JSON資料...");
        
        // TODO: 未來這裡會加入進度偵測邏輯，現在直接載入指定檔案
        string jsonPath = $"TestJosn/{currentJsonFileName}";
        
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonPath);
        
        if (jsonFile != null)
        {
            try
            {
                // 直接反序列化為DialogLine陣列
                allDialogs = JsonHelper.FromJson<DialogLine>(jsonFile.text).ToList();
                isDataLoaded = true;
                
                Debug.Log($"JSON資料載入成功！共載入 {allDialogs.Count} 筆對話資料");
                Debug.Log($"載入的檔案: {jsonPath}.json");
                
                // 顯示載入的事件範圍
                if (allDialogs.Count > 0)
                {
                    var eventIndices = allDialogs.Select(d => int.Parse(d.EventIndex)).Distinct().OrderBy(x => x);
                    Debug.Log($"載入的事件範圍: {eventIndices.First()} - {eventIndices.Last()}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"JSON解析失敗: {e.Message}");
                isDataLoaded = false;
            }
        }
        else
        {
            Debug.LogError($"找不到JSON檔案: {jsonPath}.json");
            isDataLoaded = false;
        }
    }
    
    /// <summary>
    /// 取得特定事件的特定對話
    /// </summary>
    public DialogLine GetDialog(int eventIndex, int dialogIndex)
    {
        if (!isDataLoaded)
        {
            Debug.LogWarning("資料尚未載入完成！");
            return null;
        }
        
        return allDialogs
            .Where(d => d.EventIndex == eventIndex.ToString() && d.DialogIndex == dialogIndex.ToString())
            .FirstOrDefault();
    }
    
    /// <summary>
    /// 取得特定事件的所有對話
    /// </summary>
    public List<DialogLine> GetEventDialogs(int eventIndex)
    {
        if (!isDataLoaded)
        {
            Debug.LogWarning("資料尚未載入完成！");
            return new List<DialogLine>();
        }
        
        return allDialogs
            .Where(d => d.EventIndex == eventIndex.ToString())
            .OrderBy(d => int.Parse(d.DialogIndex))
            .ToList();
    }
    
    /// <summary>
    /// 取得特定角色的所有對話
    /// </summary>
    public List<DialogLine> GetActorDialogs(string actorName)
    {
        if (!isDataLoaded)
        {
            Debug.LogWarning("資料尚未載入完成！");
            return new List<DialogLine>();
        }
        
        return allDialogs
            .Where(d => d.ActorName == actorName)
            .ToList();
    }
    
    /// <summary>
    /// 檢查資料是否載入完成
    /// </summary>
    public bool IsDataLoaded()
    {
        return isDataLoaded;
    }
    
    /// <summary>
    /// 取得總對話數量
    /// </summary>
    public int GetTotalDialogCount()
    {
        return allDialogs.Count;
    }
    
    /// <summary>
    /// 取得總事件數量
    /// </summary>
    public int GetTotalEventCount()
    {
        if (!isDataLoaded) return 0;
        
        return allDialogs
            .Select(d => d.EventIndex)
            .Distinct()
            .Count();
    }
}

/// <summary>
/// JSON陣列解析輔助類別
/// </summary>
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        return JsonUtility.FromJson<Wrapper<T>>($"{{\"Items\":{json}}}").Items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}