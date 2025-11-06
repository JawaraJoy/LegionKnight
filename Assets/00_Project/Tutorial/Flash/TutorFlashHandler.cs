using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class TutorFlashHandler : MonoBehaviour
    {
        [SerializeField]
        private FlashContent[] m_FlashContent;
        [SerializeField, MMReadOnly]
        private List<TutorFlash> m_FlashList = new();
        private TutorFlash m_CurrentFlash;
        private void Start()
        {
            TutorFlash[] targets = FindObjectsByType<TutorFlash>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            m_FlashList = new List<TutorFlash>(targets);
        }
        public void Init()
        {
            foreach(FlashContent content in m_FlashContent)
            {
                content.Init();
            }
        }
        private FlashContent GetContent(TutorFlashDefinition defi)
        {
            foreach(FlashContent content in m_FlashContent)
            {
                if (content.Definition == defi)
                {
                    return content;
                }
            }
            return null;
        }
        private bool HasContent(TutorFlashDefinition defi, out FlashContent content)
        {
            content = GetContent(defi);
            return content != null;
        }
        private TutorFlash GetFlash(TutorFlashDefinition defi)
        {
            TutorFlash flash = m_FlashList.Find(x => x.Definition == defi);
            if (flash == null)
            {
                return null;
            }
            return flash;
        }
        private bool HasFlash(TutorFlashDefinition defi, out TutorFlash flash)
        {
            flash = GetFlash(defi);
            return flash != null;
        }
        public void AddFlash(TutorFlash flash)
        {
            if (!m_FlashList.Contains(flash))
            {
                m_FlashList.Add(flash);
            }
        }
        public void RemoveFlash(TutorFlash flash)
        {
            if (HasFlash(flash.Definition, out flash))
            {
                m_FlashList.Remove(flash);
            }
        }
        public void StartFlash(TutorFlashDefinition defi)
        {
            if (m_CurrentFlash != null)
            {
                if (HasContent(m_CurrentFlash.Definition, out FlashContent currentContent))
                {
                    if (currentContent.HasPlaying)
                    {
                        return;
                    }
                }
            }
            if (HasContent(defi, out FlashContent content))
            {
                if (content.HasDone || content.HasPlaying)
                {
                    return;
                }
            }
            if (HasFlash(defi, out TutorFlash flash))
            {
                m_CurrentFlash = flash;
                StartCoroutine(PlayingFlashMessage(defi));
                m_CurrentFlash.Definition.OnStart?.Invoke();
                if (m_CurrentFlash.Definition.IsSetDoneOnStart)
                {
                    EndFlash();
                }
            }
        }

        private IEnumerator PlayingFlashMessage(TutorFlashDefinition defi)
        {
            if (HasContent(m_CurrentFlash.Definition, out FlashContent content))
            {
                content.SetIsPlaying(true);
            }
            string[] messages = defi.FlashMessages;
            for (int i = 0; i < messages.Length; i++)
            {
                string currentMessage = messages[i];
                m_CurrentFlash.ShowFlashUI(currentMessage);
                yield return new WaitForSeconds(defi.MessageInternal);
            }
            yield return new WaitForSeconds(defi.MessageInternal);
            EndFlash();
        }

        private void EndFlash()
        {
            m_CurrentFlash.HideFlashUI();
            
            if (HasContent(m_CurrentFlash.Definition, out FlashContent content))
            {
                content.SetHasDone(true);
                content.SetIsPlaying(false);
            }
            m_CurrentFlash.Definition.OnEnd?.Invoke();
        }
    }

    [System.Serializable]
    public class FlashContent
    {
        [SerializeField]
        private TutorFlashDefinition m_Definition;
        [SerializeField, MMReadOnly]
        private bool m_HasDone = false;
        [SerializeField, MMReadOnly]
        private bool m_HasPlaying = false;
        public bool HasDone => m_HasDone;
        public bool HasPlaying => m_HasPlaying;
        public TutorFlashDefinition Definition => m_Definition;
        private string HASDONEKEY => $"{m_Definition.Id}hasdone";

        public void Init()
        {
            bool hasDoneData = UnityService.Instance.HasData(HASDONEKEY);
            if (hasDoneData)
            {
                bool hasDone = UnityService.Instance.GetData<bool>(HASDONEKEY);
                SetHasDoneInternal(hasDone);
            }
        }
        public void SetIsPlaying(bool hasPlaying)
        {
            m_HasPlaying = hasPlaying;
        }
        private void SetHasDoneInternal(bool hasDone)
        {
            m_HasDone = hasDone;
            UnityService.Instance.SaveData(HASDONEKEY, hasDone);
        }
        public void SetHasDone(bool hasDone)
        {
            SetHasDoneInternal(hasDone);
        }
    }
}
