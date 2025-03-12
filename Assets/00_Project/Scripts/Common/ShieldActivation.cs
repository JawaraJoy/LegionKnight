using UnityEngine;

namespace LegionKnight
{
    public partial class ShieldActivation : MonoBehaviour
    {
        [SerializeField]
        private Shield m_Shield;

        public void ActiveShield(int set)
        {
            m_Shield.gameObject.SetActive(true);
            m_Shield.Init(1, set);
        }
        public void DeActiveShield()
        {
            m_Shield.gameObject.SetActive(false);
        }
    }
}
