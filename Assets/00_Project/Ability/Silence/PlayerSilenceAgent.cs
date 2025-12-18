using UnityEngine;

namespace LegionKnight
{
    public class PlayerSilenceAgent : MonoBehaviour
    {
        private PlayerSilence m_Silence;

        private PlayerSilence PlayerSilence
        {
            get
            {
                if (m_Silence == null)
                {
                    
                    m_Silence = Player.Instance.Silence;
                }
                return m_Silence;
            }
        }


    }
}
