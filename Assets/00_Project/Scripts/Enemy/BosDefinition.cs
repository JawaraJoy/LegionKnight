using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Legion Knight/Bos Enemy")]
    public partial class BosDefinition : ScriptableObject
    {
        [SerializeField]
        private int m_StartLevel = 1;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private Stat m_BaseStat;
        [SerializeField]
        private Stat m_StatGainPerLevel;
        [SerializeField]
        private List<StandbyPlatformDefinition> m_BosPlatforms = new();
        [SerializeField]
        private SkillDefinition[] m_Skills;


        public Sprite Icon => m_Icon;
        public List<StandbyPlatformDefinition> BosPlatformsAsset => m_BosPlatforms;
        public SkillDefinition[] Skills => m_Skills;
        public Stat BaseStat => m_BaseStat;
        public int StartLevel => m_StartLevel;

        public Stat FinalStat(int addLevel)
        {
            return Stat.GetStatByLevel(m_BaseStat, m_StatGainPerLevel, m_StartLevel + addLevel);
        }
    }
    public partial class BosEnemy
    {
        [SerializeField]
        private BosDefinition m_BosDefinition;
        private Sprite IconInternal => m_BosDefinition.Icon;
        private List<StandbyPlatformDefinition> BosPlatformsInternal => m_BosDefinition.BosPlatformsAsset;
        private SkillDefinition[] SkillsInternal => m_BosDefinition.Skills;

        [SerializeField]
        private UnityEvent<BosDefinition> m_OnSetBosDefinition = new();

        public BosDefinition BosDefinition => m_BosDefinition;


        public void SetBosDefinition(BosDefinition definition)
        {
            m_BosDefinition = definition;
            OnSetBosDefinitionInvoke(definition);

            m_BosForm.sprite = m_BosDefinition.Icon;

            Init(definition);
        }

        private void OnSetBosDefinitionInvoke(BosDefinition definition)
        {
            m_OnSetBosDefinition?.Invoke(definition);
        }
    }
}
