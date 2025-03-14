using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerPassiveSkill : PassiveSkill
    {
        
    }
    public partial class Player
    {

        [SerializeField]
        private PlayerPassiveSkill m_PassiveSkill;
        public Transform SpawnContainer => m_PassiveSkill.SpawnContainer;
        public void InitPassive()
        {
            m_PassiveSkill.Init(m_CharacterDefinition.Passives);
        }
        public void AddManaToAll(int add)
        {
            m_PassiveSkill.AddManaToAll(add);
        }
    }
}
