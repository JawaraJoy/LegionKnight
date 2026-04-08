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
        private LootStorageManager m_LootStorageManager;

        [SerializeField]
        private UnityEvent<CollectibleConfig> m_OnLoot;
        public void Init(Unit unit)
        {
            m_ModuleContext = new ModuleContext(unit, gameObject);
            if (unit.Config is EnemyUnitConfig enemyConfig)
            {
                m_LootConfig = enemyConfig.Loot;
                m_LootStorageManager = GameManager.Instance.LootStorageManager;
            }
            if (unit.HasBind(out Damageable damageable))
            {
                damageable.OnDeath.RemoveListener((context) => Loots());
                damageable.OnDeath.AddListener((context) => Loots());
            }
        }

        private void Loots()
        {
            if (m_LootConfig == null) return;
            var loot = m_LootConfig.GetRandomOneLoot();
            m_LootStorageManager.AddLoot(loot);
            m_OnLoot?.Invoke(loot.ItemLoot);
            Debug.Log($"Loot {loot.ItemLoot.BaseInfo.Name}_ {loot.Amount}");
        }
    }
}
