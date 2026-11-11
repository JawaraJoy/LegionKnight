using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class ModeSelectView : UIView
    {
        [SerializeField]
        private UIView[] m_ModeView;

        private void CloseAll()
        {
            for (int i = 0; i < m_ModeView.Length; i++)
            {
                m_ModeView[i].Hide();
            }
        }

        public void ShowModeview(string id)
        {
            CloseAll();
            for (int i = 0; i < m_ModeView.Length; i++)
            {
                if (m_ModeView[i].UniqueId == id)
                {
                    m_ModeView[i].Show();
                    break;
                }
            }
        }
    }
}
