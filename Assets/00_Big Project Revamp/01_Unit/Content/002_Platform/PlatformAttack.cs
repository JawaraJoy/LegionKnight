using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class PlatformAttack : MonoBehaviour, IAttacker
    {
        [SerializeField, MMReadOnly]
        private AttackerField m_AttackerField;
        public AttackerField AttackerField => m_AttackerField;
    }
}
