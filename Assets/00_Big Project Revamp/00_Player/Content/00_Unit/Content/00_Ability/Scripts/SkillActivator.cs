using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class SkillActivator : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private SkillContext m_SkillContext;
        [SerializeField, MMReadOnly]
        private List<AbilityDeliver> m_Delivers = new();
        [SerializeField]
        private UnityEvent<SkillContext> m_OnInit;
        [SerializeField]
        private UnityEvent<SkillContext> m_OnActivate;
        public SkillContext Context => m_SkillContext;
        public List<AbilityDeliver> Delivers => m_Delivers;
        [SerializeField]
        private Transform m_DeliverSpawnPost;
        public void Init(SkillContext context)
        {
            m_SkillContext = context;
            m_OnInit?.Invoke(m_SkillContext);
            
            SpawnDelivers();

            Debug.Log("Ability Initialized: " + m_SkillContext.Config.BaseInfo.Name);
        }
        private void SpawnDelivers()
        {
            SkillConfig config = m_SkillContext.Config;
            foreach(AbilityConfig ability in config.AbilitySets)
            {
                SpawnDeliver(ability);
            }
        }
        private void SpawnDeliver(AbilityConfig config)
        {
            AbilityDeliver spawned = Instantiate(config.Deliver, m_DeliverSpawnPost, false);
            spawned.Init(new AbilityContext(m_SkillContext, config));
            if (m_Delivers.Contains(spawned))
            {
                m_Delivers.Remove(spawned);
            }
            else
            {
                m_Delivers.Add(spawned);
            }
        }
        public void Activate()
        {
            foreach (AbilityDeliver ability in m_Delivers)
            {
                ability.Activate();
            }
            Debug.Log("Ability Activated: " + m_SkillContext.Config.BaseInfo.Name);
            m_OnActivate?.Invoke(m_SkillContext);
        }
    }
}
