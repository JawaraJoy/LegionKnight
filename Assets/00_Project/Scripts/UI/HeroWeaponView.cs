using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class HeroWeaponView : WeaponView
    {
        
    }
    public partial class HeroView
    {
        [SerializeField]
        private HeroWeaponView m_WeaponView;

        private void InitWeaponView(CharacterDefinition defi)
        {
            m_WeaponView.Init(defi);
        }
    }
}
