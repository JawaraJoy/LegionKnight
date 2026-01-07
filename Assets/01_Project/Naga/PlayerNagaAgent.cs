using UnityEngine;

namespace Rush
{
    public class PlayerNagaAgent : MonoBehaviour
    {
        private PlayerNaga m_PlayerNaga;

        private PlayerNaga PlayerNagaInternal
        {
            get
            {
                if (m_PlayerNaga == null)
                {
                    m_PlayerNaga = Player.Instance.Naga;
                }
                return m_PlayerNaga;
            }
        }
        public void SetTrigger(string triggerName)
        {
            PlayerNagaInternal.SetTrigger(triggerName);
        }
        public void ShowNaga()
        {
            PlayerNagaInternal.ShowNaga();
        }
    }
}
