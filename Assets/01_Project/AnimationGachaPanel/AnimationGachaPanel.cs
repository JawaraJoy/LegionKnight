using UnityEngine;

namespace LegionKnight
{
    public class AnimationGachaPanel : PanelView
    {
        [SerializeField]
        private SpineUI m_SpineVFX;
        [SerializeField]
        private SpineUI m_SpineChar;
        [SerializeField]
        private AnimationItemView m_AnimationItemView;
        public SpineUI SpineVFX => m_SpineVFX;
        public SpineUI SpineChar => m_SpineChar;
        public AnimationItemView AnimationItemView => m_AnimationItemView;

    }
}
