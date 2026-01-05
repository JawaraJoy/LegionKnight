using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class Ability : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private List<AbilityContext> m_AbilityContexts = new();

        public void Init(CharacterContext characterContext)
        {
            AbilityConfig[] abilities = characterContext.CharacterConfig.Abilities;
            for (int i = 0; i < abilities.Length; i++)
            {
                var abilityConfig = abilities[i];
                AbilityContext abilityContext = new (abilityConfig, characterContext.CharacterObject);
                RegisterAbilityInternal(abilityContext);

            }
        }

        private AbilityContext GetAbilityContextInternal(string id)
        {
            return m_AbilityContexts.Find(context => context.Config.BaseInfo.Id == id);
        }

        private void RegisterAbilityInternal(AbilityContext context)
        {
            if (GetAbilityContextInternal(context.Config.BaseInfo.Id) == null)
            {
                m_AbilityContexts.Add(context);
            }
        }
        private void UnregisterAbilityInternal(AbilityContext context)
        {
            if (GetAbilityContextInternal(context.Config.BaseInfo.Id) != null)
            {
                m_AbilityContexts.Remove(context);
            }
        }
    }
}
