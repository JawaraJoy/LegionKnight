
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class StatController : MonoBehaviour, IUnitExtension, IReseter
    {
        [SerializeField]
        private Transform m_ModifierPost;
        private List<StatModifier> m_Modifiers = new ();
        public Transform ModifierPost => m_ModifierPost;
        public List<StatModifier> StatModifiers => m_Modifiers;

        public IModuleContext ModuleContext => m_ModuleContext;

        private ModuleContext m_ModuleContext;

        public void Init(Unit unit)
        {
            m_ModuleContext = new ModuleContext(unit, gameObject);
            //RemoveAllModifier();
        }
        private StatModifier GetStatInfluencerInternal(AbilityConfig config)
        {
            return m_Modifiers.Find(x => x.Context.AbilityContext.AbilityDeliver.AbilityConfig.BaseInfo.Id == config.BaseInfo.Id);
        }
        private bool HasStatInfluencer(AbilityContext abilityContext, out StatModifier modifier)
        {
            AbilityConfig config = abilityContext.AbilityDeliver.AbilityConfig;
            bool hasStatInfluencer = GetStatInfluencerInternal(config) != null;
            if (hasStatInfluencer)
            {
                modifier = GetStatInfluencerInternal(config);
            }
            else
            {
                modifier = null;
            }
            return hasStatInfluencer;
        }
        public void UpdateStack(AbilityContext abilityContext)
        {
            if (HasStatInfluencer(abilityContext, out StatModifier found))
            {
                if (found.IsActive)
                {
                    found.UpdateStack();
                }
            }
        }
        public void AddModifier(AbilityContext abilityContext, StatController targetController)
        {
            if (HasStatInfluencer(abilityContext, out StatModifier found))
            {
                
                if (found.IsActive)
                {
                    found.UpdateStack();
                }
                else
                {
                    found.Activate(abilityContext, targetController);
                }
            }
            else
            {
                
                if (abilityContext.AbilityDeliver is IStatModifierDeliver modifier)
                {
                    StatModifier prefab = modifier.ModifierConfig.StatModifierPrefab;
                    StatModifier spawnedModifier = Instantiate(prefab, m_ModifierPost, false);
                    m_Modifiers.Add(spawnedModifier);
                    spawnedModifier.Activate(abilityContext, targetController);
                    
                }
            }
            AbilityUltility.OnAbilityDeliveredInvoke(abilityContext, m_ModuleContext.Unit);
            if (m_ModuleContext.Unit.HasBind(out Damageable damageable))
            {
                damageable.RefreshDamageableStat(1f, false);
            }
            //OnSkillDeliveredInvoke(abilityContext, m_ModuleContext.UnitOwner);
        }
        public StatField GetFinalStat(StatField unitStat)
        {
            StatField finalAdditionalStat = StatField.Zero;
            foreach (StatModifier modifier in m_Modifiers)
            {
                StatField additionalInfluencerStat = StatModifierUtility.GetFinalAddionalStat(modifier.Context, m_ModuleContext.Unit);
                if (modifier.IsActive)
                {
                    switch (modifier.Config.ModifierType)
                    {
                        case ModifierType.Buff:
                            finalAdditionalStat += additionalInfluencerStat;
                            break;
                        case ModifierType.Debuff:
                            finalAdditionalStat -= additionalInfluencerStat;
                            break;
                    }
                }
            }
            return unitStat + finalAdditionalStat;
        }

        private void RemoveAllModifier()
        {
            if (m_ModuleContext == null) return;
            if (m_Modifiers.Count > 0)
            {
                for (int i = 0; i < m_Modifiers.Count; i++)
                {
                    StatModifier modifier = m_Modifiers[i];
                    modifier.OnDeactiveInvoke();
                }
            }
            // Refresh stat setelah semua modifier hilang
            if (m_ModuleContext.Unit.HasBind(out Damageable damageable))
            {
                if (damageable.ModuleContext.Initialized)
                {
                    damageable.RefreshDamageableStat(1f, false);
                }
            }
        }

        public void ResetProgression()
        {
            RemoveAllModifier();
        }
    }
}
