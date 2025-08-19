using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// UI動畫控制器
/// 
/// 職責:
/// 1. 管理UI元素的動畫效果
/// 2. 提供常用的UI動畫模板
/// 3. 處理UI轉場和過渡效果
/// 4. 與Canvas系統協作
/// 
/// 基於架構文檔: NurturingMode/互動系統完整設計_重製版.md
/// 為UI元素提供豐富的動畫效果
/// </summary>
public class UIAnimationController : MonoBehaviour
{
    [Header("== 動畫配置 ==")]
    [SerializeField] private float defaultDuration = 0.3f;
    [SerializeField] private AnimationCurve defaultCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private bool useUnscaledTime = false;
    
    [Header("== 預設動畫 ==")]
    [SerializeField] private UIAnimationPreset[] animationPresets;
    
    [Header("== 狀態管理 ==")]
    [SerializeField] private bool isAnimating = false;
    [SerializeField] private bool isInitialized = false;
    
    // 動畫事件
    public UnityEvent<string> OnAnimationStart;
    public UnityEvent<string> OnAnimationComplete;
    public UnityEvent<string, float> OnAnimationProgress;
    
    // 動畫組件
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image image;
    private Text text;
    
    // 當前動畫
    private Coroutine currentAnimation;
    private string currentAnimationName;
    
    // 原始狀態
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private float originalAlpha;
    private Color originalColor;
    
    public bool IsAnimating => isAnimating;
    public bool IsInitialized => isInitialized;
    public string CurrentAnimationName => currentAnimationName;
    
    void Awake()
    {
        Initialize();
    }
    
    /// <summary>
    /// 初始化UI動畫控制器
    /// </summary>
    public void Initialize()
    {
        if (isInitialized) return;
        
        Debug.Log("[UIAnimationController] 初始化UI動畫控制器");
        
        // 獲取組件引用
        GetComponentReferences();
        
        // 保存原始狀態
        SaveOriginalState();
        
        // 設置預設動畫
        SetupAnimationPresets();
        
        isInitialized = true;
        Debug.Log("[UIAnimationController] UI動畫控制器初始化完成");
    }
    
    /// <summary>
    /// 獲取組件引用
    /// </summary>
    private void GetComponentReferences()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
        text = GetComponent<Text>();
        
        // 如果沒有CanvasGroup，自動添加
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }
    
    /// <summary>
    /// 保存原始狀態
    /// </summary>
    private void SaveOriginalState()
    {
        if (rectTransform != null)
        {
            originalPosition = rectTransform.anchoredPosition;
            originalScale = rectTransform.localScale;
        }
        
        if (canvasGroup != null)
        {
            originalAlpha = canvasGroup.alpha;
        }
        
        if (image != null)
        {
            originalColor = image.color;
        }
        else if (text != null)
        {
            originalColor = text.color;
        }
    }
    
    /// <summary>
    /// 設置預設動畫
    /// </summary>
    private void SetupAnimationPresets()
    {
        if (animationPresets == null || animationPresets.Length == 0)
        {
            CreateDefaultPresets();
        }
    }
    
    /// <summary>
    /// 創建默認預設動畫
    /// </summary>
    private void CreateDefaultPresets()
    {
        animationPresets = new UIAnimationPreset[]
        {
            new UIAnimationPreset
            {
                name = "FadeIn",
                animationType = UIAnimationType.Fade,
                duration = 0.3f,
                startValue = Vector3.zero,
                endValue = Vector3.one,
                curve = AnimationCurve.EaseInOut(0, 0, 1, 1)
            },
            new UIAnimationPreset
            {
                name = "FadeOut",
                animationType = UIAnimationType.Fade,
                duration = 0.3f,
                startValue = Vector3.one,
                endValue = Vector3.zero,
                curve = AnimationCurve.EaseInOut(0, 0, 1, 1)
            },
            new UIAnimationPreset
            {
                name = "ScaleIn",
                animationType = UIAnimationType.Scale,
                duration = 0.3f,
                startValue = Vector3.zero,
                endValue = Vector3.one,
                curve = AnimationCurve.EaseInOut(0, 0, 1, 1)
            },
            new UIAnimationPreset
            {
                name = "SlideInFromLeft",
                animationType = UIAnimationType.Position,
                duration = 0.3f,
                startValue = new Vector3(-200, 0, 0),
                endValue = Vector3.zero,
                curve = AnimationCurve.EaseInOut(0, 0, 1, 1)
            }
        };
    }
    
    #region 動畫播放控制
    
    /// <summary>
    /// 播放預設動畫
    /// </summary>
    public void PlayPreset(string presetName)
    {
        UIAnimationPreset preset = FindPreset(presetName);
        if (preset != null)
        {
            PlayAnimation(preset);
        }
        else
        {
            Debug.LogWarning($"[UIAnimationController] 找不到預設動畫: {presetName}");
        }
    }
    
    /// <summary>
    /// 播放動畫
    /// </summary>
    public void PlayAnimation(UIAnimationPreset preset)
    {
        if (preset == null)
        {
            Debug.LogWarning("[UIAnimationController] 動畫預設為空");
            return;
        }
        
        StopCurrentAnimation();
        
        currentAnimationName = preset.name;
        currentAnimation = StartCoroutine(AnimateUI(preset));
    }
    
    /// <summary>
    /// 停止當前動畫
    /// </summary>
    public void StopCurrentAnimation()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
            isAnimating = false;
            currentAnimationName = "";
        }
    }
    
    /// <summary>
    /// UI動畫協程
    /// </summary>
    private IEnumerator AnimateUI(UIAnimationPreset preset)
    {
        isAnimating = true;
        OnAnimationStart?.Invoke(preset.name);
        
        float elapsedTime = 0f;
        
        while (elapsedTime < preset.duration)
        {
            float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsedTime += deltaTime;
            
            float progress = elapsedTime / preset.duration;
            float curveValue = preset.curve.Evaluate(progress);
            
            // 應用動畫效果
            ApplyAnimationEffect(preset.animationType, preset.startValue, preset.endValue, curveValue);
            
            OnAnimationProgress?.Invoke(preset.name, progress);
            yield return null;
        }
        
        // 設置最終狀態
        ApplyAnimationEffect(preset.animationType, preset.startValue, preset.endValue, 1f);
        
        isAnimating = false;
        currentAnimation = null;
        currentAnimationName = "";
        
        OnAnimationComplete?.Invoke(preset.name);
    }
    
    /// <summary>
    /// 應用動畫效果
    /// </summary>
    private void ApplyAnimationEffect(UIAnimationType type, Vector3 startValue, Vector3 endValue, float progress)
    {
        switch (type)
        {
            case UIAnimationType.Position:
                ApplyPositionAnimation(startValue, endValue, progress);
                break;
                
            case UIAnimationType.Scale:
                ApplyScaleAnimation(startValue, endValue, progress);
                break;
                
            case UIAnimationType.Fade:
                ApplyFadeAnimation(startValue.x, endValue.x, progress);
                break;
                
            case UIAnimationType.Color:
                ApplyColorAnimation(startValue, endValue, progress);
                break;
                
            case UIAnimationType.Rotation:
                ApplyRotationAnimation(startValue, endValue, progress);
                break;
        }
    }
    
    #endregion
    
    #region 具體動畫實現
    
    /// <summary>
    /// 應用位置動畫
    /// </summary>
    private void ApplyPositionAnimation(Vector3 startPos, Vector3 endPos, float progress)
    {
        if (rectTransform != null)
        {
            Vector3 currentPos = Vector3.Lerp(originalPosition + startPos, originalPosition + endPos, progress);
            rectTransform.anchoredPosition = currentPos;
        }
    }
    
    /// <summary>
    /// 應用縮放動畫
    /// </summary>
    private void ApplyScaleAnimation(Vector3 startScale, Vector3 endScale, float progress)
    {
        if (rectTransform != null)
        {
            Vector3 currentScale = Vector3.Lerp(startScale, endScale, progress);
            rectTransform.localScale = currentScale;
        }
    }
    
    /// <summary>
    /// 應用淡入淡出動畫
    /// </summary>
    private void ApplyFadeAnimation(float startAlpha, float endAlpha, float progress)
    {
        if (canvasGroup != null)
        {
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, progress);
            canvasGroup.alpha = currentAlpha;
        }
    }
    
    /// <summary>
    /// 應用顏色動畫
    /// </summary>
    private void ApplyColorAnimation(Vector3 startColor, Vector3 endColor, float progress)
    {
        Color current = new Color(
            Mathf.Lerp(startColor.x, endColor.x, progress),
            Mathf.Lerp(startColor.y, endColor.y, progress),
            Mathf.Lerp(startColor.z, endColor.z, progress),
            originalColor.a
        );
        
        if (image != null)
        {
            image.color = current;
        }
        else if (text != null)
        {
            text.color = current;
        }
    }
    
    /// <summary>
    /// 應用旋轉動畫
    /// </summary>
    private void ApplyRotationAnimation(Vector3 startRot, Vector3 endRot, float progress)
    {
        if (rectTransform != null)
        {
            Vector3 currentRot = Vector3.Lerp(startRot, endRot, progress);
            rectTransform.localEulerAngles = currentRot;
        }
    }
    
    #endregion
    
    #region 快捷動畫方法
    
    /// <summary>
    /// 淡入
    /// </summary>
    public void FadeIn(float duration = -1f)
    {
        float animDuration = duration > 0 ? duration : defaultDuration;
        var preset = new UIAnimationPreset
        {
            name = "FadeIn",
            animationType = UIAnimationType.Fade,
            duration = animDuration,
            startValue = Vector3.zero,
            endValue = Vector3.one,
            curve = defaultCurve
        };
        PlayAnimation(preset);
    }
    
    /// <summary>
    /// 淡出
    /// </summary>
    public void FadeOut(float duration = -1f)
    {
        float animDuration = duration > 0 ? duration : defaultDuration;
        var preset = new UIAnimationPreset
        {
            name = "FadeOut",
            animationType = UIAnimationType.Fade,
            duration = animDuration,
            startValue = Vector3.one,
            endValue = Vector3.zero,
            curve = defaultCurve
        };
        PlayAnimation(preset);
    }
    
    /// <summary>
    /// 縮放進入
    /// </summary>
    public void ScaleIn(float duration = -1f)
    {
        float animDuration = duration > 0 ? duration : defaultDuration;
        var preset = new UIAnimationPreset
        {
            name = "ScaleIn",
            animationType = UIAnimationType.Scale,
            duration = animDuration,
            startValue = Vector3.zero,
            endValue = originalScale,
            curve = defaultCurve
        };
        PlayAnimation(preset);
    }
    
    /// <summary>
    /// 縮放退出
    /// </summary>
    public void ScaleOut(float duration = -1f)
    {
        float animDuration = duration > 0 ? duration : defaultDuration;
        var preset = new UIAnimationPreset
        {
            name = "ScaleOut",
            animationType = UIAnimationType.Scale,
            duration = animDuration,
            startValue = originalScale,
            endValue = Vector3.zero,
            curve = defaultCurve
        };
        PlayAnimation(preset);
    }
    
    /// <summary>
    /// 從左滑入
    /// </summary>
    public void SlideInFromLeft(float distance = 200f, float duration = -1f)
    {
        float animDuration = duration > 0 ? duration : defaultDuration;
        var preset = new UIAnimationPreset
        {
            name = "SlideInFromLeft",
            animationType = UIAnimationType.Position,
            duration = animDuration,
            startValue = new Vector3(-distance, 0, 0),
            endValue = Vector3.zero,
            curve = defaultCurve
        };
        PlayAnimation(preset);
    }
    
    /// <summary>
    /// 向右滑出
    /// </summary>
    public void SlideOutToRight(float distance = 200f, float duration = -1f)
    {
        float animDuration = duration > 0 ? duration : defaultDuration;
        var preset = new UIAnimationPreset
        {
            name = "SlideOutToRight",
            animationType = UIAnimationType.Position,
            duration = animDuration,
            startValue = Vector3.zero,
            endValue = new Vector3(distance, 0, 0),
            curve = defaultCurve
        };
        PlayAnimation(preset);
    }
    
    #endregion
    
    #region 工具方法
    
    /// <summary>
    /// 查找預設動畫
    /// </summary>
    private UIAnimationPreset FindPreset(string name)
    {
        if (animationPresets != null)
        {
            foreach (var preset in animationPresets)
            {
                if (preset.name == name)
                    return preset;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 重置到原始狀態
    /// </summary>
    public void ResetToOriginalState()
    {
        StopCurrentAnimation();
        
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = originalPosition;
            rectTransform.localScale = originalScale;
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = originalAlpha;
        }
        
        if (image != null)
        {
            image.color = originalColor;
        }
        else if (text != null)
        {
            text.color = originalColor;
        }
        
        Debug.Log("[UIAnimationController] 重置到原始狀態");
    }
    
    /// <summary>
    /// 添加預設動畫
    /// </summary>
    public void AddPreset(UIAnimationPreset preset)
    {
        if (preset != null)
        {
            var presetList = new System.Collections.Generic.List<UIAnimationPreset>();
            if (animationPresets != null)
                presetList.AddRange(animationPresets);
                
            presetList.Add(preset);
            animationPresets = presetList.ToArray();
            
            Debug.Log($"[UIAnimationController] 添加預設動畫: {preset.name}");
        }
    }
    
    #endregion
    
    #region 公共接口
    
    /// <summary>
    /// 設置動畫時間縮放
    /// </summary>
    public void SetUseUnscaledTime(bool useUnscaled)
    {
        useUnscaledTime = useUnscaled;
    }
    
    /// <summary>
    /// 獲取所有預設動畫名稱
    /// </summary>
    public string[] GetPresetNames()
    {
        if (animationPresets == null)
            return new string[0];
            
        string[] names = new string[animationPresets.Length];
        for (int i = 0; i < animationPresets.Length; i++)
        {
            names[i] = animationPresets[i].name;
        }
        return names;
    }
    
    #endregion
}

/// <summary>
/// UI動畫類型
/// </summary>
public enum UIAnimationType
{
    Position,   // 位置動畫
    Scale,      // 縮放動畫
    Fade,       // 淡入淡出
    Color,      // 顏色動畫
    Rotation    // 旋轉動畫
}

/// <summary>
/// UI動畫預設
/// </summary>
[System.Serializable]
public class UIAnimationPreset
{
    public string name;
    public UIAnimationType animationType;
    public float duration = 0.3f;
    public Vector3 startValue;
    public Vector3 endValue;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
}