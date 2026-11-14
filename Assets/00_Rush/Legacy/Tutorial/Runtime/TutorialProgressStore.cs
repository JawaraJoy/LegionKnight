using System;
using System.Collections.Generic;
using LegionKnight;
using UnityEngine;

namespace Rush
{
    public interface ITutorialProgressStore
    {
        bool Contains(string key);
        T Read<T>(string key);
        void Write<T>(string key, T value);
        void Transact(Action changes);
    }

    public sealed class LocalTutorialProgressStore : ITutorialProgressStore
    {
        private readonly LocalSave m_Save;
        public LocalTutorialProgressStore(LocalSave save) => m_Save = save ?? throw new ArgumentNullException(nameof(save));
        public bool Contains(string key) => m_Save.HasData(key);
        public T Read<T>(string key) => m_Save.GetDataValue<T>(key);
        public void Write<T>(string key, T value) => m_Save.SaveData(key, value);
        public void Transact(Action changes) => m_Save.ExecuteTransaction(changes);
    }

    [Serializable]
    public sealed class TutorialProgressRecord
    {
        private const int c_CurrentVersion = 2;
        [SerializeField] private int m_Version = c_CurrentVersion;
        [SerializeField] private bool m_Unlocked;
        [SerializeField] private bool m_Completed;
        [SerializeField] private bool m_InProgress;
        [SerializeField] private bool m_LegacyCompletionUnverified;
        [SerializeField] private string m_StepId = string.Empty;
        [SerializeField] private List<string> m_CompletedSteps = new();
        [SerializeField] private List<string> m_GrantedRewards = new();
        public int Version => m_Version;
        public bool Unlocked => m_Unlocked;
        public bool Completed => m_Completed;
        public bool InProgress => m_InProgress;
        public bool LegacyCompletionUnverified => m_LegacyCompletionUnverified;
        public string StepId => m_StepId;
        public bool HasCompletedStep(string id) => m_CompletedSteps != null && m_CompletedSteps.Contains(id);
        public bool HasGrantedReward(string id) => m_GrantedRewards != null && m_GrantedRewards.Contains(id);

        public TutorialProgressRecord(bool unlocked, bool legacyCompletionUnverified)
        {
            m_Unlocked = unlocked;
            m_LegacyCompletionUnverified = legacyCompletionUnverified;
        }

        public void SetUnlocked(bool value) => m_Unlocked = value;
        public void SetCompleted(bool value)
        {
            m_Completed = value;
            m_InProgress = false;
            m_StepId = string.Empty;
            if (!value) m_CompletedSteps = new List<string>();
            if (value) m_LegacyCompletionUnverified = false;
        }
        public void SetCheckpoint(string id)
        {
            m_StepId = id ?? string.Empty;
            m_InProgress = true;
        }
        public void AddCompletedStep(string id)
        {
            m_CompletedSteps ??= new List<string>();
            if (!m_CompletedSteps.Contains(id)) m_CompletedSteps.Add(id);
        }
        public void AddGrantedReward(string id)
        {
            m_GrantedRewards ??= new List<string>();
            if (!m_GrantedRewards.Contains(id)) m_GrantedRewards.Add(id);
        }
    }

    [Serializable]
    public sealed class TutorialQueueRecord
    {
        [SerializeField] private List<string> m_Ids = new();
        [SerializeField] private List<string> m_DeferredIds = new();
        public IReadOnlyList<string> Ids => m_Ids;
        public IReadOnlyList<string> DeferredIds => m_DeferredIds;
        public TutorialQueueRecord(IEnumerable<string> ids, IEnumerable<string> deferredIds)
        {
            m_Ids = new List<string>(ids);
            m_DeferredIds = new List<string>(deferredIds);
        }
    }

    public enum TutorialAction
    {
        None,
        HeroSelected,
        HeroUpgraded,
        HeroUsed,
        CardSelected,
        CardEquipped,
        DrawRequested,
        DrawCommitted,
        StageStarted
    }
}
