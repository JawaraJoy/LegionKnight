using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Pool", menuName = "Legion Knight/Object Pooling/Pool", order = 1)]
    public class PoolDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Id = "DefaultPool";
        [SerializeField]
        private int m_CopyCatAmount = 10;
        public string Id => m_Id;
        public int CopyCatAmount => m_CopyCatAmount;
    }
}
