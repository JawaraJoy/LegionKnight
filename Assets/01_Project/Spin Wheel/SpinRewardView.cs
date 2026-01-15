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
            CurrencyApplier(firstLoot.Item, firstLoot.Amount);
            CharacterApplier(firstLoot.Item);
            StandbyPlatformApplier(firstLoot.Item, firstLoot.Amount);
            EnergyApplier(firstLoot.Item, firstLoot.Amount);
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
            if (defi is CurrencyDefinition currency)
            {
                InitInternal(currency.Icon, amount);
            }
        }
        private void CharacterApplier(ScriptableObject defi)
        {
            if (defi is CharacterDefinition character)
            {
                InitInternal(character.SmallIcon, 0);
            }
        }
        private void StandbyPlatformApplier(ScriptableObject defi, int amount)
        {
            if (defi is PlatformConfig platform)
            {
                InitInternal(platform.Icon, amount);
            }
        }
        private void EnergyApplier(ScriptableObject defi, int amount)
        {
            if (defi is EnergyDefinition energy)
            {
                InitInternal(energy.Icon, amount);
            }
        }
    }
}
