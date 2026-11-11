using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public interface IAbilityDeliver
    {
        void Init(AbilityConfig config, ISkillContext skillContext);
        void Activate();
        Transform DeliverTransform {  get; }
        AbilityConfig AbilityConfig { get; }
        IAbilityContext AbilityContext { get; }
        List<StatusEffectConfig> GetStatusEffectsOnDelivered();
        List<StatusEffectConfig> GetStatusEffectsOnSelf();
        void AddCustomStatusEffectOnDelivered(StatusEffectConfig config);
        void AddCustomStatusEffectOnSelf(StatusEffectConfig config);
    }
}
