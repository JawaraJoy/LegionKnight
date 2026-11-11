using UnityEngine;

namespace LegionKnight
{
    public class Credit : MonoBehaviour
    {
        [SerializeField]
        private CreditDefinition m_Definition;
        public CreditDefinition Definition => m_Definition;
    }
}
