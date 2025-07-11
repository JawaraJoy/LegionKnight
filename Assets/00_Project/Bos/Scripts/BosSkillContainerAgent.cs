using UnityEngine;

namespace LegionKnight
{
    public class BosSkillContainerAgent : MonoBehaviour
    {
        private GameplayPanel GetGameplayPanel()
        {
            return GameManager.Instance.GetPanel<GameplayPanel>();
        }

        private BosSkillContainer GetBosSkillContainer()
        {
            BosSkillContainer skillContainer = GetGameplayPanel().GetBosSkillContainer();
            return skillContainer;
        }

        public void InitSkillContainer()
        {
            GetBosSkillContainer().Init();
        }
        public void InitBos(BosDefinition definition)
        {
            GetBosSkillContainer().Init(definition);
        }
    }

    public partial class GameplayPanel
    {
        public BosSkillContainer GetBosSkillContainer()
        {
            return GetBinding<BosSkillContainer>();
        }
    }
}
