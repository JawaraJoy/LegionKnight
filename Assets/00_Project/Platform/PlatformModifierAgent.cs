using UnityEngine;

namespace LegionKnight
{
    public class PlatformModifierAgent : MonoBehaviour
    {
        [SerializeField]
        private LevelDefinition m_ApplyEffectOnLevel;
        public void SetPlatformSpeed(float speedRate)
        {
            bool isLevelMatch = m_ApplyEffectOnLevel == GameManager.Instance.LevelDefinition;
            if (isLevelMatch)
            {
                GameManager.Instance.SetSpeedPlatformRate(speedRate);
            }
            
        }
    }
}
