using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class ShopItemView : UIView
    {
        [SerializeField]
        private bool m_IsBonusAvaible;
        [SerializeField]
        private bool m_IsAvaible;
        [SerializeField]
        private ShopItemDefinition m_Definition;
        [SerializeField]
        private TextMeshProUGUI m_ItemNameText;
        [SerializeField]
        private TextMeshProUGUI m_ItemAmountText;
        [SerializeField]
        private TextMeshProUGUI m_BonusText;
        [SerializeField]
        private CurrencyView m_CurrencyView;
        [SerializeField]
        private Image m_MainImage;
        [SerializeField]
        private GameObject m_BonusSignContent;
        [SerializeField]
        private GameObject m_NotAvaibleContent;
        [SerializeField]
        private Button m_SelectButton;
        [SerializeField]
        private UnityEvent<ShopItemDefinition> m_OnBought = new();
        private ShopItemControl GetShopItemControl()
        {
            var shopContainer = GameManager.Instance.GetShopItemControl(m_Definition);
            if (shopContainer == null)
            {
                Debug.LogError($"ShopItemControl not found for {m_Definition.name} in {m_Definition.TabName}");
                return null;
            }
            return shopContainer;
        }
        private void OnBoughtInvoke()
        {
            m_OnBought?.Invoke(m_Definition);
            GameManager.Instance.SetBonusAvaible(m_Definition, false);
            InitInternal();
        }
        public void Init()
        {
            InitInternal();
        }
        protected virtual void InitInternal()
        {
            if (m_Definition == null) return;

            m_IsAvaible = GetShopItemControl().IsAvailable;
            m_IsBonusAvaible = GetShopItemControl().IsBonusAvaible;

            m_ItemNameText.text = m_Definition.ItemName;
            m_MainImage.sprite = m_Definition.Icon;
            m_BonusSignContent.SetActive(m_IsBonusAvaible);
            m_NotAvaibleContent.SetActive(!m_IsAvaible);
            m_CurrencyView.SetView(new Currency(m_Definition.Currency, m_Definition.Price));
            m_BonusText.text = m_Definition.BonusDescription;
            m_SelectButton.interactable = m_IsAvaible;
            m_ItemAmountText.text = $"+{m_Definition.Amount}";
        }
        public void TryToBuy()
        {
            m_Definition.TryBuy(OnBoughtInvoke);

        }
        public void SetBonusAvaible(bool isAvaible)
        {
            m_IsBonusAvaible = isAvaible;
            m_BonusSignContent.SetActive(isAvaible);
            GetShopItemControl().SetBonusAvaible(isAvaible);
        }

        public void SetAvaible(bool isAvaible)
        {
            m_IsAvaible = isAvaible;
            m_NotAvaibleContent.SetActive(isAvaible);
            GetShopItemControl().SetAvailable(isAvaible);
        }
        private void OnEnable()
        {
            InitInternal();
        }
        protected override void OnShowInvoke()
        {
            base.OnShowInvoke();
            InitInternal();
        }
    }
}
