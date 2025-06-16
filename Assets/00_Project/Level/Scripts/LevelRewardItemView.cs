using UnityEngine;

namespace LegionKnight
{
    public class LevelRewardItemView : ItemView
    {
        protected override void InitInternal(object defi)
        {
            base.InitInternal(defi);
            if (defi is RewardObject reward)
            {
                m_Amount.text = reward.Amount.ToString();
                CurrencyApplier(reward);
                CharacterApplier(reward);
                PlatformApplier(reward);
            }
        }

        private void CurrencyApplier(RewardObject defi)
        {
            if (defi.Defi is CurrencyDefinition currency)
            {
                m_Icon.sprite = currency.Icon;
            }
        }
        private void CharacterApplier(RewardObject defi)
        {
            if (defi.Defi is CharacterDefinition character)
            {
                m_Icon.sprite = character.SmallIcon;
            }
        }
        private void PlatformApplier(RewardObject defi)
        {
            if (defi.Defi is StandbyPlatformDefinition platform)
            {
                m_Icon.sprite = platform.Icon;
            }
        }
    }
}
