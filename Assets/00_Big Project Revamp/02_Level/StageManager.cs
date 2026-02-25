using UnityEngine;

namespace Rush
{
    public class StageManager : StageHandler
    {
        
    }

    public partial class RushGameManager
    {
        [SerializeField]
        private StageManager m_StageManager;
        public StageManager StageManager => m_StageManager;
    }
}
