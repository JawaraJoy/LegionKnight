using Rush;
using UnityEngine;

namespace LegionKnight
{
    public class StageView : View
    {
        [SerializeField]
        private SpriteRenderer m_StartBackground;
        [SerializeField]
        private StageLoopTriggerView[] m_LoopTriggers;

        [SerializeField]
        private Transform m_LoopTriggersParent;

        private StageLoopTriggerView m_CurrentLoop;

        private StageConfig m_StageConfig;
        public StageConfig StageConfig => m_StageConfig;

        public void SetCurrentLoop(StageLoopTriggerView loopTrigger)
        {
            m_CurrentLoop = loopTrigger;
        }
        private void Start()
        {
            m_LoopTriggersParent.DetachChildren();
        }
        private void OnDestroy()
        {
            
        }

        public void Initialize(StageConfig stage)
        {
            m_StageConfig = stage;
            if (m_StageConfig == null)
            {
                Debug.LogError("BackgroundDefinition is not set in the LevelDefinition.");
                return;
            }
            foreach (StageLoopTriggerView loopTrigger in m_LoopTriggers)
            {
                loopTrigger.Initialize(this);
            }
            //m_StartBackground.sprite = m_StageConfig.BackgroundSetField.StartBackground;
            m_CurrentLoop = m_LoopTriggers[0];
        }
    }
}
