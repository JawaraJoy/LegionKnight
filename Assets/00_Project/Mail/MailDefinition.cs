using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Mail", menuName = "Legion Knight/Mail")]
    public class MailDefinition : ScriptableObject, IDescriptable
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Label;
        [SerializeField]
        private string m_Description;
        [SerializeField]
        private LootField[] m_Rewards;

        public string Id => m_Id;
        public string Label => m_Label;
        public string Description => m_Description;
        public LootField[] Rewards => m_Rewards;
    }
}
