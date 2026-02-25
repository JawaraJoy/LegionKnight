using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class SpinRewardView : UIView
    {
        [SerializeField]
        private SpinRewardDefinition m_Definition;
        [SerializeField]
        private Image m_FrameBack;
        [SerializeField]
        private Image m_FrameHighlight;
        [SerializeField]
        private Image m_Item;
        [SerializeField]
        private TextMeshProUGUI m_AmountText;
        private bool m_Selected = false;

        [SerializeField]
        private UnityEvent m_OnSelected;
        [SerializeField]
        private UnityEvent m_OnNotSelected;

        public SpinRewardDefinition Definition => m_Definition;
        public void Init(SpinRewardDefinition defi)
        {
            m_Definition = defi;
            ShowInternal();
        }

        private void Start()
        {
            Refresh();
        }

        private void Refresh()
        {
            m_FrameBack.color = m_Definition.FrameColor;
            LootField firstLoot = m_Definition.Rewards.LootFields[0];
            CurrencyApplier(firstLoot.ItemLoot, firstLoot.Amount);
            CharacterApplier(firstLoot.ItemLoot);
            StandbyPlatformApplier(firstLoot.ItemLoot, firstLoot.Amount);
            EnergyApplier(firstLoot.ItemLoot, firstLoot.Amount);
        }
        protected override void ShowInternal()
        {
            base.ShowInternal();
            Refresh();
        }

        public void SetSelected(SpinRewardDefinition selectedDefi)
        {
            m_Selected = selectedDefi == m_Definition;
            if (m_Selected)
            {
                m_OnSelected?.Invoke();
            }
            else
            {
                m_OnNotSelected?.Invoke();
            }
            m_FrameHighlight.enabled = m_Selected;
        }



        private void InitInternal(Sprite sprite, int amount)
        {
            m_Item.sprite = sprite;
            if (amount > 0)
            {
                m_AmountText.text = amount.ToString();
            }
            else
            {
                m_AmountText.text = string.Empty;
            }
        }
        private void CurrencyApplier(ScriptableObject defi, int amount)
        {
            if (defi is ItemConfig currency)
            {
                InitInternal(currency.CollectibleField.Icon, amount);
            }
        }
        private void CharacterApplier(ScriptableObject defi)
        {
            if (defi is HeroUnitConfig character)
            {
                InitInternal(character.CollectibleField.Icon, 0);
            }
        }
        private void StandbyPlatformApplier(ScriptableObject defi, int amount)
        {
            if (defi is PlatformConfig platform)
            {
                InitInternal(platform.CollectibleField.Icon, amount);
            }
        }
        private void EnergyApplier(ScriptableObject defi, int amount)
        {
            if (defi is EnergyConfig energy)
            {
                InitInternal(energy.CollectibleField.Icon, amount);
            }
        }
    }
}
