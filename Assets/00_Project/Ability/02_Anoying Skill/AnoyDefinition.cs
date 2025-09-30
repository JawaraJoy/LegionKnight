using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Anoy", menuName = "Legion Knight/Anoy")]
    public class AnoyDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_AnoyName = "Default Anoy";
        [SerializeField]
        private int m_InteruptDurability = 5;

        public string AnoyName => m_AnoyName;
        public int InteruptDurability => m_InteruptDurability;
    }
}
