using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class PlayerCurrencyView : CurrencyView
    {
        private PlayerCurrencyControl m_CurrencyControl;

        private PlayerCurrencyControl CurrencyControl
        {
            get
            {
                if (m_CurrencyControl == null)
                {
                    m_CurrencyControl = Player.Instance.CurrencyControl;
                }
                return m_CurrencyControl;
            }
        }
        [SerializeField]
        private Button m_BuyCurrencyButton;

        private void Awake()
        {
            CurrencyControl.OnCurrencyChanged.AddListener((_)=> InitInternal());
        }
        private void OnEnable()
        {
            InitInternal();
        }
    }
}
