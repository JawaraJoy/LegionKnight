using UnityEngine;

namespace LegionKnight
{
    // =============================
    // POOL DEFINITION (ScriptableObject)
    // =============================
    [CreateAssetMenu(fileName = "Pool", menuName = "Legion Knight/Object Pooling/Pool", order = 1)]
    public class PoolDefinition : ScriptableObject
    {
        [SerializeField] private string m_Id = "DefaultPool";
        [SerializeField] private GameObject m_Prefab;
        [SerializeField] private int m_InitialSize = 10;
        [SerializeField] private bool m_Expandable = true;


        public string Id => m_Id;
        public GameObject Prefab => m_Prefab;
        public int InitialSize => Mathf.Max(1, m_InitialSize);
        public bool Expandable => m_Expandable;


        private void OnValidate()
        {
            if (string.IsNullOrEmpty(m_Id))
                m_Id = name;
        }
    }
}
