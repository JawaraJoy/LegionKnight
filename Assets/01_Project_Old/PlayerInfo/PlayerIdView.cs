using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class PlayerIdView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_PlayerIdText;

        public override void Show()
        {
            base.Show();
            m_PlayerIdText.text = UnityService.Instance.PlayerId;
        }
    }
}
