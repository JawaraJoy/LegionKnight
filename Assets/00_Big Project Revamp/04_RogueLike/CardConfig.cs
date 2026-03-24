using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "CardConfig", menuName = "Rush/RogueLike/Card", order = 2)]
    public class CardConfig : CollectibleConfig
    {
        [SerializeField]
        private CardSkillField[] m_SkillConfigs;
        [SerializeField]
        private CardSkillCategoryModification[] m_SkillCategoryModifications;
        [SerializeField]
        private PlatformConfig[] m_PlatformToAdds;
         

        public CardSkillField[] SkillConfigs => m_SkillConfigs;

        public void Collect()
        {
            Unit player = RushPlayer.Instance.Unit;
            if (player.HasBind(out SkillController skillController))
            {
                if (SkillConfigs.Length > 0)
                {
                    for (int i = 0; i < SkillConfigs.Length; i++)
                    {
                        CardSkillField skillConfig = SkillConfigs[i];
                        if (skillConfig != null)
                        {
                            switch (skillConfig.CardPurpose)
                            {
                                case CardPurpose.Activation:
                                    skillController.ForceActive(skillConfig.SkillConfig);
                                    break;
                                case CardPurpose.SkillUp:
                                    skillController.AddNewSkill(skillConfig.SkillConfig);
                                    break;
                            }
                        }
                    }
                }

                if (m_SkillCategoryModifications.Length > 0)
                {
                    for (int i = 0; i < m_SkillCategoryModifications.Length; i++)
                    {
                        CardSkillCategoryModification modification = m_SkillCategoryModifications[i];
                        modification.ApplyModification(skillController);
                    }
                }
            }
            if (m_PlatformToAdds.Length > 0)
            {
                PlatformHandler platformHandler = RushGameManager.Instance.StageManager.PlatformHandler;
                if (player.HasBind(out PlatformController controller))
                {
                    platformHandler.AddPreparedPlatformConfigs(m_PlatformToAdds, controller);
                }
            }
            RogueLikeManager manager = RushGameManager.Instance.RogueLikeManager;
            manager.OnCardCollected?.Invoke(this);

            
        }
    }
}
