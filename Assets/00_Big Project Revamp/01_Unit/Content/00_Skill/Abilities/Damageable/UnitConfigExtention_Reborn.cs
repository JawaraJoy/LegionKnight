using UnityEngine;

namespace Rush
{
    public class UnitConfigExtention_Reborn : MonoBehaviour
    {
        
    }
    public abstract partial class UnitConfig
    {
        [SerializeField, Min(0)]
        private int m_RebornCount;
        public int RebornCount => m_RebornCount;
    }
}
