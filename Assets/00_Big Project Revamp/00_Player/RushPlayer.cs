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
        }
        public void SetPosition(Vector2 set)
        {
            SetPositionInternal(set);
        }
        public void ResetProgression()
        {
            RepositionInternal();
            if (m_Unit.Config != null)
                InitInternal(m_Unit.Config);
        }
    }
}
