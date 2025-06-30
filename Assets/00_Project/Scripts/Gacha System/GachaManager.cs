using UnityEngine;

namespace LegionKnight
{
    
    public partial class GachaManager : GachaHandler
    {
        
    }
    public partial class GameManager
    {
        [SerializeField]
        private GachaManager m_GachanBanner;

        public void InitGacha()
        {
            m_GachanBanner.Init();
        }
        public void SelectBanner(BannerDefinition definition)
        {
            m_GachanBanner.SelectBanner(definition);

        }
        public void PerformingSingleDraw()
        {
            m_GachanBanner.PerformingSingleDraw();
        }

        public void PerformingMultiDraw()
        {
            m_GachanBanner.PerformingMultiDraw();
        }
        public void AddStarConvertCount(int amount)
        {
            m_GachanBanner.AddStarConvertCount(amount);
        }
        public GachaBanner GetSelectedBanner()
        {
            return m_GachanBanner.GetSelectedBanner();
        }
    }
}
