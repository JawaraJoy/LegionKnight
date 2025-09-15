using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    [System.Serializable]
    public class BossUnit
    {
        [SerializeField]
        private BosDefinition m_BosDefinition;
        [SerializeField]
        private AssetReferenceGameObject m_BosAsset;
        [SerializeField]
        private bool m_IsDefeated;
        public BosDefinition BosDefinition => m_BosDefinition;
        public AssetReferenceGameObject BosAsset => m_BosAsset;
        public bool IsDefeated => m_IsDefeated;

        private string BossId => m_BosDefinition?.Id + "Defeated";

        public void Initialize()
        {
            if (UnityService.Instance.HasData(BossId))
            {
                UnityService.Instance.LoadData(BossId);
                m_IsDefeated = UnityService.Instance.GetData<bool>(BossId);
            }
        }
        public void Defeat()
        {
            m_IsDefeated = true;
            UnityService.Instance.SaveData(BossId, m_IsDefeated);
        }
    }
    public class BossDeck : MonoBehaviour
    {
        [SerializeField]
        private BossUnit[] m_BossUnits;
        public BossUnit[] BossUnits => m_BossUnits;

        private List<BossUnit> GetDefeatedBossUnit()
        {
            var defeatedBosses = new List<BossUnit>();
            foreach (var bossUnit in m_BossUnits)
            {
                if (bossUnit.IsDefeated)
                {
                    defeatedBosses.Add(bossUnit);
                }
            }
            return defeatedBosses;
        }

        public BossUnit GetRandomDefeatedBoss()
        {
            var defeatedBosses = GetDefeatedBossUnit();
            
            if (defeatedBosses.Count == 0)
            {
                return null;
            }
            int randomIndex = Random.Range(0, defeatedBosses.Count);
            return defeatedBosses[randomIndex];
        }

        private BossUnit GetBossUnit(string id)
        {
            foreach (var bossUnit in m_BossUnits)
            {
                if (bossUnit.BosDefinition.Id == id)
                {
                    return bossUnit;
                }
            }
            return null;
        }
        public void Initialize()
        {
            foreach (var bossUnit in m_BossUnits)
            {
                bossUnit.Initialize();
            }
        }
        public void DefeatedBoss(BosDefinition defi)
        {
            var bossUnit = GetBossUnit(defi.Id);
            if (bossUnit != null && !bossUnit.IsDefeated)
            {
                bossUnit.Defeat();
            }
        }
    }
}
