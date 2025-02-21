using UnityEngine;

namespace LegionKnight
{
    public partial class ParallaxHandler : MonoBehaviour
    {
        [SerializeField]
        private ParallaxField[] m_Parallaxs;

        private void Start()
        {
            foreach(ParallaxField parallax in m_Parallaxs)
            {
                parallax.Start();
            }
        }

        private void LateUpdate()
        {
            foreach (ParallaxField parallax in m_Parallaxs)
            {
                parallax.LateUpdate();
            }
        }
    }
}
