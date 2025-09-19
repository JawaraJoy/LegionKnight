using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class TextView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_Text;

        public void SetText(string set)
        {
            m_Text.text = set;
        }
    }
}
