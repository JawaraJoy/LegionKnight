using MoreMountains.Tools;
using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class TutorialContent 
    {
        [SerializeField]
        private TutorialDefinition m_Definition;
        [SerializeField, MMReadOnly]
        private bool m_IsDone = false;
        [SerializeField, MMReadOnly]
        private bool m_IsUnlocked = false;
        public TutorialDefinition Definition => m_Definition;
        public bool IsDone => m_IsDone;
        public bool IsUnlocked => m_IsUnlocked;

        private string ISDONEKEY => $"{m_Definition.Id}isdone";
        private string ISUNLOCKKEY => $"{m_Definition.Id}unlock";
        public void Init()
        {
            bool hasIsDoneKey = UnityService.Instance.HasData(ISDONEKEY);
            bool hasIsUnlock = UnityService.Instance.HasData(ISUNLOCKKEY);
            if (hasIsDoneKey)
            {
                m_IsDone = UnityService.Instance.GetData<bool>(ISDONEKEY);
            }
            if (hasIsUnlock)
            {
                m_IsUnlocked = UnityService.Instance.GetData<bool>(ISUNLOCKKEY);
            }
            else
            {
                m_IsUnlocked = m_Definition.UnlockedAtFirst;
            }
        }
        public void SetIsDone(bool isDone)
        {
            m_IsDone = isDone;
            UnityService.Instance.SaveData($"{ISDONEKEY}", m_IsDone);
        }
        public void SetIsUnlocked(bool isUnlocked)
        {
            m_IsUnlocked = isUnlocked;
            UnityService.Instance.SaveData($"{ISDONEKEY}", m_IsUnlocked);
        }
    }

}
