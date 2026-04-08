using Rush;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class LootItemView : UIView
    {
        [SerializeField]
        private LootField m_LootField;
        [SerializeField]
        private Image m_Icon;
        [SerializeField]
        private Image m_Frame;
        [SerializeField]
        private TextMeshProUGUI m_ItemNameText;
        [SerializeField]
        private TextMeshProUGUI m_ItemAmountText;
        [SerializeField]
        private UnityEvent<int> m_OnAmountChanged = new();
        [SerializeField]
        private UnityEvent m_OnAmountCountChanged = new();
        public LootField LootField => m_LootField;

        public void Init(LootField lootField)
        {
            InitInternal(lootField);
        }
        protected virtual void InitInternal(LootField lootField)
        {
            if (m_LootField == null) return;
            m_LootField = lootField;

            CollectibleConfig itemLoot = lootField.ItemLoot;
            int amount = lootField.Amount;
            //CurrencyApplier(itemLoot, amount);
            //CharacterApplier(itemLoot);
            //StandbyPlatformApplier(itemLoot, amount);
            //EnergyApplier(itemLoot, amount);
            SetAmountInternal(amount);
            SetNameInternal(itemLoot.BaseInfo.Name);
            Color color = itemLoot.CollectibleField.RarityConfig.Color;
            m_Frame.color = color;
            Debug.Log($"[Loot] is seted up");
        }
        private void SetNameInternal(string name)
        {
            if (m_ItemNameText != null)
                m_ItemNameText.text = name;
        }
        private void SetAmountInternal(int amount)
        {
            if (m_ItemAmountText != null)
                m_ItemAmountText.text = amount.ToString();
        }
        public void SetAmount(int amount)
        {
            SetAmountInternal(amount);
        }

        private void CurrencyApplier(CollectibleConfig collectibleConfig, int amount)
        {
            if (collectibleConfig is ItemConfig itemConfig)
            {
                m_Icon.sprite = itemConfig.CollectibleField.Icon;
                Player.Instance.CurrencyControl.AddCurrencyAmount(itemConfig, amount);
            }
        }
        private void CharacterApplier(CollectibleConfig collectibleConfig)
        {
            if (collectibleConfig is HeroUnitConfig heroConfig)
            {
                m_Icon.sprite = heroConfig.CollectibleField.Icon;
                bool owned = Player.Instance.HeroesCollection.GetHeroUnit(heroConfig).Owned;
                if (owned)
                {
                    StartCoroutine(CharcterDuplicated(heroConfig));
                }
            }
        }

        private IEnumerator CharcterDuplicated(HeroUnitConfig heroConfig)
        {
            yield return new WaitForSeconds(1.5f);
            m_Icon.sprite = heroConfig.ItemDuplicateConverter.ItemConfig.CollectibleField.Icon;
            m_ItemAmountText.text = heroConfig.ItemDuplicateConverter.Amount.ToString();
            string itemName = heroConfig.ItemDuplicateConverter.ItemConfig.BaseInfo.Name;
            if (gameObject.TryGetComponent(out TextView text))
            {
                text.SetText(itemName);
            }
            ItemConfig itemConfig = heroConfig.ItemDuplicateConverter.ItemConfig;
            int amount = heroConfig.ItemDuplicateConverter.Amount;
            Player.Instance.CurrencyControl.AddCurrencyAmount(itemConfig, amount);
        }
        private void StandbyPlatformApplier(CollectibleConfig collectibleConfig, int amount)
        {
            if (collectibleConfig is PlatformConfig platformConfig)
            {
                m_Icon.sprite = platformConfig.CollectibleField.Icon;
                m_ItemAmountText.text = amount.ToString();
            }
        }
        private void EnergyApplier(CollectibleConfig collectibleConfig, int amount)
        {
            if (collectibleConfig is EnergyConfig energy)
            {
                m_Icon.sprite = energy.CollectibleField.Icon;
            }
        }
        public void AddAmountWithCountDown(int addCount)
        {
            StartCoroutine(AddCountDown(addCount));
        }
        int m_AmountTriggerCount = 0;
        private IEnumerator AddCountDown(int addCount)
        {
            int start = m_LootField.Amount;
            int target = m_LootField.Amount + addCount;
            int amountPseudo = m_LootField.Amount;
            for (int i = start; i < target; i++)
            {

                amountPseudo = i + 1;
                m_OnAmountChanged?.Invoke(amountPseudo);

                m_AmountTriggerCount++;
                if (m_AmountTriggerCount >= 5)
                {
                    m_OnAmountCountChanged?.Invoke();
                    m_AmountTriggerCount = 0;
                }
                if (m_ItemAmountText != null)
                {
                    SetAmountInternal(amountPseudo);
                }
                Debug.Log($"Counting up loot amount: {amountPseudo}");
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
