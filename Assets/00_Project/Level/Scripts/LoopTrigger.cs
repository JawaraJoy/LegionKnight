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
        private SpriteRenderer m_Base;
        [SerializeField]
        private SpriteRenderer m_Ornament;
        [SerializeField]
        private Transform m_NextLoopTransform;

        [SerializeField]
        private LoopTrigger m_NextLoopTrigger;
        [SerializeField]
        private UnityEvent<LoopInteractor> m_OnLoopTrigger = new();

        private Background m_Background;

        public void Initialize(Background background)
        {
            m_Background = background;
            m_Base.sprite = m_Background.Definition.BaseLoop;
            m_Ornament.sprite = null;
        }
        private void SetOrnamentInternal(OrnamentType ornament)
        {
            BackgroundDefinition definition = m_Background.Definition;
            if (definition == null)
            {
                Debug.LogError("BackgroundDefinition is not set in the Background.");
                return;
            }
            switch (ornament)
            {
                case OrnamentType.NoOrnament:
                    m_Ornament.sprite = null;
                    break;
                case OrnamentType.Entrance:
                    m_Ornament.sprite = m_Background.Definition.EntranceOrnament;
                    break;
                case OrnamentType.Full:
                    m_Ornament.sprite = m_Background.Definition.EntranceOrnament;
                    break;
            }
            //bool hasOrnament = m_Ornament.sprite != null;
            //m_Ornament.gameObject.SetActive(hasOrnament);
        }
        public void SetOrnament(OrnamentType ornament)
        {
            SetOrnamentInternal(ornament);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out LoopInteractor interactor))
            {
                m_OnLoopTrigger.Invoke(interactor);
                Loop();
            }
        }

        private void Loop()
        {
            Vector2 position = m_NextLoopTransform.position;
            m_NextLoopTrigger.SetPosition(position);
            m_Background.SetCurrentLoop(m_NextLoopTrigger); 
            m_NextLoopTrigger.SetOrnamentInternal(m_Background.OrnamentType);
            if (m_Background.OrnamentType == OrnamentType.Entrance)
            {
                m_Background.SetOrnament(OrnamentType.Full);
            }
        }

        private void SetPosition(Vector2 position)
        {
            transform.localPosition = position;
        }
    }
}
