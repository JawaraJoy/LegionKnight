using Rush;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class DrawItemView : ItemView
    {
        protected override void InitInternal(CollectibleConfig collectibleConfig)
        {
            base.InitInternal(collectibleConfig);
            if (collectibleConfig is GachaRewardConfig reward)
            {
                CollectibleConfig itemGachaConfig = reward.GachaItemConfig;
                m_Amount.text = reward.Amount.ToString();
                CurrencyApplier(itemGachaConfig, reward.Amount);
                CharacterApplier(itemGachaConfig);
                PlatformApplier(itemGachaConfig, reward.Amount);

                string itemName = itemGachaConfig.BaseInfo.Name;
                if (gameObject.TryGetComponent(out TextView text))
                {
                    text.SetText(itemName);
                }
            }
        }

        private void CurrencyApplier(CollectibleConfig collectibleConfig, int amount)
        {
            if (collectibleConfig is ItemConfig itemConfig)
            {
                m_Icon.sprite = itemConfig.CollectibleField.Icon;
                Player.Instance.CurrencyControl.AddCurrencyAmount(itemConfig, amount);
            }
        }
        private void CharacterApplier(ScriptableObject collectibleConfig)
        {
            if (collectibleConfig is HeroUnitConfig heroConfig)
            {
                m_Icon.sprite = heroConfig.CollectibleField.Icon;
                bool owned = Player.Instance.HeroesCollection.GetHeroUnit(heroConfig).Owned;
                if (owned)
                {
                    StartCoroutine(CharcterDuplicated(heroConfig));
                }
                else
                {
                    Player.Instance.HeroesCollection.SetOwned(heroConfig, true);
                }
            }
        }

        private IEnumerator CharcterDuplicated(HeroUnitConfig heroConfig)
        {
            Player.Instance.CurrencyControl.AddCurrencyAmount(heroConfig.ItemDuplicateConverter.ItemConfig, heroConfig.ItemDuplicateConverter.Amount);
            for (int i = 0; i < 6; i++)
            {
                m_OnDuplicateCharacterShow.Invoke();
                CharacterShow(heroConfig);
                yield return new WaitForSeconds(1.5f);

                m_OnDuplicaterCharacterHide.Invoke();
                DuplicateShow(heroConfig);
                yield return new WaitForSeconds(1.5f);
            }

        }
        [SerializeField]
        private UnityEvent m_OnDuplicateCharacterShow;
        [SerializeField]
        private UnityEvent m_OnDuplicaterCharacterHide;
        private void CharacterShow(HeroUnitConfig heroConfig)
        {
            m_Icon.sprite = heroConfig.CollectibleField.Icon;
            m_Amount.text = "";
            string charName = heroConfig.BaseInfo.Name;
            if (gameObject.TryGetComponent(out TextView textView))
            {
                textView.SetText(charName);
            }
        }
        private void DuplicateShow(HeroUnitConfig heroConfig)
        {
            m_Icon.sprite = heroConfig.ItemDuplicateConverter.ItemConfig.CollectibleField.Icon;
            m_Amount.text = heroConfig.ItemDuplicateConverter.Amount.ToString();
            string itemName = heroConfig.ItemDuplicateConverter.ItemConfig.BaseInfo.Name;
            if (gameObject.TryGetComponent(out TextView text))
            {
                text.SetText(itemName);
            }
        }
        private void PlatformApplier(CollectibleConfig collectibleConfig, int amount)
        {
            if (collectibleConfig is PlatformConfig platformConfig)
            {
                m_Icon.sprite = platformConfig.CollectibleField.Icon;
                Player.Instance.PlatformDeck.AddPlatformAmount(platformConfig, amount);
            }
        }
    }
}
