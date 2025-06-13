using UnityEngine;

namespace LegionKnight
{
    public partial class AbilityDescriptionView : UIView
    {
        [SerializeField]
        private AbilityTitle m_SkillTitle;
        [SerializeField]
        private AbilityTitle m_Platformtitle;
        [SerializeField]
        private AbilityEffectDescription m_Description;

        public void SetSkill(CharacterDefinition defi)
        {
            ShowInternal();
            m_Platformtitle.Hide();
            m_SkillTitle.Show();
            m_SkillTitle.SetAbility(defi);
            m_Description.SetDescription(defi.Passives[0].Description);
        }
        public void SetPlatform(CharacterDefinition defi)
        {
            ShowInternal();
            m_SkillTitle.Hide();
            m_Platformtitle.Show();
            m_Platformtitle.SetAbility(defi);
            m_Description.SetDescription(defi.UniquePlatform.Description);
        }
    }
}
