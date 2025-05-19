using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Legion Knight/Dialogue")]
    public partial class DialogueDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Title;
        [SerializeField, TextArea]
        private string[] m_Descriptions;

        public string Id => m_Id;
        public string Title => m_Title;
        public string[] Descriptions => m_Descriptions;
        public string GetDescription(int index)
        {
            if (index < 0 || index >= m_Descriptions.Length)
            {
                Debug.LogError($"Index {index} is out of range for descriptions.");
                return string.Empty;
            }
            return m_Descriptions[index];
        }
        public int DescriptionCount => m_Descriptions.Length;

        public void StartTutorial()
        {
            GameManager.Instance.StartTutorial(m_Id);
        }
    }
}
