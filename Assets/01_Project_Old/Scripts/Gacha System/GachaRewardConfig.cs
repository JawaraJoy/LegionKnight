using UnityEngine;
using Rush;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Gacha Reward", menuName = "Rush/Gacha/Reward")]
    public class GachaRewardConfig : CollectibleConfig
    {
        [SerializeField] private CollectibleConfig m_GachaItemConfig;
        [SerializeField] private int m_Amount = 1;
        [SerializeField] private float m_Weight = 1f;

        public CollectibleConfig GachaItemConfig => m_GachaItemConfig;
        public int Amount => m_Amount;
        public float Weight => m_Weight;

        public void Apply()
        {
            if (m_GachaItemConfig is ItemConfig itemConfig)
            {
                Player.Instance.CurrencyControl.AddCurrencyAmount(itemConfig, m_Amount);
            }

            if (m_GachaItemConfig is HeroUnitConfig heroConfig)
            {
                if (Player.Instance.HeroesCollection.GetHeroUnit(heroConfig).Owned)
                {
                    Player.Instance.CurrencyControl.AddCurrencyAmount(heroConfig.ItemDuplicateConverter.ItemConfig, heroConfig.ItemDuplicateConverter.Amount);
                }
                else
                {
                    Player.Instance.HeroesCollection.SetOwned(heroConfig, true);
                }
            }
        }
    }

    

}
