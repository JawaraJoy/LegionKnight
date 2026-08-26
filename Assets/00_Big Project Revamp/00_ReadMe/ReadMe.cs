#if UNITY_EDITOR
using UnityEngine;

namespace Rush
{
    public class ReadMe : MonoBehaviour
    {
        [System.Serializable]
        public class Explaination
        {
            [SerializeField]
            private Object m_Object;
            [SerializeField, TextArea(3, 5)]
            private string m_Description;
        }
        [SerializeField]
        private Explaination[] m_Explainations;
    }
}
#endif
