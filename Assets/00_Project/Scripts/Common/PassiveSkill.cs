using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    [System.Serializable]
    public partial class SkillActivation
    {
        [SerializeField]
        private string m_SkillName;
        [SerializeField]
        private int m_ManaThreshold;
        private int m_Mana;
        private AssetReferenceGameObject m_SkillPrefab;
        private AsyncOperationHandle<GameObject> m_Handle;
        [SerializeField]
        private UnityEvent<string> m_OnActive = new();
        [SerializeField]
        private UnityEvent<string, float> m_OnManacharge = new();

        public string SkillName => m_SkillName;
        public void Init()
        {
            //spawn to ui;
        }
        public SkillActivation(SkillDefinition definition)
        {
            m_SkillName = definition.SkillName;
            m_ManaThreshold = definition.Manathreshold;
            m_SkillPrefab = definition.SkillPrefab;
        }

        private float m_FillRate;
        public void ForceActivated()
        {
            OnActiveInvoke();
        }
        public void AddMana(int add)
        {
            m_Mana += add;
            OnManaChargeInvoke();
        }
        public void SetMana(int set)
        {
            m_Mana = set;
            OnManaChargeInvoke();
        }

        private void OnActiveInvoke()
        {
            //SpawnSkill();
            m_OnActive?.Invoke(m_SkillName);
        }
        private void OnManaChargeInvoke()
        {
            if (m_Mana >= m_ManaThreshold)
            {
                OnActiveInvoke();
                int rest = m_Mana - m_ManaThreshold;
                m_Mana = Mathf.Clamp(rest, 0, int.MaxValue);
            }
            m_FillRate = (float)m_Mana / (float)m_ManaThreshold;
            m_OnManacharge?.Invoke(m_SkillName, m_FillRate);
        }

        private void SpawnSkill()
        {
            m_Handle = m_SkillPrefab.InstantiateAsync();
            m_Handle.Completed += OnSpawnSkill;
        }

        private void OnSpawnSkill(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out SkillSpawner spawner))
                {
                    spawner.SpawnProjectile();
                }
            }
        }
    }

    public partial class PassiveSkill : MonoBehaviour
    {
        [SerializeField]
        private Transform m_SpawnContainer;
        [SerializeField]
        private List<SkillActivation> m_SkillActivations = new();
        public Transform SpawnContainer => m_SpawnContainer;

        private SkillActivation GetSkillActivation(string skillName)
        {
            SkillActivation skill = m_SkillActivations.Find(x => x.SkillName == skillName);
            return skill;
        }

        public void AddManaToAll(int add)
        {
            foreach(SkillActivation skill in m_SkillActivations)
            {
                skill.AddMana(add);
            }
        }
        public void Init(List<SkillDefinition> definitions)
        {
            foreach(SkillDefinition skill in definitions)
            {
                AddSkillInternal(skill);
            }
        }
        public void ForceActivated(int indexSkill)
        {
            m_SkillActivations[indexSkill].ForceActivated();
        }
        public void ForceActivated(string skillName)
        {
            GetSkillActivation(skillName).ForceActivated();
        }

        public void AddOneMana(int indexSkill)
        {
            m_SkillActivations[indexSkill].AddMana(1);
        }
        public void AddMana(string skillName, int add)
        {
            GetSkillActivation(skillName).AddMana(add);
        }
        public void SetMana(string skillName, int add)
        {
            GetSkillActivation(skillName).SetMana(add);
        }
        public void ResetMana(int indexSkill)
        {
            m_SkillActivations[indexSkill].SetMana(0);
        }
        public void AddSkill(SkillDefinition definition)
        {
            AddSkillInternal(definition);
        }
        public void AddSkillInternal(SkillDefinition definition)
        {
            SkillActivation newSkill = new SkillActivation(definition);
            if (m_SkillActivations.Contains(newSkill)) return;
            m_SkillActivations.Add(newSkill);
            newSkill.Init();
        }
    }
}
