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
            TryGetComponent(out m_Rigidbody);
        }
        public void Init(UnitConfig lastUsedHero)
        {
            InitInternal(lastUsedHero);
        }
        private void InitInternal(UnitConfig lastUsedHero)
        {
            OnInitializedInvoke(lastUsedHero);
            Debug.Log($"RushPlayer Initialized with {lastUsedHero.BaseInfo.Name}");
        }
        private void RepositionInternal()
        {
            SetPositionInternal(m_InitialPosition);
        }
        private void SetPositionInternal(Vector2 set)
        {
            if (m_Rigidbody == null)
                TryGetComponent(out m_Rigidbody);
            if (m_Rigidbody != null)
            {
                m_Rigidbody.linearVelocity = Vector2.zero;
                m_Rigidbody.angularVelocity = 0f;
                m_Rigidbody.position = set;
            }
            transform.position = set;
        }
        public void SetPosition(Vector2 set)
        {
            SetPositionInternal(set);
        }
        public void ResetProgression()
        {
            m_CardBonuses?.ResetProgression();
            RepositionInternal();
            if (m_Unit.Config != null)
                m_Unit.RefreshInit();
            OnResetInvoke();
        }

private void OnInitializedInvoke(UnitConfig unitConfig)
        {
            m_OnInitizlied?.Invoke(unitConfig);
        }

        private void OnResetInvoke()
        {
            m_OnReset?.Invoke();
        }

    }
}
