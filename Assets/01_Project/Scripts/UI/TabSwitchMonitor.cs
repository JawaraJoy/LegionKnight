using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class TabSwitchMonitor : UIView
    {
        [SerializeField]
        private TabButton[] m_Tabs;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            foreach (var tab in m_Tabs)
            {
                tab.Init(HideAll);
            }
            m_Tabs[0].Show(HideAll);
        }   

        private void HideAll()
        {
            foreach(var tab in m_Tabs)
            {
                tab.Hide();
            }
        }
    }

    [System.Serializable]
    public class TabButton
    {
        [SerializeField]
        private bool m_Active = true;
        [SerializeField]
        private TextMeshProUGUI m_ButtonName;
        [SerializeField]
        private TextMeshProUGUI m_HightlightName;
        [SerializeField]
        private Button m_OpenButton;
        [SerializeField]
        private Image m_Hightlight;
        [SerializeField]
        private UIView m_View;

        public void Init(UnityAction onShow = null)
        {
            if (!m_Active) return;

            m_ButtonName.text = m_View.UniqueId;
            m_HightlightName.text = m_View.UniqueId;
            //onShow?.Invoke();
            m_OpenButton.onClick.RemoveAllListeners();
            m_OpenButton.onClick.AddListener(() => ShowInternal(onShow));
        }

        public void Show(UnityAction onShow = null)
        {
            ShowInternal(onShow);
        }
        private void ShowInternal(UnityAction onShow = null)
        {
            if (!m_Active) return;
            onShow.Invoke();
            m_View.gameObject.SetActive(true);
            m_View.Show();
            m_Hightlight.gameObject.SetActive(true);
            //m_OpenButton.gameObject.SetActive(false);
        }

        public void Hide()
        {
            m_View.Hide();
            m_View.gameObject.SetActive(false);
            m_Hightlight.gameObject.SetActive(false);
            //m_OpenButton.gameObject.SetActive(true);
        }
    }
}
