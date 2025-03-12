using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerSkill : PassiveSkill
    {
        
    }
    public partial class Player
    {

        [SerializeField]
        private PlayerSkill m_Passive;

        public void InitPassive()
        {
            m_Passive.Init(m_CharacterDefinition.Passives);
        }
    }
}
