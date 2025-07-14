using UnityEngine;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    [Header("測試設定")]
    [SerializeField] private int testEventIndex = 0;
    [SerializeField] private int testDialogIndex = 1;
    
    void Start()
    {
        // 等待一幀確保JsonDataManager已經初始化
        Invoke("TestDialogSystem", 0.1f);
    }
    
    void TestDialogSystem()
    {
        if (JsonDataManager.Instance == null)
        {
            Debug.LogError("JsonDataManager實例不存在！");
            return;
        }
        
        if (!JsonDataManager.Instance.IsDataLoaded())
        {
            Debug.LogWarning("資料尚未載入完成，等待中...");
            Invoke("TestDialogSystem", 0.5f);
            return;
        }
        
        Debug.Log("=== 開始測試對話系統 ===");
        
        // 測試1: 取得特定對話
        DialogLine testDialog = JsonDataManager.Instance.GetDialog(testEventIndex, testDialogIndex);
        if (testDialog != null)
        {
            Debug.Log($"測試對話 ({testEventIndex}, {testDialogIndex}): {testDialog.ActorName} - {testDialog.Dialog}");
        }
        else
        {
            Debug.LogWarning($"找不到對話 ({testEventIndex}, {testDialogIndex})");
        }
        
        // 測試2: 取得整個事件的對話
        List<DialogLine> eventDialogs = JsonDataManager.Instance.GetEventDialogs(testEventIndex);
        Debug.Log($"事件 {testEventIndex} 共有 {eventDialogs.Count} 句對話");
        
        foreach (var dialog in eventDialogs)
        {
            Debug.Log($"  對話{dialog.DialogIndex}: {dialog.ActorName} - {dialog.Dialog} (表情: {dialog.ActorFace})");
        }
        
        // 測試3: 取得系統資訊
        Debug.Log($"總對話數: {JsonDataManager.Instance.GetTotalDialogCount()}");
        Debug.Log($"總事件數: {JsonDataManager.Instance.GetTotalEventCount()}");
        
        // 測試4: 取得特定角色的對話
        List<DialogLine> actorDialogs = JsonDataManager.Instance.GetActorDialogs("由香");
        Debug.Log($"角色'由香'共有 {actorDialogs.Count} 句對話");
        
        Debug.Log("=== 對話系統測試完成 ===");
    }
    
    [ContextMenu("重新測試")]
    void RetestSystem()
    {
        TestDialogSystem();
    }
}