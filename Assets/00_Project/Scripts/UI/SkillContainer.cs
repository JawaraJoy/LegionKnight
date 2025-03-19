using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public partial class SkillContainer : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_SkillViewAsset;

        [SerializeField]
        private List<SkillView> m_SkillViews = new();

        private CharacterDefinition m_CharacterDefnition;

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
        public void Init()
        {
            ClearViews();
            m_CharacterDefnition = Player.Instance.CharacterDefinition;
            List<SkillDefinition> skills = m_CharacterDefnition.Passives;
            foreach (SkillDefinition skill in skills)
            {
                SpawnSkillView(skill);
            }
        }
        public void Init(CharacterDefinition definition)
        {
            ClearViews();
            m_CharacterDefnition = definition;
            List<SkillDefinition> skills = m_CharacterDefnition.Passives;
            foreach(SkillDefinition skill in skills)
            {
                SpawnSkillView(skill);
            }
        }
        private IEnumerator SpawningSkillView(SkillDefinition skill)
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
        public void Active(string skillName)
        {
            GetSkillView(skillName).Active();
        }
        private void SpawnSkillView(SkillDefinition skill)
        {
            StartCoroutine(SpawningSkillView(skill));
        }
    }
    
}
