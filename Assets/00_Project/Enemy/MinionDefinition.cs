using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Minion", menuName = ("Legion Knight/Minion"))]
    public class MinionDefinition : ScriptableObject
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
        private AssetReferenceGameObject m_ModelPrefab;

        [SerializeField]
        private int m_RewardKilled;
        [SerializeField]
        private Currency m_ItemRewardKilled;

        public string Label => m_Label;
        public Sprite Looks => m_Looks;
        public AbilityDefinition AbilityDefinition => m_AbilityDefinition;
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
        }

        public void SetCanSpawnUnit(bool set)
        {
            GameManager.Instance.SetCanSpawnUnit(this, set);
        }

        public void AddReward()
        {
            GameManager.Instance.AddCurrencyRewardAmount(m_RewardKilled);
            Player.Instance.AddCurrencyAmount(m_ItemRewardKilled.CurrencyDefinition, m_ItemRewardKilled.Amount);
        }
    }


}
