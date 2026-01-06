using UnityEngine;
using UnityEngine.InputSystem;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Custom Image", menuName = "Legion Knight/Custom Image")]
    public class CustomImageDefinition : ScriptableObject, IDescriptable
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Label;
        [SerializeField, TextArea]
        private string m_Description;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private CustomImageType m_Type;
        public string Id => m_Id;
        public string Label => m_Label;
        public string Description => m_Description;
        public Sprite Icon => m_Icon;
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
