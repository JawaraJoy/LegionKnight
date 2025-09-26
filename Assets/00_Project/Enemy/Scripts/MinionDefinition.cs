using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Minion", menuName = ("Legion Knight/Minion"))]
    public class MinionDefinition : ScriptableObject, IAbilityOwner
    {
        [SerializeField]
        private string m_Label = "Minion";
        [SerializeField]
        private int m_StartLevel = 0;
        [SerializeField]
        private Sprite m_Looks;
        [SerializeField]
        private AbilityDefinition m_AbilityDefinition;
        [SerializeField]
        private LootDefinition m_LootDefinition;
        [SerializeField]
        private AssetReferenceGameObject m_ModelPrefab;

        [SerializeField]
        private int m_RewardKilled;
        [SerializeField]
        private Currency m_ItemRewardKilled;

        public string Label => m_Label;
        public Sprite Looks => m_Looks;
        public AbilityDefinition AbilityDefinition => m_AbilityDefinition;
        public LootDefinition LootDefinition => m_LootDefinition;
        public AssetReferenceGameObject ModelPrefab => m_ModelPrefab;
        public int RewrdKilled => m_RewardKilled;
        private int StartLevelInternal
        {
            get
            {
                int level= Mathf.Max(1, m_StartLevel);
                return level;
            }
        }
        public int StartLevel => StartLevelInternal;
        public void SpawnMinion()
        {
            GameManager.Instance.SpawnMinion(this);
            m_AbilityDefinition.SetOwner(this);
        }

        public void SetCanSpawnUnit(bool set)
        {
            GameManager.Instance.SetCanSpawnUnit(this, set);
        }

        public void AddReward()
        {
            GameManager.Instance.AddScoreAmount(m_RewardKilled);

            LootField lootField = new(m_ItemRewardKilled.CurrencyDefinition, false, m_ItemRewardKilled.Amount, 1f);
            LootStorage lootStorage = GameManager.Instance.GetLootStorageManager();
            lootStorage.AddLoot(lootField);
            //Player.Instance.AddCurrencyAmount(m_ItemRewardKilled.CurrencyDefinition, m_ItemRewardKilled.Amount);
        }

        public int GetOwnerLevel()
        {
            return m_StartLevel;
        }
    }


}
