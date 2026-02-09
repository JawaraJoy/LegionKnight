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
        private SkillOwner m_SkillOwner = SkillOwner.Player;
        [SerializeField]
        private AssetReferenceGameObject m_SkillViewAsset;

        [SerializeField]
        private List<SkillView> m_SkillViews = new();

        private CharacterDefinition m_CharacterDefnition;
        private BosDefinition m_BosDefinition;

        private List<SkillDefinition> m_Skills = new();

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
        public virtual void Init()
        {
            ClearViews();
            
            switch(m_SkillOwner)
            {
                case SkillOwner.Player:
                    m_CharacterDefnition = Player.Instance.CharacterDefinition;
                    m_Skills = new(m_CharacterDefnition.Passives);
                    break;
                case SkillOwner.Boss:
                    m_BosDefinition = GameManager.Instance.GetSpawnedBosEnemy().BosDefinition;
                    m_Skills = new(m_BosDefinition.Skills);
                    break;
            }
            foreach (SkillDefinition skill in m_Skills)
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
        public void Init(BosDefinition definition)
        {
            ClearViews();
            m_BosDefinition = definition;
            List<SkillDefinition> skills = new (m_BosDefinition.Skills);

            foreach (SkillDefinition skill in skills)
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
        public void ChargeAmount(string skillName, int amount)
        {
            GetSkillView(skillName).ChargeAmount(amount);
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
