using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoveTide.Interaction
{
    /// <summary>
    /// 互動結果數據結構
    /// </summary>
    [System.Serializable]
    public class InteractionResult
    {
        public string InteractionType;
        public bool Success;
        public object Data;
        public string ErrorMessage;
        public float ExecutionTime;
        public Dictionary<string, object> CustomData;
        
        public InteractionResult()
        {
            CustomData = new Dictionary<string, object>();
        }
    }
    
    /// <summary>
    /// 互動配置數據結構
    /// </summary>
    [System.Serializable]
    public class InteractionData
    {
        [Header("基本設定")]
        public string Name;
        public string Description;
        
        [Header("條件檢查")]
        public int[] RequiredTimeSlots;
        public bool? RequireWorkTime;
        public int RequiredAffection;
        public int RequiredProgress;
        
        [Header("互動效果")]
        public int AffectionChange;
        public int MoneyChange;
        public int TimeCost;
        public int DialogID;
        
        [Header("音效")]
        public string StartSoundEffect;
        public string EndSoundEffect;
    }
    
    /// <summary>
    /// 互動配置管理器
    /// </summary>
    [CreateAssetMenu(fileName = "InteractionConfig", menuName = "LoveTide/Interaction Config")]
    public class InteractionConfig : ScriptableObject
    {
        [SerializeField] private List<InteractionDataEntry> interactions = new List<InteractionDataEntry>();
        
        [System.Serializable]
        public class InteractionDataEntry
        {
            public string Key;
            public InteractionData Data;
        }
        
        public InteractionData GetInteractionData(string key)
        {
            var entry = interactions.Find(x => x.Key == key);
            return entry?.Data;
        }
        
        public string[] GetAllInteractions()
        {
            return interactions.ConvertAll(x => x.Key).ToArray();
        }
    }
}