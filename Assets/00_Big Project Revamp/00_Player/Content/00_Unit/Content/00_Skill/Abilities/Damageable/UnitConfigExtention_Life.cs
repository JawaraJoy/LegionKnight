using UnityEngine;

namespace Rush
{
    public class UnitConfigExtention_Life : MonoBehaviour
    {
        
    }
    public partial class UnitConfig
    {
        [SerializeField, Min(1)]
        private int m_RebornCount;
        public int RebornCount => m_RebornCount;
    }
}
