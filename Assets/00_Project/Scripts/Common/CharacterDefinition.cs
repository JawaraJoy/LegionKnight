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
        private int Health;
        [SerializeField]
        private int Attack;

        public Sprite Icon => m_Icon;
    }
}
