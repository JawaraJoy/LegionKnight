using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New NameSupply", menuName = "Legion Knight/NameSupply")]
    public partial class NameSupplyDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string[] m_FirstName;
        [SerializeField]
        private string[] m_LastName;

        public string Id => m_Id;
        private int GetRandomNumbers()
        {
            return Random.Range(0, 999);
        }

        private string GetRandomFirstName()
        {
            int r = Random.Range(0, m_FirstName.Length);
            return m_FirstName[0];
        }
        private string GetRandomLastName()
        {
            int r = Random.Range(0, m_LastName.Length);
            return m_LastName[0];
        }

        public string GetRandomName()
        {
            return $"{GetRandomFirstName()} {GetRandomLastName()} * {GetRandomNumbers()}";
        }
    }
}
