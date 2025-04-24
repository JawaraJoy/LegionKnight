using UnityEngine;

namespace LegionKnight
{
    public partial class GachaManagerAgent : MonoBehaviour
    {
        public void Init()
        {
            GameManager.Instance.InitGacha();
        }
        private BannerPanel GetBannerPanel()
        {
            return GameManager.Instance.GetPanel<BannerPanel>();
        }
        public void SelectBanner(BannerDefinition definition)
        {
            GameManager.Instance.SelectBanner(definition);
        }
        public void PerformingSingleDraw()
        {
            GameManager.Instance.PerformingSingleDraw();
        }

        public void PerformingMultiDraw()
        {
            GameManager.Instance.PerformingMultiDraw();
        }
    }
}
