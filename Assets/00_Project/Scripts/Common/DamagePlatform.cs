using UnityEngine;

namespace LegionKnight
{
    public partial class DamagePlatform : Damageable
    {
        
    }

    public partial class Platform
    {
        [SerializeField]
        DamagePlatform m_DamagePlatform;

        public void SetFatal(bool isFatal)
        {
            if (m_DamagePlatform != null)
            {
                m_DamagePlatform.SetFatal(isFatal);
            }
            else
            {
                Debug.LogWarning("DamagePlatform is not assigned in Platform.");
            }
        }
    }
}
