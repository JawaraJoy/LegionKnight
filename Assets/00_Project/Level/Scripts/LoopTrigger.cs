using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public enum OrnamentType
    {
        NoOrnament,
        Entrance,
        Full,
    }
    public class LoopTrigger : View
    {
        [SerializeField]
        private OrnamentType m_OrnamentType = OrnamentType.NoOrnament;
        [SerializeField]
        private OrnamentType m_NextLoopOrnamentType = OrnamentType.NoOrnament;

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
        }

        private void SetPosition(Vector2 position)
        {
            transform.localPosition = position;
        }
    }
}
