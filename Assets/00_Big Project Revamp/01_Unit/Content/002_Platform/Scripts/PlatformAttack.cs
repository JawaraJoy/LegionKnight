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
        private List<PlatformAttackField> m_AttackFields = new();
        [SerializeField, MMReadOnly]
        private AttackerField m_AttackerField;
        public AttackerField AttackerField => m_AttackerField;

        public void Init(IAbilityDeliver[] abilityDelivers)
        {
            int finalAttack = 0;
            float finalDamageBaseTargetMaxHp = 0f;
            m_AttackFields.Clear();
            foreach (var deliver in abilityDelivers)
            {
                AbilityConfig damageConfig = deliver.AbilityConfig;
                if (damageConfig is DamageAbilityConfig damageAbilityConfig)
                {
                    float attack = AbilityUltility.GetFinalPowerAmount(deliver.AbilityContext);
                    int roundedAttack = Mathf.RoundToInt(attack);
                    float damageBasedTargetMaxHp = damageAbilityConfig.DamageBasedTargetMaxHP;
                    DamageType damageType = damageAbilityConfig.DamageType;

                    PlatformAttackField attackField = new(roundedAttack, damageBasedTargetMaxHp, damageType);
                    attackField.Init(deliver.AbilityContext);
                    if (GetAttackField(deliver.AbilityContext) == null)
                    {
                        m_AttackFields.Add(attackField);
                    }
                    finalAttack += roundedAttack;
                    finalDamageBaseTargetMaxHp += damageBasedTargetMaxHp;

                }
            }
            m_AttackerField = new(finalAttack, finalDamageBaseTargetMaxHp, DamageType.TrueDamage);
        }

        private PlatformAttackField GetAttackField(IAbilityContext abilityContext)
        {
            string abilityId = abilityContext.AbilityDeliver.AbilityConfig.BaseInfo.Id;
            return m_AttackFields.Find(x => x.AbilityContext.AbilityDeliver.AbilityConfig.BaseInfo.Id == abilityId);
        }
    }
}
