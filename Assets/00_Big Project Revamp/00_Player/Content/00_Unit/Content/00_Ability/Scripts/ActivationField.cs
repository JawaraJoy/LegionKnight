using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class ActivationField
    {
        [SerializeField]
        private bool m_AutoActiveOnReady = false;
        [SerializeField]
        private SkillReadyState m_ReadyState = SkillReadyState.OnChargeFull;
        [SerializeField]
        private float m_ScoreToReady = 10f;

        public bool AutoActiveOnReady => m_AutoActiveOnReady;
        public SkillReadyState ReadyState => m_ReadyState;
        public float ScoreToReady => m_ScoreToReady;
    }

    public enum SkillReadyState
    {
        OnCooldownDone = 0,
        OnChargeFull = 1,
    }
}
