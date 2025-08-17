using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoveTide.Interaction
{
    /// <summary>
    /// 互動結果處理器 - 處理互動完成後的結果和效果
    /// 包括數值變化、旗標設置、音效播放等
    /// </summary>
    public class InteractionResultProcessor : MonoBehaviour
    {
        [Header("=== 處理設定 ===")]
        [SerializeField] private bool enableVisualEffects = true;
        [SerializeField] private bool enableSoundEffects = true;
        [SerializeField] private bool enableValueAnimation = true;
        
        [Header("=== 效果預製件 ===")]
        [SerializeField] private GameObject affectionUpEffect;
        [SerializeField] private GameObject moneyUpEffect;
        [SerializeField] private GameObject progressUpEffect;
        [SerializeField] private GameObject affectionDownEffect;
        
        [Header("=== 音效 ===")]
        [SerializeField] private AudioClip affectionUpSound;
        [SerializeField] private AudioClip moneyUpSound;
        [SerializeField] private AudioClip progressUpSound;
        [SerializeField] private AudioClip affectionDownSound;
        
        // 組件引用
        private NumericalRecords numericalRecords;
        private LoveTide.Core.NewGameManager gameManager;
        private AudioSource audioSource;
        
        // 處理佇列
        private Queue<InteractionResult> processingQueue = new Queue<InteractionResult>();
        private bool isProcessing = false;
        
        // 動畫控制
        private Dictionary<string, Coroutine> activeAnimations = new Dictionary<string, Coroutine>();
        
        #region Unity生命週期
        
        void Awake()
        {
            InitializeComponents();
        }
        
        void Start()
        {
            InitializeReferences();
        }
        
        void Update()
        {
            ProcessQueue();
        }
        
        #endregion
        
        #region 初始化
        
        void InitializeComponents()
        {
            // 獲取或添加AudioSource
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }
        
        void InitializeReferences()
        {
            // 獲取數值記錄引用
            numericalRecords = FindObjectOfType<NumericalRecords>();
            if (numericalRecords == null)
            {
                Debug.LogWarning("[InteractionResultProcessor] 找不到NumericalRecords組件");
            }
            
            // 獲取遊戲管理器引用
            gameManager = LoveTide.Core.NewGameManager.Instance;
            if (gameManager == null)
            {
                Debug.LogWarning("[InteractionResultProcessor] 找不到NewGameManager實例");
            }
        }
        
        #endregion
        
        #region 結果處理主要方法
        
        /// <summary>
        /// 處理互動結果
        /// </summary>
        public void ProcessInteractionResult(InteractionResult result)
        {
            if (result == null)
            {
                Debug.LogWarning("[InteractionResultProcessor] 互動結果為null");
                return;
            }
            
            // 添加到處理佇列
            processingQueue.Enqueue(result);
        }
        
        /// <summary>
        /// 立即處理互動結果（不排隊）
        /// </summary>
        public void ProcessInteractionResultImmediate(InteractionResult result)
        {
            if (result == null) return;
            
            StartCoroutine(ProcessResultCoroutine(result));
        }
        
        void ProcessQueue()
        {
            if (isProcessing || processingQueue.Count == 0) return;
            
            InteractionResult result = processingQueue.Dequeue();
            StartCoroutine(ProcessResultCoroutine(result));
        }
        
        IEnumerator ProcessResultCoroutine(InteractionResult result)
        {
            isProcessing = true;
            
            Debug.Log($"[InteractionResultProcessor] 開始處理互動結果: {result.InteractionType}");
            
            // 處理數值變化
            yield return StartCoroutine(ProcessValueChanges(result));
            
            // 處理旗標設置
            ProcessFlagChanges(result);
            
            // 處理時間推進
            ProcessTimeAdvancement(result);
            
            // 處理音效
            ProcessSoundEffects(result);
            
            // 處理視覺效果
            yield return StartCoroutine(ProcessVisualEffects(result));
            
            // 處理特殊邏輯
            ProcessSpecialLogic(result);
            
            Debug.Log($"[InteractionResultProcessor] 互動結果處理完成: {result.InteractionType}");
            
            isProcessing = false;
        }
        
        #endregion
        
        #region 數值變化處理
        
        IEnumerator ProcessValueChanges(InteractionResult result)
        {
            if (numericalRecords == null) yield break;
            
            var changes = ExtractValueChanges(result);
            
            if (changes.affectionChange != 0)
            {
                yield return StartCoroutine(ProcessAffectionChange(changes.affectionChange));
            }
            
            if (changes.moneyChange != 0)
            {
                yield return StartCoroutine(ProcessMoneyChange(changes.moneyChange));
            }
            
            if (changes.progressChange != 0)
            {
                yield return StartCoroutine(ProcessProgressChange(changes.progressChange));
            }
        }
        
        (int affectionChange, int moneyChange, int progressChange) ExtractValueChanges(InteractionResult result)
        {
            // 從結果數據中提取數值變化
            // 這裡可以根據InteractionResult的實際結構調整
            
            int affectionChange = 0;
            int moneyChange = 0;
            int progressChange = 0;
            
            // 根據互動類型設置預設的數值變化
            switch (result.InteractionType)
            {
                case "NormalTalk":
                    affectionChange = 1;
                    break;
                case "FlirtTalk":
                    affectionChange = 3;
                    break;
                case "HelpWork":
                    affectionChange = 2;
                    moneyChange = 50;
                    break;
                case "CatPlay":
                    affectionChange = 1;
                    break;
                case "InviteDrinking":
                    affectionChange = 4;
                    moneyChange = -20;
                    break;
                case "GoOutTogether":
                    affectionChange = 5;
                    moneyChange = -100;
                    break;
                case "SexualInteraction":
                    affectionChange = 10;
                    progressChange = 5;
                    break;
            }
            
            return (affectionChange, moneyChange, progressChange);
        }
        
        IEnumerator ProcessAffectionChange(int change)
        {
            if (change == 0) yield break;
            
            int oldValue = GetFieldValue<int>(numericalRecords, "aAffection", 0);
            int newValue = Mathf.Max(0, oldValue + change);
            
            if (enableValueAnimation)
            {
                yield return StartCoroutine(AnimateValueChange(
                    oldValue, newValue, 1f,
                    value => SetFieldValue<int>(numericalRecords, "aAffection", value)
                ));
            }
            else
            {
                SetFieldValue<int>(numericalRecords, "aAffection", newValue);
            }
            
            // 播放效果
            if (enableVisualEffects)
            {
                GameObject effect = change > 0 ? affectionUpEffect : affectionDownEffect;
                if (effect != null)
                {
                    ShowValueChangeEffect(effect, change, "💖");
                }
            }
            
            Debug.Log($"[數值變化] 好感度: {oldValue} → {newValue} ({change:+0;-0})");
        }
        
        IEnumerator ProcessMoneyChange(int change)
        {
            if (change == 0) yield break;
            
            int oldValue = GetFieldValue<int>(numericalRecords, "aMoney", 0);
            int newValue = oldValue + change;
            
            if (enableValueAnimation)
            {
                yield return StartCoroutine(AnimateValueChange(
                    oldValue, newValue, 0.8f,
                    value => SetFieldValue<int>(numericalRecords, "aMoney", value)
                ));
            }
            else
            {
                SetFieldValue<int>(numericalRecords, "aMoney", newValue);
            }
            
            // 播放效果
            if (enableVisualEffects && change > 0 && moneyUpEffect != null)
            {
                ShowValueChangeEffect(moneyUpEffect, change, "💰");
            }
            
            Debug.Log($"[數值變化] 金錢: {oldValue} → {newValue} ({change:+0;-0})");
        }
        
        IEnumerator ProcessProgressChange(int change)
        {
            if (change == 0) yield break;
            
            int oldValue = GetFieldValue<int>(numericalRecords, "aProgress", 0);
            int newValue = Mathf.Clamp(oldValue + change, 0, 100);
            
            if (enableValueAnimation)
            {
                yield return StartCoroutine(AnimateValueChange(
                    oldValue, newValue, 1.2f,
                    value => SetFieldValue<int>(numericalRecords, "aProgress", value)
                ));
            }
            else
            {
                SetFieldValue<int>(numericalRecords, "aProgress", newValue);
            }
            
            // 播放效果
            if (enableVisualEffects && progressUpEffect != null)
            {
                ShowValueChangeEffect(progressUpEffect, change, "📈");
            }
            
            Debug.Log($"[數值變化] 進度: {oldValue} → {newValue} ({change:+0;-0})");
        }
        
        IEnumerator AnimateValueChange(int startValue, int endValue, float duration, System.Action<int> setValue)
        {
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                t = Mathf.SmoothStep(0, 1, t); // 平滑曲線
                
                int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, t));
                setValue(currentValue);
                
                yield return null;
            }
            
            setValue(endValue);
        }
        
        #endregion
        
        #region 視覺效果處理
        
        void ShowValueChangeEffect(GameObject effectPrefab, int change, string icon)
        {
            if (effectPrefab == null) return;
            
            // 在螢幕上顯示數值變化效果
            GameObject effect = Instantiate(effectPrefab, transform);
            
            // 設置效果文字
            var textComponent = effect.GetComponentInChildren<UnityEngine.UI.Text>();
            if (textComponent != null)
            {
                textComponent.text = $"{icon} {change:+0;-0}";
                textComponent.color = change > 0 ? Color.green : Color.red;
            }
            
            // 播放動畫
            StartCoroutine(PlayValueEffectAnimation(effect));
        }
        
        IEnumerator PlayValueEffectAnimation(GameObject effect)
        {
            if (effect == null) yield break;
            
            RectTransform rectTransform = effect.GetComponent<RectTransform>();
            if (rectTransform == null) yield break;
            
            // 動畫參數
            Vector3 startPos = rectTransform.localPosition;
            Vector3 endPos = startPos + Vector3.up * 100f;
            Vector3 startScale = Vector3.one * 0.5f;
            Vector3 endScale = Vector3.one * 1.2f;
            
            float duration = 2f;
            float elapsed = 0f;
            
            rectTransform.localScale = startScale;
            
            // 第一階段：放大並上移
            while (elapsed < duration * 0.3f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration * 0.3f);
                
                rectTransform.localPosition = Vector3.Lerp(startPos, startPos + Vector3.up * 30f, t);
                rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
                
                yield return null;
            }
            
            // 第二階段：淡出並繼續上移
            var canvasGroup = effect.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = effect.AddComponent<CanvasGroup>();
            
            float fadeStart = elapsed;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float totalT = elapsed / duration;
                float fadeT = (elapsed - fadeStart) / (duration - fadeStart);
                
                rectTransform.localPosition = Vector3.Lerp(startPos, endPos, totalT);
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeT);
                
                yield return null;
            }
            
            // 銷毀效果物件
            Destroy(effect);
        }
        
        IEnumerator ProcessVisualEffects(InteractionResult result)
        {
            if (!enableVisualEffects) yield break;
            
            // 根據互動類型播放特定的視覺效果
            switch (result.InteractionType)
            {
                case "SexualInteraction":
                    // 特殊互動的特殊效果
                    yield return StartCoroutine(PlaySpecialInteractionEffect());
                    break;
            }
        }
        
        IEnumerator PlaySpecialInteractionEffect()
        {
            // 播放特殊互動的視覺效果
            Debug.Log("[視覺效果] 播放特殊互動效果");
            yield return new WaitForSeconds(1f);
        }
        
        #endregion
        
        #region 音效處理
        
        void ProcessSoundEffects(InteractionResult result)
        {
            if (!enableSoundEffects || audioSource == null) return;
            
            AudioClip clipToPlay = GetSoundClipForInteraction(result.InteractionType);
            
            if (clipToPlay != null)
            {
                audioSource.PlayOneShot(clipToPlay);
                Debug.Log($"[音效] 播放互動音效: {result.InteractionType}");
            }
        }
        
        AudioClip GetSoundClipForInteraction(string interactionType)
        {
            switch (interactionType)
            {
                case "FlirtTalk":
                case "InviteDrinking":
                case "GoOutTogether":
                case "SexualInteraction":
                    return affectionUpSound;
                case "HelpWork":
                    return moneyUpSound;
                default:
                    return affectionUpSound; // 預設音效
            }
        }
        
        #endregion
        
        #region 旗標和特殊邏輯處理
        
        void ProcessFlagChanges(InteractionResult result)
        {
            // 根據互動類型設置旗標
            switch (result.InteractionType)
            {
                case "SexualInteraction":
                    SetFlag("FirstSexualInteraction", true);
                    break;
                case "GoOutTogether":
                    SetFlag("WentOutTogether", true);
                    break;
            }
        }
        
        void ProcessTimeAdvancement(InteractionResult result)
        {
            if (gameManager == null) return;
            
            // 根據互動類型推進時間
            int timeAdvancement = GetTimeAdvancementForInteraction(result.InteractionType);
            
            if (timeAdvancement > 0)
            {
                gameManager.AdvanceTime(timeAdvancement);
                Debug.Log($"[時間推進] {result.InteractionType} 推進 {timeAdvancement} 時段");
            }
        }
        
        int GetTimeAdvancementForInteraction(string interactionType)
        {
            switch (interactionType)
            {
                case "HelpWork":
                    return 1;
                case "GoOutTogether":
                    return 2;
                case "SexualInteraction":
                    return 1;
                case "InviteDrinking":
                    return 1;
                default:
                    return 0; // 不推進時間
            }
        }
        
        void ProcessSpecialLogic(InteractionResult result)
        {
            // 處理特殊的互動邏輯
            switch (result.InteractionType)
            {
                case "SexualInteraction":
                    ProcessSexualInteractionLogic();
                    break;
                case "CatPlay":
                    ProcessCatInteractionLogic();
                    break;
            }
        }
        
        void ProcessSexualInteractionLogic()
        {
            // 特殊互動的額外邏輯
            Debug.Log("[特殊邏輯] 處理親密互動後續");
            
            // 可能觸發特殊劇情或解鎖新內容
            if (numericalRecords != null && GetFieldValue<int>(numericalRecords, "aAffection", 0) >= 80)
            {
                SetFlag("HighAffectionUnlocked", true);
            }
        }
        
        void ProcessCatInteractionLogic()
        {
            // 貓咪互動的邏輯
            Debug.Log("[特殊邏輯] 處理貓咪互動");
            
            // 可能獲得魚類或觸發釣魚相關劇情
            int catInteractionCount = PlayerPrefs.GetInt("CatInteractionCount", 0) + 1;
            PlayerPrefs.SetInt("CatInteractionCount", catInteractionCount);
            
            if (catInteractionCount >= 5)
            {
                SetFlag("CatFriendshipUnlocked", true);
            }
        }
        
        void SetFlag(string flagName, bool value)
        {
            PlayerPrefs.SetInt($"Flag_{flagName}", value ? 1 : 0);
            Debug.Log($"[旗標設置] {flagName} = {value}");
        }
        
        #endregion
        
        #region 公開API
        
        /// <summary>
        /// 停止所有處理中的動畫
        /// </summary>
        public void StopAllAnimations()
        {
            foreach (var kvp in activeAnimations)
            {
                if (kvp.Value != null)
                {
                    StopCoroutine(kvp.Value);
                }
            }
            activeAnimations.Clear();
        }
        
        /// <summary>
        /// 設置效果開關
        /// </summary>
        public void SetEffectsEnabled(bool visual, bool sound, bool animation)
        {
            enableVisualEffects = visual;
            enableSoundEffects = sound;
            enableValueAnimation = animation;
        }
        
        /// <summary>
        /// 清空處理佇列
        /// </summary>
        public void ClearProcessingQueue()
        {
            processingQueue.Clear();
        }
        
        /// <summary>
        /// 獲取當前佇列長度
        /// </summary>
        public int GetQueueLength()
        {
            return processingQueue.Count;
        }
        
        #endregion
        
        #region 反射輔助方法
        
        /// <summary>
        /// 通用的字段值獲取方法
        /// </summary>
        T GetFieldValue<T>(object target, string fieldName, T defaultValue)
        {
            if (target == null) return defaultValue;
            
            try
            {
                var field = target.GetType().GetField(fieldName);
                if (field != null)
                    return (T)field.GetValue(target);
                    
                var property = target.GetType().GetProperty(fieldName);
                if (property != null)
                    return (T)property.GetValue(target);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[InteractionResultProcessor] 無法獲取 {fieldName}: {e.Message}");
            }
            
            return defaultValue;
        }
        
        /// <summary>
        /// 通用的字段值設置方法
        /// </summary>
        void SetFieldValue<T>(object target, string fieldName, T value)
        {
            if (target == null) return;
            
            try
            {
                var field = target.GetType().GetField(fieldName);
                if (field != null)
                {
                    field.SetValue(target, value);
                    return;
                }
                    
                var property = target.GetType().GetProperty(fieldName);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(target, value);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[InteractionResultProcessor] 無法設置 {fieldName}: {e.Message}");
            }
        }
        
        #endregion
        
        #region 清理
        
        void OnDestroy()
        {
            StopAllAnimations();
            ClearProcessingQueue();
        }
        
        #endregion
    }
}