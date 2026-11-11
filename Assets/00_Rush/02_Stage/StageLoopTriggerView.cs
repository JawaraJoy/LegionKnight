using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class StageLoopTriggerView : View
    {
        [SerializeField]
        private SpriteRenderer m_Base;
        [SerializeField]
        private Transform m_NextLoopTransform;

        [SerializeField]
        private StageLoopTriggerView m_NextLoopTrigger;
        [SerializeField]
        private UnityEvent<LoopInteractor> m_OnLoopTrigger = new();

        private StageView m_StageView;

        private bool m_Triggered = false;
        public bool Triggered => m_Triggered;

        public void Initialize(StageView stageView)
        {
            m_StageView = stageView;
            //m_Base.sprite = m_StageView.StageConfig.BackgroundSetField.BaseLoop;
            m_Triggered = false;
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out LoopInteractor interactor))
            {
                m_OnLoopTrigger.Invoke(interactor);
                Loop();
            }
        }

        public void SetTriggered(bool triggered)
        {
            m_Triggered = triggered;
        }

        private void Loop()
        {
            m_Triggered = false;
            Vector2 position = m_NextLoopTransform.position;
            m_NextLoopTrigger.SetPosition(position);
            
            m_StageView.SetCurrentLoop(m_NextLoopTrigger);
            m_NextLoopTrigger.SetTriggered(true);
        }

        private void SetPosition(Vector2 position)
        {
            transform.localPosition = position;
        }
    }
}
