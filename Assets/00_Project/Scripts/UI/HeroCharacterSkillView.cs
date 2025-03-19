using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class HeroCharacterSkillView : CharacterSkillView
    {
        
    }
    public partial class HeroView
    {
        [SerializeField]
        private HeroCharacterSkillView m_SkillView;
        public void InitSkillView(CharacterDefinition defi)
        {
            m_SkillView.Init(defi);
        }
    }
}
