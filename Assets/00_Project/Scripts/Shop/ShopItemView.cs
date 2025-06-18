using System.Collections;
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
        private UnityEvent m_OnAvaible = new();
        [SerializeField]
        private UnityEvent m_OnNotAvaible = new();
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
        private void OnAvaiableInvoke()
        {
            m_OnAvaible?.Invoke();
        }
        private void OnNotAvaibleInvoke()
        {
            m_OnNotAvaible?.Invoke();
        }
        private void OnBoughtInvoke()
        {
            m_OnBought?.Invoke(m_Definition);
            SetBonusAvaibleInternal(false);
            StartCoroutine(WaitForBonusToApply());
            
        }

        private IEnumerator WaitForBonusToApply()
        {
            //GameManager.Instance.SetBonusAvaible(m_Definition, false);
            bool isBonusAvaible = GameManager.Instance.GetShopItemControl(m_Definition).IsBonusAvaible;
            yield return new WaitUntil(() => !isBonusAvaible);
            InitInternal();
        }
        public void Init()
        {
            InitInternal();
        }
        protected virtual void InitInternal()
        {
            if (m_Definition == null) return;

            GetShopItemControl().CheckAvailableInternal();

            SetAvaibleInternal(GetShopItemControl().IsAvailable);
            SetBonusAvaibleInternal(GetShopItemControl().IsBonusAvaible);

            m_ItemNameText.text = m_Definition.ItemName;
            m_MainImage.sprite = m_Definition.Icon;
            if (m_BonusSignContent != null)
            {
                m_BonusSignContent.SetActive(m_IsBonusAvaible);
            }
            m_NotAvaibleContent.SetActive(!m_IsAvaible);
            m_CurrencyView.SetView(new Currency(m_Definition.Currency, m_Definition.Price));
            m_BonusText.text = m_Definition.BonusDescription;
            m_SelectButton.interactable = m_IsAvaible;
            m_ItemAmountText.gameObject.SetActive(m_Definition.Amount > 1);
            m_ItemAmountText.text = $"+{m_Definition.Amount}";
        }
        public void TryToBuy()
        {
            m_Definition.TryBuy(OnBoughtInvoke);

        }
        public void SetBonusAvaible(bool isAvaible)
        {
            SetBonusAvaibleInternal(isAvaible);
        }

        private void SetBonusAvaibleInternal(bool isAvaible)
        {
            m_IsBonusAvaible = isAvaible;
            if (m_BonusSignContent != null)
            {
                m_BonusSignContent.SetActive(m_IsBonusAvaible);
            }
            //GetShopItemControl().SetBonusAvaible(isAvaible);
        }

        public void SetAvaible(bool isAvaible)
        {
            SetAvaibleInternal(isAvaible);
        }
        private void SetAvaibleInternal(bool isAvaible)
        {
            m_IsAvaible = isAvaible;
            m_NotAvaibleContent.SetActive(isAvaible);
            GetShopItemControl().SetAvailable(isAvaible);
            if (m_IsAvaible)
            {
                OnAvaiableInvoke();
            }
            else
            {
                OnNotAvaibleInvoke();
            }
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
