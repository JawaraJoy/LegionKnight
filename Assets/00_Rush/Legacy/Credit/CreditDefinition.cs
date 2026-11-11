using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Credit", menuName = "Legion Knight/Credit")]
    public class CreditDefinition : ScriptableObject
    {
        [SerializeField]
        private CreditField[] m_Credits;
        public CreditField[] Credits => m_Credits;
    }

    [System.Serializable]
    public class CreditField
    {
        [SerializeField]
        private string m_JobDesk;
        [SerializeField]
        private string[] m_StaffNames;
        public string JobDesk => m_JobDesk;
        public string[] StaffNames => m_StaffNames;
        public CreditField(string jobDesk, string[] staffNames)
        {
            m_JobDesk = jobDesk;
            m_StaffNames = staffNames;
        }
    }
}
