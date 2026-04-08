using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class RushPlayer : Singleton<RushPlayer>, IReseter
    {
        [SerializeField]
        private Vector2 m_InitialPosition;
        [SerializeField]
        private UnityEvent<UnitConfig> m_OnInitizlied;

        private Rigidbody2D m_Rigidbody;
        public Vector2 InitialPosition => m_InitialPosition;

        [SerializeField]
        private UnityEvent m_OnReset;
        private void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();
        }
        public void Init(UnitConfig lastUsedHero)
        {
            InitInternal(lastUsedHero);
        }
        private void InitInternal(UnitConfig lastUsedHero)
        {
            m_OnInitizlied?.Invoke(lastUsedHero);
            Debug.Log($"RushPlayer Initialized with {lastUsedHero.BaseInfo.Name}");
        }
        private void RepositionInternal()
        {
            transform.position = m_InitialPosition;
        }
        private void SetPositionInternal(Vector2 set)
        {
            transform.position = set;
            m_Rigidbody.linearVelocity = Vector2.zero;
        }
        public void SetPosition(Vector2 set)
        {
            SetPositionInternal(set);
        }
        public void ResetProgression()
        {
            RepositionInternal();
            if (m_Unit.Config != null)
                m_Unit.RefreshInit();
            m_OnReset?.Invoke();
        }
    }
}
