using LegionKnight;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class LootController : MonoBehaviour, IUnitExtension
    {
        private ModuleContext m_ModuleContext;
        public IModuleContext ModuleContext => m_ModuleContext;

        private LootChestDefinition m_LootConfig;
        
        private Damageable m_Damageable;
private LootStorageManager m_LootStorageManager;

        private LootStorageManager LootStorageManager
        {
            get
            {
                if (m_LootStorageManager == null)
                {
                    m_LootStorageManager = GameManager.Instance.LootStorageManager;
                }
                return m_LootStorageManager;
            }
        }

        [SerializeField]
        private UnityEvent<CollectibleConfig> m_OnLoot;
public void Init(Unit unit)
        {
            m_ModuleContext = new ModuleContext(unit, gameObject);
            if (unit.Config is EnemyUnitConfig enemyConfig)
            {
                m_LootConfig = enemyConfig.Loot;
            }

            UnregisterDeathListenerInternal();
            if (!unit.HasBind(out Damageable damageable))
            {
                return;
            }

            m_Damageable = damageable;
            m_Damageable.OnDeath.RemoveListener(HandleDeath);
            m_Damageable.OnDeath.AddListener(HandleDeath);
        }

private void HandleDeath(IAbilityContext context)
        {
            Loots();
        }

        private void UnregisterDeathListenerInternal()
        {
            if (m_Damageable == null)
            {
                return;
            }

            m_Damageable.OnDeath.RemoveListener(HandleDeath);
            m_Damageable = null;
        }

        private void OnDestroy()
        {
            UnregisterDeathListenerInternal();
        }


private void Loots()
        {
            if (m_LootConfig == null) return;

            LootField loot = m_LootConfig.GetRandomOneLoot();
            if (loot == null) return;

            OnLootInvoke(loot.ItemLoot);
            LootStorageManager.AddLoot(loot);
            Debug.Log($"Loot {loot.ItemLoot.BaseInfo.Name}_ {loot.Amount}");
        }

private void OnLootInvoke(CollectibleConfig collectibleConfig)
        {
            m_OnLoot?.Invoke(collectibleConfig);
        }

    }
}
