using UnityEngine;

namespace Rush
{
    public class StageManager : StageHandler
    {
        
    }
    // for you know this is singleton
    // you can get this just RushGameManager.Instance.StageManager to get it
    public partial class RushGameManager 
    {
        [SerializeField]
        private StageManager m_StageManager;
        public StageManager StageManager => m_StageManager;
    }
}
