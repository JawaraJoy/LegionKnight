using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class StatusEffectController : MonoBehaviour, IUnitExtension
    {
        [SerializeField, MMReadOnly]
        private List<StatusEffector> m_Effectors = new();
        [SerializeField]
        private UnityEvent<StatusEffector> m_OnApplied;
        [SerializeField]
        private UnityEvent<StatusEffector> m_OnDone;
        private ModuleContext m_ModuleContext;
        public IModuleContext ModuleContext => m_ModuleContext;
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
        public void ApplyEffector(StatusEffectConfig config, IAbilityContext context, Unit unitTarget)
        {
            StatusEffector existed = GetEffectorInternal(config);
            if (existed == null)
            {
                existed = SpawnEffector(config);
            }
            
            existed.gameObject.SetActive(true);
            existed.ApplyEffect(config, context, unitTarget);
            m_OnApplied?.Invoke(existed);
            Debug.Log($"Applied by Controller {config.BaseInfo.Name} to {unitTarget.name}");
        }
        public void RemoveEffector(StatusEffectConfig config)
        {
            if (HasEffectorInternal(config, out StatusEffector effector))
            {
                effector.RemoveEffect();
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
    }
}
