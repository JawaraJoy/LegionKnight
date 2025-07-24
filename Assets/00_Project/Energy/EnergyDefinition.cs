using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Energy", menuName = "Legion Knight/Energy", order = 1)]
    public class EnergyDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Label; // name
        [SerializeField]
        private Sprite m_Icon; // icon
        [SerializeField]
        private bool m_CanBreakMaxAmount = false; // Optional, if true, allows exceeding max amount temporarily
        [SerializeField]
        private int m_MaxAmount;

        public string Id => m_Id;
        public string Label => m_Label;
        public Sprite Icon => m_Icon;
        public int MaxAmount => m_MaxAmount;
        public bool CanBreakMaxAmount => m_CanBreakMaxAmount;
    }
}
