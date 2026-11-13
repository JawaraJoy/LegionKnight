using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class GachaRateItemUI : GachaCollectableItemUI
    {
        [SerializeField] private TextMeshProUGUI m_ChanceText;
        [SerializeField] private Image m_FeaturedHighlight;

        protected override bool ShowSingleAmount => true;
        protected override bool TintRarityLabel => false;

        public void Setup(GachaRateInfo rate) => BindInternal(rate, false);
        public void Setup(GachaRateInfo rate, bool featured) => BindInternal(rate, featured);

        private void BindInternal(GachaRateInfo rate, bool featured)
        {
            SetupBase(rate.Collectable);
            m_ChanceText.text = $"{rate.Percent:F2}%";
            m_FeaturedHighlight.enabled = featured;
            OnSetupComplete(rate.Collectable.Collect, rate.Collectable.Amount);
        }
    }
}
