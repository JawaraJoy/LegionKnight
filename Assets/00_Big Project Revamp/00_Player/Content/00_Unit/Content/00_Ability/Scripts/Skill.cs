using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class Skill : MonoBehaviour
    {
        [SerializeField]
        private Transform m_ActivatorSpawnPost;
        [SerializeField, MMReadOnly]
        private ModuleContext m_ModuleContext;
        [SerializeField, MMReadOnly]
        private List<SkillActivator> m_SkillActivators = new();

        public void Init(Unit unitOwner)
        {
            m_ModuleContext = new ModuleContext(unitOwner, gameObject);
            SpawnActivators();
        }

        private SkillActivator GetSkillContextInternal(string id)
        {
            return m_SkillActivators.Find(x => x.Context.Activator.SkillConfig.BaseInfo.Id == id);
        }
        private void SpawnActivators()
        {
            if (!m_ModuleContext.Initialized) return;
            SkillConfig[] skills = m_ModuleContext.UnitOwner.Config.Skills;
            
            for (int i = 0; i < skills.Length; i++)
            {
                var skillConfig = skills[i];
                SpawnActivator(skillConfig);
            }
        }
        private void SpawnActivator(SkillConfig config)
        {
            SkillActivator spawned = Instantiate(config.ActivatorPrefab, m_ActivatorSpawnPost, false);
            spawned.Init(config, m_ModuleContext);
            RegisterSkillInternal(spawned);
        }
        private void RegisterSkillInternal(SkillActivator activator)
        {
            if (GetSkillContextInternal(activator.SkillConfig.BaseInfo.Id) == null)
            {
                m_SkillActivators.Add(activator);
            }
        }
        private void UnregisterSkillInternal(SkillActivator activator)
        {
            if (GetSkillContextInternal(activator.SkillConfig.BaseInfo.Id) != null)
            {
                m_SkillActivators.Remove(activator);
            }
        }
    }
}
