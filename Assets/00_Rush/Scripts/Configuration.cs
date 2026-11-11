using UnityEngine;

namespace Rush
{
    // the base all scriptable object configuration should inherit from, it contains the basic info of the configuration, such as id, name and description
    public abstract partial class Configuration : ScriptableObject
    {
        [SerializeField]
        protected BaseInfo m_BaseInfo;
        public BaseInfo BaseInfo => m_BaseInfo;
    }

    [System.Serializable]
    public partial class BaseInfo
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Name;
        [SerializeField, TextArea(3, 20)]
        private string m_Description;
        public string Id => m_Id;
        public string Name => m_Name;
        public string Description => m_Description;
    }
}
