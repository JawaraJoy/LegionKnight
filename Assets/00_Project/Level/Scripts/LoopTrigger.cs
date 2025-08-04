using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public enum OrnamentType
    {
        NoOrnament = 0,
        Entrance = 1,
        Full = 2,
    }
    public class LoopTrigger : View
    {
        [SerializeField]
        private OrnamentType m_OrnamentType = OrnamentType.NoOrnament;
        [SerializeField]
        private SpriteRenderer m_Base;
        [SerializeField]
        private SpriteRenderer m_Ornament;
        [SerializeField]
        private Transform m_NextLoopTransform;

        [SerializeField]
        private LoopTrigger m_NextLoopTrigger;
        [SerializeField]
        private UnityEvent<LoopInteractor> m_OnLoopTrigger = new();
        public OrnamentType OrnamentType => m_OrnamentType;

        private Background m_Background;

        private bool m_Triggered = false;
        public bool Triggered => m_Triggered;

        public void Initialize(Background background)
        {
            m_Background = background;
            m_Base.sprite = m_Background.Definition.BaseLoop;
            m_Ornament.sprite = null;
            m_Triggered = false;
        }
        private void SetOrnamentInternal(OrnamentType ornament)
        {
            BackgroundDefinition definition = m_Background.Definition;
            if (definition == null)
            {
                Debug.LogError("BackgroundDefinition is not set in the Background.");
                return;
            }
            m_OrnamentType = ornament;
            switch (ornament)
            {
                case OrnamentType.NoOrnament:
                    m_Ornament.enabled = false;
                    break;
                case OrnamentType.Entrance:
                    m_Ornament.sprite = m_Background.Definition.EntranceOrnament;
                    m_Ornament.enabled = true;
                    break;
                case OrnamentType.Full:
                    m_Ornament.sprite = m_Background.Definition.OrnamentLoop;
                    m_Ornament.enabled = true;
                    break;
            }
            //bool hasOrnament = m_Ornament.sprite != null;
            //m_Ornament.gameObject.SetActive(hasOrnament);
        }
        public void SetOrnament(OrnamentType ornament)
        {
            // Only allow Entrance to be set once
            if (ornament == OrnamentType.Entrance)
            {
                if (m_Triggered)
                {
                    // If entrance already set, use Full instead
                    SetOrnamentInternal(OrnamentType.Full);
                }
                else
                {
                    SetOrnamentInternal(OrnamentType.Entrance);
                    m_Triggered = true;
                }
            }
            else
            {
                SetOrnamentInternal(ornament);
            }
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
            
            m_Background.SetCurrentLoop(m_NextLoopTrigger);
            /*if (m_NextLoopTrigger.Triggered == false)
            {
                m_NextLoopTrigger.SetOrnament(m_Background.OrnamentType);
            }
            if (m_Background.OrnamentType == OrnamentType.Entrance)
            {
                m_Background.SetOrnament(OrnamentType.Full);
            }*/
            m_NextLoopTrigger.SetTriggered(true);
        }

        private void SetPosition(Vector2 position)
        {
            transform.localPosition = position;
        }
    }
}
