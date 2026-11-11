using UnityEngine;
using System.Collections.Generic;

namespace Rush
{
    [CreateAssetMenu(fileName = "VerticalBackgroundConfig", menuName = "Rush/Level/Vertical Background Config")]
    public class VerticalBackgroundConfig : ScriptableObject
    {
        [SerializeField] 
        private Sprite m_BaseSprite;
        [SerializeField]
        private Sprite[] m_SkySegmentSprites;
        [SerializeField] 
        private float m_SkyStartOffset = 0f;

        [SerializeField]
        private bool m_ShowForeground;

        public bool ShowForeground => m_ShowForeground;
        public Sprite BaseSprite => m_BaseSprite;
        public Sprite[] SkySegmentSprites => m_SkySegmentSprites;
        public float SkyStartOffset => m_SkyStartOffset;
    }
}