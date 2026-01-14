using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class SkillActivator : Bindable, IUpdater
    {
        [SerializeField]
        private SkillActivatorConfig m_SkillConfig;
        [SerializeField]
        private ProgressField m_Progression;
        [SerializeField]
        private Transform m_DeliverSpawnPost;
        [SerializeField, MMReadOnly]
        private SkillContext m_Context;
        [SerializeField, MMReadOnly]
        private List<AbilityDeliver> m_Delivers = new();
        [SerializeField]
        private UnityEvent<SkillContext> m_OnInit;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnActivateIndividu;
        [SerializeField]
        private UnityEvent<SkillContext> m_OnActivates;
        public SkillActivatorConfig SkillConfig => m_SkillConfig;
        public ProgressField Progression => m_Progression;
        public SkillContext Context => m_Context;
        public List<AbilityDeliver> Delivers => m_Delivers;

        public bool IsActive => gameObject.activeInHierarchy;

        [SerializeField, MMReadOnly]
        private float m_RemainingCharge;
        [SerializeField, MMReadOnly]
        private float m_RemainingCooldown = 0f;
        [SerializeField, MMReadOnly]
        private bool m_CooldownTriggered = false;
        [SerializeField, MMReadOnly]
        private bool m_IsSilenced = false;
        public virtual void Init(SkillActivatorConfig skillConfig, ModuleContext ownerContext)
        {
            m_Context = new SkillContext(this, ownerContext);
            m_SkillConfig = skillConfig;
            m_OnInit?.Invoke(m_Context);
            if (m_SkillConfig.Trigger.TriggerState == SkillTriggerState.OnHit)
            {
                if (m_Context.ModuleContext.UnitOwner.HasBind(out Damageable damageable))
                {
                    damageable.OnHit.RemoveAllListeners();
                    damageable.OnHit.AddListener((context) => ActivatesInternal());
                }
            }
            SpawnDelivers();
            Debug.Log("Ability Initialized: " + m_SkillConfig.BaseInfo.Name);
        }
        private AbilityDeliver GetAbilityDeliverInternal(string id)
        {
            return m_Delivers.Find(x => x.Config.BaseInfo.Id == id);
        }
        private bool HasAbilityInternal(string id, out AbilityDeliver abilityDeliver)
        {
            bool has = GetAbilityDeliverInternal(id) != null;
            if (has)
            {
                abilityDeliver = GetAbilityDeliverInternal(id);
            }
            else
            {
                abilityDeliver = null;
            }
            return has;
        }
        private void SetSilencedInternal(bool isSilenced)
        {
            m_IsSilenced = isSilenced;
        }
        private bool CanActivate()
        {
            if (m_IsSilenced)
                return false;

            switch (m_SkillConfig.Trigger.TriggerState)
            {
                case SkillTriggerState.OnCooldownDone:
                    return m_RemainingCooldown <= 0f;

                case SkillTriggerState.OnChargeFull:
                    return m_RemainingCharge >= m_SkillConfig.Trigger.Charge;

                case SkillTriggerState.OnHit:
                    return true; // event-driven, dicek di event

                default:
                    return false;
            }
        }
        public void AddCharge(float amount)
        {
            AddChargeInternal(amount);
        }
        private void AddChargeInternal(float amount)
        {
            if (m_SkillConfig.Trigger.TriggerState == SkillTriggerState.OnChargeFull)
            {
                m_RemainingCharge += amount;
                if (IsChargeReady)
                {
                    if (m_SkillConfig.Trigger.AutoActiveOnReady)
                    {
                        ActivatesInternal();
                    }
                    float exceedCharge = m_RemainingCharge - m_SkillConfig.Trigger.Charge;
                    m_RemainingCharge = exceedCharge;
                }
            }
        }
        private bool IsChargeReady
        {
            get
            {
                return m_SkillConfig.Trigger.TriggerState != SkillTriggerState.OnChargeFull || m_RemainingCharge >= m_SkillConfig.Trigger.Charge;
            }
        }
        public void Tick()
        {
            if (!IsActive || m_IsSilenced)
                return;

            if (m_SkillConfig.Trigger.TriggerState != SkillTriggerState.OnCooldownDone)
                return;

            if (m_RemainingCooldown > 0f)
            {
                m_RemainingCooldown -= Time.deltaTime;

                if (m_RemainingCooldown <= 0f)
                {
                    m_RemainingCooldown = 0f;

                    if (!m_CooldownTriggered)
                    {
                        m_CooldownTriggered = true;

                        if (m_SkillConfig.Trigger.AutoActiveOnReady)
                        {
                            ActivatesInternal();
                        }
                    }
                }
            }
        }


        private void SpawnDelivers()
        {
            SkillActivatorConfig config = m_SkillConfig;
            foreach(AbilityConfig ability in config.AbilitySets)
            {
                SpawnDeliver(ability);
            }
        }
        private void SpawnDeliver(AbilityConfig config)
        {
            AbilityDeliver spawned = Instantiate(config.DeliverPrefab, m_DeliverSpawnPost, false);
            spawned.Init(config, m_Context);

            // Register Check
            if (m_Delivers.Contains(spawned))
            {
                m_Delivers.Remove(spawned);
            }
            else
            {
                m_Delivers.Add(spawned);
            }
        }
        public void ForceActivates()
        {
            ForceActivatesInternal();
        }
        private void ActivatesInternal()
        {
            if (!CanActivate())
                return;

            switch (m_SkillConfig.Trigger.TriggerState)
            {
                case SkillTriggerState.OnCooldownDone:
                    m_RemainingCooldown = m_SkillConfig.Trigger.Cooldown;
                    m_CooldownTriggered = false;
                    break;

                case SkillTriggerState.OnChargeFull:
                    m_RemainingCharge = 0f;
                    break;
            }

            ForceActivatesInternal();
        }
        private void ForceActivatesInternal()
        {
            if (m_IsSilenced) return;
            foreach (AbilityDeliver deliver in m_Delivers)
            {
                ActivateInternal(deliver.Config);
            }
            OnActivatesInvoke();
        }

        public void ForceActivate(AbilityConfig config)
        {
            if (m_IsSilenced) return;
            ActivateInternal(config);
            OnActivatesInvoke();
        }
        public void ForceActivate(int index)
        {
            if (m_Delivers[index] == null) return;
            if (m_IsSilenced) return;
            ActivateInternal(m_Delivers[index].Config);
            OnActivatesInvoke();
        }

        private void ActivateInternal(AbilityConfig config)
        {
            if (HasAbilityInternal(config.BaseInfo.Id, out AbilityDeliver abilityDeliver))
            {
                abilityDeliver.Activate();
                OnActivateIndividuInvoke(abilityDeliver.AbilityContext);
            }
        }
        private void OnActivateIndividuInvoke(AbilityContext abilityContext)
        {
            m_OnActivateIndividu?.Invoke(abilityContext);
        }
        private void OnActivatesInvoke()
        {
            m_OnActivates?.Invoke(m_Context);
        }
    }
}
