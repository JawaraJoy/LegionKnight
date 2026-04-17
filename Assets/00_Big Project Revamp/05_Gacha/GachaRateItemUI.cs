using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class GachaRateItemUI : GachaCollectableItemUI
    {
        // Field tambahan khusus rate item — tidak ada di base
        [SerializeField] private TextMeshProUGUI m_ChanceText;

        public void Setup(GachaRateInfo rate)
        {
            SetupBase(rate.Collectable);

            if (m_ChanceText != null) m_ChanceText.text = $"{rate.Percent:F2}%";

            OnSetupComplete(rate.Collectable);
        }
    }
}