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
        private void SelectCardInternal()
        {
            Player.Instance.PlayerCardDeck.SelectStandbyCardConfig(m_CardConfig);
            OnCharacterSelectedInvoke();
        }
        public void SelectPlatform()
        {
            SelectCardInternal();
        }
        private void RefreshInternal()
        {
            CardUnit platform = Player.Instance.PlayerCardDeck.GetCardOwned(m_CardConfig);
            InitInternal(platform);
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

            m_EquipedSign.gameObject.SetActive(cardUnit.IsEquiped);

            m_SelectButton.onClick.RemoveAllListeners();
            m_SelectButton.onClick.AddListener(SelectCardInternal);

            HideInternal();
        }

        public void RefreshEquiped()
        {
            CardUnit platform = Player.Instance.PlayerCardDeck.GetCardOwned(m_CardConfig);
            m_EquipedSign.gameObject.SetActive(platform.IsEquiped);
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
