using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class SpriteModelView : ModelView
    {
        [SerializeField]
        private List<Sprite> m_Sprites = new();
        [SerializeField]
        private SpriteRenderer m_SpriteModel;
        private Sprite GetSprite(string spriteName)
        {
            Sprite match = m_Sprites.Find(x => x.name == spriteName);
            return match;
        }

        public void SetSprite(string spriteName)
        {
            m_SpriteModel.sprite = GetSprite(spriteName);
        }
        public void SetSprite(Sprite set)
        {
            m_SpriteModel.sprite = set;
        }

    }
}
