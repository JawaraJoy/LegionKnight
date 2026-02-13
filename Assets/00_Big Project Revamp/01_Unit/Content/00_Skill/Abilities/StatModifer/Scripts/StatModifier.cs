using LegionKnight;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class StatModifier : MonoBehaviour
    {
        [SerializeField]
        private Transform m_InfluencerPost;
        [SerializeField]
        private List<StatInfluencer> m_Influencers = new List<StatInfluencer>();
        public Transform InfluencerPost => m_InfluencerPost;
        public List<StatInfluencer> StatInfluencers => m_Influencers;
        private ModuleContext m_ModuleContext;

        public void Init(Unit unit)
        {
            if (!m_ModuleContext.Initialized)
            {
                m_ModuleContext = new ModuleContext(unit, gameObject);
            }
        }
        private StatInfluencer GetStatInfluencerInternal(AbilityConfig config)
        {
            return m_Influencers.Find(x => x.Context.AbilityContext.AbilityDeliver.Config.BaseInfo.Id == config.BaseInfo.Id);
        }
        private bool HasStatInfluencer(AbilityContext abilityContext, out StatInfluencer influencer)
        {
            AbilityConfig config = abilityContext.AbilityDeliver.Config;
            bool hasStatInfluencer = GetStatInfluencerInternal(config) != null;
            if (hasStatInfluencer)
            {
                influencer = GetStatInfluencerInternal(config);
            }
            else
            {
                influencer = null;
            }
            return hasStatInfluencer;
        }
        public void AddModifier(AbilityContext abilityContext)
        {
            if (HasStatInfluencer(abilityContext, out StatInfluencer found))
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
                
                if (abilityContext.AbilityDeliver is IStatInfluencer influencer)
                {
                    StatInfluencer prefab = influencer.InfluencerConfig.StatInfluencerPrefab;
                    StatInfluencer spawnedInfluencer = Component.Instantiate(prefab, m_InfluencerPost, false);
                    spawnedInfluencer.Activate(abilityContext);
                    m_Influencers.Add(spawnedInfluencer);
                }
            }
            OnSkillDeliveredInvoke(abilityContext, m_ModuleContext.UnitOwner);
        }
        private static void OnSkillDeliveredInvoke(AbilityContext senderContext, Bindable bindableTarget)
        {
            SkillActivator senderActivator = senderContext.SkillContext.Activator;
            Unit unit = null;
            if (bindableTarget.HasBind(out Unit targetUnit))
            {
                unit = targetUnit;
            }
            if (bindableTarget is Unit unitItSelf)
            {
                unit = unitItSelf;
            }
            if (unit == null)
            {
                Debug.LogError($"{nameof(OnSkillDeliveredInvoke)} cant found Unit component");
                return;
            }
            senderActivator.OnAbilityDeliverd?.Invoke(unit);
            AbilityUltility.ApplyStatusEffect(senderContext, unit);
        }
        public StatField GetFinalStat(StatField unitStat)
        {
            StatField finalAdditionalStat = StatField.Zero;
            foreach (StatInfluencer influencer in m_Influencers)
            {
                StatField additionalInfluencerStat = StatModifierUtility.GetFinalAddionalStat(influencer.Context, m_ModuleContext.UnitOwner);
                if (influencer.IsActive)
                {
                    switch (influencer.Config.ModifierType)
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
