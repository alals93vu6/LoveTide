using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 角色互動視覺反饋控制器
/// 
/// 功能:
/// 1. 點擊效果 - 點擊時的視覺回饋
/// 2. 波紋效果 - 圓形擴散動畫
/// 3. 粒子效果 - 互動時的粒子系統
/// 4. 色彩變化 - 角色點擊時的色調調整
/// 
/// 設計理念:
/// - 提供豐富的視覺回饋
/// - 增強使用者體驗
/// - 模組化效果系統
/// </summary>
public class CharacterInteractionFeedback : MonoBehaviour
{
    [Header("=== 點擊效果設定 ===")]
    [SerializeField] private bool enableClickEffect = true;
    [SerializeField] private GameObject clickEffectPrefab;       // 點擊效果預製體
    [SerializeField] private float clickEffectDuration = 1f;     // 效果持續時間
    [SerializeField] private float clickEffectScale = 1f;        // 效果縮放
    
    [Header("=== 波紋效果設定 ===")]
    [SerializeField] private bool enableRippleEffect = true;
    [SerializeField] private Image rippleImage;                  // 波紋圖片
    [SerializeField] private float rippleMaxScale = 3f;          // 波紋最大縮放
    [SerializeField] private float rippleDuration = 0.8f;        // 波紋持續時間
    [SerializeField] private AnimationCurve rippleScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve rippleAlphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    [Header("=== 色彩閃爍效果 ===")]
    [SerializeField] private bool enableColorFlash = true;
    [SerializeField] private Image characterImage;               // 角色圖片組件
    [SerializeField] private Color flashColor = Color.white;     // 閃爍顏色
    [SerializeField] private float flashDuration = 0.3f;        // 閃爍持續時間
    [SerializeField] private AnimationCurve flashIntensityCurve = AnimationCurve.EaseInOut(0, 0, 0.5f, 1);
    
    [Header("=== 粒子效果設定 ===")]
    [SerializeField] private bool enableParticleEffect = true;
    [SerializeField] private ParticleSystem clickParticles;      // 點擊粒子系統
    [SerializeField] private int particleCount = 10;             // 粒子數量
    [SerializeField] private float particleLifetime = 2f;       // 粒子壽命
    
    [Header("=== 震動效果設定 ===")]
    [SerializeField] private bool enableShakeEffect = true;
    [SerializeField] private float shakeIntensity = 5f;          // 震動強度
    [SerializeField] private float shakeDuration = 0.2f;        // 震動持續時間
    [SerializeField] private int shakeVibrations = 10;          // 震動次數
    
    [Header("=== 音效設定 ===")]
    [SerializeField] private bool enableAudioFeedback = true;
    [SerializeField] private AudioSource audioSource;           // 音效播放器
    [SerializeField] private AudioClip clickSound;              // 點擊音效
    [SerializeField] private float audioVolume = 1f;            // 音效音量
    
    // 私有變量
    private Vector3 originalPosition;
    private Color originalColor;
    private Coroutine currentRippleCoroutine;
    private Coroutine currentFlashCoroutine;
    private Coroutine currentShakeCoroutine;
    private bool isInitialized = false;
    
    #region Unity 生命週期
    
    void Awake()
    {
        InitializeComponents();
    }
    
    void Start()
    {
        Initialize();
    }
    
    #endregion
    
    #region 初始化
    
    /// <summary>
    /// 初始化組件
    /// </summary>
    private void InitializeComponents()
    {
        // 自動獲取組件
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
            
        if (characterImage == null)
            characterImage = GetComponent<Image>();
            
        // 創建波紋效果 (如果沒有設置)
        if (rippleImage == null && enableRippleEffect)
        {
            CreateRippleEffect();
        }
        
        // 創建粒子系統 (如果沒有設置)
        if (clickParticles == null && enableParticleEffect)
        {
            CreateParticleSystem();
        }
    }
    
    /// <summary>
    /// 初始化系統
    /// </summary>
    private void Initialize()
    {
        // 記錄原始狀態
        originalPosition = transform.localPosition;
        
        if (characterImage != null)
        {
            originalColor = characterImage.color;
        }
        
        // 初始化波紋效果
        if (rippleImage != null)
        {
            rippleImage.gameObject.SetActive(false);
            rippleImage.transform.localScale = Vector3.zero;
        }
        
        isInitialized = true;
        Debug.Log("[CharacterInteractionFeedback] 視覺反饋系統初始化完成");
    }
    
    /// <summary>
    /// 創建波紋效果
    /// </summary>
    private void CreateRippleEffect()
    {
        GameObject rippleObj = new GameObject("RippleEffect");
        rippleObj.transform.SetParent(transform);
        rippleObj.transform.localPosition = Vector3.zero;
        
        rippleImage = rippleObj.AddComponent<Image>();
        rippleImage.sprite = CreateCircleSprite();
        rippleImage.color = new Color(1f, 1f, 1f, 0.5f);
        
        // 設置為最低層級
        rippleImage.transform.SetAsFirstSibling();
    }
    
    /// <summary>
    /// 創建粒子系統
    /// </summary>
    private void CreateParticleSystem()
    {
        GameObject particleObj = new GameObject("ClickParticles");
        particleObj.transform.SetParent(transform);
        particleObj.transform.localPosition = Vector3.zero;
        
        clickParticles = particleObj.AddComponent<ParticleSystem>();
        ConfigureParticleSystem();
    }
    
    /// <summary>
    /// 配置粒子系統
    /// </summary>
    private void ConfigureParticleSystem()
    {
        if (clickParticles == null) return;
        
        var main = clickParticles.main;
        main.startLifetime = particleLifetime;
        main.startSpeed = 5f;
        main.startSize = 0.1f;
        main.startColor = Color.yellow;
        main.maxParticles = particleCount;
        
        var emission = clickParticles.emission;
        emission.enabled = false; // 手動觸發
        
        var shape = clickParticles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.5f;
    }
    
    /// <summary>
    /// 創建圓形精靈圖
    /// </summary>
    private Sprite CreateCircleSprite()
    {
        // 這裡應該載入一個圓形貼圖
        // 暫時返回預設的Unity UI精靈圖
        return Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
    }
    
    #endregion
    
    #region 公開接口
    
    /// <summary>
    /// 播放點擊效果
    /// </summary>
    public void PlayClickEffect()
    {
        if (!isInitialized) return;
        
        Debug.Log("[CharacterInteractionFeedback] 播放點擊效果");
        
        // 播放各種效果
        if (enableRippleEffect) PlayRippleEffect();
        if (enableColorFlash) PlayColorFlash();
        if (enableShakeEffect) PlayShakeEffect();
        if (enableParticleEffect) PlayParticleEffect();
        if (enableAudioFeedback) PlayAudioFeedback();
        if (enableClickEffect) SpawnClickEffect();
    }
    
    /// <summary>
    /// 播放懸停效果
    /// </summary>
    public void PlayHoverEffect()
    {
        if (!isInitialized) return;
        
        Debug.Log("[CharacterInteractionFeedback] 播放懸停效果");
        
        // 輕微的色彩變化
        if (enableColorFlash && characterImage != null)
        {
            StartCoroutine(HoverColorEffect());
        }
    }
    
    /// <summary>
    /// 停止所有效果
    /// </summary>
    public void StopAllEffects()
    {
        StopAllCoroutines();
        
        if (rippleImage != null)
        {
            rippleImage.gameObject.SetActive(false);
        }
        
        if (characterImage != null)
        {
            characterImage.color = originalColor;
        }
        
        transform.localPosition = originalPosition;
    }
    
    #endregion
    
    #region 波紋效果
    
    /// <summary>
    /// 播放波紋效果
    /// </summary>
    private void PlayRippleEffect()
    {
        if (rippleImage == null) return;
        
        if (currentRippleCoroutine != null)
        {
            StopCoroutine(currentRippleCoroutine);
        }
        
        currentRippleCoroutine = StartCoroutine(RippleEffectCoroutine());
    }
    
    /// <summary>
    /// 波紋效果協程
    /// </summary>
    private IEnumerator RippleEffectCoroutine()
    {
        rippleImage.gameObject.SetActive(true);
        rippleImage.transform.localScale = Vector3.zero;
        
        float elapsed = 0f;
        
        while (elapsed < rippleDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / rippleDuration;
            
            // 縮放動畫
            float scale = rippleScaleCurve.Evaluate(progress) * rippleMaxScale;
            rippleImage.transform.localScale = Vector3.one * scale;
            
            // 透明度動畫
            float alpha = rippleAlphaCurve.Evaluate(progress);
            Color color = rippleImage.color;
            color.a = alpha;
            rippleImage.color = color;
            
            yield return null;
        }
        
        rippleImage.gameObject.SetActive(false);
        currentRippleCoroutine = null;
    }
    
    #endregion
    
    #region 色彩效果
    
    /// <summary>
    /// 播放色彩閃爍效果
    /// </summary>
    private void PlayColorFlash()
    {
        if (characterImage == null) return;
        
        if (currentFlashCoroutine != null)
        {
            StopCoroutine(currentFlashCoroutine);
        }
        
        currentFlashCoroutine = StartCoroutine(ColorFlashCoroutine());
    }
    
    /// <summary>
    /// 色彩閃爍協程
    /// </summary>
    private IEnumerator ColorFlashCoroutine()
    {
        float elapsed = 0f;
        
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / flashDuration;
            
            float intensity = flashIntensityCurve.Evaluate(progress);
            Color currentColor = Color.Lerp(originalColor, flashColor, intensity);
            characterImage.color = currentColor;
            
            yield return null;
        }
        
        characterImage.color = originalColor;
        currentFlashCoroutine = null;
    }
    
    /// <summary>
    /// 懸停色彩效果
    /// </summary>
    private IEnumerator HoverColorEffect()
    {
        Color hoverColor = new Color(originalColor.r * 1.2f, originalColor.g * 1.2f, originalColor.b * 1.2f, originalColor.a);
        
        // 淡入
        float elapsed = 0f;
        float duration = 0.2f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            Color currentColor = Color.Lerp(originalColor, hoverColor, progress);
            characterImage.color = currentColor;
            
            yield return null;
        }
        
        // 淡出
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            Color currentColor = Color.Lerp(hoverColor, originalColor, progress);
            characterImage.color = currentColor;
            
            yield return null;
        }
        
        characterImage.color = originalColor;
    }
    
    #endregion
    
    #region 震動效果
    
    /// <summary>
    /// 播放震動效果
    /// </summary>
    private void PlayShakeEffect()
    {
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
        }
        
        currentShakeCoroutine = StartCoroutine(ShakeEffectCoroutine());
    }
    
    /// <summary>
    /// 震動效果協程
    /// </summary>
    private IEnumerator ShakeEffectCoroutine()
    {
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            
            // 生成隨機偏移
            Vector3 randomOffset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0f
            );
            
            transform.localPosition = originalPosition + randomOffset;
            
            yield return null;
        }
        
        transform.localPosition = originalPosition;
        currentShakeCoroutine = null;
    }
    
    #endregion
    
    #region 粒子效果
    
    /// <summary>
    /// 播放粒子效果
    /// </summary>
    private void PlayParticleEffect()
    {
        if (clickParticles == null) return;
        
        clickParticles.Emit(particleCount);
    }
    
    #endregion
    
    #region 音效
    
    /// <summary>
    /// 播放音效反饋
    /// </summary>
    private void PlayAudioFeedback()
    {
        if (audioSource == null || clickSound == null) return;
        
        audioSource.volume = audioVolume;
        audioSource.PlayOneShot(clickSound);
    }
    
    #endregion
    
    #region 特效生成
    
    /// <summary>
    /// 生成點擊效果
    /// </summary>
    private void SpawnClickEffect()
    {
        if (clickEffectPrefab == null) return;
        
        GameObject effect = Instantiate(clickEffectPrefab, transform.position, Quaternion.identity);
        effect.transform.localScale = Vector3.one * clickEffectScale;
        
        // 自動銷毀
        Destroy(effect, clickEffectDuration);
    }
    
    #endregion
}