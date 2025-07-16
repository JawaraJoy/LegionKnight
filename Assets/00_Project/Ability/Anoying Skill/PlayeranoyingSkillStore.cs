using UnityEngine;

namespace LegionKnight
{
    public class PlayeranoyingSkillStore : AnoyingSkillStore
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayeranoyingSkillStore m_PlayerAnoyingSkillStore;
        public void AddAnoy(IAnoy anoy)
        {
            m_PlayerAnoyingSkillStore.AddAnoy(anoy);
        }
        public void RemoveAnoy(IAnoy anoy)
        {
            m_PlayerAnoyingSkillStore.RemoveAnoy(anoy);
        }
        public void AddInteruptAnoy(AnoyDefinition anoyDefinition, int add)
        {
            m_PlayerAnoyingSkillStore.AddInterupt(anoyDefinition, add);
        }
    }
}
