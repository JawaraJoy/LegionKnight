using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public partial class CharacterUnit
    {
        [SerializeField]
        private CharacterDefinition m_Definition;
        [SerializeField]
        private bool m_Owned;
        [SerializeField]
        private UnityEvent<CharacterUnit> m_OnCharacterStarUp = new();
        [SerializeField]
        private UnityEvent<CharacterUnit> m_OnCharacterShardUpdate = new();
        public bool Owned => m_Owned;
        private void OnCharacterStarUpInvoke()
        {
            m_OnCharacterStarUp?.Invoke(this);
        }
        private void OnCharacterShardUpdateInvoke()
        {
            m_OnCharacterShardUpdate?.Invoke(this);
        }
        public void SetOwned(bool set)
        {
            
            SetOwnedInternal(set);
        }
        private void SetOwnedInternal(bool set)
        {
            m_Owned = set;
            UnityService.Instance.SaveData(m_Definition.Id + "Owned", m_Owned);
        }
        public void Init()
        {
            if (UnityService.Instance.HasData(m_Definition.Id + "Owned"))
            {
                m_Owned = UnityService.Instance.GetData<bool>(m_Definition.Id + "Owned");
            }
            //m_Owned = UnityService.Instance.GetData<bool>(m_Definition.Id + "Owned");
            if (m_Definition == Player.Instance.DefaultCharacter)
            {
                SetOwnedInternal(true);
            }
        }
        public string CharacterName => m_Definition.name;
        public Sprite Icon => m_Definition.Icon;
        public Sprite SmallIcon => m_Definition.SmallIcon;
        public StandbyPlatformDefinition UniquePlatform => m_Definition.UniquePlatform;
        public List<SkillDefinition> Passives => m_Definition.Passives;
        public List<SkillDefinition> Weapons => m_Definition.Weapons;
        public CharacterDefinition Definition => m_Definition;
        public Stat FinalStat(int level)
        {
            return m_Definition.FinalStat(level);
        }
    }
}
