using System;
using MoreMountains.Tools;
using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [Serializable]
    public class TutorialContent
    {
        private const string c_RecordPrefix = "tutorial.v2.";
        [SerializeField] private TutorialDefinition m_Definition;
        [SerializeField, MMReadOnly] private bool m_IsDone;
        [SerializeField, MMReadOnly] private bool m_IsUnlocked;
        [SerializeField] private UnityEvent<TutorialDefinition> m_OnTutorialStart = new();
        [SerializeField] private UnityEvent<TutorialDefinition> m_OnTutorialEnd = new();
        private ITutorialProgressStore m_Store;
        private TutorialProgressRecord m_Record;
        public TutorialDefinition Definition => m_Definition;
        public bool IsDone => m_Record != null && m_Record.Completed;
        public bool IsUnlocked => m_Record != null && m_Record.Unlocked;
        public bool InProgress => m_Record != null && m_Record.InProgress;
        public string CheckpointStepId => m_Record?.StepId ?? string.Empty;
        public bool LegacyCompletionUnverified => m_Record != null && m_Record.LegacyCompletionUnverified;
        public UnityEvent<TutorialDefinition> OnTutorialStart => m_OnTutorialStart;
        public UnityEvent<TutorialDefinition> OnTutorialEnd => m_OnTutorialEnd;
        private string RecordKey => c_RecordPrefix + m_Definition.Id;
        private string DoneKey => m_Definition.Id + "isdone";
        private string UnlockKey => m_Definition.Id + "unlock";

        public TutorialContent() { }
        public TutorialContent(TutorialDefinition definition) => m_Definition = definition;
        public void Init() => InitializeInternal(new LocalTutorialProgressStore(UnityService.Instance.LocalSave));
        public void Initialize(ITutorialProgressStore store) => InitializeInternal(store);
        private void InitializeInternal(ITutorialProgressStore store)
        {
            m_Store = store ?? throw new ArgumentNullException(nameof(store));
            if (m_Definition == null || string.IsNullOrWhiteSpace(m_Definition.Id)) return;
            m_Record = store.Contains(RecordKey) ? store.Read<TutorialProgressRecord>(RecordKey) : null;
            if (m_Record == null)
            {
                bool legacyDone = store.Contains(DoneKey) && store.Read<bool>(DoneKey);
                bool legacyUnlocked = store.Contains(UnlockKey) && store.Read<bool>(UnlockKey);
                // Old unlock writes corrupted 'isdone'. Preserve access, but do not invent completion.
                m_Record = new TutorialProgressRecord(m_Definition.UnlockedAtFirst || legacyUnlocked || legacyDone, legacyDone);
                store.Transact(SaveInternal);
            }
            RefreshFlagsInternal();
        }

        public bool HasCompletedStep(string id) => m_Record != null && m_Record.HasCompletedStep(id);
        public bool HasGrantedReward(string id) => m_Record != null && m_Record.HasGrantedReward(id);
        public void SetIsDone(bool value) => MutateInternal(() => m_Record.SetCompleted(value));
        public void SetIsUnlocked(bool value) => MutateInternal(() => m_Record.SetUnlocked(value));
        public void SetCheckpoint(string id) => MutateInternal(() => m_Record.SetCheckpoint(id));
        public void AddCompletedStep(string id) => MutateInternal(() => m_Record.AddCompletedStep(id));
        public void GrantRewardOnce(string id, Action grant)
        {
            if (m_Record == null || m_Record.HasGrantedReward(id) || grant == null) return;
            MutateInternal(() =>
            {
                grant();
                m_Record.AddGrantedReward(id);
            });
        }
        private void MutateInternal(Action mutation)
        {
            if (m_Record == null || m_Store == null) return;
            string before = JsonUtility.ToJson(m_Record);
            try
            {
                m_Store.Transact(() => { mutation(); SaveInternal(); });
                RefreshFlagsInternal();
            }
            catch
            {
                m_Record = JsonUtility.FromJson<TutorialProgressRecord>(before);
                RefreshFlagsInternal();
                throw;
            }
        }
        private void SaveInternal()
        {
            m_Store.Write(RecordKey, m_Record);
            // Keep legacy readers compatible, with the correct independent keys.
            m_Store.Write(DoneKey, m_Record.Completed);
            m_Store.Write(UnlockKey, m_Record.Unlocked);
        }
        private void RefreshFlagsInternal()
        {
            m_IsDone = m_Record.Completed;
            m_IsUnlocked = m_Record.Unlocked;
        }
        public void NotifyStarted() => OnTutorialStartInvoke();
        public void NotifyEnded() => OnTutorialEndInvoke();
        private void OnTutorialStartInvoke() => m_OnTutorialStart?.Invoke(m_Definition);
        private void OnTutorialEndInvoke() => m_OnTutorialEnd?.Invoke(m_Definition);
    }
}
