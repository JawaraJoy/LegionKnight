using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public partial class CharacterUnit
    {
        [SerializeField]
        private CharacterDefinition m_Definition;
        [SerializeField]
        private bool m_Owned;
        public bool Owned => m_Owned;
        public void SetOwned(bool set)
        {
            m_Owned = set;
        }
        public string CharacterName => m_Definition.name;
        public Sprite Icon => m_Definition.Icon;
        public Sprite SmallIcon => m_Definition.SmallIcon;
        public Platform UniquePlatform => m_Definition.UniquePlatform;
        public List<SkillDefinition> Passives => m_Definition.Passives;
        public List<SkillDefinition> Weapons => m_Definition.Weapons;
        public CharacterDefinition Definition => m_Definition;
    }
}
