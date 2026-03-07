using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField]
        private UnityEvent<IDamageable> m_OnAttackDelivered;
        [SerializeField]
        private UnityEvent<ITargetable> m_OnAttackDeliveredTarget;
        public AttackerField AttackerField => m_AttackerField;

        public IAbilityContext AbilityContext => m_AbilityContext;

        public bool Initialized => m_AbilityContext.Initialized;

        public UnityEvent<IDamageable> OnAttackDelivered => m_OnAttackDelivered;

        public UnityEvent<ITargetable> OnAttackDeliveredTarget => m_OnAttackDeliveredTarget;

        private void Start()
        {
            m_OnAttackDelivered.AddListener((context) => DesSpawnPlatform());
        }
        public void Init(IAbilityContext abilityContext)
        {
            m_AbilityContext = new AbilityContext(abilityContext.AbilityDeliver, abilityContext.SkillContext);
            m_AttackerField = AbilityUltility.GetAttacker(abilityContext);

        }

        private void DesSpawnPlatform()
        {
            RushGameManager.Instance.StageManager.PlatformHandler.ReturnToPool(m_MainPlatform);
            RushGameManager.Instance.StageManager.PlatformHandler.SpawnNextPlatformFromWaitingList(0.2f);
        }
    }
}
