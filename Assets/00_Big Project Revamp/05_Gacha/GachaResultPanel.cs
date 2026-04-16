using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class GachaResultPanel : PanelView
    {
        [SerializeField] private GachaResultItemPool m_ResultItemPool;
        [SerializeField] private Button m_CloseButton;
        [SerializeField] private TextMeshProUGUI m_PityNoticeText;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_CloseButton != null) m_CloseButton.onClick.AddListener(Hide);
        }

        protected override void HideInternal()
        {
            if (m_CloseButton != null) m_CloseButton.onClick.RemoveListener(Hide);
            m_ResultItemPool?.ReturnAll();
            base.HideInternal();
        }

        public void Show(GachaDrawResult result)
        {
            PopulateResultsInternal(result);
            Show();
        }

        private void PopulateResultsInternal(GachaDrawResult result)
        {
            if (m_ResultItemPool == null) return;
            m_ResultItemPool.ReturnAll();

            foreach (var item in result.Items)
            {
                var ui = m_ResultItemPool.Rent();
                ui.Setup(item);
            }

            if (m_PityNoticeText != null)
                m_PityNoticeText.gameObject.SetActive(result.WasPityTriggered);
        }
    }
}