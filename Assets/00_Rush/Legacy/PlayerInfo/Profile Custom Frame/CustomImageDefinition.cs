using Spine.Unity;
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
        public RuntimeAnimatorController runtimeAnim => m_runtimeAnim;
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
