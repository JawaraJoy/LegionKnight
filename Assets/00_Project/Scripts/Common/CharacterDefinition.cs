using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Legion Knight/Character")]
    public partial class CharacterDefinition : ScriptableObject
    {
        [SerializeField]
        private Sprite m_Icon;
        [Header("Stat")]
        [SerializeField]
        private int m_Health;
        [SerializeField]
        private int m_Attack;
        public Sprite Icon => m_Icon;
        public int Attack => m_Attack;
        public int Health => m_Health;
        [SerializeField]
        private List<SkillDefinition> m_Weapons = new();
        [SerializeField]
        private List<SkillDefinition> m_Passives = new();
        public List<SkillDefinition> Weapons => m_Weapons;
        public List<SkillDefinition> Passives => m_Passives;
    }
    [System.Serializable]
    public partial class SkillDefinition
    {
        [SerializeField]
        private string m_SkillName;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private int m_ManaThreshold;
        [SerializeField]
        private AssetReferenceGameObject m_SkillPrefab;
        public AssetReferenceGameObject SkillPrefab => m_SkillPrefab;
        public string SkillName => m_SkillName;
        public Sprite Icon => m_Icon;
        public int Manathreshold => m_ManaThreshold;
    }
}
