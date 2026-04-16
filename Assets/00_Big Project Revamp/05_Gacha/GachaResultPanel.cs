using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class GachaResultPanel : PanelView
    {
        [SerializeField] private Transform m_ItemContainer;
        [SerializeField] private GachaResultItemUI m_ResultItemPrefab;
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
            base.HideInternal();
        }

        public void Show(GachaDrawResult result)
        {
            PopulateResultsInternal(result);
            Show();
        }

        private void PopulateResultsInternal(GachaDrawResult result)
        {
            if (m_ItemContainer == null || m_ResultItemPrefab == null) return;

            foreach (Transform child in m_ItemContainer)
                Destroy(child.gameObject);

            foreach (var item in result.Items)
            {
                var ui = Instantiate(m_ResultItemPrefab, m_ItemContainer);
                ui.Setup(item);
            }

            if (m_PityNoticeText != null)
                m_PityNoticeText.gameObject.SetActive(result.WasPityTriggered);
        }
    }
}