using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

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

        // taruh semua field yang diperlukan disini untuk mengontrol summon
        // misal lifetime, behavior, dll
        // compoenent ini di add ke unit yang di summon (1 game object dengan unit)

        public void SetSummoner(Summoner summoner)
        {
            m_Summoner = summoner;

            var durationConfig = m_Summoner.SummonConfig.SpawnDuration;

            if (durationConfig.HasDuration)
                m_RemainingLifeTime = durationConfig.Duration;
            else
                m_RemainingLifeTime = 0f;

            if (HasBindInternal(out Damageable damageable))
            {
                damageable.OnDeath.RemoveListener((context) => DeSpawn());
                damageable.OnDeath.AddListener((context) => DeSpawn());
            }
            Unit owner = summoner.AbilityContext.SkillContext.ModuleContext.Unit;
            if (owner.HasBind(out SummonController summonController))
            {
                if (!summonController.Summoners.Contains(summoner))
                    summonController.Summoners.Add(summoner);

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
            m_Summoner.ReturnToPool(this);
        }
    }
}
