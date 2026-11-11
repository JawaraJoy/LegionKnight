using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class SummonerUnit : Unit, IUpdater
    {

        [SerializeField, MMReadOnly]
        private Summoner m_Summoner;
        [SerializeField, MMReadOnly]
        private float m_RemainingLifeTime = 1.0f;
        public bool IsActive => gameObject.activeSelf;
        public Summoner Summoner => m_Summoner;

        private Damageable m_DeathPublisher;

        public void SetSummoner(Summoner summoner)
        {
            if (m_DeathPublisher != null) m_DeathPublisher.OnDeath.RemoveListener(HandleDeath);
            m_DeathPublisher = null;
            m_Summoner = summoner;
            if (summoner == null) return;

            var duration = summoner.SummonConfig.SpawnDuration;
            m_RemainingLifeTime = duration.HasDuration ? duration.Duration : 0f;
            if (HasBindInternal(out m_DeathPublisher))
                m_DeathPublisher.OnDeath.AddListener(HandleDeath);
        }

        private void HandleDeath(IAbilityContext context) => DeSpawn();

        private void OnEnable()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }
        private void OnDisable()
        {
            if (UpdateBank.Instance != null) UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        public void Tick()
        {
            if (m_Summoner == null)
                return;

            var durationConfig = m_Summoner.SummonConfig.SpawnDuration;

            if (!durationConfig.HasDuration)
                return;

            m_RemainingLifeTime -= Time.deltaTime;

            if (m_RemainingLifeTime <= 0f)
            {
                DeSpawn();
            }
        }

        private void DeSpawn()
        {
            if (m_Summoner != null) m_Summoner.ReturnToPool(this);
        }
    }
}
