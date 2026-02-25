using Rush;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public partial class SkillContainerView : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_SkillViewAsset;

        [SerializeField]
        private List<SkillView> m_SkillViews = new();

        private UnitConfig m_UnitConfig;

        private List<SkillConfig> m_Skills = new();

        public List<SkillView> SkillViews => m_SkillViews;

        private SkillView GetSkillView(string skillName)
        {
            SkillView match = m_SkillViews.Find(x => x.SkillName == skillName);
            return match;
        }
        private void AddSkillViewInternal(SkillView skill)
        {
            if (m_SkillViews.Contains(GetSkillView(skill.SkillName))) return;
            m_SkillViews.Add(skill);
        }
        private void RemoveSkillViewInternal(SkillView skill)
        {
            if (!m_SkillViews.Contains(GetSkillView(skill.SkillName))) return;
            m_SkillViews.Remove(skill);
            Destroy(skill.gameObject);
        }
        private void ClearViews()
        {
            foreach(SkillView skill in m_SkillViews)
            {
                Destroy(skill.gameObject);
            }
            m_SkillViews.Clear();
        }
        public virtual void Refresh()
        {
            ClearViews();

            m_Skills = new(m_UnitConfig.Skills);

            foreach (SkillConfig skill in m_Skills)
            {
                SpawnSkillView(skill);
            }
        }
        public void Init(UnitConfig unitConfig)
        {
            ClearViews();
            m_UnitConfig = unitConfig;
            SkillConfig[] skills = m_UnitConfig.Skills;
            foreach(SkillConfig skill in skills)
            {
                SpawnSkillView(skill);
            }
        }
        private IEnumerator SpawningSkillView(SkillConfig skill)
        {
            AsyncOperationHandle<GameObject> handle = m_SkillViewAsset.InstantiateAsync(m_Content.transform, false);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if(result.TryGetComponent(out SkillView view))
                {
                    view.Init(skill);
                    AddSkillViewInternal(view);
                }
            }
        }
        public void SetFill(string skillName, float fill)
        {
            GetSkillView(skillName).SetFill(fill);
        }
        public void ChargeAmount(string skillName, int amount)
        {
            GetSkillView(skillName).ChargeAmount(amount);
        }
        public void Active(string skillName)
        {
            GetSkillView(skillName).Active();
        }
        private void SpawnSkillView(SkillConfig skillConfig)
        {
            StartCoroutine(SpawningSkillView(skillConfig));
        }
    }
    
}
