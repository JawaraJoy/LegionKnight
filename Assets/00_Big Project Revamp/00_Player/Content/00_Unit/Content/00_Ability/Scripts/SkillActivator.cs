using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class SkillActivator : MonoBehaviour
    {
        [SerializeField]
        private SkillConfig m_SkillConfig;
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
        private UnityEvent<AbilityContext> m_OnActivate;
        public SkillConfig SkillConfig => m_SkillConfig;
        public ProgressField Progression => m_Progression;
        public SkillContext Context => m_Context;
        public List<AbilityDeliver> Delivers => m_Delivers;
        public void Init(SkillConfig skillConfig, ModuleContext ownerContext)
        {
            m_Context = new SkillContext(this, ownerContext);
            m_SkillConfig = skillConfig;
            m_OnInit?.Invoke(m_Context);
            
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
        private void SpawnDelivers()
        {
            SkillConfig config = m_SkillConfig;
            foreach(DamageAbilityConfig ability in config.AbilitySets)
            {
                SpawnDeliver(ability);
            }
        }
        private void SpawnDeliver(DamageAbilityConfig config)
        {
            AbilityDeliver spawned = Instantiate(config.Deliver, m_DeliverSpawnPost, false);
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
        public void Activates()
        {
            foreach (AbilityDeliver abilityDeliver in m_Delivers)
            {
                abilityDeliver.Activate();
                OnActivateInvoke(abilityDeliver.AbilityContext);
            }
            Debug.Log("Ability Activated: " + m_SkillConfig.BaseInfo.Name);
        }
        public void Activate(DamageAbilityConfig config)
        {
            if (HasAbilityInternal(config.BaseInfo.Id, out AbilityDeliver abilityDeliver))
            {
                abilityDeliver.Activate();
                OnActivateInvoke(abilityDeliver.AbilityContext);
            }
        }
        public void Activate(int index)
        {
            if (m_Delivers[index] == null) return;
            m_Delivers[index].Activate();
            OnActivateInvoke(m_Delivers[index].AbilityContext);
        }
        private void OnActivateInvoke(AbilityContext abilityContext)
        {
            m_OnActivate?.Invoke(abilityContext);
        }
    }
}
