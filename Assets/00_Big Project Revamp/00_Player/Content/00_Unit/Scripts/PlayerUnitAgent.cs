using UnityEngine;

namespace Rush
{
    public class PlayerUnitAgent : MonoBehaviour
    {
        private PlayerUnit m_Unit;
        private PlayerUnit UnitInternal
        {
            get
            {
                if (m_Unit == null)
                {
                    m_Unit = Player.Instance.Unit;
                }
                return m_Unit;
            }
        }
        public void Init(UnitConfig unitConfig)
        {
            UnitInternal.Init(unitConfig);
        }
    }
}
