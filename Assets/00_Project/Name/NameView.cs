using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public partial class NameView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_NameText;

        public void SetName(string name)
        {
            m_NameText.text = name;
        }
    }

    public partial class HomePanel
    {
        [SerializeField]
        private NameView m_NameView;
        public void SetName(string name)
        {
            GetBinding<NameView>().SetName(name);
        }
    }

    public partial class GameManager
    {
        public void SetPlayerNameView(string val)
        {
            GetPanelInternal<HomePanel>().SetName(val);
        }
    }
}
