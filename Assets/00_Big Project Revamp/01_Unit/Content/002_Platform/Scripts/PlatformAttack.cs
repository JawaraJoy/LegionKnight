using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class PlatformAttack : MonoBehaviour, IHasAttacker, IHasAbilityContext
    {
        [SerializeField]
        private Platform2D m_MainPlatform;
        [SerializeField, MMReadOnly]
        private AttackerField m_AttackerField;
        public AttackerField AttackerField => m_AttackerField;
        private AbilityContext m_AbilityContext;
        public AbilityContext AbilityContext => throw new System.NotImplementedException();

        public bool Initialized => m_AbilityContext.Initialized;

        public void Init(AbilityContext abilityContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
