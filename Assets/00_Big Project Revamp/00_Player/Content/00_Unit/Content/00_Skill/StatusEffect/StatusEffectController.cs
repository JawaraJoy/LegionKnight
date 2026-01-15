using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class StatusEffectController : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private List<StatusEffector> m_Effectors = new();
        [SerializeField]
        private UnityEvent<StatusEffector> m_OnApplied;
        [SerializeField]
        private UnityEvent<StatusEffector> m_OnDone;
        private StatusEffector GetEffectorInternal(StatusEffectConfig config)
        {
            return m_Effectors.Find(x => x.Config.BaseInfo.Id == config.BaseInfo.Id);
        }
        private bool HasEffectorInternal(StatusEffectConfig config, out StatusEffector statusEffector)
        {
            bool hasEffector = GetEffectorInternal(config) != null;
            if (hasEffector)
            {
                statusEffector = GetEffectorInternal(config);
            }
            else
            {
                statusEffector = null;
            }
            return hasEffector;
        }
        public void ApplyEffector(StatusEffectConfig config, AbilityContext context, Unit unitTarget)
        {
            StatusEffector existed = GetEffectorInternal(config);
            if (existed == null)
            {
                existed = SpawnEffector(config.EffectorPrefab);
            }
            existed.gameObject.SetActive(true);
            existed.ApplyEffect(config, context, unitTarget);
            m_OnApplied?.Invoke(existed);
        }
        public void CancelEffector(StatusEffectConfig config)
        {
            if (HasEffectorInternal(config, out StatusEffector effector))
            {
                effector.CancelEffect();
                m_OnDone?.Invoke(effector);
            }
        }

        private StatusEffector SpawnEffector(StatusEffector prefab)
        {
            StatusEffector spawned = Instantiate(prefab, transform, false);
            m_Effectors.Add(spawned);
            return spawned;
        }
    }
}
