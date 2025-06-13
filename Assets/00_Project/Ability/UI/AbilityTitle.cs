using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class AbilityTitle : UIView
    {
        [SerializeField]
        protected TextMeshProUGUI m_AbilityName;
        [SerializeField]
        protected Image m_AbilityIcon;
        public virtual void SetAbility(CharacterDefinition defi)
        {
            List<SkillDefinition> skills = defi.Passives;
            if (skills.Count > 0)
            {
                //ShowInternal();
                m_AbilityName.text = skills[0].SkillName;
                m_AbilityIcon.sprite = skills[0].Icon;
            }
            else
            {
                m_AbilityName.text = "No Ability";
                //HideInternal();
            }
        }
    }
}
