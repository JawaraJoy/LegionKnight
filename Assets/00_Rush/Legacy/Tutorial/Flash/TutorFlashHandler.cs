using System;
using System.Collections;
using System.Collections.Generic;
using Rush;
using UnityEngine;

namespace LegionKnight
{
    public class TutorFlashHandler : MonoBehaviour
    {
        private const float c_MinInterval = 0.05f;
        [SerializeField] private FlashContent[] m_FlashContent = Array.Empty<FlashContent>();
        [SerializeField] private List<TutorFlash> m_FlashList = new();
        [SerializeField] private TutorialFlashChannel m_Channel;
        [SerializeField, Min(1f)] private float m_LoadTimeout = 10f;
        private readonly List<TutorFlashDefinition> m_Pending = new();
        private TutorFlash m_CurrentFlash;
        private Coroutine m_Playing;
        private bool m_Initialized;
        private void OnEnable()
        {
            if (m_Channel == null) return;
            m_Channel.TargetAdded += AddFlash;
            m_Channel.TargetRemoved += RemoveFlash;
            foreach (var flash in m_Channel.Targets) AddFlashInternal(flash);
        }
        private void OnDisable()
        {
            if (m_Channel != null)
            {
                m_Channel.TargetAdded -= AddFlash;
                m_Channel.TargetRemoved -= RemoveFlash;
            }
            CancelCurrentInternal();
        }
        public void Init()
        {
            if (m_Initialized) return;
            foreach (var content in m_FlashContent) content?.Init();
            m_Initialized = true;
        }
        public void AddFlash(TutorFlash flash) => AddFlashInternal(flash);
        private void AddFlashInternal(TutorFlash flash)
        {
            if (flash != null && !m_FlashList.Contains(flash)) m_FlashList.Add(flash);
        }
        public void RemoveFlash(TutorFlash flash)
        {
            m_FlashList.Remove(flash);
            if (flash == m_CurrentFlash) CancelCurrentInternal();
        }
        private FlashContent ContentFor(TutorFlashDefinition definition)
        {
            foreach (var content in m_FlashContent) if (content?.Definition == definition) return content;
            return null;
        }
        public void StartFlash(TutorFlashDefinition definition)
        {
            if (!isActiveAndEnabled || definition == null || definition.FlashMessages == null || definition.FlashMessages.Length == 0 || ContentFor(definition) == null) return;
            if (!m_Pending.Contains(definition) && (m_CurrentFlash == null || m_CurrentFlash.Definition != definition)) m_Pending.Add(definition);
        }
        private void Update()
        {
            if (!m_Initialized || m_CurrentFlash != null) return;
            for (int i = 0; i < m_Pending.Count; i++)
            {
                var definition = m_Pending[i];
                var content = ContentFor(definition);
                if (content == null || content.HasDone) { m_Pending.RemoveAt(i--); continue; }
                var flash = m_FlashList.Find(candidate => candidate != null && candidate.Definition == definition && candidate.isActiveAndEnabled);
                if (flash == null) continue;
                m_Pending.RemoveAt(i);
                m_CurrentFlash = flash;
                m_Playing = StartCoroutine(PlayInternal(flash, content));
                return;
            }
        }
        private IEnumerator PlayInternal(TutorFlash flash, FlashContent content)
        {
            content.SetIsPlaying(true);
            OnFlashStartInvoke(flash.Definition);
            foreach (var message in flash.Definition.FlashMessages)
            {
                flash.ShowFlashUI(message);
                float started = Time.realtimeSinceStartup;
                while (flash != null && !flash.IsReady && Time.realtimeSinceStartup - started < m_LoadTimeout) yield return null;
                if (flash == null || !flash.IsReady)
                {
                    Debug.LogWarning("[Tutorial] Flash UI was unavailable; hint was not marked read.", this);
                    content.SetIsPlaying(false);
                    if (flash != null) flash.HideFlashUI();
                    m_CurrentFlash = null;
                    m_Playing = null;
                    yield break;
                }
                yield return new WaitForSecondsRealtime(Mathf.Max(c_MinInterval, flash.Definition.MessageInternal));
            }
            flash.HideFlashUI();
            content.SetHasDone(true);
            content.SetIsPlaying(false);
            m_CurrentFlash = null;
            m_Playing = null;
            OnFlashEndInvoke(flash.Definition);
        }
        private void CancelCurrentInternal()
        {
            if (m_Playing != null) StopCoroutine(m_Playing);
            m_Playing = null;
            if (m_CurrentFlash != null)
            {
                ContentFor(m_CurrentFlash.Definition)?.SetIsPlaying(false);
                m_CurrentFlash.HideFlashUI();
            }
            m_CurrentFlash = null;
        }
        private void OnFlashStartInvoke(TutorFlashDefinition definition) => definition.OnStart?.Invoke();
        private void OnFlashEndInvoke(TutorFlashDefinition definition) => definition.OnEnd?.Invoke();
    }

    [Serializable]
    public class FlashContent
    {
        [SerializeField] private TutorFlashDefinition m_Definition;
        [SerializeField] private bool m_HasDone;
        [SerializeField] private bool m_HasPlaying;
        private ITutorialProgressStore m_Store;
        public bool HasDone => m_HasDone;
        public bool HasPlaying => m_HasPlaying;
        public TutorFlashDefinition Definition => m_Definition;
        private string DoneKey => m_Definition.Id + "hasdone";
        public FlashContent() { }
        public FlashContent(TutorFlashDefinition definition) => m_Definition = definition;
        public void Init() => InitializeInternal(new LocalTutorialProgressStore(UnityService.Instance.LocalSave));
        public void Initialize(ITutorialProgressStore store) => InitializeInternal(store);
        private void InitializeInternal(ITutorialProgressStore store)
        {
            m_Store = store;
            m_HasPlaying = false;
            m_HasDone = m_Definition != null && store != null && store.Contains(DoneKey) && store.Read<bool>(DoneKey);
        }
        public void SetIsPlaying(bool value) => m_HasPlaying = value;
        public void SetHasDone(bool value)
        {
            m_HasDone = value;
            if (m_Definition != null) m_Store?.Write(DoneKey, value);
        }
    }
}
