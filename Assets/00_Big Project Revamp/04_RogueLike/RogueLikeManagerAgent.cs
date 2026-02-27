using UnityEngine;

namespace Rush
{
    public class RogueLikeManagerAgent : MonoBehaviour
    {
        private RogueLikeManager m_Handler;

        private RogueLikeManager Handler
        {
            get
            {
                if (m_Handler == null)
                {
                    m_Handler = RushGameManager.Instance.RogueLikeManager;
                }
                return m_Handler;
            }
        }

        public void AddExperience(int amount)
        {
            Handler.AddExperience(amount);
        }
        public void ResetProgress()
        {
            Handler.ResetProgress();
        }
    }
}
