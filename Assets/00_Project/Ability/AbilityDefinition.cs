using NUnit.Framework;
using UnityEngine;

namespace LegionKnight
{

    [CreateAssetMenu(fileName = "New Ability", menuName = "Legion Knight/Ability", order = 0)]
    public partial class AbilityDefinition : ScriptableObject
    {
        [SerializeField]
        private AbilityDescription[] m_Descriptions;

        public string GetFinalDescription(int level)
        {
            if (m_Descriptions == null || m_Descriptions.Length == 0)
                return "No description available.";

            string description = "";
            for (int i = 0; i < m_Descriptions.Length; i++)
            {
                description += m_Descriptions[i].GetDescription(level);
            }
            return description;
        }
    }

    [System.Serializable]
    public class AbilityDescription
    {
        [SerializeField]
        private float m_BaseVal;
        [SerializeField]
        private float m_UpgradeVal;

        [SerializeField, TextArea]
        private string m_Description;

        public string GetDescription(int level)
        {
            return string.Format(m_Description, m_BaseVal + (m_UpgradeVal * (level - 1)));
        }
    }
}
