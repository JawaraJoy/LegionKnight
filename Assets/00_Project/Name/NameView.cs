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
        private NameView GetNameView()
        {
            return GetBinding<NameView>();
        }
        public void SetPlayerNameView(string val)
        {
            GetNameView().SetName(val);
        }
    }

    public partial class GameManager
    {
        public void SetPlayerNameView(string val)
        {
            GetPanelInternal<HomePanel>().SetPlayerNameView(val);
        }
    }
}
