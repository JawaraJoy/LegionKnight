using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Rush;

namespace LegionKnight
{
    public partial class CardSelectView : UIView
    {
        [SerializeField]
        private CardConfig m_CardConfig;
        [SerializeField]
        private Image m_UnitIcon;
        [SerializeField]
        private Image m_RarityColor;
        [SerializeField]
        private Image m_EquipedSign;
        [SerializeField]
        private GameObject m_LockIcon;
        [SerializeField]
        private TextMeshProUGUI m_AmountText;
        [SerializeField]
        private Button m_SelectButton;
        [SerializeField]
        private UnityEvent<CardConfig> m_OnCardSelected = new();
        public CardConfig CardConfig => m_CardConfig;
        // Called when player selects a card (preview only)

        private void Awake()
        {
            Player.Instance.PlayerCardDeck.OnCardAdded.AddListener(InitInternal);
        }
        private void SelectCardInternal()
        {
            Player.Instance.PlayerCardDeck.SelectStandbyCardConfig(m_CardConfig);
            OnCharacterSelectedInvoke();
        }
        private void InitInternal(CardUnit cardUnit)
        {
            cardUnit.Init();
            m_CardConfig = cardUnit.CardConfig;
            m_LockIcon.SetActive(!cardUnit.IsOwned);
            m_SelectButton.interactable = cardUnit.IsOwned;
            m_UnitIcon.sprite = m_CardConfig.CollectibleField.Icon;
            m_AmountText.text = cardUnit.Amount.ToString();
            m_RarityColor.color = cardUnit.CardConfig.CollectibleField.RarityConfig.Color;

            m_EquipedSign.gameObject.SetActive(cardUnit.IsAdded);

            m_SelectButton.onClick.RemoveListener(SelectCardInternal);
            m_SelectButton.onClick.AddListener(SelectCardInternal);

            HideInternal();
        }
        public void Init(CardUnit unit)
        {
            InitInternal(unit);
        }

        private void OnCharacterSelectedInvoke()
        {
            m_OnCardSelected?.Invoke(m_CardConfig);
        }
    }
}
