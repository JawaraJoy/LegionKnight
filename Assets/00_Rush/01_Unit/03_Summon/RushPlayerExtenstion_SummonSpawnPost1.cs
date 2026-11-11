using UnityEngine;

namespace Rush
{
    public class RushPlayerExtenstion_SummonPost
    {
        
    }
    public partial class RushPlayer
    {
        [SerializeField]
        private Transform m_SummonSpawnPost;
        public Transform SummonSpawnPost => m_SummonSpawnPost;
    }
}
