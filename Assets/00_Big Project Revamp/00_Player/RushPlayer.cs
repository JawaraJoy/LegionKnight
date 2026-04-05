using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class RushPlayer : Singleton<RushPlayer>
    {
        [SerializeField]
        private UnityEvent<UnitConfig> m_OnInitizlied;

        public void Init(UnitConfig lastUsedHero)
        {
            m_OnInitizlied?.Invoke(lastUsedHero);
            Debug.Log($"RushPlayer Initialized with {lastUsedHero.BaseInfo.Name}");
        }

    }
}
