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
    }
}
