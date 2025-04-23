using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Currency", menuName = "Legion Knight/Currency")]
    public class CurrencyDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private Sprite m_Icon;
        public string Id => m_Id;
        public Sprite Icon => m_Icon;
    }
    
}
