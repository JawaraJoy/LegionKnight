using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class AehterStateComboView : UIView
    {
        [SerializeField]
        private Button m_ComboButton;
        [SerializeField]
        private Transform m_ComboButtonSpawnPoint;
        [SerializeField]
        private float m_ComboButtonSpawnRadius = 5f; // jangan melebihi camera view radius
        [SerializeField]
        private Slider m_ComboStateDurationSlider;
        [SerializeField]
        private TextMeshProUGUI m_ComboStateCountText;
        private PlatformHandler m_Handler;

        private PlatformHandler Handler
        {
            get
            {
                if (m_Handler == null)
                    m_Handler = RushGameManager.Instance.StageManager.PlatformHandler;
                return m_Handler;
            }
        }
    }
}
