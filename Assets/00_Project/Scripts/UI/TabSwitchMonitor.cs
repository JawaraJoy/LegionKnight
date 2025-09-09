using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class TabSwitchMonitor : MonoBehaviour
    {
        [SerializeField]
        private TabButton[] m_Tabs;

        private void Start()
        {
            foreach (var tab in m_Tabs)
            {
                tab.Init(HideAll);
            }
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
        private TextMeshProUGUI m_ButtonName;
        [SerializeField]
        private Button m_OpenButton;
        [SerializeField]
        private Image m_Hightlight;
        [SerializeField]
        private UIView m_View;

        public void Init(UnityAction onShow = null)
        {
            m_ButtonName.text = m_View.UniqueId;
            onShow?.Invoke();
            m_OpenButton.onClick.RemoveAllListeners();
            m_OpenButton.onClick.AddListener(Show);
        }

        private void Show()
        {
            m_View.Show();
            m_Hightlight.enabled = true;
            m_OpenButton.gameObject.SetActive(false);
        }

        public void Hide()
        {
            m_View.Hide();
            m_Hightlight.enabled = false;
            m_OpenButton.gameObject.SetActive(true);
        }
    }
}
