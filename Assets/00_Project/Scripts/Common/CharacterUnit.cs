using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public partial class CharacterUnit
    {
        [SerializeField]
        private CharacterDefinition m_Definition;
        [SerializeField]
        private bool m_Owned;
        [SerializeField]
        private int m_CurrentStars;
        private const int m_MaxStars = 5;
        [SerializeField]
        private StarShardControl m_StarShardControl;
        [SerializeField]
        private UnityEvent<CharacterUnit> m_OnCharacterStarUp = new();
        [SerializeField]
        private UnityEvent<CharacterUnit> m_OnCharacterShardUpdate = new();
        public int CurrentStars => m_CurrentStars;
        public int MaxStars => m_MaxStars;
        public bool Owned => m_Owned;
        private void OnCharacterStarUpInvoke()
        {
            m_OnCharacterStarUp?.Invoke(this);
        }
        private void OnCharacterShardUpdateInvoke()
        {
            m_OnCharacterShardUpdate?.Invoke(this);
        }
        public void SetOwned(bool set)
        {
            m_Owned = set;
        }
        public void Init()
        {
            m_CurrentStars = m_Definition.StartingStar;
        }
        public string CharacterName => m_Definition.name;
        public Sprite Icon => m_Definition.Icon;
        public Sprite SmallIcon => m_Definition.SmallIcon;
        public Platform UniquePlatform => m_Definition.UniquePlatform;
        public List<SkillDefinition> Passives => m_Definition.Passives;
        public List<SkillDefinition> Weapons => m_Definition.Weapons;
        public CharacterDefinition Definition => m_Definition;
        public int Attack => m_Definition.Attack * m_CurrentStars;
        public int Health => m_Definition.Health;
        public int StartingStar => m_Definition.StartingStar;
        public int CurrentShardAmount => m_StarShardControl.CurrentShardAmount;
        public int MaxShardLevel => GetMaxShardLevelInternal();
        public int ShardEachStars => GetShardAmountInternal();

        public float GetCurrentShardRate()
        {
            return (float)m_StarShardControl.CurrentShardAmount / (float)GetMaxShardLevelInternal();
        }
        private int GetMaxShardLevelInternal()
        {
            return StarShardControl.MaxShardLevel[m_CurrentStars];
        }
        private int GetShardAmountInternal()
        {
            return StarShardControl.GetShardAmount(m_CurrentStars);
        }
        public void AddShardInternal(int amount)
        {
            m_StarShardControl.AddShard(amount);
            if (m_StarShardControl.CurrentShardAmount >= GetMaxShardLevelInternal())
            {
                m_CurrentStars++;
                if (m_CurrentStars >= m_MaxStars)
                {
                    m_CurrentStars = m_MaxStars;
                    return;
                }
                else
                {
                    m_StarShardControl.AddShard(-GetMaxShardLevelInternal());
                }
                OnCharacterStarUpInvoke();
            }
            OnCharacterShardUpdateInvoke();
        }
    }

    [System.Serializable]
    public partial class StarShardControl
    {
        [SerializeField]
        private int m_CurrenShardAmount;
        private static readonly List<int> m_ShardEachStars = new() { 1, 2, 4, 8, 16, 32 };
        private static readonly List<int> m_MaxShardLevels = new() { 1, 2, 4, 8, 16, 32 };
        public int CurrentShardAmount => m_CurrenShardAmount;
        public static int GetShardAmount(int star)
        {
            return m_ShardEachStars[star];
        }
        public static List<int> MaxShardLevel => m_MaxShardLevels;
        public void AddShard(int amount)
        {
            m_CurrenShardAmount += amount;
        }
    }
}
