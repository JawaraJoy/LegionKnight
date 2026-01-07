using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class Skill : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private List<SkillActivator> m_SkillContexts = new();
        private OwnerContext m_OwnerContext;
        public void Init(OwnerContext ownerContext)
        {
            m_OwnerContext = ownerContext;
            SkillConfig[] skills = ownerContext.UnitConfig.Skills;
            for (int i = 0; i < skills.Length; i++)
            {
                var skillonfig = skills[i];
                SkillContext skillContext = new (skillonfig, ownerContext.UnitObject);
                RegisterSkillInternal(skillContext);
            }
        }

        private SkillContext GetSkillContextInternal(string id)
        {
            return m_SkillContexts.Find(context => context.Config.BaseInfo.Id == id);
        }

        private void RegisterSkillInternal(SkillContext context)
        {
            if (GetSkillContextInternal(context.Config.BaseInfo.Id) == null)
            {
                m_SkillContexts.Add(context);
                SkillContext existed = GetSkillContextInternal(context.Config.BaseInfo.Id);
                existed.Init();
            }
        }
        private void UnregisterSkillInternal(SkillContext context)
        {
            if (GetSkillContextInternal(context.Config.BaseInfo.Id) != null)
            {
                m_SkillContexts.Remove(context);
            }
        }
    }

    public partial class OwnerContext
    {

    }
}
