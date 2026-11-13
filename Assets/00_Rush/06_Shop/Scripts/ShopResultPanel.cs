using UnityEngine;

namespace Rush
{
    public class ShopResultPanel : CollectibleResultPanel
    {
        private const string c_SpinSource = "spin";
        [SerializeField] private SpinWheelResultPanel m_SpinReceipt;

        public override void Show(CollectibleResultData result)
        {
            if (result?.Source == c_SpinSource && result.Entries.Count == 1 &&
                m_SpinReceipt != null && m_SpinReceipt.isActiveAndEnabled)
            {
                m_SpinReceipt.Show(result);
                return;
            }

            base.Show(result);
        }
    }
}
