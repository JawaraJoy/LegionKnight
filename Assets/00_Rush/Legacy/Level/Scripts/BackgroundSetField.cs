using UnityEngine;

namespace LegionKnight
{
    public class BackgroundSetField
    {
        [SerializeField]
        private Sprite m_StartBackground;
        [SerializeField]
        private Sprite m_LoopBackground;
        public Sprite StartBackground => m_StartBackground;
        public Sprite BaseLoop => m_LoopBackground;
    }
}
