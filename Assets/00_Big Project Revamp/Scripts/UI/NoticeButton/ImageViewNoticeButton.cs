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
            CustomImageType imageType = m_Definition.Type;
            ImageContent imageContent = Player.Instance.CustomProfile.GetIcon(m_Definition);
            
            switch (imageType)
            {
                case CustomImageType.Frame:
                    imageContent = Player.Instance.CustomProfile.GetFrame(m_Definition);
                    break;
                case CustomImageType.Icon:
                    imageContent = Player.Instance.CustomProfile.GetIcon(m_Definition);
                    break;
            }
            ProductCondition condition = imageContent.Condition;
            return condition == ProductCondition.NewUnlocked;
        }
    }
}
