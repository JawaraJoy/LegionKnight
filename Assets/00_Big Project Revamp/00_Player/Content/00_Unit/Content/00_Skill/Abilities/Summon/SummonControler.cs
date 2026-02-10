using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class SummonControler : MonoBehaviour, IUpdater
    {
        [SerializeField, MMReadOnly]
        private Summoner m_Summoner;
        [SerializeField, MMReadOnly]
        private float m_RemainingLifeTime = 1.0f;
        [SerializeField, MMReadOnly]
        private Unit m_Unit;
        public bool IsActive => gameObject.activeSelf;

        // taruh semua field yang diperlukan disini untuk mengontrol summon
        // misal lifetime, behavior, dll
        // compoenent ini di add ke unit yang di summon (1 game object dengan unit)
        
        public void Init(Summoner summoner)
        {
            m_Summoner = summoner;
            if (TryGetComponent<Unit>(out var unit))
            {
                m_Unit = unit;
                m_RemainingLifeTime = m_Summoner.SummonConfig.SpawnDuration.Duration;
            }
        }

        private void OnEnable()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }
        private void OnDisable()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        public void Tick()
        {
            bool hasDuration = m_Summoner.SummonConfig.SpawnDuration.HasDuration;
            if (!hasDuration) return;
            m_RemainingLifeTime -= Time.deltaTime;
            if (m_RemainingLifeTime < 0.0f)
            {
                DeSpawn();
            }
        }
        private void DeSpawn()
        {
            m_Summoner.ReturnToPool(m_Unit);
        }
    }
}
