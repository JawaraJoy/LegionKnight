using UnityEngine;

namespace LegionKnight
{
    public class PlatformAbilityTitle : AbilityTitle
    {
        public override void SetAbility(CharacterDefinition defi)
        {
            m_AbilityIcon.sprite = defi.UniquePlatform.Icon;
            m_AbilityName.text = defi.UniquePlatform.Label;
        }
    }
}
