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

        
        [SerializeField] private bool m_BattlePoolOnly;
        [SerializeField] private BattleCardBonus m_Bonus;
        [SerializeField] private float m_BonusAmount;
        public override bool CanSell => false;
        public bool IsBattlePoolOnly => m_BattlePoolOnly;
        public string BonusDescription => BattleCardRunBonuses.Describe(m_Bonus, m_BonusAmount);

        // Authored hero copy binds its power tokens to the card's existing skill fields.
        internal string LearnTemplate => m_SimpleDescription;
        internal string PowerTemplate => m_UpgradeDescription;
        internal bool HasHeroCardText => !string.IsNullOrEmpty(m_SimpleDescription) &&
            !string.IsNullOrEmpty(m_UpgradeDescription) && m_UpgradeDescription.Contains("{");

        public string Description => GameplayDescription.Card(this);
        public string SimpleDescription => GameplayDescription.NormalCard(this);
        public string UpgradeDescription => GameplayDescription.UpgradeCard(this);
        public PlatformConfig[] PlatformToAdds => m_PlatformToAdds;
        public CardSkillCategoryModification[] SkillCategoryModifications => m_SkillCategoryModifications;
        public Currency CurrencyReward => m_GetCurrency;
        public int AetherEssenceReward => m_GetAetherEssence;

        public CardSkillField[] SkillConfigs => m_SkillConfigs;

        public bool AllowsCrossHeroLearning
        {
            get
            {
                if (m_BattlePoolOnly) return false;
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

        internal int CollectionRank
        {
            get
            {
                if (m_BattlePoolOnly || !Application.isPlaying) return 0;
                return Player.Instance?.PlayerCardDeck?.GetCardOwned(this)?.Upgrade ?? 0;
            }
        }
        internal bool HasSkillUpgrade => System.Array.Exists(m_SkillConfigs ?? System.Array.Empty<CardSkillField>(),
            field => field != null && field.CardPurpose == CardPurpose.SkillUp);
        internal bool HasPlatformUpgrade => m_PlatformToAdds != null && m_PlatformToAdds.Length > 0;
        internal bool IsRepeatGain(RogueLikeHandler run) => !m_BattlePoolOnly && !HasSkillUpgrade && !HasPlatformUpgrade &&
            run != null && run.HasCollectedCard(BaseInfo.Id, out _);

        internal string CollectionUpgradePreview(int rank, int next)
        {
            if (HasHeroCardText)
                return GameplayDescription.HeroCollectionPower(this, rank, next);
            if (HasSkillUpgrade)
                return GameplayDescription.CollectionUpgrade(this, rank) + (rank == next ? "" : "\n→  " + GameplayDescription.CollectionUpgrade(this, next));
            if (HasPlatformUpgrade)
                return $"Spawn weight per repeat pick\n+{1f + rank * .01f:0.##}" +
                    (rank == next ? "" : $"  →  +{1f + next * .01f:0.##}");
            return $"Extra power on repeat picks\n+{rank}%" + (rank == next ? "" : $"  →  +{next}%");
        }
        public void Collect() => CollectInternal();
        private void CollectInternal()
        {
            var playerOwner = RushPlayer.Instance;
            int rank = CollectionRank;
            if (m_Bonus != BattleCardBonus.None)
                playerOwner.CardBonuses.Apply(m_Bonus, m_BonusAmount);
            Unit player = playerOwner.Unit;
            bool repeatGain = IsRepeatGain(RushGameManager.Instance.RogueLikeManager);
            if (repeatGain && rank > 0) playerOwner.CardBonuses.AddRepeatGain(this, rank, player);
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
                                    skillController.LearnOrUpgradeCard(skillConfig.SkillConfig, rank);
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
                    platformHandler.AddCardPlatforms(m_PlatformToAdds, controller, HasSkillUpgrade ? 0 : rank);
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
