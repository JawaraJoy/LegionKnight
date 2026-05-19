// SpinRewardView.cs
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

        [SerializeField]
        private UnityEvent m_OnSelected;
        [SerializeField]
        private UnityEvent m_OnNotSelected;

        public SpinRewardDefinition Definition => m_Definition;

        // ── Init ──────────────────────────────────────────────────────────────────

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
            if (m_Definition == null || m_Definition.Collectible == null) return;

            CollectibleConfig collectible = m_Definition.Collectible;
            int amount = m_Definition.Amount;
            m_FrameBack.color = collectible.CollectibleField.RarityConfig.Color;
            if (collectible is ItemConfig currency)
                InitInternal(currency.CollectibleField.Icon, amount);
            else if (collectible is HeroUnitConfig character)
                InitInternal(character.CollectibleField.Icon, 0);
            else if (collectible is PlatformConfig platform)
                InitInternal(platform.CollectibleField.Icon, amount);
            else if (collectible is EnergyConfig energy)
                InitInternal(energy.CollectibleField.Icon, amount);
            else if (collectible is CardConfig card)
                InitInternal(card.CollectibleField.Icon, amount);
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            Refresh();
        }

        // ── Highlight ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Dipanggil oleh SpinWheelPanel setiap step spin.
        /// true  = slot ini sedang disorot oleh jarum spin.
        /// false = slot ini tidak aktif.
        /// </summary>
        public void SetHighlight(bool active)
        {
            if (m_FrameHighlight != null)
                m_FrameHighlight.gameObject.SetActive(active);

            if (active)
                m_OnSelected?.Invoke();
            else
                m_OnNotSelected?.Invoke();
        }

        // ── Private ───────────────────────────────────────────────────────────────

        private void InitInternal(Sprite sprite, int amount)
        {
            m_Item.sprite = sprite;
            m_AmountText.text = amount > 0 ? amount.ToString() : string.Empty;
        }
    }
}