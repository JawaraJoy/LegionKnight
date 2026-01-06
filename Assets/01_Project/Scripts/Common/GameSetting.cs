using UnityEngine;

namespace LegionKnight
{

    public partial class GameSetting : Singleton<GameSetting>
    {
        [SerializeField]
        private int m_FrameTarget = 30;

        private void Start()
        {
            //Application.targetFrameRate = m_FrameTarget;
        }
    }
}
