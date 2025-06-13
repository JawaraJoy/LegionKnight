using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class SkillAbilityTitle : AbilityTitle
    {
        [SerializeField]
        private TextMeshProUGUI m_ManaText;

        public override void SetAbility(CharacterDefinition defi)
        {
            base.SetAbility(defi);
            m_ManaText.text = defi.Passives[0].Manathreshold.ToString();
        }
    }
}
