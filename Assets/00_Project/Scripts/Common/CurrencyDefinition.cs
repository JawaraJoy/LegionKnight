using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Currency", menuName = "Legion Knight/Currency")]
    public class CurrencyDefinition : ScriptableObject, IDescriptable
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Label;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField, TextArea]
        private string m_Description;
        public string Id => m_Id;
        public Sprite Icon => m_Icon;
        public string Description => m_Description;

        public string Label => m_Label;
    }
    
}
