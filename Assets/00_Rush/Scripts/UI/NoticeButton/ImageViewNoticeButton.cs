using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class ImageViewNoticeButton : NoticeButton
    {
        [SerializeField]
        private CustomImageDefinition m_Definition;
        public void SetDefinition(CustomImageDefinition definition)
        {
            m_Definition = definition;
        }
        protected override bool HasNewContent()
        {
            return Player.Instance.CustomProfile.HasNewAppearance(m_Definition);
        }
    }
}
