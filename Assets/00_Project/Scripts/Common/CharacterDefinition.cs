using UnityEngine;

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
    }
}
