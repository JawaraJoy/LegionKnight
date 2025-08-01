using UnityEngine;

namespace LegionKnight
{
    public class Background : View
    {
        [SerializeField]
        private OrnamentType m_OrnamentType = OrnamentType.NoOrnament;

        [SerializeField]
        private SpriteRenderer m_StartBackground;
        [SerializeField]
        private LoopTrigger[] m_LoopTriggers;

        [SerializeField]
        private Transform m_LoopTriggersParent;

        private LoopTrigger m_CurrentLoop;

        private BackgroundDefinition m_Definition;
        public BackgroundDefinition Definition => m_Definition;
        public OrnamentType OrnamentType => m_OrnamentType;

        public void SetCurrentLoop(LoopTrigger loopTrigger)
        {
            m_CurrentLoop = loopTrigger;
        }
        public void SetOrnament(OrnamentType ornament)
        {
            m_OrnamentType = ornament;
        }
        private void Awake()
        {
            GameManager.Instance.SetBackGround(this);
        }
        private void Start()
        {
            m_LoopTriggersParent.DetachChildren();
        }

        public void Initialize(LevelDefinition level)
        {
            m_Definition = level.BackgroundDefinition;
            if (m_Definition == null)
            {
                Debug.LogError("BackgroundDefinition is not set in the LevelDefinition.");
                return;
            }
            foreach (LoopTrigger loopTrigger in m_LoopTriggers)
            {
                loopTrigger.Initialize(this);
            }
            m_StartBackground.sprite = m_Definition.StartBackground;
            m_OrnamentType = m_Definition.StartOrnament;
            m_CurrentLoop = m_LoopTriggers[0];
        }

        private void SetOrnamentsInternal(OrnamentType ornament)
        {
            m_OrnamentType = ornament;
            foreach (LoopTrigger loopTrigger in m_LoopTriggers)
            {
                loopTrigger.SetOrnament(ornament);
            }
        }

        public void SetOrnaments(OrnamentType ornament)
        {
            SetOrnamentsInternal(ornament);
        }
    }
}
