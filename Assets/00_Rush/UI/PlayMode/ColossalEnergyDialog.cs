using System;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class ColossalEnergyDialog : UIView
    {
        [Serializable]
        private class BalanceField
        {
            [SerializeField] private TMP_Text m_Title, m_Message;
            [SerializeField] private TMP_Text m_LeftLabel, m_RightLabel, m_LeftValue, m_RightValue, m_Operator;
            [SerializeField] private GameObject m_Arrow;
            [SerializeField] private Color m_ValueColor, m_ShortageColor;

            public void Display(int balance, int cost, string stageName)
            {
                bool enough = balance >= cost;
                m_Title.text = enough ? $"Enter {stageName}?" : "Not Enough Energy";
                m_Message.text = enough ? $"Spend {cost} Energy to enter {stageName}." :
                    $"You need {cost - balance} more Energy to enter {stageName}.";
                m_LeftLabel.text = enough ? "Current" : "Available";
                m_RightLabel.text = enough ? "After entry" : "Required";
                m_LeftValue.SetText("{0}", balance);
                m_RightValue.SetText("{0}", enough ? balance - cost : cost);
                m_LeftValue.color = enough ? m_ValueColor : m_ShortageColor;
                m_Arrow.SetActive(enough);
                m_Operator.gameObject.SetActive(!enough);
            }
        }

        [Serializable]
        private class ActionField
        {
            [SerializeField] private Button m_Confirm, m_Shop;
            [SerializeField] private TMP_Text m_DismissLabel;

            public void SetAffordable(bool enough)
            {
                m_Confirm.gameObject.SetActive(enough);
                m_Shop.gameObject.SetActive(!enough);
                m_DismissLabel.text = enough ? "Cancel" : "Close";
            }
        }

        [SerializeField] private BalanceField m_Balance;
        [SerializeField] private ActionField m_Actions;
        [SerializeField] private RectTransform m_Card;
        [SerializeField] private Vector2 m_ReferenceSize;
        [SerializeField] private EnergyConfirmationPanel m_Panel;
        [SerializeField] private ShopTabConfig m_EnergyTab;

        private EnergyController m_Wallet;
        private Energy[] m_Quote;
        private string m_StageName;
        private Func<bool> m_CanEnter;
        private Action m_OnPaid;
        private int m_LastBalance;

        private int Balance => m_Wallet.GetEnergy(m_Quote[0].Config)?.Amount ?? 0;

        public void Open(StageConfig stage, Func<bool> canEnter, Action onPaid)
        {
            if (stage == null || stage.EnergyConfig == null || stage.EnergyAmountToPay <= 0 || onPaid == null) return;
            m_Panel.Show();
            if (!m_Panel.IsShown) return;

            m_Wallet = Player.Instance.EnergyController;
            m_Quote = new[] { new Energy(stage.EnergyConfig, stage.EnergyAmountToPay) };
            m_StageName = stage.BaseInfo.Name;
            m_CanEnter = canEnter;
            m_OnPaid = onPaid;
            ShowInternal();
            FitCardInternal();
            RefreshBalanceInternal(Balance);
        }

        private void LateUpdate()
        {
            if (m_OnPaid == null) return;
            int current = Balance;
            if (current != m_LastBalance) RefreshBalanceInternal(current);
        }

        private void OnRectTransformDimensionsChange() => FitCardInternal();

        private void FitCardInternal()
        {
            if (m_Card == null || m_ReferenceSize.x <= 0 || m_ReferenceSize.y <= 0) return;
            var space = ((RectTransform)transform).rect;
            float scale = Mathf.Min(space.width / m_ReferenceSize.x, space.height / m_ReferenceSize.y);
            if (scale > 0) m_Card.localScale = Vector3.one * scale;
        }

        private void RefreshBalanceInternal(int current)
        {
            m_LastBalance = current;
            int cost = m_Quote[0].Amount;
            m_Balance.Display(current, cost, m_StageName);
            m_Actions.SetAffordable(current >= cost);
        }

        public void Confirm() => ConfirmInternal();

        private void ConfirmInternal()
        {
            if (m_OnPaid == null || !IsShown || !isActiveAndEnabled) return;
            if (m_CanEnter != null && !m_CanEnter())
            {
                m_Panel.Hide();
                return;
            }
            int current = Balance;
            if (current < m_Quote[0].Amount)
            {
                RefreshBalanceInternal(current);
                return;
            }

            // EnergyController notifies the wallet and other listeners synchronously.
            // Consume this confirmation first so a second click cannot pay the same quote.
            var onPaid = m_OnPaid;
            var quote = m_Quote;
            m_OnPaid = null;
            m_Wallet.Pay(quote, _ =>
            {
                m_Panel.Hide();
                onPaid();
            }, _ =>
            {
                if (m_Quote != quote) return;
                m_OnPaid = onPaid;
                RefreshBalanceInternal(Balance);
            });
        }

        public void GoToShop() => GoToShopInternal();

        private void GoToShopInternal()
        {
            if (m_OnPaid == null || !IsShown || !isActiveAndEnabled) return;
            if (Balance >= m_Quote[0].Amount)
            {
                RefreshBalanceInternal(Balance);
                return;
            }
            var shop = CanvasManager.Instance.GetPanel<ShopPanel>();
            if (shop == null) return;
            m_Panel.Hide();
            shop.ShowTab(m_EnergyTab);
        }

        private void OnDisable() => ClearQuoteInternal();

        protected override void HideInternal()
        {
            ClearQuoteInternal();
            base.HideInternal();
        }

        private void ClearQuoteInternal()
        {
            m_Quote = null;
            m_Wallet = null;
            m_CanEnter = null;
            m_OnPaid = null;
        }
    }
}
