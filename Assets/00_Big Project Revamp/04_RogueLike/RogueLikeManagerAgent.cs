using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class RogueLikeManagerAgent : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<int> m_OnLevelUp;

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
        private void Start()
        {
            Handler.OnLevelUp.AddListener(OnLevelUpInvoke);
        }
        public void AddExperience(int amount)
        {
            Handler.AddExperience(amount);
        }
        public void ResetProgress()
        {
            Handler.ResetProgress();
        }
        private void OnLevelUpInvoke(int level)
        {
            m_OnLevelUp.Invoke(level);
        }
    }
}
