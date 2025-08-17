using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoveTide.UI
{
    /// <summary>
    /// UI動畫控制器 - 管理UI元素的動畫效果
    /// 提供淡入淡出、縮放、滑動等常用UI動畫
    /// </summary>
    public class UIAnimationController : MonoBehaviour
    {
        [Header("=== 動畫設定 ===")]
        [SerializeField] private float defaultFadeDuration = 0.3f;
        [SerializeField] private float defaultScaleDuration = 0.25f;
        [SerializeField] private float defaultSlideDuration = 0.4f;
        
        [Header("=== 動畫曲線 ===")]
        [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        // UI管理器引用
        private NewUIManager uiManager;
        
        // 動畫協程追蹤
        private Dictionary<GameObject, Coroutine> activeAnimations = new Dictionary<GameObject, Coroutine>();
        
        #region 初始化
        
        public void Initialize(NewUIManager manager)
        {
            uiManager = manager;
            Debug.Log("[UIAnimationController] UI動畫控制器初始化");
        }
        
        #endregion
        
        #region 淡入淡出動畫
        
        /// <summary>
        /// 淡入動畫
        /// </summary>
        public Coroutine FadeIn(GameObject target, float duration = -1, Action onComplete = null)
        {
            if (duration < 0) duration = defaultFadeDuration;
            
            StopExistingAnimation(target);
            
            Coroutine animation = StartCoroutine(DoFadeIn(target, duration, onComplete));
            activeAnimations[target] = animation;
            
            return animation;
        }
        
        /// <summary>
        /// 淡出動畫
        /// </summary>
        public Coroutine FadeOut(GameObject target, float duration = -1, Action onComplete = null)
        {
            if (duration < 0) duration = defaultFadeDuration;
            
            StopExistingAnimation(target);
            
            Coroutine animation = StartCoroutine(DoFadeOut(target, duration, onComplete));
            activeAnimations[target] = animation;
            
            return animation;
        }
        
        IEnumerator DoFadeIn(GameObject target, float duration, Action onComplete)
        {
            CanvasGroup canvasGroup = GetOrAddCanvasGroup(target);
            
            float startAlpha = canvasGroup.alpha;
            float targetAlpha = 1f;
            float elapsed = 0f;
            
            target.SetActive(true);
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float curveValue = fadeInCurve.Evaluate(t);
                
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, curveValue);
                yield return null;
            }
            
            canvasGroup.alpha = targetAlpha;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            
            activeAnimations.Remove(target);
            onComplete?.Invoke();
        }
        
        IEnumerator DoFadeOut(GameObject target, float duration, Action onComplete)
        {
            CanvasGroup canvasGroup = GetOrAddCanvasGroup(target);
            
            float startAlpha = canvasGroup.alpha;
            float targetAlpha = 0f;
            float elapsed = 0f;
            
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float curveValue = fadeOutCurve.Evaluate(t);
                
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, curveValue);
                yield return null;
            }
            
            canvasGroup.alpha = targetAlpha;
            target.SetActive(false);
            
            activeAnimations.Remove(target);
            onComplete?.Invoke();
        }
        
        #endregion
        
        #region 縮放動畫
        
        /// <summary>
        /// 縮放進入動畫
        /// </summary>
        public Coroutine ScaleIn(GameObject target, float duration = -1, Action onComplete = null)
        {
            if (duration < 0) duration = defaultScaleDuration;
            
            StopExistingAnimation(target);
            
            Coroutine animation = StartCoroutine(DoScaleIn(target, duration, onComplete));
            activeAnimations[target] = animation;
            
            return animation;
        }
        
        /// <summary>
        /// 縮放退出動畫
        /// </summary>
        public Coroutine ScaleOut(GameObject target, float duration = -1, Action onComplete = null)
        {
            if (duration < 0) duration = defaultScaleDuration;
            
            StopExistingAnimation(target);
            
            Coroutine animation = StartCoroutine(DoScaleOut(target, duration, onComplete));
            activeAnimations[target] = animation;
            
            return animation;
        }
        
        IEnumerator DoScaleIn(GameObject target, float duration, Action onComplete)
        {
            Transform targetTransform = target.transform;
            Vector3 startScale = Vector3.zero;
            Vector3 targetScale = Vector3.one;
            float elapsed = 0f;
            
            target.SetActive(true);
            targetTransform.localScale = startScale;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float curveValue = scaleCurve.Evaluate(t);
                
                targetTransform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
                yield return null;
            }
            
            targetTransform.localScale = targetScale;
            
            activeAnimations.Remove(target);
            onComplete?.Invoke();
        }
        
        IEnumerator DoScaleOut(GameObject target, float duration, Action onComplete)
        {
            Transform targetTransform = target.transform;
            Vector3 startScale = targetTransform.localScale;
            Vector3 targetScale = Vector3.zero;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float curveValue = scaleCurve.Evaluate(1f - t);
                
                targetTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }
            
            targetTransform.localScale = targetScale;
            target.SetActive(false);
            
            activeAnimations.Remove(target);
            onComplete?.Invoke();
        }
        
        #endregion
        
        #region 滑動動畫
        
        /// <summary>
        /// 滑入動畫
        /// </summary>
        public Coroutine SlideIn(GameObject target, Vector2 fromDirection, float duration = -1, Action onComplete = null)
        {
            if (duration < 0) duration = defaultSlideDuration;
            
            StopExistingAnimation(target);
            
            Coroutine animation = StartCoroutine(DoSlideIn(target, fromDirection, duration, onComplete));
            activeAnimations[target] = animation;
            
            return animation;
        }
        
        /// <summary>
        /// 滑出動畫
        /// </summary>
        public Coroutine SlideOut(GameObject target, Vector2 toDirection, float duration = -1, Action onComplete = null)
        {
            if (duration < 0) duration = defaultSlideDuration;
            
            StopExistingAnimation(target);
            
            Coroutine animation = StartCoroutine(DoSlideOut(target, toDirection, duration, onComplete));
            activeAnimations[target] = animation;
            
            return animation;
        }
        
        IEnumerator DoSlideIn(GameObject target, Vector2 fromDirection, float duration, Action onComplete)
        {
            RectTransform rectTransform = target.GetComponent<RectTransform>();
            if (rectTransform == null) yield break;
            
            Vector2 targetPosition = rectTransform.anchoredPosition;
            Vector2 startPosition = targetPosition + fromDirection * Screen.width;
            float elapsed = 0f;
            
            target.SetActive(true);
            rectTransform.anchoredPosition = startPosition;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                t = Mathf.SmoothStep(0, 1, t);
                
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
            
            rectTransform.anchoredPosition = targetPosition;
            
            activeAnimations.Remove(target);
            onComplete?.Invoke();
        }
        
        IEnumerator DoSlideOut(GameObject target, Vector2 toDirection, float duration, Action onComplete)
        {
            RectTransform rectTransform = target.GetComponent<RectTransform>();
            if (rectTransform == null) yield break;
            
            Vector2 startPosition = rectTransform.anchoredPosition;
            Vector2 targetPosition = startPosition + toDirection * Screen.width;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                t = Mathf.SmoothStep(0, 1, t);
                
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
            
            rectTransform.anchoredPosition = targetPosition;
            target.SetActive(false);
            
            activeAnimations.Remove(target);
            onComplete?.Invoke();
        }
        
        #endregion
        
        #region 組合動畫
        
        /// <summary>
        /// 彈跳進入動畫（縮放+淡入）
        /// </summary>
        public Coroutine BounceIn(GameObject target, float duration = -1, Action onComplete = null)
        {
            if (duration < 0) duration = defaultScaleDuration;
            
            StopExistingAnimation(target);
            
            Coroutine animation = StartCoroutine(DoBounceIn(target, duration, onComplete));
            activeAnimations[target] = animation;
            
            return animation;
        }
        
        IEnumerator DoBounceIn(GameObject target, float duration, Action onComplete)
        {
            Transform targetTransform = target.transform;
            CanvasGroup canvasGroup = GetOrAddCanvasGroup(target);
            
            Vector3 startScale = Vector3.zero;
            Vector3 overshootScale = Vector3.one * 1.1f;
            Vector3 targetScale = Vector3.one;
            
            target.SetActive(true);
            targetTransform.localScale = startScale;
            canvasGroup.alpha = 0f;
            
            float halfDuration = duration * 0.5f;
            float elapsed = 0f;
            
            // 第一階段：縮放到超過目標大小 + 淡入
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                t = scaleCurve.Evaluate(t);
                
                targetTransform.localScale = Vector3.Lerp(startScale, overshootScale, t);
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }
            
            elapsed = 0f;
            
            // 第二階段：回彈到正常大小
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                t = Mathf.SmoothStep(0, 1, t);
                
                targetTransform.localScale = Vector3.Lerp(overshootScale, targetScale, t);
                yield return null;
            }
            
            targetTransform.localScale = targetScale;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            
            activeAnimations.Remove(target);
            onComplete?.Invoke();
        }
        
        #endregion
        
        #region 輔助方法
        
        CanvasGroup GetOrAddCanvasGroup(GameObject target)
        {
            CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = target.AddComponent<CanvasGroup>();
            }
            return canvasGroup;
        }
        
        void StopExistingAnimation(GameObject target)
        {
            if (activeAnimations.ContainsKey(target))
            {
                if (activeAnimations[target] != null)
                {
                    StopCoroutine(activeAnimations[target]);
                }
                activeAnimations.Remove(target);
            }
        }
        
        /// <summary>
        /// 停止目標物件的所有動畫
        /// </summary>
        public void StopAnimation(GameObject target)
        {
            StopExistingAnimation(target);
        }
        
        /// <summary>
        /// 停止所有動畫
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
        /// 檢查物件是否正在播放動畫
        /// </summary>
        public bool IsAnimating(GameObject target)
        {
            return activeAnimations.ContainsKey(target) && activeAnimations[target] != null;
        }
        
        #endregion
        
        #region 清理
        
        void OnDestroy()
        {
            StopAllAnimations();
        }
        
        #endregion
    }
}