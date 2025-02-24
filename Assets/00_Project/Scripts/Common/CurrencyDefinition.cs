using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Currency", menuName = "Legion Knight/Currency")]
    public class CurrencyDefinition : ScriptableObject
    {
        [SerializeField]
        private Sprite m_Icon;
        public Sprite Icon => m_Icon;
    }
}
