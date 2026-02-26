using UnityEngine;
using System.Collections.Generic;

namespace Rush
{
    [CreateAssetMenu(fileName = "VerticalBackgroundConfig", menuName = "Rush/Level/Vertical Background Config")]
    public class VerticalBackgroundConfig : ScriptableObject
    {
        [Header("Base Environment")]
        [SerializeField] 
        private Sprite m_BaseSprite;

        [Header("Sky Segments")]
        [SerializeField]
        private Sprite[] m_SkySegmentSprites;

        [Header("Optional Offset")]
        [SerializeField] 
        private float m_SkyStartOffset = 0f;

        public Sprite BaseSprite => m_BaseSprite;
        public Sprite[] SkySegmentSprites => m_SkySegmentSprites;
        public float SkyStartOffset => m_SkyStartOffset;
    }
}