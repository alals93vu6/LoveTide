using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 對話選項管理器
/// 
/// 職責:
/// 1. 管理對話中的選項顯示和交互
/// 2. 處理選項點擊和結果傳遞
/// 3. 與對話系統和數值系統協作
/// 4. 支持動態選項生成和條件檢查
/// 
/// 基於架構文檔: SharedSystems/對話系統架構.md
/// 實現對話選項的統一管理
/// </summary>
public class DialogChoiceManager : MonoBehaviour
{
    [Header("== UI組件引用 ==")]
    [SerializeField] private Transform choiceContainer;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    
    [Header("== 選項配置 ==")]
    [SerializeField] private List<DialogChoice> currentChoices = new List<DialogChoice>();
    [SerializeField] private int maxChoices = 6;
    [SerializeField] private bool allowMultipleSelection = false;
    
    [Header("== 當前狀態 ==")]
    [SerializeField] private bool isInitialized = false;
    [SerializeField] private bool isShowingChoices = false;
    
    // 選項事件
    public UnityEvent<DialogChoice> OnChoiceSelected;
    public UnityEvent<List<DialogChoice>> OnChoicesConfirmed;
    public UnityEvent OnChoicesCancelled;
    
    // 當前選中的選項
    private List<DialogChoice> selectedChoices = new List<DialogChoice>();
    private List<GameObject> choiceButtons = new List<GameObject>();
    
    public bool IsInitialized => isInitialized;
    public bool IsShowingChoices => isShowingChoices;
    public List<DialogChoice> SelectedChoices => selectedChoices;
    
    /// <summary>
    /// 初始化對話選項管理器
    /// </summary>
    public void Initialize()
    {
        Debug.Log("[DialogChoiceManager] 初始化對話選項管理器");
        
        // 查找UI組件
        FindUIComponents();
        
        // 設置按鈕事件
        SetupButtons();
        
        // 初始化狀態
        InitializeState();
        
        isInitialized = true;
        Debug.Log("[DialogChoiceManager] 對話選項管理器初始化完成");
    }
    
    /// <summary>
    /// 查找UI組件
    /// </summary>
    private void FindUIComponents()
    {
        // 查找選項容器
        if (choiceContainer == null)
        {
            choiceContainer = transform.Find("ChoiceContainer");
        }
        
        // 查找確認按鈕
        if (confirmButton == null)
        {
            confirmButton = transform.Find("ConfirmButton")?.GetComponent<Button>();
        }
        
        // 查找取消按鈕
        if (cancelButton == null)
        {
            cancelButton = transform.Find("CancelButton")?.GetComponent<Button>();
        }
        
        // 如果沒有預設的選項按鈕prefab，創建一個簡單的
        if (choiceButtonPrefab == null)
        {
            CreateDefaultChoiceButtonPrefab();
        }
    }
    
    /// <summary>
    /// 創建默認的選項按鈕prefab
    /// </summary>
    private void CreateDefaultChoiceButtonPrefab()
    {
        GameObject buttonPrefab = new GameObject("DefaultChoiceButton");
        buttonPrefab.AddComponent<RectTransform>();
        
        // 添加Button組件
        Button button = buttonPrefab.AddComponent<Button>();
        
        // 添加背景Image
        Image bgImage = buttonPrefab.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // 添加文字
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonPrefab.transform);
        Text text = textObj.AddComponent<Text>();
        text.text = "選項文字";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 16;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        // 設置RectTransform
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        choiceButtonPrefab = buttonPrefab;
        
        // 設置為不活躍，作為prefab使用
        buttonPrefab.SetActive(false);
    }
    
    /// <summary>
    /// 設置按鈕事件
    /// </summary>
    private void SetupButtons()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(ConfirmChoices);
        }
        
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(CancelChoices);
        }
    }
    
    /// <summary>
    /// 初始化狀態
    /// </summary>
    private void InitializeState()
    {
        selectedChoices.Clear();
        isShowingChoices = false;
        
        // 隱藏選項界面
        gameObject.SetActive(false);
    }
    
    #region 選項顯示和管理
    
    /// <summary>
    /// 顯示對話選項
    /// </summary>
    public void ShowChoices(List<DialogChoice> choices)
    {
        if (choices == null || choices.Count == 0)
        {
            Debug.LogWarning("[DialogChoiceManager] 沒有可顯示的選項");
            return;
        }
        
        currentChoices = choices;
        selectedChoices.Clear();
        
        // 顯示選項界面
        gameObject.SetActive(true);
        isShowingChoices = true;
        
        // 創建選項按鈕
        CreateChoiceButtons();
        
        Debug.Log($"[DialogChoiceManager] 顯示 {choices.Count} 個對話選項");
    }
    
    /// <summary>
    /// 創建選項按鈕
    /// </summary>
    private void CreateChoiceButtons()
    {
        // 清除舊的按鈕
        ClearChoiceButtons();
        
        // 創建新的按鈕
        for (int i = 0; i < currentChoices.Count && i < maxChoices; i++)
        {
            DialogChoice choice = currentChoices[i];
            CreateChoiceButton(choice, i);
        }
        
        // 更新確認按鈕狀態
        UpdateConfirmButton();
    }
    
    /// <summary>
    /// 創建單個選項按鈕
    /// </summary>
    private void CreateChoiceButton(DialogChoice choice, int index)
    {
        GameObject buttonObj = Instantiate(choiceButtonPrefab, choiceContainer);
        buttonObj.SetActive(true);
        buttonObj.name = $"ChoiceButton_{index}";
        
        // 設置按鈕文字
        Text buttonText = buttonObj.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = choice.choiceText;
        }
        
        // 設置按鈕事件
        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnChoiceButtonClicked(choice, buttonObj));
            
            // 檢查選項是否可用
            button.interactable = IsChoiceAvailable(choice);
        }
        
        // 添加選項數據組件
        DialogChoiceButton choiceButton = buttonObj.AddComponent<DialogChoiceButton>();
        choiceButton.Initialize(choice, index);
        
        choiceButtons.Add(buttonObj);
    }
    
    /// <summary>
    /// 清除選項按鈕
    /// </summary>
    private void ClearChoiceButtons()
    {
        foreach (GameObject button in choiceButtons)
        {
            if (button != null)
            {
                Destroy(button);
            }
        }
        choiceButtons.Clear();
    }
    
    /// <summary>
    /// 處理選項按鈕點擊
    /// </summary>
    private void OnChoiceButtonClicked(DialogChoice choice, GameObject buttonObj)
    {
        Debug.Log($"[DialogChoiceManager] 選項被點擊: {choice.choiceText}");
        
        if (allowMultipleSelection)
        {
            // 多選模式
            HandleMultipleSelection(choice, buttonObj);
        }
        else
        {
            // 單選模式
            HandleSingleSelection(choice, buttonObj);
        }
        
        // 觸發選項選中事件
        OnChoiceSelected?.Invoke(choice);
        
        // 更新確認按鈕狀態
        UpdateConfirmButton();
    }
    
    /// <summary>
    /// 處理單選
    /// </summary>
    private void HandleSingleSelection(DialogChoice choice, GameObject buttonObj)
    {
        selectedChoices.Clear();
        selectedChoices.Add(choice);
        
        // 更新按鈕視覺狀態
        UpdateButtonVisualState();
        
        // 如果是單選且不需要確認，直接執行
        if (confirmButton == null)
        {
            ConfirmChoices();
        }
    }
    
    /// <summary>
    /// 處理多選
    /// </summary>
    private void HandleMultipleSelection(DialogChoice choice, GameObject buttonObj)
    {
        if (selectedChoices.Contains(choice))
        {
            selectedChoices.Remove(choice);
        }
        else
        {
            selectedChoices.Add(choice);
        }
        
        // 更新按鈕視覺狀態
        UpdateButtonVisualState();
    }
    
    /// <summary>
    /// 更新按鈕視覺狀態
    /// </summary>
    private void UpdateButtonVisualState()
    {
        foreach (GameObject buttonObj in choiceButtons)
        {
            DialogChoiceButton choiceButton = buttonObj.GetComponent<DialogChoiceButton>();
            if (choiceButton != null)
            {
                bool isSelected = selectedChoices.Contains(choiceButton.Choice);
                choiceButton.SetSelected(isSelected);
            }
        }
    }
    
    #endregion
    
    #region 選項條件檢查
    
    /// <summary>
    /// 檢查選項是否可用
    /// </summary>
    private bool IsChoiceAvailable(DialogChoice choice)
    {
        // 檢查條件要求
        if (choice.conditions != null && choice.conditions.Count > 0)
        {
            foreach (var condition in choice.conditions)
            {
                if (!CheckCondition(condition))
                {
                    return false;
                }
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 檢查單個條件
    /// </summary>
    private bool CheckCondition(DialogCondition condition)
    {
        switch (condition.conditionType)
        {
            case DialogConditionType.Money:
                return CheckMoneyCondition(condition);
                
            case DialogConditionType.Affection:
                return CheckAffectionCondition(condition);
                
            case DialogConditionType.Time:
                return CheckTimeCondition(condition);
                
            case DialogConditionType.Item:
                return CheckItemCondition(condition);
                
            default:
                return true;
        }
    }
    
    private bool CheckMoneyCondition(DialogCondition condition)
    {
        // 假設從NumericalRecords檢查金錢
        // int currentMoney = numericalRecords.GetMoney();
        // return currentMoney >= condition.requiredValue;
        return true; // 暫時返回true
    }
    
    private bool CheckAffectionCondition(DialogCondition condition)
    {
        // 假設從NumericalRecords檢查好感度
        // int currentAffection = numericalRecords.GetAffection();
        // return currentAffection >= condition.requiredValue;
        return true; // 暫時返回true
    }
    
    private bool CheckTimeCondition(DialogCondition condition)
    {
        // 假設從TimeManager檢查時間
        // int currentHour = timeManager.GetCurrentHour();
        // return currentHour >= condition.requiredValue;
        return true; // 暫時返回true
    }
    
    private bool CheckItemCondition(DialogCondition condition)
    {
        // 假設檢查物品數量
        // int itemCount = inventory.GetItemCount(condition.itemName);
        // return itemCount >= condition.requiredValue;
        return true; // 暫時返回true
    }
    
    #endregion
    
    #region 確認和取消
    
    /// <summary>
    /// 確認選項
    /// </summary>
    public void ConfirmChoices()
    {
        if (selectedChoices.Count == 0)
        {
            Debug.LogWarning("[DialogChoiceManager] 沒有選中任何選項");
            return;
        }
        
        Debug.Log($"[DialogChoiceManager] 確認選項: {selectedChoices.Count} 個");
        
        // 觸發確認事件
        OnChoicesConfirmed?.Invoke(new List<DialogChoice>(selectedChoices));
        
        // 隱藏選項界面
        HideChoices();
    }
    
    /// <summary>
    /// 取消選項
    /// </summary>
    public void CancelChoices()
    {
        Debug.Log("[DialogChoiceManager] 取消選項選擇");
        
        // 觸發取消事件
        OnChoicesCancelled?.Invoke();
        
        // 隱藏選項界面
        HideChoices();
    }
    
    /// <summary>
    /// 隱藏選項界面
    /// </summary>
    public void HideChoices()
    {
        isShowingChoices = false;
        gameObject.SetActive(false);
        
        // 清除選項按鈕
        ClearChoiceButtons();
        
        // 清除當前數據
        currentChoices.Clear();
        selectedChoices.Clear();
        
        Debug.Log("[DialogChoiceManager] 選項界面已隱藏");
    }
    
    /// <summary>
    /// 更新確認按鈕狀態
    /// </summary>
    private void UpdateConfirmButton()
    {
        if (confirmButton != null)
        {
            confirmButton.interactable = selectedChoices.Count > 0;
        }
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 設置是否允許多選
    /// </summary>
    public void SetMultipleSelection(bool allow)
    {
        allowMultipleSelection = allow;
    }
    
    /// <summary>
    /// 設置最大選項數量
    /// </summary>
    public void SetMaxChoices(int max)
    {
        maxChoices = max;
    }
    
    /// <summary>
    /// 獲取當前選中的選項數量
    /// </summary>
    public int GetSelectedCount()
    {
        return selectedChoices.Count;
    }
    
    #endregion
}

/// <summary>
/// 對話選項數據結構
/// </summary>
[System.Serializable]
public class DialogChoice
{
    public string choiceText;
    public string choiceDescription;
    public List<DialogCondition> conditions = new List<DialogCondition>();
    public List<DialogResult> results = new List<DialogResult>();
    public bool isAvailable = true;
}

/// <summary>
/// 對話條件
/// </summary>
[System.Serializable]
public class DialogCondition
{
    public DialogConditionType conditionType;
    public string itemName;
    public int requiredValue;
}

/// <summary>
/// 對話結果
/// </summary>
[System.Serializable]
public class DialogResult
{
    public DialogResultType resultType;
    public string targetName;
    public int changeValue;
}

/// <summary>
/// 對話選項按鈕組件
/// </summary>
public class DialogChoiceButton : MonoBehaviour
{
    private DialogChoice choice;
    private int index;
    private bool isSelected = false;
    
    private Button button;
    private Image backgroundImage;
    private Text text;
    
    public DialogChoice Choice => choice;
    public bool IsSelected => isSelected;
    
    public void Initialize(DialogChoice dialogChoice, int buttonIndex)
    {
        choice = dialogChoice;
        index = buttonIndex;
        
        // 獲取組件引用
        button = GetComponent<Button>();
        backgroundImage = GetComponent<Image>();
        text = GetComponentInChildren<Text>();
        
        // 設置初始狀態
        SetSelected(false);
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        // 更新視覺效果
        if (backgroundImage != null)
        {
            backgroundImage.color = selected ? 
                new Color(0.5f, 0.8f, 1f, 0.8f) :  // 選中顏色
                new Color(0.2f, 0.2f, 0.2f, 0.8f); // 正常顏色
        }
        
        if (text != null)
        {
            text.color = selected ? Color.yellow : Color.white;
        }
    }
}

/// <summary>
/// 對話條件類型
/// </summary>
public enum DialogConditionType
{
    Money,      // 金錢條件
    Affection,  // 好感度條件
    Time,       // 時間條件
    Item        // 物品條件
}

/// <summary>
/// 對話結果類型
/// </summary>
public enum DialogResultType
{
    ChangeMoney,     // 改變金錢
    ChangeAffection, // 改變好感度
    ChangeTime,      // 改變時間
    AddItem,         // 添加物品
    RemoveItem       // 移除物品
}