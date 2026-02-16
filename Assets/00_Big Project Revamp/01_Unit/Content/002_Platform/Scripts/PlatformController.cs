using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public partial class PlatformController : MonoBehaviour, IUnitExtension, IHasSkills
    {
        [SerializeField, MMReadOnly]
        private List<PlatformConfig> m_PlatformConfigs = new();
        [SerializeField, MMReadOnly]
        private ModuleContext m_ModuleContext;

        public List<PlatformConfig> PlatformConfigs => m_PlatformConfigs;

        private SkillController m_SkillController;
        private IReadOnlyList<Skill> SkillsInternal => m_SkillController.Skills;
        public IReadOnlyList<Skill> Skills => SkillsInternal;
        
        public IModuleContext ModuleContext =>  m_ModuleContext;

        
        public Skill GetSkillActivator(SkillConfig config)
        {
            return m_SkillController.GetSkillActivator(config);
        }

        public IReadOnlyList<Skill> GetSkillsByMultiCategory(SkillCategoryConfig[] skillCategories)
        {
            return m_SkillController.GetSkillsByMultiCategory(skillCategories);
        }

        public bool HasSkillActivator(SkillConfig config, out Skill skill)
        {
            return m_SkillController.HasSkillActivator(config, out skill);
        }

        public void Init(Unit unitOwner)
        {
            m_ModuleContext = new ModuleContext(unitOwner, gameObject);
            if (unitOwner.Config is IHasPlatform platformConfig)
            {
                m_PlatformConfigs.Clear();
                m_PlatformConfigs = new(platformConfig.UniquePlatforms);
            }
            if (m_ModuleContext.Unit.HasBind(out SkillController skillController))
            {
                m_SkillController = skillController;
                foreach(PlatformConfig config in m_PlatformConfigs)
                {
                    RushGameManager.Instance.PlatformManager.AddPreparedPlatformConfig(config, m_ModuleContext.Unit.gameObject);
                    m_SkillController.AddNewSkills(config.SkillOnLeftTouch.OnNormalTouchSkill);
                    m_SkillController.AddNewSkills(config.SkillOnLeftTouch.OnPerfectTouchSkill);
                    m_SkillController.AddNewSkills(config.SkillOnRightTouch.OnNormalTouchSkill);
                    m_SkillController.AddNewSkills(config.SkillOnRightTouch.OnPerfectTouchSkill);
                    
                }
            }
            
        }
    }
}
