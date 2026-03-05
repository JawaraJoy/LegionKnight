using UnityEngine;
using LegionKnight;
using MoreMountains.Tools;
using System.Collections.Generic;

namespace Rush
{
    public class SkillControllerView : UIView
    {
        [SerializeField]
        private SkillCategoryConfig m_SkillCategoryConfig;
        [SerializeField]
        private ActivationMode m_ActivationMode = ActivationMode.Queue;
        [SerializeField]
        private SkillView m_SkillViewPrefab;

        private CategorySkillController m_SkillController;
        [SerializeField, MMReadOnly]
        private List<SkillView> m_SpawnedSkillViews = new List<SkillView>();    

        public virtual void AddSkillView(Skill skill)
        {
            SkillView view = Instantiate(m_SkillViewPrefab, transform);
            view.Init(skill);
        }
    }
}
