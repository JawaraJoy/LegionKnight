using UnityEngine;
using UnityEngine.AddressableAssets;

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
            CharacterUnit unit = Player.Instance.GetCharacterUnit(defi);
            int level = unit.Level;
            m_Description.SetDescription(defi.Ability.GetFinalDescription(level));
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
