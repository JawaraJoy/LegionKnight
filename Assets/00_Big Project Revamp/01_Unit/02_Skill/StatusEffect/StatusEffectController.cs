
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class StatusEffectController : MonoBehaviour, IUnitExtension, IReseter
    {
        private List<StatusEffector> m_Effectors = new();
        private List<StatusEffector> m_RemovedEffectors = new();
        [SerializeField]
        private UnityEvent<StatusEffector> m_OnApplied;
        [SerializeField]
        private UnityEvent<StatusEffector> m_OnDone;
        private ModuleContext m_ModuleContext;
        public IModuleContext ModuleContext => m_ModuleContext;

        private StatusEffector existed;
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
        private bool HasRemovedEffector(StatusEffectConfig config, out StatusEffector statusEffector)
        {
            bool hasRemovedEffector = m_RemovedEffectors.Find(x => x.Config.BaseInfo.Id == config.BaseInfo.Id) != null;
            if (hasRemovedEffector)
            {
                statusEffector = m_RemovedEffectors.Find(x => x.Config.BaseInfo.Id == config.BaseInfo.Id);
            }
            else
            {
                statusEffector = null;
            }
            return hasRemovedEffector;
        }
        public void ApplyEffector(StatusEffectConfig config, IAbilityContext context, Unit unitTarget)
        {
            if (HasRemovedEffector(config, out StatusEffector removed))
            {
                existed = removed;
                m_RemovedEffectors.Remove(removed);
                Debug.Log($"Re-applied by Controller {config.BaseInfo.Name} to {unitTarget.name}");
            }
            else
            {
                if (HasEffectorInternal(config, out StatusEffector effector))
                {
                    existed = effector;
                }
                else
                {
                    existed = SpawnEffector(config);
                }
            }

            existed.gameObject.SetActive(true);
            existed.ApplyEffect(config, context, unitTarget);
            m_OnApplied?.Invoke(existed);
            Debug.Log($"Applied by Controller {config.BaseInfo.Name} to {unitTarget.name}");
        }
        private void RemoveEffector(StatusEffectConfig config)
        {
            if (HasEffectorInternal(config, out StatusEffector effector))
            {
                effector.RemoveEffect();
                m_RemovedEffectors.Add(effector);
                m_OnDone?.Invoke(effector);
            }
        }

        private StatusEffector SpawnEffector(StatusEffectConfig config)
        {
            StatusEffector spawned = Instantiate(config.EffectorPrefab, transform, false);
            spawned.Initialize(config);
            m_Effectors.Add(spawned);
            return spawned;
        }

        public void Init(Unit unit)
        {
            m_ModuleContext = new ModuleContext(unit, gameObject);
        }

        public void ResetProgression()
        {
            foreach (StatusEffector effector in m_Effectors)
            {
                if (ModuleContext.Initialized)
                {
                    RemoveEffector(effector.Config);
                }
            }
        }
    }
}
