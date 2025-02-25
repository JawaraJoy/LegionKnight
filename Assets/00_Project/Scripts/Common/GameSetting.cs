using UnityEngine;

namespace LegionKnight
{

    public partial class GameSetting : MonoBehaviour
    {
        [SerializeField]
        private int m_TargetFrameRate = 30;

        private void Start()
        {
            Application.targetFrameRate = m_TargetFrameRate;
        }
    }
}
