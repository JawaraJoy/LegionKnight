using LegionKnight;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class StatController : MonoBehaviour, IUnitExtension
    {
        [SerializeField]
        private Transform m_ModifierPost;
        [SerializeField]
        private List<StatModifier> m_Modifiers = new List<StatModifier>();
        public Transform ModifierPost => m_ModifierPost;
        public List<StatModifier> StatModifiers => m_Modifiers;

        public IModuleContext ModuleContext => m_ModuleContext;

        private ModuleContext m_ModuleContext;

        public void Init(Unit unit)
        {
            if (!m_ModuleContext.Initialized)
            {
                m_ModuleContext = new ModuleContext(unit, gameObject);
            }
        }
        private StatModifier GetStatInfluencerInternal(AbilityConfig config)
        {
            return m_Modifiers.Find(x => x.Context.AbilityContext.AbilityDeliver.Config.BaseInfo.Id == config.BaseInfo.Id);
        }
        private bool HasStatInfluencer(AbilityContext abilityContext, out StatModifier modifier)
        {
            AbilityConfig config = abilityContext.AbilityDeliver.Config;
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
        public void AddModifier(AbilityContext abilityContext)
        {
            if (HasStatInfluencer(abilityContext, out StatModifier found))
            {
                
                if (found.IsActive)
                {
                    found.UpdateStack();
                }
                else
                {
                    found.Activate(abilityContext);
                }
            }
            else
            {
                
                if (abilityContext.AbilityDeliver is IStatModifierDeliver modifier)
                {
                    StatModifier prefab = modifier.ModifierConfig.StatModifierPrefab;
                    StatModifier spawnedModifier = Component.Instantiate(prefab, m_ModifierPost, false);
                    spawnedModifier.Activate(abilityContext);
                    m_Modifiers.Add(spawnedModifier);
                }
            }
            AbilityUltility.OnAbilityDeliveredInvoke(abilityContext, m_ModuleContext.Unit);
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
    }
}
