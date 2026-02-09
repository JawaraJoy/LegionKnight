using LegionKnight;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public struct WelcomePanelQue
    {
        [SerializeField]
        private bool m_DontShowThisAgain;
        [SerializeField]
        private PanelView m_Panel;
    }
}
