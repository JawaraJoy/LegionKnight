using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public enum SkillOwner
    {
        Player = 0,
        Boss = 1,
    }
    [System.Serializable]
    public partial class SkillActivation
    {
        [SerializeField]
        private string m_SkillName;
        [SerializeField]
        private SkillOwner m_SkillOwner = SkillOwner.Player;
        [SerializeField]
        private int m_ManaThreshold;
        private int m_Mana;
        [SerializeField]
        private AssetReferenceGameObject m_SkillAsset;
        private AsyncOperationHandle<GameObject> m_Handle;
        [SerializeField]
        private UnityEvent<string> m_OnActive = new();
        [SerializeField]
        private UnityEvent<string, float> m_OnManacharge = new();
        private float m_FillRate;
        public string SkillName => m_SkillName;

        private PassiveSkill m_SkillHandle;
        public void Init(PassiveSkill passive)
        {
            m_SkillHandle = passive;
            //spawn to ui;
        }
        public SkillActivation(SkillDefinition definition)
        {
            m_SkillName = definition.SkillName;
            m_ManaThreshold = definition.Manathreshold;
            m_SkillAsset = definition.SkillAsset;
        }

        private Transform GetSpawnPost()
        {
            Transform target = null;
            switch(m_SkillOwner)
            {
                case SkillOwner.Player:
                    target = Player.Instance.SkillSpawnPost;
                    break;
                case SkillOwner.Boss:
                    target = GameManager.Instance.SpawnedBosenemy.SkillSpawnPost;
                    break;
            }
            return target;
        }

        
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
            SpawnSkill();
            m_OnActive?.Invoke(m_SkillName);
            m_SkillHandle.OnActiveInvoke(SkillName);
        }
        private void OnManaChargeInvoke()
        {
            if (m_Mana >= m_ManaThreshold)
            {
                if (!m_SkillHandle.CanActive) return;
                OnActiveInvoke();
                int rest = m_Mana - m_ManaThreshold;
                m_Mana = Mathf.Clamp(rest, 0, int.MaxValue);
            }
            m_FillRate = (float)m_Mana / (float)m_ManaThreshold;
            m_OnManacharge?.Invoke(m_SkillName, m_FillRate);
            m_SkillHandle.OnManaChargeInvoke(m_SkillName, m_FillRate);
        }

        private void SpawnSkill()
        {
            m_Handle = m_SkillAsset.InstantiateAsync(GetSpawnPost(), false);
            m_Handle.Completed += OnSpawnSkill;
        }

        private void OnSpawnSkill(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out SkillSpawner spawner))
                {
                   //spawner.SpawnProjectile();
                }
                
            }
        }
    }

    public partial class PassiveSkill : MonoBehaviour
    {
        [SerializeField]
        private bool m_CanActive = true;
        [SerializeField]
        private int m_SkillIndex;
        [SerializeField]
        private List<SkillActivation> m_SkillActivations = new();

        [SerializeField]
        private UnityEvent<string> m_OnActive = new();
        [SerializeField]
        private UnityEvent<string, float> m_OnManacharge = new();

        [SerializeField]
        private List<ProjectileAbility> m_ProjectileAbilities = new();

        public bool CanActive => m_CanActive;
        private void Start()
        {
            foreach(SkillActivation skill in m_SkillActivations)
            {
                skill.Init(this);
            }
        }
        public void SetCanActive(bool set)
        {
            m_CanActive = set;
        }
        private ProjectileAbility GetAbilityByName(string abilityName)
        {
            ProjectileAbility match = m_ProjectileAbilities.Find(x => x.AbilityName == abilityName);
            return match;   
        }
        public void AddProjectileAbilities(ProjectileAbility ability)
        {
            m_ProjectileAbilities.Add(ability);
        }
        public void RemoveProjectileAbilities(ProjectileAbility ability)
        {
            m_ProjectileAbilities.Remove(ability);
        }

        public void ActiveProjectileAbility(string abilityName)
        {
            foreach(ProjectileAbility projectile in m_ProjectileAbilities)
            {
                if (projectile.AbilityName == abilityName)
                {
                    projectile.TriggerAbility();
                }
            }
        }

        private SkillActivation GetSkillActivation(string skillName)
        {
            SkillActivation skill = m_SkillActivations.Find(x => x.SkillName == skillName);
            return skill;
        }

        private void ClampSkillIndex()
        {
            m_SkillIndex = Mathf.Clamp(m_SkillIndex, 0, m_SkillActivations.Count - 1);
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
            m_SkillActivations.Clear();
            foreach(SkillDefinition skill in definitions)
            {
                AddSkillInternal(skill);
            }
        }
        public void ForceActivated(int indexSkill)
        {
            if (!m_CanActive) return;
            SetSkillIndexInternal(indexSkill);
            m_SkillActivations[m_SkillIndex].ForceActivated();
        }
        private void SetSkillIndexInternal(int i)
        {
            m_SkillIndex = i;
            ClampSkillIndex();
        }
        public void ForceActivated(string skillName)
        {
            if (!m_CanActive) return;
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
            m_SkillActivations.Add(newSkill);
            newSkill.Init(this);
        }
        public void OnActiveInvoke(string skillName)
        {
            m_OnActive?.Invoke(skillName);
        }
        public void OnManaChargeInvoke(string skillName, float rate)
        {
            m_OnManacharge?.Invoke(skillName, rate);
        }
    }
}
