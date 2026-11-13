using UnityEngine;
using Rush;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Custom Image", menuName = "Legion Knight/Custom Image")]
    public partial class CustomImageDefinition : CollectibleConfig
    {
        [SerializeField]
        private RuntimeAnimatorController m_runtimeAnim;
        [SerializeField]
        private CustomImageType m_Type;
        [SerializeField] private CustomImageDefinition m_StaticVariant;
        private const bool c_ShowAnimatedProfiles = false;
        public CustomImageDefinition ProfileAppearance => !c_ShowAnimatedProfiles && m_StaticVariant != null ? m_StaticVariant : this;
        public RuntimeAnimatorController runtimeAnim => c_ShowAnimatedProfiles ? m_runtimeAnim : null;
        public CustomImageType Type => m_Type;

        public void SetOwned(bool owned)
        {
            Player.Instance.CustomProfile.SetOwned(this, owned);
        }
        public void SetSelected()
        {
            Player.Instance.CustomProfile.SetSelected(this);
        }
    }

    public enum CustomImageType
    {
        Frame,
        Icon
    }
}
