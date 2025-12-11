using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "PadDefinition", menuName = "Legion Knight/Pad", order = 1)]
    public class PadDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Id = "ident";
        public string Id => m_Id;
        [SerializeField]
        private float m_DelayBeforeFly = 1f;
        [SerializeField]
        private float m_FlySpeed = 1.0f;
        public float DelayBeforeFly => m_DelayBeforeFly;
        public float FlySpeed => m_FlySpeed;

    }
}
