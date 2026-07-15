
using UnityEngine;
using Rush;

namespace LegionKnight
{
    public class StageViewManager : MonoBehaviour
    {
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
        public void SetBackGround(StageView stageView)
        {
            m_StageView = stageView;
        }
    }
}
