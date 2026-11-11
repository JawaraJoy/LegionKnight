using UnityEngine;

namespace Rush
{
    public class PlayerDamageableAgent : MonoBehaviour
    {
        private Damageable m_Damageable;
        private PlayerUnit m_PlayerUnit;
        private PlayerUnit PlayerUnitInternal
        {
            get
            {
                if (m_PlayerUnit == null)
                {
                    m_PlayerUnit = RushPlayer.Instance.Unit;
                }
                return m_PlayerUnit;
            }
        }

        private Damageable DamageableInternal
        {
            get
            {
                if (m_Damageable == null)
                {
                    if (PlayerUnitInternal.HasBind(out Damageable damageable))
                    {
                        m_Damageable = damageable;
                    }
                }
                return m_Damageable;
            }
        }

        public void SetImmortal(bool isInvicible)
        {
            if (DamageableInternal != null)
            {
                DamageableInternal.SetImmortal(isInvicible);
            }
            else
            {
                Debug.LogError($"Player doesnt has [{m_Damageable.GetType()}] in Binds");
            }
        }
    }
}
