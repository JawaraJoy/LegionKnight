using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "WheelItem", menuName = "Legion Knight/Wheel Item", order = 1)]
    public class WheelItemDefinition : ScriptableObject, IDescriptable
    {
        [SerializeField] private string m_Id;
        [SerializeField] private string m_Label;
        [SerializeField] private string m_Description;
        [SerializeField] private Sprite m_Icon;

        public string Id => m_Id;
        public string Description => m_Description;
        public Sprite Icon => m_Icon;

        public string Label => m_Label;
    }
}
