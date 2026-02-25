using Rush;
using UnityEngine;

namespace LegionKnight
{
    public interface IPopUpInfo 
    {
        string Info { get; }
    }

    public partial class CustomImageDefinition
    {
        [SerializeField, TextArea]
        private string m_InfoUnlock;
        public string Info => m_InfoUnlock;
    }
}
