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

        
        public string Description => GameplayDescription.Card(this);
        public string SimpleDescription => GameplayDescription.Card(this);
        public string UpgradeDescription => GameplayDescription.Card(this, null, true);
        public PlatformConfig[] PlatformToAdds => m_PlatformToAdds;
        public CardSkillCategoryModification[] SkillCategoryModifications => m_SkillCategoryModifications;
        public Currency CurrencyReward => m_GetCurrency;
        public int AetherEssenceReward => m_GetAetherEssence;

        public CardSkillField[] SkillConfigs => m_SkillConfigs;

        public bool AllowsCrossHeroLearning
        {
            get
            {
                if (m_SkillConfigs == null) return true;
                foreach (CardSkillField skillField in m_SkillConfigs)
                {
                    // Activation applies a one-off reward; only SkillUp learns an ability.
                    if (skillField == null || skillField.CardPurpose != CardPurpose.SkillUp) continue;
                    SkillConfig skill = skillField.SkillConfig;
                    if (skill == null || skill.Category == null || !skill.Category.AllowsCrossHeroLearning)
                        return false;
                }
                return true;
            }
        }

        public bool IsAvailableForHero(HeroUnitConfig hero)
        {
            if (hero != null && hero.HeroDeckConfig != null && hero.HeroDeckConfig.CardConfigs != null)
            {
                foreach (CardConfig card in hero.HeroDeckConfig.CardConfigs)
                    if (card == this) return true;
            }
            return AllowsCrossHeroLearning;
        }

        public void Collect() => CollectInternal();
        private void CollectInternal()
        {
            Unit player = RushPlayer.Instance.Unit;
            if (player.HasBind(out SkillController skillController))
            {
                if (SkillConfigs != null && SkillConfigs.Length > 0)
                {
                    for (int i = 0; i < SkillConfigs.Length; i++)
                    {
                        CardSkillField skillConfig = SkillConfigs[i];
                        if (skillConfig != null && skillConfig.SkillConfig != null)
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

                if (m_SkillCategoryModifications != null && m_SkillCategoryModifications.Length > 0)
                {
                    for (int i = 0; i < m_SkillCategoryModifications.Length; i++)
                    {
                        CardSkillCategoryModification modification = m_SkillCategoryModifications[i];
                        modification?.ApplyModification(skillController);
                    }
                }
            }
            PlatformHandler platformHandler = RushGameManager.Instance.StageManager.PlatformHandler;
            platformHandler.AddBoostStock(m_GetAetherEssence);
            if (m_PlatformToAdds != null && m_PlatformToAdds.Length > 0)
            {
                
                if (player.HasBind(out PlatformController controller))
                {
                    platformHandler.AddPreparedPlatformConfigs(m_PlatformToAdds, controller);
                }
            }
            if (m_GetCurrency != null && m_GetCurrency.ItemConfig != null && m_GetCurrency.Amount > 0)
            {
                Player.Instance.CurrencyControl.AddCurrencyAmount(m_GetCurrency.ItemConfig, m_GetCurrency.Amount);
            }
            RushGameManager.Instance.RogueLikeManager.NotifyCardCollected(this);
        }
    }
}
