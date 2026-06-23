using LegionKnight;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "CardConfig", menuName = "Rush/RogueLike/Card", order = 2)]
    public class CardConfig : CollectibleConfig
    {
        
        [SerializeField, TextArea(3, 10)]   
        private string m_SimpleDescription;
        [SerializeField, TextArea(3, 10)]
        private string m_UpgradeDescription;
        [SerializeField]
        private CardSkillField[] m_SkillConfigs;
        [SerializeField]
        private CardSkillCategoryModification[] m_SkillCategoryModifications;
        [SerializeField]
        private PlatformConfig[] m_PlatformToAdds;
        [SerializeField]
        private Currency m_GetCurrency;
        [SerializeField]
        private int m_GetAetherEssence;

        
        public string SimpleDescription => m_SimpleDescription;
        public string UpgradeDescription => m_UpgradeDescription;

        public CardSkillField[] SkillConfigs => m_SkillConfigs;

        public void Collect()
        {
            RushGameManager.Instance.RogueLikeManager.AddCollectedCard(this);
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
            PlatformHandler platformHandler = RushGameManager.Instance.StageManager.PlatformHandler;
            platformHandler.AddBoostStock(m_GetAetherEssence);
            if (m_PlatformToAdds.Length > 0)
            {
                
                if (player.HasBind(out PlatformController controller))
                {
                    platformHandler.AddPreparedPlatformConfigs(m_PlatformToAdds, controller);
                }
            }
            RogueLikeManager manager = RushGameManager.Instance.RogueLikeManager;
            manager.OnCardCollected?.Invoke(this);

            if (m_GetCurrency.ItemConfig != null && m_GetCurrency.Amount > 0)
            {
                Player.Instance.CurrencyControl.AddCurrencyAmount(m_GetCurrency.ItemConfig, m_GetCurrency.Amount);
            }
        }
    }
}
