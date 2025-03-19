using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class WeaponView : MonoBehaviour
    {

        [SerializeField]
        private List<Image> m_WeaponStateIcon = new();

        private List<SkillDefinition> m_Weapons = new();

        public void Init(CharacterDefinition defi)
        {
            m_Weapons = new List<SkillDefinition>(defi.Weapons);
        }
    }
}
