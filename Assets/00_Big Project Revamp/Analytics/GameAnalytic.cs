using UnityEngine;

namespace Rush
{
    public class GameAnalytic : Singleton<GameAnalytic>
    {
        [SerializeField]
        private FirebaseAnalytic m_FirebaseAnalyitc;

        protected override void Awake()
        {
            base.Awake();
            m_FirebaseAnalyitc.Init();
        }
    }
}
