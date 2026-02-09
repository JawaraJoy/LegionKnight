using UnityEngine;

namespace LegionKnight
{
    public interface IPopUpInfo 
    {
        Sprite Icon { get; }
        string Info { get; }
    }

    public partial class CustomImageDefinition : IPopUpInfo
    {
        [SerializeField, TextArea]
        private string m_InfoUnlock;
        public string Info => m_InfoUnlock;
    }
}
