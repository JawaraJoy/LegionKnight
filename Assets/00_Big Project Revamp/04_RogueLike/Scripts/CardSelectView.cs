using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class CardSelectView : UIView
    {
        [SerializeField] private CardConfig m_CardConfig;
        [SerializeField] private Image m_UnitIcon;
        [SerializeField] private Image m_RarityColor;
        [SerializeField] private Image m_EquipedSign;
        [SerializeField] private GameObject m_LockIcon;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private Button m_SelectButton;
        [SerializeField] private UnityEvent<CardConfig> m_OnCardSelected = new();

        public CardConfig CardConfig => m_CardConfig;

        private void Awake()
        {
            // Refresh tampilan saat card di-add ke deck
            Player.Instance.PlayerCardDeck.OnCardAdded.AddListener(OnCardAddedRefresh);
            // ✅ Refresh equip sign saat used list berubah (add atau remove)
            Player.Instance.PlayerCardDeck.OnUsedCardsChanged.AddListener(_ => RefreshEquipSign());
        }

        // ── Init (dipanggil saat spawn oleh CardSelectTabView) ────────────────
        public void Init(CardUnit unit)
        {
            InitInternal(unit);
        }

        private void InitInternal(CardUnit cardUnit)
        {
            m_CardConfig = cardUnit.CardConfig;

            m_LockIcon.SetActive(!cardUnit.IsOwned);
            m_SelectButton.interactable = cardUnit.IsOwned;
            m_UnitIcon.sprite = m_CardConfig.CollectibleField.Icon;
            m_AmountText.text = cardUnit.Amount.ToString();
            m_RarityColor.color = cardUnit.CardConfig.CollectibleField.RarityConfig.Color;
            m_EquipedSign.gameObject.SetActive(cardUnit.IsAdded);

            m_SelectButton.onClick.RemoveListener(SelectCardInternal);
            m_SelectButton.onClick.AddListener(SelectCardInternal);
        }

        // ── Refresh hanya amount dan lock state (dipanggil saat OnCardAdded) ──
        private void OnCardAddedRefresh(CardUnit cardUnit)
        {
            if (cardUnit == null || cardUnit.CardConfig.BaseInfo.Id != m_CardConfig?.BaseInfo.Id)
                return;

            m_AmountText.text = cardUnit.Amount.ToString();
            m_LockIcon.SetActive(!cardUnit.IsOwned);
            m_SelectButton.interactable = cardUnit.IsOwned;
        }

        // ── Refresh equip sign saja (dipanggil saat OnUsedCardsChanged) ───────
        private void RefreshEquipSign()
        {
            if (m_CardConfig == null) return;

            CardUnit unit = Player.Instance.PlayerCardDeck.GetCardOwned(m_CardConfig);
            if (unit == null) return;

            m_EquipedSign.gameObject.SetActive(unit.IsAdded);
            m_AmountText.text = unit.Amount.ToString();
        }

        // ── Select (preview ke CardDetailView) ────────────────────────────────
        private void SelectCardInternal()
        {
            Player.Instance.PlayerCardDeck.SelectStandbyCardConfig(m_CardConfig);
            m_OnCardSelected?.Invoke(m_CardConfig);
        }
    }
}