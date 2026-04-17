using System;
using TMPro;
using UnityEngine;
using Rush;

namespace LegionKnight
{
    public class AnimationItemView : ItemView
    {
        [SerializeField]
        private TextMeshProUGUI m_ItemName;
        protected override void OnConfigSetInvoke(CollectibleConfig collectibleConfig)
        {
            base.OnConfigSetInvoke(collectibleConfig);
            m_Amount.gameObject.SetActive(false);
            m_ItemName.gameObject.SetActive(true);

            /*string itemName = collectibleConfig.BaseInfo.Name;
            string amountText = reward.Amount.ToString();
            m_ItemName.text = $"{itemName}[x{amountText}]";


            if (reward.GachaItemConfig is HeroUnitConfig heroConfig)
            {
                m_Icon.sprite = heroConfig.CollectibleField.SplashImage;
                m_ItemName.text = GetHeroNameTextFormat(heroConfig);
            }*/
        }
        private string GetHeroNameTextFormat(HeroUnitConfig heroConfig)
        {
            string hex = ColorUtility.ToHtmlStringRGB(heroConfig.CollectibleField.RarityConfig.Color);
            return $"{heroConfig.BaseInfo.Name} [<color=#{hex}>{heroConfig.CollectibleField.RarityConfig.BaseInfo.Name}</color>]"; // Format: "{Rarity} {HeroName}"
        }
    }
}
