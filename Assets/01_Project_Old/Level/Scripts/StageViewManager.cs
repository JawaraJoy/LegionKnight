
using UnityEngine;
using Rush;
using MoreMountains.Tools;

namespace LegionKnight
{
    public class StageViewManager : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private StageView m_StageView;

        public void Initialize(StageConfig stageConfig)
        {
            if (m_StageView == null)
            {
                Debug.LogError("Background is not set. Please assign a Background instance to the BackgroundManager.");
                return;
            }
            m_StageView.Initialize(stageConfig);
        }
        public void SetBackGround(StageView background)
        {
            m_StageView = background;
        }
    }

    public partial class GameManager
    {
        [SerializeField]
        private StageViewManager m_BackgroundManager;
        public StageViewManager BackgroundManager => m_BackgroundManager;
    }
}
