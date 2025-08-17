using System;
using System.Collections;
using UnityEngine;

namespace LoveTide.Core
{
    /// <summary>
    /// 場景狀態管理器 - 管理場景狀態切換和場景物件的顯示隱藏
    /// 負責工作場景和宿舍場景之間的切換邏輯
    /// </summary>
    public class SceneStateManager : MonoBehaviour
    {
        [Header("=== 場景狀態 ===")]
        [SerializeField] private SceneLocation currentLocation = SceneLocation.WorkPlace;
        [SerializeField] private bool isWorkTime = true;
        [SerializeField] private bool isVacation = false;
        [SerializeField] private int currentTimeSlot = 1;
        
        [Header("=== 場景物件引用 ===")]
        [SerializeField] private GameObject workPlaceObjects;
        [SerializeField] private GameObject dormitoryObjects;
        [SerializeField] private GameObject yukaRoomObjects; // 已棄用，但保留兼容性
        
        [Header("=== 背景控制 ===")]
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private Sprite workPlaceBackground;
        [SerializeField] private Sprite dormitoryBackground;
        
        [Header("=== 角色位置管理 ===")]
        [SerializeField] private Transform yukaWorkPosition;
        [SerializeField] private Transform yukaDormPosition;
        
        // 核心管理器引用
        private NewGameManager gameManager;
        private ActorManagerTest actorManager;
        
        // 公開屬性
        public SceneLocation CurrentLocation => currentLocation;
        public bool IsWorkTime => isWorkTime;
        public bool IsVacation => isVacation;
        
        // 事件系統
        public static event Action<SceneLocation> OnSceneLocationChanged;
        public static event Action<bool, bool> OnSceneStateChanged;
        
        #region 場景位置枚舉
        
        public enum SceneLocation
        {
            WorkPlace,     // 工作場所
            DormHall,      // 宿舍大廳
            YukaRoom       // 由香房間 (已棄用，重製版統一使用DormHall)
        }
        
        #endregion
        
        #region Unity生命週期
        
        void Awake()
        {
            InitializeReferences();
        }
        
        void Start()
        {
            StartCoroutine(InitializeSceneState());
        }
        
        #endregion
        
        #region 初始化
        
        void InitializeReferences()
        {
            if (gameManager == null)
                gameManager = NewGameManager.Instance;
                
            if (actorManager == null)
                actorManager = FindObjectOfType<ActorManagerTest>();
        }
        
        public void Initialize(NewGameManager manager)
        {
            gameManager = manager;
            Debug.Log("[SceneStateManager] 場景狀態管理器初始化");
        }
        
        IEnumerator InitializeSceneState()
        {
            // 等待遊戲管理器初始化完成
            while (gameManager == null || !gameManager.IsGameInitialized())
            {
                yield return null;
            }
            
            // 驗證場景物件引用
            ValidateSceneObjects();
            
            // 設置初始場景狀態
            SetInitialSceneState();
            
            // 訂閱事件
            SubscribeToEvents();
            
            Debug.Log("[SceneStateManager] 場景狀態管理器初始化完成");
        }
        
        void ValidateSceneObjects()
        {
            if (workPlaceObjects == null)
                Debug.LogWarning("[SceneStateManager] WorkPlaceObjects引用缺失");
                
            if (dormitoryObjects == null)
                Debug.LogWarning("[SceneStateManager] DormitoryObjects引用缺失");
                
            if (backgroundRenderer == null)
                Debug.LogWarning("[SceneStateManager] BackgroundRenderer引用缺失");
        }
        
        void SetInitialSceneState()
        {
            // 根據當前時間設置初始場景
            var timeInfo = gameManager.GetCurrentTime();
            currentTimeSlot = timeInfo.timer;
            
            bool initialWorkTime = DetermineWorkTime(currentTimeSlot);
            UpdateSceneState(initialWorkTime, false);
        }
        
        void SubscribeToEvents()
        {
            if (gameManager != null)
            {
                NewGameManager.OnWorkTimeChanged += OnWorkTimeChanged;
            }
        }
        
        #endregion
        
        #region 場景狀態更新
        
        public void UpdateSceneState(bool newIsWorkTime, bool newIsVacation)
        {
            bool stateChanged = false;
            
            if (isWorkTime != newIsWorkTime)
            {
                isWorkTime = newIsWorkTime;
                stateChanged = true;
            }
            
            if (isVacation != newIsVacation)
            {
                isVacation = newIsVacation;
                stateChanged = true;
            }
            
            if (stateChanged)
            {
                ApplySceneStateChanges();
                OnSceneStateChanged?.Invoke(isWorkTime, isVacation);
                
                Debug.Log($"[SceneStateManager] 場景狀態更新 - 工作時間: {isWorkTime}, 假期: {isVacation}");
            }
        }
        
        void ApplySceneStateChanges()
        {
            if (isVacation)
            {
                HandleVacationMode();
            }
            else if (isWorkTime)
            {
                SwitchToWorkPlace();
            }
            else
            {
                SwitchToDormitory();
            }
        }
        
        void OnWorkTimeChanged(bool workTime)
        {
            UpdateSceneState(workTime, isVacation);
        }
        
        #endregion
        
        #region 場景切換邏輯
        
        void SwitchToWorkPlace()
        {
            if (currentLocation == SceneLocation.WorkPlace) return;
            
            Debug.Log("[SceneStateManager] 切換到工作場所");
            
            currentLocation = SceneLocation.WorkPlace;
            
            // 顯示/隱藏場景物件
            SetSceneObjectsActive(workPlaceObjects, true);
            SetSceneObjectsActive(dormitoryObjects, false);
            
            // 更換背景
            SetBackground(workPlaceBackground);
            
            // 移動角色到工作位置
            MoveYukaToPosition(yukaWorkPosition);
            
            // 通知場景變更
            OnSceneLocationChanged?.Invoke(currentLocation);
        }
        
        void SwitchToDormitory()
        {
            // 重製版重要變更：所有宿舍時間都使用大廳場景
            SceneLocation targetLocation = SceneLocation.DormHall;
            
            if (currentLocation == targetLocation) return;
            
            Debug.Log("[SceneStateManager] 切換到宿舍大廳");
            
            currentLocation = targetLocation;
            
            // 顯示/隱藏場景物件
            SetSceneObjectsActive(workPlaceObjects, false);
            SetSceneObjectsActive(dormitoryObjects, true);
            
            // 隱藏由香房間物件（重製版不再使用）
            if (yukaRoomObjects != null)
            {
                SetSceneObjectsActive(yukaRoomObjects, false);
            }
            
            // 更換背景
            SetBackground(dormitoryBackground);
            
            // 移動角色到宿舍位置
            MoveYukaToPosition(yukaDormPosition);
            
            // 通知場景變更
            OnSceneLocationChanged?.Invoke(currentLocation);
        }
        
        void HandleVacationMode()
        {
            Debug.Log("[SceneStateManager] 假期模式 - 保持宿舍場景");
            
            // 假期時保持在宿舍場景
            SwitchToDormitory();
        }
        
        #endregion
        
        #region 時間管理
        
        public void OnTimeAdvanced(int newTimeSlot)
        {
            currentTimeSlot = newTimeSlot;
            
            bool newIsWorkTime = DetermineWorkTime(newTimeSlot);
            UpdateSceneState(newIsWorkTime, isVacation);
            
            // 處理特殊時段邏輯
            HandleSpecialTimeSlots(newTimeSlot);
        }
        
        bool DetermineWorkTime(int timeSlot)
        {
            // 時間1-6為工作時間，7-9為宿舍時間
            return timeSlot >= 1 && timeSlot <= 6;
        }
        
        void HandleSpecialTimeSlots(int timeSlot)
        {
            switch (timeSlot)
            {
                case 7:
                case 8:
                case 9:
                    // 夜晚時段：確保由香在宿舍大廳
                    // 重製版重要變更：不再有由香房間
                    EnsureYukaInDormHall();
                    break;
            }
        }
        
        void EnsureYukaInDormHall()
        {
            if (currentLocation != SceneLocation.DormHall)
            {
                SwitchToDormitory();
            }
            
            // 確保由香在正確位置
            MoveYukaToPosition(yukaDormPosition);
        }
        
        #endregion
        
        #region 角色位置管理
        
        void MoveYukaToPosition(Transform targetPosition)
        {
            if (actorManager == null || targetPosition == null) return;
            
            // 獲取由香的Transform（通過ActorManager）
            var yukaTransform = GetYukaTransform();
            if (yukaTransform != null)
            {
                // 平滑移動到目標位置
                StartCoroutine(SmoothMoveToPosition(yukaTransform, targetPosition.position));
            }
        }
        
        Transform GetYukaTransform()
        {
            if (actorManager != null)
            {
                // 嘗試從ActorManager獲取由香的Transform
                // 這裡可能需要根據實際的ActorManager API調整
                return actorManager.transform; // 暫時返回ActorManager的transform
            }
            return null;
        }
        
        IEnumerator SmoothMoveToPosition(Transform character, Vector3 targetPosition)
        {
            Vector3 startPosition = character.position;
            float moveTime = 1.0f;
            float elapsed = 0f;
            
            while (elapsed < moveTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / moveTime;
                
                // 使用平滑曲線
                t = Mathf.SmoothStep(0, 1, t);
                
                character.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
            
            character.position = targetPosition;
            Debug.Log($"[SceneStateManager] 由香移動到位置: {targetPosition}");
        }
        
        #endregion
        
        #region 場景物件控制
        
        void SetSceneObjectsActive(GameObject sceneObject, bool active)
        {
            if (sceneObject != null)
            {
                sceneObject.SetActive(active);
            }
        }
        
        void SetBackground(Sprite backgroundSprite)
        {
            if (backgroundRenderer != null && backgroundSprite != null)
            {
                backgroundRenderer.sprite = backgroundSprite;
                Debug.Log($"[SceneStateManager] 背景更換為: {backgroundSprite.name}");
            }
        }
        
        #endregion
        
        #region 公開API
        
        /// <summary>
        /// 強制切換到指定場景
        /// </summary>
        public void ForceSceneChange(SceneLocation targetLocation)
        {
            switch (targetLocation)
            {
                case SceneLocation.WorkPlace:
                    SwitchToWorkPlace();
                    break;
                case SceneLocation.DormHall:
                    SwitchToDormitory();
                    break;
                case SceneLocation.YukaRoom:
                    // 重製版：由香房間重定向到宿舍大廳
                    Debug.LogWarning("[SceneStateManager] 由香房間已棄用，重定向到宿舍大廳");
                    SwitchToDormitory();
                    break;
            }
        }
        
        /// <summary>
        /// 檢查當前是否為特定場景
        /// </summary>
        public bool IsCurrentScene(SceneLocation location)
        {
            // 重製版：由香房間檢查重定向到宿舍大廳
            if (location == SceneLocation.YukaRoom)
            {
                return currentLocation == SceneLocation.DormHall;
            }
            
            return currentLocation == location;
        }
        
        /// <summary>
        /// 獲取由香當前位置
        /// </summary>
        public Vector3 GetYukaCurrentPosition()
        {
            var yukaTransform = GetYukaTransform();
            return yukaTransform != null ? yukaTransform.position : Vector3.zero;
        }
        
        /// <summary>
        /// 設置假期狀態
        /// </summary>
        public void SetVacationMode(bool vacation)
        {
            UpdateSceneState(isWorkTime, vacation);
        }
        
        #endregion
        
        #region 調試功能
        
        [Header("=== 調試功能 ===")]
        [SerializeField] private bool enableDebugGizmos = true;
        
        void OnDrawGizmos()
        {
            if (!enableDebugGizmos) return;
            
            // 繪製角色位置標記
            if (yukaWorkPosition != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(yukaWorkPosition.position, 0.5f);
                Gizmos.DrawIcon(yukaWorkPosition.position, "d_GameObject Icon", true);
            }
            
            if (yukaDormPosition != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(yukaDormPosition.position, 0.5f);
                Gizmos.DrawIcon(yukaDormPosition.position, "d_GameObject Icon", true);
            }
        }
        
        #endregion
        
        #region 事件清理
        
        void OnDestroy()
        {
            // 取消事件訂閱
            if (gameManager != null)
            {
                NewGameManager.OnWorkTimeChanged -= OnWorkTimeChanged;
            }
            
            // 清理靜態事件
            OnSceneLocationChanged = null;
            OnSceneStateChanged = null;
        }
        
        #endregion
    }
}