using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class CardUpgradeView : MonoBehaviour
    {
        [System.Serializable] private sealed class CardField
        {
            [SerializeField] internal Image icon, frame;
            [SerializeField] internal TMP_Text name, rarity, rank, preview, copies, remainder, action;
            [SerializeField] internal Slider progress;
            [SerializeField] internal Button upgrade, close;
        }
        [SerializeField] private CardField m_View;
        private CardUnit m_Card;
        private CardDeck m_Deck;
        private int m_ShownRank;
        private void Awake()
        {
            m_View.upgrade.onClick.AddListener(UpgradeInternal);
            m_View.close.onClick.AddListener(CloseInternal);
        }

        public void Display(CardUnit card, CardDeck deck)
        {
            if (deck == null || card?.CardConfig == null) return;
            var owned = deck.GetCardOwned(card.CardConfig);
            if (owned == null || !owned.IsOwned || owned.CardConfig.IsBattlePoolOnly) return;
            if (m_Deck != null) m_Deck.OnCardAdded.RemoveListener(ChangedInternal);
            m_Card = owned;
            m_Deck = deck;
            m_Deck.OnCardAdded.AddListener(ChangedInternal);
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            RenderInternal();
        }
        private void ChangedInternal(CardUnit card)
        {
            if (card?.CardConfig != m_Card.CardConfig) return;
            m_Card = card;
            RenderInternal();
        }
        private void CloseInternal() => gameObject.SetActive(false);
        private void OnDisable()
        {
            if (m_Deck != null) m_Deck.OnCardAdded.RemoveListener(ChangedInternal);
            if (gameObject.activeSelf) gameObject.SetActive(false);
        }
        private void UpgradeInternal()
        {
            if (!m_Card.CanUpgrade) return;
            m_Deck.UpgradeCard(m_Card, m_ShownRank);
        }
        private void RenderInternal()
        {
            var config = m_Card.CardConfig;
            var rarity = config.CollectibleField.RarityConfig;
            m_ShownRank = m_Card.Upgrade;
            int next = Mathf.Min(m_ShownRank + 1, CardUnit.MaxUpgrade);
            m_View.icon.sprite = config.CollectibleField.Icon;
            m_View.frame.color = rarity.Color;
            m_View.name.text = config.BaseInfo.Name;
            m_View.rarity.text = rarity.BaseInfo.Name.ToUpperInvariant();
            m_View.rank.text = m_Card.IsMax ? $"Card +{m_ShownRank}  •  MAX" : $"Card +{m_ShownRank}  →  +{next}";
            m_View.preview.text = config.CollectionUpgradePreview(m_ShownRank, next);
            m_View.copies.text = m_Card.IsMax ? $"{m_Card.Amount:N0} owned" : $"{m_Card.Amount:N0} owned / {m_Card.UpgradeCost:N0} needed";
            m_View.progress.maxValue = m_Card.IsMax ? 1 : m_Card.UpgradeCost;
            m_View.progress.SetValueWithoutNotify(m_Card.IsMax ? 1 : m_Card.Amount);
            m_View.remainder.text = m_Card.IsMax ? "Maximum card upgrade reached." : m_Card.CanUpgrade ?
                $"Copies after upgrade   {m_Card.Amount - m_Card.UpgradeCost:N0}" : $"{m_Card.UpgradeCost - m_Card.Amount:N0} more copies needed";
            m_View.action.text = m_Card.IsMax ? "MAX +12" : $"Upgrade to +{next}";
            m_View.upgrade.interactable = m_Card.CanUpgrade;
        }
    }
}

