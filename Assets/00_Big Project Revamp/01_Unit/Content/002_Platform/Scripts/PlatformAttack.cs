using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlatformAttack : MonoBehaviour, IHasAttacker
    {
        [SerializeField]
        private Platform2D m_MainPlatform;
        [SerializeField, MMReadOnly]
        private AttackerField m_AttackerField;

        private AbilityContext m_AbilityContext;
        public AttackerField AttackerField => m_AttackerField;

        public IAbilityContext AbilityContext => m_AbilityContext;

        public bool Initialized => m_AbilityContext.Initialized;


        public void Init(IAbilityContext abilityContext)
        {
            m_AbilityContext = new AbilityContext(abilityContext.AbilityDeliver, abilityContext.SkillContext);
            m_AttackerField = AbilityUltility.GetAttacker(abilityContext);
        }

    }
}
