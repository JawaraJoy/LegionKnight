using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class AnimationItemView : ItemView
    {
        [SerializeField]
        private TextMeshProUGUI m_ItemName;
        protected override void OnDefinitionSetInvoke(object defi)
        {
            base.OnDefinitionSetInvoke(defi);
            m_Amount.gameObject.SetActive(false);
            if (defi is GachaReward reward)
            {
                if (reward.Definition is IDescriptable descriptable)
                {
                    m_ItemName.text = descriptable.Label;
                    string amountText = reward.Amount.ToString();
                    m_ItemName.text = $"{descriptable.Label}[{amountText}]";
                }
                if (reward.Definition is CurrencyDefinition currency)
                {
                    m_Icon.sprite = currency.Icon;
                }
                if (reward.Definition is StandbyPlatformDefinition standbyPlatform)
                {
                    m_Icon.sprite = standbyPlatform.BigIcon;
                }
                if (reward.Definition is CharacterDefinition character)
                {
                    m_Icon.sprite = character.LargeIcon;
                    m_ItemName.text = GetHeroNameTextFormat(character);
                }
            }
            
        }
        private string GetHeroNameTextFormat(CharacterDefinition defi)
        {
            string hex = ColorUtility.ToHtmlStringRGB(defi.ColorRarity);
            return $"{defi.Label} [<color=#{hex}>{defi.Rarity}</color>]"; // Format: "{Rarity} {HeroName}"
        }
    }
}
